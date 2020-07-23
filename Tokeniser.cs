using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace SimpleC {

	public enum TokenType {
		None = 0,
		NotDefined = 0,
		EndFile = 1,
		Whitespace = 2,
		EndLine = 3,
		Indentation = 4,
		LParen = 5,
		RParen = 6,
		Comma = 7,
		Colon = 8,
		Assign = 9,
		AllTypes = 16,              // for matching groups of all types
		VoidType = AllTypes + 1,
		IntType = AllTypes + 2,
		FloatType = AllTypes + 3,
		DoubleType = AllTypes + 4,
		Commands = 32,              // for matching groups of all commands
		If = Commands + 1,
		Return = Commands + 2,
		Literals = 64,              // for matching groups of all literal values
		IntNum = Literals + 1,
		FloatNum = Literals + 2,
		DoubleNum = Literals + 3,
		AllBinOp = 128,             // for matching groups of all binary operations
		Plus = AllBinOp + 1,
		Minus = AllBinOp + 2,
		Mult = AllBinOp + 3,
		Div = AllBinOp + 4,
		Pow = AllBinOp + 5,
		AllIdentifiers = 256,       // for matching groups of all identifiers
		Identifier = AllIdentifiers + 1,
		Main = AllIdentifiers + 2,
	}



	public class Token {
		public TokenType tokenType { get; }
		public string value { get; }
		public int indent;
		public string filename;
		public int line;
		public int offset;  // from start of line
		public Token(TokenType tokenType, string value, int indent, string filename, int line, int offset) {
			this.tokenType = tokenType;
			this.value = value;
			this.indent = indent;
			this.filename = filename;
			this.line = line;
			this.offset = offset;
		}
	}

	class Tokeniser {
		string alltext;

		public static readonly Dictionary<TokenType, string> TokenNames = new Dictionary<TokenType, string>() {
			{TokenType.NotDefined, "NotDefined"},
			{TokenType.Indentation, "Indentation"},
			{TokenType.EndLine, "EndLine"},
			{TokenType.EndFile, "EndFile"},
			{TokenType.Whitespace, "Whitespace"},
			{TokenType.LParen, "LParen"},
			{TokenType.RParen, "RParen"},
			{TokenType.Comma, "Comma"},
			{TokenType.Colon, "Colon" },
			{TokenType.Assign, "Assign" },
			{TokenType.IntType, "IntType"},
			{TokenType.VoidType, "VoidType"},
			{TokenType.If, "If"},
			{TokenType.Return, "Return"},
			{TokenType.Main, "Main"},
			{TokenType.IntNum, "IntNum"},
			{TokenType.Plus, "Plus"},
			{TokenType.Minus, "Minus"},
			{TokenType.Mult, "Mult"},
			{TokenType.Div, "Div"},
			{TokenType.Identifier, "IdentifierAST"},
		};


		readonly TokenMatch[] tokMatch = {
			// TokenType.NotDefined should be here
			new TokenMatch(TokenType.Indentation, @"\A[\t]+"),
			new TokenMatch(TokenType.EndLine, @"\A[\n]+|\A[\r\n]+"),
			new TokenMatch(TokenType.LParen, @"\A\("),
			new TokenMatch(TokenType.RParen, @"\A\)"),
			new TokenMatch(TokenType.Whitespace, @"\A[ \f\r\v]+"),
			new TokenMatch(TokenType.LParen, @"\A\("),
			new TokenMatch(TokenType.RParen, @"\A\)"),
			new TokenMatch(TokenType.Comma, @"\A\,"),
			new TokenMatch(TokenType.Colon, @"\A\:"),
			new TokenMatch(TokenType.Assign, @"\A="),
			//new TokenMatch(TokenType.IntType, @"\Aint"),
			//new TokenMatch(TokenType.VoidType, @"\Avoid"),
			new TokenMatch(TokenType.If, @"\Aif"),
			new TokenMatch(TokenType.Return, @"\Areturn"),
			//new TokenMatch(TokenType.Main, @"\A[Mm]ain"),
			new TokenMatch(TokenType.IntNum, @"\A\d+"),
			new TokenMatch(TokenType.Plus, @"\A\+"),
			new TokenMatch(TokenType.Minus, @"\A\-"),
			new TokenMatch(TokenType.Mult, @"\A\*"),
			new TokenMatch(TokenType.Div, @"\A/"),
			new TokenMatch(TokenType.Identifier, @"\A[a-zA-Z_]\w*"),
		};



		class TokenMatch {
			public TokenType tokenType { get; }
			public Regex re;

			public TokenMatch(TokenType tokenType, string pattern) {
				this.tokenType = tokenType;
				re = new Regex(pattern, RegexOptions.Compiled);
				Test();
			}
			public Match Re(string inputstr) {
				return re.Match(inputstr);
			}
			void Test() {
				string teststr = "int Main()\r\n\treturn 2\n";
				for (int i = 0; i < teststr.Length; i++) {
					teststr = teststr.Substring(i);
					try {
						Re(teststr);
					} catch {
						ConsoleEx.WriteLine("Regexp is broken: " + TokenNames[tokenType], ConsoleColor.Red);
						//Console.WriteLine($"Regexp is broken: {TokenNames[tokenType]}");
					}
				}
			}
		}



		public Token[] Run(string filename) {
			alltext = File.ReadAllText(filename);
			string inputstr = alltext;
			string baseFilename = Path.GetFileName(filename);
			int longest;
			int longestIndex;
			int index;
			int indent = 0;
			int line = 0;
			int offset = 0;
			List<Token> tokens = new List<Token>();
			while (inputstr.Length > 0) {
				longest = 0;
				longestIndex = -1;
				for (index = 0; index < tokMatch.Length; index++) {
					var match = tokMatch[index].Re(inputstr);
					if (match.Success && match.Length > longest) {
						longest = match.Length;
						longestIndex = index;
					}
				}
				if (longestIndex > -1) {
					TokenType tokType = tokMatch[longestIndex].tokenType;
					if (tokType == TokenType.Indentation) {
						indent = longest;
					} else {
						tokens.Add(new Token(tokType, inputstr.Substring(0, longest), indent, filename, line, offset));
					}
					inputstr = inputstr.Substring(longest);
					offset = offset + longest;
					if (tokType == TokenType.EndLine) {
						line++;
						offset = 0;
					}
				} else {
					Error("Unknown token:", line, offset++);
					longest = 1;
					inputstr = inputstr.Substring(longest);
				}
			}
			tokens.Add(new Token(TokenType.EndFile, string.Empty, 0, filename, line, offset));
			tokens.RemoveAll(item => item.tokenType == TokenType.Whitespace);
			return tokens.ToArray();
		}



		// pretty print token to console
		public void Show(Token[] tokens) {
			ConsoleEx.WriteLine("\n{0}Tokens:", ConsoleColor.White);
			foreach (var token in tokens) {
				ConsoleEx.Write("{0}" + Path.GetFileName(token.filename), ConsoleColor.DarkBlue);
				ConsoleEx.WriteLine(": {0}.{1} {2} {3}{4}: {5}{6}", token.line, token.offset, token.indent, ConsoleColor.Cyan, TokenNames[token.tokenType], ConsoleColor.Blue, token.value);
			}
		}


		// display an error in tokenising
		void Error(string msg, int lineNo, int offset) {
			string GetLine(int lineNo) {
				string[] lines = alltext.Replace("\r", "").Split('\n');
				return lines.Length >= lineNo ? lines[lineNo] : null;
			}
			string lineText = GetLine(lineNo);
			ConsoleEx.WriteLine("{0}{1} line:{2} offset:{3}: {4}{5}{6}{7}{8}{9}", ConsoleColor.Red, msg, lineNo, offset, ConsoleColor.Green, lineText.Substring(0, offset), ConsoleColor.Red, lineText[offset], ConsoleColor.Green, lineText.Substring(offset + 1));
		}

	}



}
