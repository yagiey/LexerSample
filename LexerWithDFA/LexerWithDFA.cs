using LexerSample;
using LexerWithDFA.Extensions.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexerWithDFA
{
	internal class LexerWithDFA : IDisposable
	{
		internal class Wrapping<T>
		{
			public T Value { get; private set; }

			public Wrapping(T value)
			{
				Value = value;
			}
		}

		private const char Ht = '\t';
		private const char Lf = '\n';
		private const char Cr = '\r';
		private const char Sp = ' ';

		private readonly TextFileEnumerator _reader;
		private readonly IEnumerator<Tuple<char, char?>> _itorCh;
		private bool _isDisposed;
		private bool _foundCrLf;

		private int _nLf;
		private int _nCr;
		private int _nCrLf;

		private IEnumerable<Tuple<DFA, TokenType?>> _automata;
		private readonly DFA _comLine;
		private readonly DFA _comBlock;
		private readonly DFA _literalInt;
		private readonly DFA _identifier;
		private readonly DFA _keywordBegin;
		private readonly DFA _keywordConst;
		private readonly DFA _keywordDo;
		private readonly DFA _keywordEnd;
		private readonly DFA _keywordFunction;
		private readonly DFA _keywordIf;
		private readonly DFA _keywordReturn;
		private readonly DFA _keywordThen;
		private readonly DFA _keywordVar;
		private readonly DFA _keywordWhile;
		private readonly DFA _puncSemicolon;
		private readonly DFA _puncPeriod;
		private readonly DFA _puncComma;
		private readonly DFA _puncLeftParen;
		private readonly DFA _puncRightParen;
		private readonly DFA _puncColonEqual;
		private readonly DFA _puncEqual;
		private readonly DFA _puncLTGT;
		private readonly DFA _puncLT;
		private readonly DFA _puncLE;
		private readonly DFA _puncGT;
		private readonly DFA _puncGE;
		private readonly DFA _puncPlus;
		private readonly DFA _puncMinus;
		private readonly DFA _puncAsterisk;
		private readonly DFA _puncSlash;

		public LexerWithDFA(string filePath)
		{
			_automata = null!;
			_isDisposed = false;
			_foundCrLf = false;
			_reader = new TextFileEnumerator(filePath);
			_itorCh = _reader.GetEnumerator();
			_nLf = 0;
			_nCr = 0;
			_nCrLf = 0;

			_comLine = DFA.GenerateLineComment();
			_comBlock = DFA.GenerateBlockComment();

			// tokens
			const bool IgnoreCase = false;
			_literalInt = DFA.GenerateIntegerNumber();
			_identifier = DFA.GenerateIdentifier();
			_keywordBegin = DFA.GenerateString(TokenType.KeywordBegin, "begin", IgnoreCase);
			_keywordConst = DFA.GenerateString(TokenType.KeywordConst, "const", IgnoreCase);
			_keywordDo = DFA.GenerateString(TokenType.KeywordDo, "do", IgnoreCase);
			_keywordEnd = DFA.GenerateString(TokenType.KeywordEnd, "end", IgnoreCase);
			_keywordFunction = DFA.GenerateString(TokenType.KeywordFunction, "function", IgnoreCase);
			_keywordIf = DFA.GenerateString(TokenType.KeywordIf, "if", IgnoreCase);
			_keywordReturn = DFA.GenerateString(TokenType.KeywordReturn, "return", IgnoreCase);
			_keywordThen = DFA.GenerateString(TokenType.KeywordThen, "then", IgnoreCase);
			_keywordVar = DFA.GenerateString(TokenType.KeywordVar, "var", IgnoreCase);
			_keywordWhile = DFA.GenerateString(TokenType.KeywordWhile, "while", IgnoreCase);
			_puncSemicolon = DFA.GenerateString(TokenType.PunctuationSemicolon, ";", IgnoreCase);
			_puncPeriod = DFA.GenerateString(TokenType.PunctuationPeriod, ".", IgnoreCase);
			_puncComma = DFA.GenerateString(TokenType.PunctuationComma, ",", IgnoreCase);
			_puncLeftParen = DFA.GenerateString(TokenType.PunctuationLeftParen, "(", IgnoreCase);
			_puncRightParen = DFA.GenerateString(TokenType.PunctuationRightParen, ")", IgnoreCase);
			_puncColonEqual = DFA.GenerateString(TokenType.PunctuationColonEqual, ":=", IgnoreCase);
			_puncEqual = DFA.GenerateString(TokenType.PunctuationEqual, "=", IgnoreCase);
			_puncLTGT = DFA.GenerateString(TokenType.PunctuationLTGT, "<>", IgnoreCase);
			_puncLT = DFA.GenerateString(TokenType.PunctuationLT, "<", IgnoreCase);
			_puncLE = DFA.GenerateString(TokenType.PunctuationLE, "<=", IgnoreCase);
			_puncGT = DFA.GenerateString(TokenType.PunctuationGT, ">", IgnoreCase);
			_puncGE = DFA.GenerateString(TokenType.PunctuationGE, ">=", IgnoreCase);
			_puncPlus = DFA.GenerateString(TokenType.PunctuationPlus, "+", IgnoreCase);
			_puncMinus = DFA.GenerateString(TokenType.PunctuationMinus, "-", IgnoreCase);
			_puncAsterisk = DFA.GenerateString(TokenType.PunctuationAsterisk, "*", IgnoreCase);
			_puncSlash = DFA.GenerateString(TokenType.PunctuationSlash, "/", IgnoreCase);
			SetupAutomata();
		}

		public int CountNewLine()
		{
			return _nLf + _nCr + _nCrLf;
		}

		public Token? GetNextToken()
		{
			SetupAutomata();
			StringBuilder sb = new();

			while (true)
			{
				if (!_itorCh.MoveNext())
				{
					// EOF
					if (0 < sb.Length)
					{
						var tup2 = GetToken(sb.ToString());
						if (tup2 != null && tup2.Value != null)
						{
							return tup2.Value;
						}
						else
						{
							string errorMsg = string.Format("invalid token '{0}' detected", sb.ToString());
							throw new Exception(errorMsg);
						}
					}

					return null;
				}

				char current = _itorCh.Current.Item1;
				char? next = _itorCh.Current.Item2;

				// count line break
				if (current == Cr && next != null && next.Value == Lf)
				{
					_nCrLf++;
					_foundCrLf = true;
				}
				else if (_foundCrLf)
				{
					_foundCrLf = false;
				}
				else
				{
					if (current == Lf)
					{
						_nLf++;
					}
					else if (current == Cr)
					{
						_nCr++;
					}
				}

				if (IsInitialState() && IsWhiteSpace(current))
				{
					// skip whitespace
					continue;
				}

				MoveNextAutomata(current);
				sb.Append(current);

				int nowAlive = _automata.Count();
				if (nowAlive == 0)
				{
					string errorMsg = string.Format("invalid token '{0}' detected", sb.ToString());
					throw new Exception(errorMsg);
				}

				if (next.HasValue)
				{
					int nextAlive = CountNextAlive(next.Value);
					if (nextAlive == 0)
					{
						int nowAcceptable = _automata.Count(it => it.Item1.IsAcceptable());
						if (0 < nowAcceptable)
						{
							Wrapping<Token?>? tup1 = GetToken(sb.ToString());
							if (tup1 != null && tup1.Value != null)
							{
								return tup1.Value;
							}

							SetupAutomata();
							sb.Clear();
						}
						else
						{
							// just before error
						}
					}
				}
				else
				{
					// just before EOF
				}
			}
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}

			_reader.Dispose();
			_isDisposed = true;
		}

		#region private methods

		private void SetupAutomata()
		{
			_automata = Enumerable.Empty<Tuple<DFA, TokenType?>>();
			_automata = _automata.Append(Tuple.Create(_comLine, (TokenType?)null));
			_automata = _automata.Append(Tuple.Create(_comBlock, (TokenType?)null));
			_automata = _automata.Append(Tuple.Create(_literalInt, (TokenType?)TokenType.LiteralInteger));
			_automata = _automata.Append(Tuple.Create(_keywordBegin, (TokenType?)TokenType.KeywordBegin));
			_automata = _automata.Append(Tuple.Create(_keywordConst, (TokenType?)TokenType.KeywordConst));
			_automata = _automata.Append(Tuple.Create(_keywordDo, (TokenType?)TokenType.KeywordDo));
			_automata = _automata.Append(Tuple.Create(_keywordEnd, (TokenType?)TokenType.KeywordEnd));
			_automata = _automata.Append(Tuple.Create(_keywordFunction, (TokenType?)TokenType.KeywordFunction));
			_automata = _automata.Append(Tuple.Create(_keywordIf, (TokenType?)TokenType.KeywordIf));
			_automata = _automata.Append(Tuple.Create(_keywordReturn, (TokenType?)TokenType.KeywordReturn));
			_automata = _automata.Append(Tuple.Create(_keywordThen, (TokenType?)TokenType.KeywordThen));
			_automata = _automata.Append(Tuple.Create(_keywordVar, (TokenType?)TokenType.KeywordVar));
			_automata = _automata.Append(Tuple.Create(_keywordWhile, (TokenType?)TokenType.KeywordWhile));
			_automata = _automata.Append(Tuple.Create(_puncSemicolon, (TokenType?)TokenType.PunctuationSemicolon));
			_automata = _automata.Append(Tuple.Create(_puncPeriod, (TokenType?)TokenType.PunctuationPeriod));
			_automata = _automata.Append(Tuple.Create(_puncComma, (TokenType?)TokenType.PunctuationComma));
			_automata = _automata.Append(Tuple.Create(_puncLeftParen, (TokenType?)TokenType.PunctuationLeftParen));
			_automata = _automata.Append(Tuple.Create(_puncRightParen, (TokenType?)TokenType.PunctuationRightParen));
			_automata = _automata.Append(Tuple.Create(_puncColonEqual, (TokenType?)TokenType.PunctuationColonEqual));
			_automata = _automata.Append(Tuple.Create(_puncEqual, (TokenType?)TokenType.PunctuationEqual));
			_automata = _automata.Append(Tuple.Create(_puncLTGT, (TokenType?)TokenType.PunctuationLTGT));
			_automata = _automata.Append(Tuple.Create(_puncLT, (TokenType?)TokenType.PunctuationLT));
			_automata = _automata.Append(Tuple.Create(_puncLE, (TokenType?)TokenType.PunctuationLE));
			_automata = _automata.Append(Tuple.Create(_puncGT, (TokenType?)TokenType.PunctuationGT));
			_automata = _automata.Append(Tuple.Create(_puncGE, (TokenType?)TokenType.PunctuationGE));
			_automata = _automata.Append(Tuple.Create(_puncPlus, (TokenType?)TokenType.PunctuationPlus));
			_automata = _automata.Append(Tuple.Create(_puncMinus, (TokenType?)TokenType.PunctuationMinus));
			_automata = _automata.Append(Tuple.Create(_puncAsterisk, (TokenType?)TokenType.PunctuationAsterisk));
			_automata = _automata.Append(Tuple.Create(_puncSlash, (TokenType?)TokenType.PunctuationSlash));
			_automata = _automata.Append(Tuple.Create(_identifier, (TokenType?)TokenType.Identifer));
			_automata.ForEach(c => c.Item1.Reset());
		}

		private bool IsInitialState()
		{
			return _automata.All(it => it.Item1.IsInitialState());
		}

		private void MoveNextAutomata(char ch)
		{
			_automata.ForEach(it => it.Item1.MoveNext(ch));
			_automata = _automata.Where(it => !it.Item1.IsError());
		}

		private int CountNextAlive(char ch)
		{
			return _automata.Count(it => !it.Item1.IsNextError(ch));
		}

		private Wrapping<Token?>? GetToken(string str)
		{
			Tuple<DFA, TokenType?>? tup =
				_automata
				.Where(it => it.Item1.IsAcceptable())
				.FirstOrDefault();

			if (tup == null)
			{
				return null;
			}

			// return if it is token
			Token? token =
				tup.Item2 == null ?
				null :
				GetToken(tup.Item1, str, tup.Item2!.Value);

			return new Wrapping<Token?>(token);
		}

		private static Token GetToken(DFA dfa, string strToken, TokenType tokenType)
		{
			TokenType type = tokenType;
			string src = strToken;
			return new Token(type, src);
		}

		private static bool IsWhiteSpace(char ch)
		{
			return ch == Ht || ch == Lf || ch == Cr || ch == Sp;
		}

		#endregion
	}
}
