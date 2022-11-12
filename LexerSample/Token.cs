using System;

namespace LexerSample
{
	public class Token<T> where T : struct
	{
		public T TokenType { get; private set; }

		public string? StringValue { get; private set; }

		public int IntegerValue { get; private set; }

		public Token(T type) : this(type, string.Empty, 0)
		{
		}

		public Token(T type, string src) : this(type, src, 0) { }

		public Token(T type, string src, int n)
		{
			TokenType = type;
			StringValue = src;
			IntegerValue = n;
		}

		public bool Is(T tokenType)
		{
			return TokenType.Equals(tokenType);
		}
	}
}
