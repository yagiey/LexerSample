using System;

namespace PsudoDfa
{
	public struct Character : IComparable<Character>, IEquatable<Character>
	{
		private const char HT = '\t';
		private const char LF = '\n';
		private const char CR = '\r';
		private const char SP = ' ';

		public CharacterType CharacterType { get; private set; }
		public int Value { get; private set; }

		/// <summary>not equal</summary>
		public static readonly Character NotEqual;
		/// <summary>any character but LF</summary>
		public static readonly Character AnyCharacter;
		/// <summary>digit</summary>
		public static readonly Character Digit;
		/// <summary>digit 1 - 9</summary>
		public static readonly Character Digit1_9;
		/// <summary>character for identifier head</summary>
		public static readonly Character IdentifierHead;
		/// <summary>character for identifier</summary>
		public static readonly Character IdentifierCharacter;
		/// <summary>neither LF nor CR</summary>
		public static readonly Character NeitherLfNorCr;

		static Character()
		{
			NotEqual = new Character(CharacterType.NotEqual);
			AnyCharacter = new Character(CharacterType.AnyCharacter);
			Digit = new Character(CharacterType.Digit);
			Digit1_9 = new Character(CharacterType.Digit1_9);
			IdentifierHead = new Character(CharacterType.IdentifierHead);
			IdentifierCharacter = new Character(CharacterType.IdentifierCharacter);
			NeitherLfNorCr = new Character(CharacterType.NeitherLfNorCr);
		}

		private Character(CharacterType charType) : this(charType, char.MinValue) { }

		public Character(CharacterType charType, char ch)
		{
			CharacterType = charType;
			Value = ch;
		}

		public Character(CharacterType charType, int ch)
		{
			CharacterType = charType;
			Value = ch;
		}

		public static bool IsLowerCase(int ch)
		{
			return 'a' <= ch && ch <= 'z';
		}

		public static bool IsUpperCase(int ch)
		{
			return 'A' <= ch && ch <= 'Z';
		}

		private static bool IsWhiteSpace(int ch)
		{
			return IsHtabOrSpace(ch) || ch == LF || ch == CR;
		}

		private static bool IsHtabOrSpace(int ch)
		{
			return ch == HT || ch == SP;
		}

		public bool IsLiteralCharacter()
		{
			return CharacterType == CharacterType.Literal;
		}

		public bool Match(int ch)
		{
			if (CharacterType == CharacterType.Literal)
			{
				return ch == Value;
			}
			else if (CharacterType == CharacterType.NotEqual)
			{
				return ch != Value;
			}
			else if (CharacterType == CharacterType.AnyCharacter)
			{
				return ch != LF;
			}
			else if (CharacterType == CharacterType.Digit)
			{
				return '0' <= ch && ch <= '9';
			}
			else if (CharacterType == CharacterType.Digit1_9)
			{
				return '1' <= ch && ch <= '9';
			}
			else if (CharacterType == CharacterType.IdentifierHead)
			{
				return IsLowerCase(ch) || IsUpperCase(ch) || ch == '_';
			}
			else if (CharacterType == CharacterType.IdentifierCharacter)
			{
				return ('0' <= ch && ch <= '9') || IsLowerCase(ch) || IsUpperCase(ch) || ch == '_';
			}
			else if (CharacterType == CharacterType.NeitherLfNorCr)
			{
				return ch != LF && ch != CR;
			}
			return false;
		}

		public int CompareTo(Character other)
		{
			int n = CharacterType.CompareTo(other.CharacterType);
			if (n == 0)
			{
				if (CharacterType == CharacterType.Literal || CharacterType == CharacterType.NotEqual)
				{
					return Value.CompareTo(other.Value);
				}
				else
				{
					return 0;
				}
			}
			else
			{
				return n;
			}
		}

		public bool Equals(Character other)
		{
			if (CharacterType == other.CharacterType)
			{
				if (CharacterType == CharacterType.Literal || CharacterType == CharacterType.NotEqual)
				{
					return Value == other.Value;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		public override bool Equals(object? other)
		{
			if (other is null || other is not Character)
			{
				return false;
			}
			Character o = (Character)other;
			return Equals(o);
		}

		public static bool operator ==(Character lhs, Character rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Character lhs, Character rhs)
		{
			bool eq = lhs == rhs;
			return !eq;
		}

		public override int GetHashCode()
		{
			byte[] ch = BitConverter.GetBytes(Value);

			byte[] bytes = new byte[4];
			bytes[0] = (byte)CharacterType;
			bytes[1] = ch[1];
			bytes[2] = ch[2];
			bytes[3] = ch[3];

			return BitConverter.ToInt32(bytes);
		}

		public override string? ToString()
		{
			if (CharacterType == CharacterType.Literal)
			{
				return Value.ToString();
			}
			else if (CharacterType == CharacterType.NotEqual)
			{
				return $"<NOT {Value}>";
			}
			else if (CharacterType == CharacterType.AnyCharacter)
			{
				return "<AnyCharacter>";
			}
			else if (CharacterType == CharacterType.Digit)
			{
				return "<Digit>";
			}
			else if (CharacterType == CharacterType.Digit1_9)
			{
				return "<Digit1_9>";
			}
			else if (CharacterType == CharacterType.IdentifierHead)
			{
				return "<IdentifierHead>";
			}
			else if (CharacterType == CharacterType.IdentifierCharacter)
			{
				return "<IdentifierCharacter>";
			}
			else if (CharacterType == CharacterType.NeitherLfNorCr)
			{
				return "<NeitherLfNorCr>";
			}
			else
			{
				return null;
			}
		}

		public static int ToLower(int ch)
		{
			int diff = ch - 'A';
			return Convert.ToChar('a' + diff);
		}

		public static int ToUpper(int ch)
		{
			int diff = ch - 'a';
			return Convert.ToChar('A' + diff);
		}
	}
}
