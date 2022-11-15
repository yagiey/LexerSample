using LexerSample;
using PsudoDfa;
using System;
using System.Collections.Generic;

namespace LexerWithDFA
{
	internal class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				return;
			}

			// tokenInfo
			// tuple of automata and T
			const bool IgnoreCase = false;
			IEnumerable<Tuple<Dfa, TokenType?>> tokenInfos = new Tuple<Dfa, TokenType?>[]
			{
				Tuple.Create(Dfa.GenerateLineComment(), (TokenType?)null),
				Tuple.Create(Dfa.GenerateBlockComment(), (TokenType?)null),
				Tuple.Create(Dfa.GenerateIntegerNumber(), (TokenType?)TokenType.LiteralInteger),
				Tuple.Create(Dfa.GenerateString("begin", IgnoreCase), (TokenType?)TokenType.KeywordBegin),
				Tuple.Create(Dfa.GenerateString("const", IgnoreCase), (TokenType?)TokenType.KeywordConst),
				Tuple.Create(Dfa.GenerateString("do", IgnoreCase), (TokenType?)TokenType.KeywordDo),
				Tuple.Create(Dfa.GenerateString("end", IgnoreCase), (TokenType?)TokenType.KeywordEnd),
				Tuple.Create(Dfa.GenerateString("function", IgnoreCase), (TokenType?)TokenType.KeywordFunction),
				Tuple.Create(Dfa.GenerateString("if", IgnoreCase), (TokenType?)TokenType.KeywordIf),
				Tuple.Create(Dfa.GenerateString("return", IgnoreCase), (TokenType?)TokenType.KeywordReturn),
				Tuple.Create(Dfa.GenerateString("then", IgnoreCase), (TokenType?)TokenType.KeywordThen),
				Tuple.Create(Dfa.GenerateString("var", IgnoreCase), (TokenType?)TokenType.KeywordVar),
				Tuple.Create(Dfa.GenerateString("while", IgnoreCase), (TokenType?)TokenType.KeywordWhile),
				Tuple.Create(Dfa.GenerateString(";", IgnoreCase), (TokenType?)TokenType.PunctuationSemicolon),
				Tuple.Create(Dfa.GenerateString(".", IgnoreCase), (TokenType?)TokenType.PunctuationPeriod),
				Tuple.Create(Dfa.GenerateString(",", IgnoreCase), (TokenType?)TokenType.PunctuationComma),
				Tuple.Create(Dfa.GenerateString("(", IgnoreCase), (TokenType?)TokenType.PunctuationLeftParen),
				Tuple.Create(Dfa.GenerateString(")", IgnoreCase), (TokenType?)TokenType.PunctuationRightParen),
				Tuple.Create(Dfa.GenerateString(":=", IgnoreCase), (TokenType?)TokenType.PunctuationColonEqual),
				Tuple.Create(Dfa.GenerateString("=", IgnoreCase), (TokenType?)TokenType.PunctuationEqual),
				Tuple.Create(Dfa.GenerateString("<>", IgnoreCase), (TokenType?)TokenType.PunctuationLTGT),
				Tuple.Create(Dfa.GenerateString("<", IgnoreCase), (TokenType?)TokenType.PunctuationLT),
				Tuple.Create(Dfa.GenerateString("<=", IgnoreCase), (TokenType?)TokenType.PunctuationLE),
				Tuple.Create(Dfa.GenerateString(">", IgnoreCase), (TokenType?)TokenType.PunctuationGT),
				Tuple.Create(Dfa.GenerateString(">=", IgnoreCase), (TokenType?)TokenType.PunctuationGE),
				Tuple.Create(Dfa.GenerateString("+", IgnoreCase), (TokenType?)TokenType.PunctuationPlus),
				Tuple.Create(Dfa.GenerateString("-", IgnoreCase), (TokenType?)TokenType.PunctuationMinus),
				Tuple.Create(Dfa.GenerateString("*", IgnoreCase), (TokenType?)TokenType.PunctuationAsterisk),
				Tuple.Create(Dfa.GenerateString("/", IgnoreCase), (TokenType?)TokenType.PunctuationSlash),
				Tuple.Create(Dfa.GenerateIdentifier(), (TokenType?)TokenType.Identifer),
			};

			Console.WriteLine("////////////////////");
			Console.WriteLine("// LexerWithDFA");
			Console.WriteLine("////////////////////");
			DateTime start = DateTime.Now;
			string filePath = args[0];
			using var lexer = new LexerWithDFA<TokenType>(filePath, tokenInfos);

			Token<TokenType>? token;
			while ((token = lexer.GetNextToken()) != null)
			{
				Console.WriteLine("{0}:{1}", token.TokenType, token.StringValue);
			}
			Console.WriteLine("{0} lines", lexer.CountNewLine() + 1);
			Console.WriteLine("{0} ms", DateTime.Now.Subtract(start).TotalMilliseconds);
		}
	}
}
