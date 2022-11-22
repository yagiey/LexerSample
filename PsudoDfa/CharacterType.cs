namespace PsudoDfa
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
		/// <summary>0-9,a-f,A-F</summary>
		HexDigt,
		/// <summary>a-zA-z_</summary>
		IdentifierHead,
		/// <summary>0-9a-zA-z_</summary>
		IdentifierCharacter,
		/// <summary>neither \n or \r</summary>
		NeitherLfNorCr,
		/// <summary>%x20-21, %x23-5B, %x5D-10FFFF</summary>
		/// <see cref="https://www.rfc-editor.org/info/rfc8259"/>
		JsonUnescapedChar
	}
}
