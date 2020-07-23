using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleC {
	class Program {

		static void Main(string[] args) {
			Token[] tokens;
			List<Token> tokenList = new List<Token>();
			Tokeniser tokeniser = new Tokeniser();
			ConsoleEx.WriteLine("{0}Starting Lex", ConsoleColor.DarkGray);
			foreach (var filename in args)
				tokenList.AddRange(tokeniser.Run(filename));
			tokens = tokenList.ToArray();
			tokeniser.Show(tokens);
			ConsoleEx.WriteLine("\n{0}Parsing", ConsoleColor.DarkGray);
			Parser parser = new Parser(tokens);
			parser.Parse();
			parser.Show();
			BlockAST blockAST = parser.GetAst();
			Compiler compiler = new Compiler();
			string output = compiler.Compile(blockAST);
			Console.WriteLine(output);
		}


	}



}
