using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace LexerSample
{
	public class TextFileEnumerator : IEnumerable<Tuple<int, int?>>, IDisposable
	{
		private readonly string _filePath;
		private StreamReader _sr;
		private int _charIndex;
		private bool _isDisposed;
		private const char CR = '\r';
		private const int BufferSize = 512;
		private bool _eof;
		private readonly List<char> _characters;

		public TextFileEnumerator(string filePath)
		{
			_filePath = filePath;
			_characters = new();
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
			_characters.Clear();
			_eof = false;
			_isDisposed = false;
			_sr = new StreamReader(_filePath);
		}

		private int? GetNextChar()
		{
			if (_eof)
			{
				return null;
			}

			//if (_charIndex < 0)
			while (_charIndex < 0)
			{
				_characters.Clear();

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

					for (int i = 0; i < n; i++)
					{
						_characters.Add(buff[i]);
					}

					char tail = buff[^1];
					if (tail == CR || char.IsHighSurrogate(tail))
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

							_characters.Add(Convert.ToChar(n));

							if (n != CR && !char.IsHighSurrogate(Convert.ToChar(n)))
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

			if (_charIndex < 0 || _characters.Count <= 0)
			{
				// EOF
				return null;
			}

			char ch = _characters[_charIndex];
			if (_charIndex == _characters.Count - 1)
			{
				_charIndex = -1;
			}
			else
			{
				_charIndex++;
			}

			if (char.IsHighSurrogate(ch))
			{
				char low = _characters[_charIndex];
				if (_charIndex == _characters.Count - 1)
				{
					_charIndex = -1;
				}
				else
				{
					_charIndex++;
				}
				return char.ConvertToUtf32(ch, low);
			}
			else
			{
				return ch;
			}
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

		public IEnumerator<Tuple<int, int?>> GetEnumerator()
		{
			int? ch1 = GetNextChar();
			if (ch1 == null)
			{
				yield break;
			}
			int? ch2 = GetNextChar();
			yield return Tuple.Create(ch1.Value, ch2);

			while (true)
			{
				if (ch2 == null || !ch2.HasValue)
				{
					yield break;
				}
				int? ch3 = GetNextChar();
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
