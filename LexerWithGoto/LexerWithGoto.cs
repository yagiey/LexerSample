using LexerSample;
using System;
using System.Collections.Generic;
using System.Text;

namespace LexerWithGoto
{
	internal class LexerWithGoto : IDisposable
	{
		private readonly TextFileEnumerator _reader;
		private readonly IEnumerator<Tuple<char, char?>> _itorCh;
		private bool _isDisposed;
		private bool _invalidChar;

		private int _nLf;
		private int _nCr;
		private int _nCrLf;

		private const char Ht = '\t';
		private const char Lf = '\n';
		private const char Cr = '\r';
		private const char Sp = ' ';

		public LexerWithGoto(string filePath)
		{
			_isDisposed = false;
			_reader = new TextFileEnumerator(filePath);
			_itorCh = _reader.GetEnumerator();
			_invalidChar = false;
			_nLf = 0;
			_nCr = 0;
			_nCrLf = 0;
		}

		public int CountNewLine()
		{
			return _nLf + _nCr + _nCrLf;
		}

		public Token? GetNextToken()
		{
			StringBuilder sb = new();

		//////////////////////////////////////////////////////////////
		// initial state
		//////////////////////////////////////////////////////////////
		#region initial state
		state0000:
			{
				if (_invalidChar)
				{
					_invalidChar = false;
					string errMsg = $"invalid character '{_itorCh.Current.Item1}' found. line: {CountNewLine() + 1}";
					Console.WriteLine(errMsg);

					while (true)
					{
						if (!_itorCh.MoveNext())
						{
							// EOF
							return null;
						}

						char? next = _itorCh.Current.Item2;
						if (next != null && (next.Value == Cr || next.Value == Lf))
						{
							// skip until next LF or CR
							break;
						}
					}
				}

				char ch;
				while (true)
				{
					if (!_itorCh.MoveNext())
					{
						// EOF
						return null;
					}

					ch = _itorCh.Current.Item1;
					if (!IsTabOrSpace(ch))
					{
						break;
					}
				}

				if (ch == 'b') { sb.Append(ch); goto state0001; }
				else if (ch == 'c') { sb.Append(ch); goto state0006; }
				else if (ch == 'd') { sb.Append(ch); goto state0011; }
				else if (ch == 'e') { sb.Append(ch); goto state0013; }
				else if (ch == 'f') { sb.Append(ch); goto state0016; }
				else if (ch == 'i') { sb.Append(ch); goto state0024; }
				else if (ch == 'r') { sb.Append(ch); goto state0026; }
				else if (ch == 't') { sb.Append(ch); goto state0032; }
				else if (ch == 'v') { sb.Append(ch); goto state0036; }
				else if (ch == 'w') { sb.Append(ch); goto state0039; }
				else if (ch == '(') { sb.Append(ch); goto state1044; }
				else if (ch == ')') { sb.Append(ch); goto state1045; }
				else if (ch == '*') { sb.Append(ch); goto state1046; }
				else if (ch == '+') { sb.Append(ch); goto state1047; }
				else if (ch == ',') { sb.Append(ch); goto state1048; }
				else if (ch == '-') { sb.Append(ch); goto state1049; }
				else if (ch == '.') { sb.Append(ch); goto state1050; }
				else if (ch == '/') { sb.Append(ch); goto state1051; }
				else if (ch == ':') { sb.Append(ch); goto state1052; }
				else if (ch == ';') { sb.Append(ch); goto state1054; }
				else if (ch == '<') { sb.Append(ch); goto state1055; }
				else if (ch == '=') { sb.Append(ch); goto state1058; }
				else if (ch == '>') { sb.Append(ch); goto state1059; }
				else if (ch == Lf) { goto state1061; }
				else if (ch == Cr) { goto state1062; }
				else if (IsCharacterForIdHead(ch)) { sb.Append(ch); goto state5001; }
				else if (ch == '0') { sb.Append(ch); goto state5003; }
				else if (IsDigit(ch)) { sb.Append(ch); goto state5004; }
				else { goto error; }
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// begin
		//////////////////////////////////////////////////////////////
		#region begin
		state0001:
			{
				//////////////////////////////////
				// decition by next character
				//  * EOF
				//  * punctuation marks
				//  * characters not for identifier
				//////////////////////////////////
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					// EOF
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					// punctuation mark or character not for identifier
					return new Token(TokenType.Identifer, sb.ToString());
				}

				//////////////////////////////////
				// decition by current character
				//////////////////////////////////
				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'e')
				{
					goto state0002;
				}
				else// if (IsCharacterForID(ch))
				{
					goto state5001;
				}
			}

		state0002:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'g')
				{
					goto state0003;
				}
				else
				{
					goto state5001;
				}
			}

		state0003:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'i')
				{
					goto state0004;
				}
				else
				{
					goto state5001;
				}
			}

		state0004:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'n')
				{
					goto state0005;
				}
				else
				{
					goto state5001;
				}
			}

		state0005:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.KeywordBegin);
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.KeywordBegin);
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				if (IsCharacterForId(ch))
				{
					// identifier
					sb.Append(ch);
					goto state5001;
				}
				else
				{
					return new Token(TokenType.KeywordBegin);
				}
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// const
		//////////////////////////////////////////////////////////////
		#region const
		state0006:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'o')
				{
					goto state0007;
				}
				else
				{
					goto state5001;
				}
			}

		state0007:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'n')
				{
					goto state0008;
				}
				else
				{
					goto state5001;
				}
			}

		state0008:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 's')
				{
					goto state0009;
				}
				else
				{
					goto state5001;
				}
			}

		state0009:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 't')
				{
					goto state0010;
				}
				else
				{
					goto state5001;
				}
			}

		state0010:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.KeywordConst);
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.KeywordConst);
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				if (IsCharacterForId(ch))
				{
					sb.Append(ch);
					goto state5001;
				}
				else
				{
					return new Token(TokenType.KeywordConst);
				}
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// do
		//////////////////////////////////////////////////////////////
		#region do
		state0011:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'o')
				{
					goto state0012;
				}
				else
				{
					goto state5001;
				}
			}

		state0012:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.KeywordDo);
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.KeywordDo);
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				if (IsCharacterForId(ch))
				{
					sb.Append(ch);
					goto state5001;
				}
				else
				{
					return new Token(TokenType.KeywordDo);
				}
			}

		#endregion

		//////////////////////////////////////////////////////////////
		// end
		//////////////////////////////////////////////////////////////
		#region end
		state0013:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'n')
				{
					goto state0014;
				}
				else
				{
					goto state5001;
				}
			}

		state0014:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'd')
				{
					goto state0015;
				}
				else
				{
					goto state5001;
				}
			}

		state0015:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.KeywordEnd);
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.KeywordEnd);
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				if (IsCharacterForId(ch))
				{
					sb.Append(ch);
					goto state5001;
				}
				else
				{
					return new Token(TokenType.KeywordEnd);
				}
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// function
		//////////////////////////////////////////////////////////////
		#region function
		state0016:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'u')
				{
					goto state0017;
				}
				else
				{
					goto state5001;
				}
			}

		state0017:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'n')
				{
					goto state0018;
				}
				else
				{
					goto state5001;
				}
			}

		state0018:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'c')
				{
					goto state0019;
				}
				else
				{
					goto state5001;
				}
			}

		state0019:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 't')
				{
					goto state0020;
				}
				else
				{
					goto state5001;
				}
			}

		state0020:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'i')
				{
					goto state0021;
				}
				else
				{
					goto state5001;
				}
			}

		state0021:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'o')
				{
					goto state0022;
				}
				else
				{
					goto state5001;
				}
			}

		state0022:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'n')
				{
					goto state0023;
				}
				else
				{
					goto state5001;
				}
			}

		state0023:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.KeywordFunction);
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.KeywordFunction);
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				if (IsCharacterForId(ch))
				{
					sb.Append(ch);
					goto state5001;
				}
				else
				{
					return new Token(TokenType.KeywordFunction);
				}
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// if
		//////////////////////////////////////////////////////////////
		#region if
		state0024:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'f')
				{
					goto state0025;
				}
				else
				{
					goto state5001;
				}
			}

		state0025:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.KeywordIf);
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.KeywordIf);
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				if (IsCharacterForId(ch))
				{
					sb.Append(ch);
					goto state5001;
				}
				else
				{
					return new Token(TokenType.KeywordIf);
				}
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// return
		//////////////////////////////////////////////////////////////
		#region return
		state0026:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'e')
				{
					goto state0027;
				}
				else
				{
					goto state5001;
				}
			}

		state0027:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 't')
				{
					goto state0028;
				}
				else
				{
					goto state5001;
				}
			}

		state0028:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'u')
				{
					goto state0029;
				}
				else
				{
					goto state5001;
				}
			}

		state0029:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'r')
				{
					goto state0030;
				}
				else
				{
					goto state5001;
				}
			}

		state0030:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'n')
				{
					goto state0031;
				}
				else
				{
					goto state5001;
				}
			}

		state0031:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.KeywordReturn);
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.KeywordReturn);
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				if (IsCharacterForId(ch))
				{
					sb.Append(ch);
					goto state5001;
				}
				else
				{
					return new Token(TokenType.KeywordReturn);
				}
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// then
		//////////////////////////////////////////////////////////////
		#region then
		state0032:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'h')
				{
					goto state0033;
				}
				else
				{
					goto state5001;
				}
			}

		state0033:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'e')
				{
					goto state0034;
				}
				else
				{
					goto state5001;
				}
			}

		state0034:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'n')
				{
					goto state0035;
				}
				else
				{
					goto state5001;
				}
			}

		state0035:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.KeywordThen);
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.KeywordThen);
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				if (IsCharacterForId(ch))
				{
					sb.Append(ch);
					goto state5001;
				}
				else
				{
					return new Token(TokenType.KeywordThen);
				}
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// var
		//////////////////////////////////////////////////////////////
		#region var
		state0036:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'a')
				{
					goto state0037;
				}
				else
				{
					goto state5001;
				}
			}

		state0037:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'r')
				{
					goto state0038;
				}
				else
				{
					goto state5001;
				}
			}

		state0038:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.KeywordVar);
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.KeywordVar);
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				if (IsCharacterForId(ch))
				{
					sb.Append(ch);
					goto state5001;
				}
				else
				{
					return new Token(TokenType.KeywordVar);
				}
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// while
		//////////////////////////////////////////////////////////////
		#region while
		state0039:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'h')
				{
					goto state0040;
				}
				else
				{
					goto state5001;
				}
			}

		state0040:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'i')
				{
					goto state0041;
				}
				else
				{
					goto state5001;
				}
			}

		state0041:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'l')
				{
					goto state0042;
				}
				else
				{
					goto state5001;
				}
			}

		state0042:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				sb.Append(ch);
				if (ch == 'e')
				{
					goto state0043;
				}
				else
				{
					goto state5001;
				}
			}

		state0043:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					return new Token(TokenType.KeywordWhile);
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.KeywordWhile);
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				if (IsCharacterForId(ch))
				{
					sb.Append(ch);
					goto state5001;
				}
				else
				{
					return new Token(TokenType.KeywordWhile);
				}
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// (
		//////////////////////////////////////////////////////////////
		#region (
		state1044:
			return new Token(TokenType.PunctuationLeftParen);
		#endregion

		//////////////////////////////////////////////////////////////
		// )
		//////////////////////////////////////////////////////////////
		#region )
		state1045:
			return new Token(TokenType.PunctuationRightParen);
		#endregion

		//////////////////////////////////////////////////////////////
		// *
		//////////////////////////////////////////////////////////////
		#region *
		state1046:
			return new Token(TokenType.PunctuationAsterisk);
		#endregion

		//////////////////////////////////////////////////////////////
		// +, integer number
		//////////////////////////////////////////////////////////////
		#region +
		state1047:
			{
				char? next = _itorCh.Current.Item2;
				if (next != null)
				{
					if (next.Value == '0')
					{
						goto state5003;
					}
					else if (IsDigit(next.Value))
					{
						goto state5004;
					}
				}

				return new Token(TokenType.PunctuationPlus);
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// ,
		//////////////////////////////////////////////////////////////
		#region ,
		state1048:
			return new Token(TokenType.PunctuationComma);
		#endregion

		//////////////////////////////////////////////////////////////
		// -
		//////////////////////////////////////////////////////////////
		#region -
		state1049:
			{
				char? next = _itorCh.Current.Item2;
				if (next != null)
				{
					if (next.Value == '0')
					{
						goto state5003;
					}
					else if (IsDigit(next.Value))
					{
						goto state5004;
					}
				}

				return new Token(TokenType.PunctuationMinus);
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// .
		//////////////////////////////////////////////////////////////
		#region .
		state1050:
			return new Token(TokenType.PunctuationPeriod);
		#endregion

		//////////////////////////////////////////////////////////////
		// /, line comment, block comment
		//////////////////////////////////////////////////////////////
		#region /
		state1051:
			{
				char? next = _itorCh.Current.Item2;
				if (next != null)
				{
					if (next.Value == '/')
					{
						// line comment
						goto state5006;
					}
					else if (next.Value == '*')
					{
						// block comment
						goto state5008;
					}
				}
				return new Token(TokenType.PunctuationSlash);
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// :=
		//////////////////////////////////////////////////////////////
		#region :=
		state1052:
			{
				char? next = _itorCh.Current.Item2;
				if (next != null && next.HasValue && next.Value == '=')
				{
					goto state1053;
				}
				else
				{
					goto error;
				}
			}

		state1053:
			{
				_itorCh.MoveNext();
				return new Token(TokenType.PunctuationColonEqual);
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// ;
		//////////////////////////////////////////////////////////////
		#region ;
		state1054:
			return new Token(TokenType.PunctuationSemicolon);
		#endregion

		//////////////////////////////////////////////////////////////
		// <, <>, <=
		//////////////////////////////////////////////////////////////
		#region < <> <=
		state1055:
			{
				char? next = _itorCh.Current.Item2;
				if (next != null && next.HasValue)
				{
					if (next.Value == '>')
					{
						goto state1056;
					}
					else if (next.Value == '=')
					{
						goto state1057;
					}
				}
				return new Token(TokenType.PunctuationLT);
			}

		state1056:
			_itorCh.MoveNext();
			return new Token(TokenType.PunctuationLTGT);

		state1057:
			_itorCh.MoveNext();
			return new Token(TokenType.PunctuationLE);
		#endregion

		//////////////////////////////////////////////////////////////
		// =
		//////////////////////////////////////////////////////////////
		#region =
		state1058:
			return new Token(TokenType.PunctuationEqual);
		#endregion

		//////////////////////////////////////////////////////////////
		// >
		//////////////////////////////////////////////////////////////
		#region > >=
		state1059:
			{
				char? next = _itorCh.Current.Item2;
				if (next != null && next.HasValue && next.Value == '=')
				{
					goto state1060;
				}
				return new Token(TokenType.PunctuationGT);
			}
		state1060:
			_itorCh.MoveNext();
			return new Token(TokenType.PunctuationGE);
		#endregion

		//////////////////////////////////////////////////////////////
		// LF - not a token
		//////////////////////////////////////////////////////////////
		#region LF
		state1061:
			_nLf++;
			goto state0000;
		#endregion

		//////////////////////////////////////////////////////////////
		// CR, CRLF - not a token
		//////////////////////////////////////////////////////////////
		#region CR CRLF
		state1062:
			{
				char? next = _itorCh.Current.Item2;
				if (next != null && next.Value == Lf)
				{
					goto state1063;
				}

				_nCr++;
				goto state0000;
			}

		state1063:
			_nCrLf++;
			_itorCh.MoveNext();
			goto state0000;
		#endregion

		//////////////////////////////////////////////////////////////
		// identifier
		//////////////////////////////////////////////////////////////
		#region identifier
		state5001:
			{
				char? next = _itorCh.Current.Item2;
				if (next == null)
				{
					// EOF
					return new Token(TokenType.Identifer, sb.ToString());
				}
				else if (IsCharacterForOperator(next.Value)
					|| !IsCharacterForId(next.Value))
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}

				_itorCh.MoveNext();
				char ch = _itorCh.Current.Item1;
				if (IsCharacterForId(ch))
				{
					sb.Append(ch);
					goto state5001;
				}
				else
				{
					return new Token(TokenType.Identifer, sb.ToString());
				}
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// integer number
		//////////////////////////////////////////////////////////////
		#region integer number
		state5003:
			return new Token(TokenType.LiteralInteger, sb.ToString(), 0);

		state5004:
			{
				char? next = _itorCh.Current.Item2;
				if (next != null && IsDigit(next.Value))
				{
					_itorCh.MoveNext();
					sb.Append(_itorCh.Current.Item1);
					goto state5004;
				}
				else
				{
					return new Token(TokenType.LiteralInteger, sb.ToString(), int.Parse(sb.ToString()));
				}
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// line comment - not a token
		//////////////////////////////////////////////////////////////
		#region line comment
		state5006:
			{
				if (!_itorCh.MoveNext())
				{
					// EOF
					return null;
				}

				char ch = _itorCh.Current.Item1;
				sb.Append(ch);

				char? next = _itorCh.Current.Item2;
				if (next != null && (next.Value == Lf || next.Value == Cr))
				{
					// end of comment
					sb.Clear();
					goto state0000;
				}
				else
				{
					goto state5006;
				}
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// block comment - not a token
		//////////////////////////////////////////////////////////////
		#region block comment
		state5008:
			{
				if (!_itorCh.MoveNext())
				{
					// EOF
					// unclosed block comment
					string errMsg = $"unclosed block comment found. line: {CountNewLine() + 1}";
					Console.WriteLine(errMsg);

					return null;
				}

				char ch = _itorCh.Current.Item1;
				sb.Append(ch);

				if (ch == Cr)
				{
					goto state5008_cr;
				}
				else if (ch == Lf)
				{
					_nLf++;
				}
			}
		state5008_tail:
			{
				char? next = _itorCh.Current.Item2;
				if (next != null && next.Value == '*')
				{
					goto state5009;
				}
				else
				{
					goto state5008;
				}
			}

		state5008_cr:
			{
				char? next = _itorCh.Current.Item2;
				if (next != null && next.Value == Lf)
				{
					_itorCh.MoveNext();
					_nCrLf++;
				}
				else
				{
					_nCr++;
				}
				goto state5008_tail;
			}

		state5009:
			{
				_itorCh.MoveNext();

				char ch = _itorCh.Current.Item1;
				sb.Append(ch);

				char? next = _itorCh.Current.Item2;
				if (next != null && next.Value == '/')
				{
					goto state5010;
				}
				else
				{
					goto state5008;
				}
			}

		state5010:
			{
				_itorCh.MoveNext();
				sb.Append(_itorCh.Current.Item1);

				// end of comment
				sb.Clear();
				goto state0000;
			}
		#endregion

		//////////////////////////////////////////////////////////////
		// invalid character - not a token
		//////////////////////////////////////////////////////////////
		error:
			{
				_invalidChar = true;
				goto state0000;
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

		private static bool IsDigit(char ch)
		{
			return char.IsDigit(ch);
		}

		private static bool IsLower(char ch)
		{
			return 'a' <= ch && ch <= 'z';
		}

		private static bool IsUpper(char ch)
		{
			return 'A' <= ch && ch <= 'Z';
		}

		private static bool IsTabOrSpace(char ch)
		{
			return ch == Ht || ch == Sp;
		}

		private static bool IsCharacterForId(char ch)
		{
			return IsDigit(ch) || IsLower(ch) || IsUpper(ch) || ch == '_';
		}

		private static bool IsCharacterForIdHead(char ch)
		{
			return IsLower(ch) || IsUpper(ch) || ch == '_';
		}

		private static bool IsCharacterForOperator(char ch)
		{
			return
				ch == '('
					|| ch == ')'
					|| ch == '*'
					|| ch == '+'
					|| ch == ','
					|| ch == '-'
					|| ch == '.'
					|| ch == '/'
					|| ch == ':'
					|| ch == ';'
					|| ch == '<'
					|| ch == '='
					|| ch == '>';
		}

		#endregion
	}
}
