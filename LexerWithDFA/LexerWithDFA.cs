using LexerSample;
using LexerWithDFA.Extensions.Generic;
using PsudoDfa;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LexerWithDFA
{
	public class LexerWithDFA<TokenType> : IDisposable where TokenType : struct
	{
		private class Wrapping<T>
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
		private readonly IEnumerator<Tuple<int, int?>> _itorCh;
		private readonly IEnumerable<Tuple<Dfa, TokenType?>> _tokenInfos;
		private bool _isDisposed;
		private bool _foundCrLf;

		private int _nLf;
		private int _nCr;
		private int _nCrLf;

		private IEnumerable<Tuple<Dfa, TokenType?>> _automata;

		public LexerWithDFA(string filePath, IEnumerable<Tuple<Dfa, TokenType?>> tokenInfos)
		{
			_automata = null!;
			_isDisposed = false;
			_foundCrLf = false;
			_reader = new TextFileEnumerator(filePath);
			_itorCh = _reader.GetEnumerator();
			_nLf = 0;
			_nCr = 0;
			_nCrLf = 0;

			// DFAs for tokens
			_tokenInfos = tokenInfos;
			SetupAutomata();
		}

		public int CountNewLine()
		{
			return _nLf + _nCr + _nCrLf;
		}

		public Token<TokenType>? GetNextToken()
		{
			SetupAutomata();
			List<int> characters = new();

			while (true)
			{
				if (!_itorCh.MoveNext())
				{
					// EOF
					if (0 < characters.Count)
					{
						string strToken = GetString(characters);
						var tup2 = GetToken(strToken);
						if (tup2 != null && tup2.Value != null)
						{
							return tup2.Value;
						}
						else
						{
							string errorMsg = string.Format("invalid token '{0}' detected", strToken);
							throw new Exception(errorMsg);
						}
					}

					return null;
				}

				int current = _itorCh.Current.Item1;
				int? next = _itorCh.Current.Item2;

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
				characters.Add(current);

				int nowAlive = _automata.Count();
				if (nowAlive == 0)
				{
					string strToken = GetString(characters);
					string errorMsg = string.Format("invalid token '{0}' detected", strToken);
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
							string strToken = GetString(characters);
							Wrapping<Token<TokenType>?>? tup1 = GetToken(strToken);
							if (tup1 != null && tup1.Value != null)
							{
								return tup1.Value;
							}

							SetupAutomata();
							characters.Clear();
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

		private static string GetString(IEnumerable<int> characters)
		{
			string strToken = string.Join(string.Empty, characters.Select(it => char.ConvertFromUtf32(it)));
			return strToken;
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
			_automata =
				Enumerable.Empty<Tuple<Dfa, TokenType?>>()
				.Concat(_tokenInfos);
			_automata.ForEach(c => c.Item1.Reset());
		}

		private bool IsInitialState()
		{
			return _automata.All(it => it.Item1.IsInitialState());
		}

		private void MoveNextAutomata(int ch)
		{
			_automata.ForEach(it => it.Item1.MoveNext(ch));
			_automata = _automata.Where(it => !it.Item1.IsError());
		}

		private int CountNextAlive(int ch)
		{
			return _automata.Count(it => !it.Item1.IsNextError(ch));
		}

		private Wrapping<Token<TokenType>?>? GetToken(string str)
		{
			Tuple<Dfa, TokenType?>? tup =
				_automata
				.Where(it => it.Item1.IsAcceptable())
				.FirstOrDefault();

			if (tup == null)
			{
				return null;
			}

			// return if it is token
			Token<TokenType>? token =
				tup.Item2 == null ?
				null :
				GetToken(tup.Item1, str, tup.Item2!.Value);

			return new Wrapping<Token<TokenType>?>(token);
		}

		private static Token<TokenType> GetToken(Dfa dfa, string strToken, TokenType tokenType)
		{
			TokenType type = tokenType;
			string src = strToken;
			return new Token<TokenType>(type, src);
		}

		private static bool IsWhiteSpace(int ch)
		{
			return ch == Ht || ch == Lf || ch == Cr || ch == Sp;
		}

		#endregion
	}
}
