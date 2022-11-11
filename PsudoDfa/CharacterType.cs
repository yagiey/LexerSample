namespace LexerWithDFA
{
	public enum CharacterType : byte
	{
		/// <summary>literal character</summary>
		Literal,
		/// <summary>not equal</summary>
		NotEqual,
		/// <summary>any character but LF(ASCII: 0x0A)</summary>
		AnyCharacter,
		/// <summary>0-9</summary>
		Digit,
		/// <summary>1-9</summary>
		Digit1_9,
		/// <summary>a-zA-z_</summary>
		IdentifierHead,
		/// <summary>0-9a-zA-z_</summary>
		IdentifierCharacter,
		/// <summary>neither \n or \r</summary>
		NeitherLfNorCr,
	}
}
