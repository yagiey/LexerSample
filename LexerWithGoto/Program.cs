using LexerSample;
using System;

namespace LexerWithGoto
{
	internal class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				return;
			}

			Console.WriteLine("////////////////////");
			Console.WriteLine("// LexerWithGoto");
			Console.WriteLine("////////////////////");
			DateTime start = DateTime.Now;
			string filePath = args[0];
			using var lexer = new LexerWithGoto(filePath);
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
