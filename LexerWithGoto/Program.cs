using LexerSample;
using System;
using System.Collections.Generic;
using System.IO;

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
			List<string> output = new();
			while ((token = lexer.GetNextToken()) != null)
			{
				string line = $"{token.TokenType}:{token.StringValue}";
				Console.WriteLine(line);
				output.Add(line);
			}
			Console.WriteLine("{0} lines", lexer.CountNewLine() + 1);
			Console.WriteLine("{0} ms", DateTime.Now.Subtract(start).TotalMilliseconds);

			string dirName = Path.GetDirectoryName(filePath)!;
			string fileName = @"result_LexerWithGoto.txt";
			using StreamWriter fw = new(Path.Combine(dirName, fileName));
			string result = string.Join("\r\n", output);
			fw.Write(result);
		}
	}
}
