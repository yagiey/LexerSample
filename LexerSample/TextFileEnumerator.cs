using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LexerSample
{
	public class TextFileEnumerator : IEnumerable<Tuple<char, char?>>, IDisposable
	{
		private readonly string _filePath;
		private StreamReader _sr;
		private int _charIndex;
		private bool _isDisposed;
		private const char CR = '\r';
		private const int BufferSize = 512;
		private bool _eof;
		private readonly StringBuilder _sb;

		public TextFileEnumerator(string filePath)
		{
			_filePath = filePath;
			_sb = new();
			_sr = null!;
			_isDisposed = true;
			Reset();
		}

		public void Reset()
		{
			if (!_isDisposed)
			{
				_sr.Dispose();
			}

			_charIndex = -1;
			_sb.Clear();
			_eof = false;
			_isDisposed = false;
			_sr = new StreamReader(_filePath);
		}

		private char? GetNextChar()
		{
			if (_eof)
			{
				return null;
			}

			//if (_charIndex < 0)
			while (_charIndex < 0)
			{
				_sb.Clear();

				char[] buff;
				int n;
				while (true)
				{
					buff = new char[BufferSize];
					n = _sr.Read(buff, 0, BufferSize);

					if (n == 0)
					{
						_eof = true;
						break;
					}

					_sb.Append(buff, 0, n);

					if (buff[^1] == CR)
					{
						bool found = false;
						while (true)
						{
							n = _sr.Read();
							if (n < 0)
							{
								_eof = true;
								break;
							}

							_sb.Append(Convert.ToChar(n));

							if (n != CR)
							{
								found = true;
								break;
							}
						}
						if (found)
						{
							_charIndex = 0;
							break;
						}
					}
					else
					{
						_charIndex = 0;
						break;
					}
				}

				if (_eof)
				{
					break;
				}
			}

			if (_charIndex < 0 || _sb.Length <= 0)
			{
				// EOF
				return null;
			}

			char ch = _sb[_charIndex];
			if (_charIndex == _sb.Length - 1)
			{
				_charIndex = -1;
			}
			else
			{
				_charIndex++;
			}

			return ch;
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}

			_sr.Dispose();
			_isDisposed = true;
		}

		public IEnumerator<Tuple<char, char?>> GetEnumerator()
		{
			char? ch1 = GetNextChar();
			if (ch1 == null)
			{
				yield break;
			}
			char? ch2 = GetNextChar();
			yield return Tuple.Create(ch1.Value, ch2);

			while (true)
			{
				if (ch2 == null || !ch2.HasValue)
				{
					yield break;
				}
				char? ch3 = GetNextChar();
				yield return Tuple.Create(ch2.Value, ch3);
				ch2 = ch3;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
