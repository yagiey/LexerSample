namespace LexerSample
{
	public class Token
	{
		public TokenType TokenType { get; private set; }

		public string? StringValue { get; private set; }

		public int IntegerValue { get; private set; }

		public Token(TokenType type) : this(type, string.Empty, 0)
		{
		}

		public Token(TokenType type, string src) : this(type, src, 0) { }

		public Token(TokenType type, string src, int n)
		{
			TokenType = type;

			if (type == TokenType.LiteralInteger
				|| type == TokenType.Identifer)
			{
				StringValue = src;
			}

			if (type == TokenType.LiteralInteger)
			{
				IntegerValue = n;
			}
		}

		public bool Is(TokenType tokenType)
		{
			return TokenType == tokenType;
		}
	}
}
