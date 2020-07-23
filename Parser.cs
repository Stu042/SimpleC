using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleC {

	// AST visitor class
	public interface ASTVisitor {
		public virtual object DoForAST(AST ast, object options = null) {
			return options;
		}
		public virtual object DoForBinOp(BinOpAST bo, object options = null) {
			return options;
		}
		public virtual object DoForNum(NumAST num, object options = null) {
			return options;
		}
		public virtual object DoForRetCmd(RetCmdAST rc, object options = null) {
			return options;
		}
		public virtual object DoForIdentifier(IdentifierAST id, object options = null) {
			return options;
		}
		public virtual object DoForBlock(BlockAST stmnt, object options = null) {
			return options;
		}
		public virtual object DoForFuncDef(FuncDefAST fd, object options = null) {
			return options;
		}
		public virtual object DoForObjDef(ObjDefAST od, object options = null) {
			return options;
		}
		public virtual object DoForObjInst(ObjInstAST od, object options = null) {
			return options;
		}
		public virtual object DoForType(TypeAST t, object options = null) {
			return options;
		}
		public virtual object DoForArg(ArgAST a, object options = null) {
			return options;
		}
		public virtual object DoForExpr(ExprAST a, object options = null) {
			return options;
		}
	}



	// base class for all ast's
	public interface AST {
		public virtual object Accept(ASTVisitor visitor, object options) {
			return options;
		}

	}


	public class BinOpAST:AST {
		public Token token;
		public string op;
		public AST left;
		public AST right;
		public BinOpAST(Token token, AST left, AST right) {
			this.token = token;
			this.left = left;
			this.op = token.value;
			this.right = right;
		}
		public object Accept(ASTVisitor visitor, object options) {
			return visitor.DoForBinOp(this, options);
		}
	}

	public class NumAST:AST {
		public Token token;
		public string value;
		TokenType valType;
		public NumAST(Token token, string value, TokenType valType) {
			this.token = token;
			this.value = value;
			this.valType = valType;
		}
		public object Accept(ASTVisitor visitor, object options = null) {
			return visitor.DoForNum(this, options);
		}
	}

	public class ExprAST:AST {
		public Token token;
		public AST[] body;
		public ExprAST(Token token, AST[] body) {
			this.token = token;
			this.body = body;
		}
		public object Accept(ASTVisitor visitor, object options = null) {
			return visitor.DoForExpr(this, options);
		}
	}

	public class RetCmdAST:AST {
		public Token token;
		public ExprAST expression;
		public RetCmdAST(Token token, ExprAST expression) {
			this.token = token;
			this.expression = expression;
		}
		public object Accept(ASTVisitor visitor, object options = null) {
			return visitor.DoForRetCmd(this, options);
		}
	}

	public class IdentifierAST:AST {
		public Token token;
		public string name;
		public IdentifierAST(Token token, string name) {
			this.token = token;
			this.name = name;
		}
		public object Accept(ASTVisitor visitor, object options = null) {
			return visitor.DoForIdentifier(this, options);
		}
	}

	public class BlockAST:AST {
		public Token token;
		public AST[] stmts;
		public BlockAST(Token token, AST[] stmts) {
			this.token = token;
			this.stmts = stmts;
		}
		public object Accept(ASTVisitor visitor, object options = null) {
			return visitor.DoForBlock(this, options);
		}
	}

	public class FuncDefAST:AST {
		public Token token;
		public string name;
		public TypeAST retType;    // int, bool etc
		public ArgAST[] args;
		public BlockAST stmt;
		public FuncDefAST(Token token, TypeAST type, ArgAST[] args, BlockAST stmt) {
			this.token = token;
			this.name = token.value;
			this.retType = type;
			this.args = args;
			this.stmt = stmt;
		}
		public object Accept(ASTVisitor visitor, object options = null) {
			return visitor.DoForFuncDef(this, options);
		}
	}

	public class ObjDefAST:AST {  // namespace class, struct or variable
		public Token token;
		public string name;
		public BlockAST stmt;
		public ObjDefAST(Token token, BlockAST stmt) {
			this.token = token;
			this.name = token.value;
			this.stmt = stmt;
		}
		public object Accept(ASTVisitor visitor, object options = null) {
			return visitor.DoForObjDef(this, options);
		}
	}

	public class ObjInstAST:AST {  // instance of a class, struct or variable
		public Token instToken;
		public string name;
		public TypeAST typeAST;
		public AST value;
		public ObjInstAST(Token instToken, TypeAST typeAST, AST value) {
			this.instToken = instToken;
			this.name = instToken.value;
			this.typeAST = typeAST;
			this.value = value;
		}
		public object Accept(ASTVisitor visitor, object options = null) {
			return visitor.DoForObjInst(this, options);
		}
	}
	

	public class TypeAST:AST {  // int, bool etc
		public Token token;
		public string name;
		public TypeAST(Token token) {
			this.token = token;
			this.name = token.value;
		}
		public object Accept(ASTVisitor visitor, object options = null) {
			return visitor.DoForType(this, options);
		}
	}
	public class ArgAST:AST {  // function argument, will be paired with a Type
		public Token token;
		public TypeAST typeAST;
		public string name;
		public ArgAST(Token token, TypeAST typeAST) {
			this.token = token;
			this.name = token.value;
			this.typeAST = typeAST;
		}
		public object Accept(ASTVisitor visitor, object options = null) {
			return visitor.DoForArg(this, options);
		}
	}





	public class Parser {
		TokenList tokens;
		SymbolList symbols;
		private BlockAST ast;


		public Parser(Token[] tokens) {
			this.tokens = new TokenList(tokens);
			symbols = new SymbolList();
		}

		public BlockAST GetAst() {
			return ast;
		}




		class TokenList {
			public TokenType curTokenType;
			public Token current;
			Token[] tokens;
			TokenType[] tokenTypes;
			int index;

			public TokenList(Token[] tokens) {
				this.tokens = tokens;
				tokenTypes = new TokenType[tokens.Length];
				for (int i = 0; i < tokens.Length; i++)
					tokenTypes[i] = tokens[i].tokenType;
				index = 0;
				current = tokens[index];
				curTokenType = tokenTypes[index];
			}
			public Token Next() {
				index++;
				try {
					current = tokens[index];
					curTokenType = tokenTypes[index];
					return current;
				} catch (IndexOutOfRangeException) {
					return null;
				}
			}
			public int Remaining() {
				return tokens.Length - index - 1;
			}
			public TokenType PeekType(int advance = 1) {
				try {
					return tokenTypes[index + advance];
				} catch (IndexOutOfRangeException) {
					return TokenType.None;
				}
			}

			public bool IsType(TokenType tt) {
				return ((curTokenType & tt) == tt);
			}

			public Token Prev() {
				index--;
				try {
					current = tokens[index];
					curTokenType = tokenTypes[index];
					return current;
				} catch (IndexOutOfRangeException) {
					return null;
				}
			}
			public void SkipPastEndLine() {
				do
					Next();
				while (!IsType(TokenType.EndLine) && !IsType(TokenType.EndFile));
				if (!IsType(TokenType.EndFile))
					Next();
			}

		}

		class Symbol {
			Symbol parent;
			TypeAST type;
			string name;
			ArgAST[] args;

			public Symbol(Symbol parent, TypeAST type, string name, ArgAST[] args) {
				this.parent = parent;
				this.type = type;
				this.name = name;
				this.args = args;
			}
		}

		class SymbolList {
			List<Symbol> symbols;
			Queue<Symbol> prevSymbols;
			public Symbol curSymbol = null;

			public SymbolList() {
				symbols = new List<Symbol>();
				prevSymbols = new Queue<Symbol>();
			}

			public void Add(TypeAST typeAST, string name, ArgAST[] argsAST) {
				Symbol sym = new Symbol(curSymbol, typeAST, name, argsAST);
				symbols.Add(sym);
			}
			public void AddParent(TypeAST typeAST, string name, ArgAST[] argsAST) {
				Symbol sym = new Symbol(curSymbol, typeAST, name, argsAST);
				prevSymbols.Enqueue(curSymbol);
				symbols.Add(sym);
				curSymbol = sym;
			}
			public void RemParent() {
				curSymbol = prevSymbols.Dequeue();
			}
		}



			void ShowError(string message, Token token) {
			ConsoleEx.WriteLine("{0}{1} at line {2}: {3}, {4}", ConsoleColor.Red, token.filename, token.line, message, token.value);
		}


		// simple token list:
		//	ID: int MAIN: main OPEN_BRACKET: ( CLOSE_BRACKET: ) END_LINE:
		//	INDENTATION:     COMMAND: return INT: 2 END_LINE:
		public void Parse() {
			ast = Statments();
		}


		/*
		 * Command -> command (return, if, while, etc)
		 * NewType Colon -> Object def
		 * Type Identifier -> instantiated object
		 * Type Identifier Assign (Num | Identifier)  -> instantiated object assigned a value
		 * Type Identifier LParen -> function, if called [M|m]ain will be application entry point
		 */
		class MatchStatement {
			string name;
			TokenType[] ttype;
			public MatchStatement(string name, TokenType[] ttype) {
				this.name = name;
				this.ttype = ttype;
			}
			public string Match(TokenList tokenList) {
				int idx = 0;
				if (tokenList.Remaining() >= ttype.Length) {
					while (idx < ttype.Length && (ttype[idx] & tokenList.PeekType(idx)) == ttype[idx])
						idx++;
					if (idx == ttype.Length)
						return name;
				}
				return null;
			}
		}

		static readonly MatchStatement[] StmntDefs = new MatchStatement[] {
			new MatchStatement("Command", new TokenType[] {TokenType.Commands}),
			new MatchStatement("Main", new TokenType[] {TokenType.AllTypes, TokenType.Identifier, TokenType.LParen}),
			new MatchStatement("Function", new TokenType[] {TokenType.Identifier, TokenType.Identifier, TokenType.LParen}),
			new MatchStatement("Function", new TokenType[] {TokenType.AllTypes, TokenType.Identifier, TokenType.LParen}),
			new MatchStatement("ObjectDef", new TokenType[] {TokenType.Identifier, TokenType.Colon}),
			new MatchStatement("SimpleObjectInst", new TokenType[] {TokenType.AllTypes, TokenType.Identifier}),
			new MatchStatement("ObjectInst", new TokenType[] {TokenType.Identifier, TokenType.Identifier}),
			new MatchStatement("SimpleObjectInstAssigned", new TokenType[] {TokenType.AllTypes, TokenType.Identifier, TokenType.Assign}),
			new MatchStatement("ObjectInstAssigned", new TokenType[] {TokenType.Identifier, TokenType.Identifier, TokenType.Assign}),
		};

		string GetStmntMatch(TokenList tokenList) {
			foreach(MatchStatement ms in StmntDefs) {
				string res = ms.Match(tokenList);
				if (res != null)
					return res;
			}
			return null;
		}

		BlockAST Statments(int startIndent = 0) {
			List<AST> stmts = new List<AST>();
			Token firstTok = tokens.current;
			do {
				string found = GetStmntMatch(tokens);
				switch (found) {
					case "Command": {
						AST stmtAST = null;
						switch (tokens.curTokenType) {
							case TokenType.Return:
								Token retTok = tokens.current;
								ExprAST expAST = ParseExpr();
								stmtAST = new RetCmdAST(retTok, expAST);
								break;
							default:
								ShowError("Unknown command", tokens.current);
								break;
						}
						if (stmtAST != null)
							stmts.Add(stmtAST);
						break;
					}
					case "Main": {
						TypeAST typeAST = new TypeAST(tokens.current);
						Token mainTok = tokens.Next();
						ArgAST[] argsAST = ParseArgs();
						symbols.AddParent(typeAST, mainTok.value, argsAST);
						BlockAST stmtAST = Statments(startIndent + 1);
						symbols.RemParent();
						AST functAST = new FuncDefAST(mainTok, typeAST, argsAST, stmtAST);
						stmts.Add(functAST);
						break;
					}
					case "Function": {
						TypeAST typeAST = new TypeAST(tokens.current);
						Token instTok = tokens.Next();
						ArgAST[] argsAST = ParseArgs();
						symbols.AddParent(typeAST, instTok.value, argsAST);
						BlockAST stmtAST = Statments(startIndent + 1);
						symbols.RemParent();
						AST functAST = new FuncDefAST(instTok, typeAST, argsAST, stmtAST);
						stmts.Add(functAST);
						break;
					}
					case "ObjectDef": {     // TokenType.Identifier, TokenType.Colon
						TypeAST typeAST = new TypeAST(tokens.current);
						tokens.Next();
						symbols.AddParent(typeAST, "", null);
						tokens.Next();
						BlockAST stmtAST = Statments(startIndent + 1);
						symbols.RemParent();
						ObjDefAST odAST = new ObjDefAST(tokens.current, stmtAST);
						stmts.Add(odAST);
						break;
					}
					case "SimpleObjectInst": {  // TokenType.AllTypes, TokenType.Identifier
						TypeAST typeAST = new TypeAST(tokens.current);
						tokens.Next();
						symbols.Add(typeAST, tokens.current.value, null);
						ObjInstAST odAST = new ObjInstAST(tokens.current, typeAST, null);
						stmts.Add(odAST);
						break;
					}
					case "ObjectInst": {
						TypeAST typeAST = new TypeAST(tokens.current);
						tokens.Next();
						symbols.Add(typeAST, tokens.current.value, null);
						ObjInstAST odAST = new ObjInstAST(tokens.current, typeAST, null);
						stmts.Add(odAST);
						break;
					}
					case "SimpleObjectInstAssigned": {
						TypeAST typeAST = new TypeAST(tokens.current);
						Token instTok = tokens.Next();
						symbols.Add(typeAST, instTok.value, null);
						tokens.Next();
						NumAST numAST = new NumAST(tokens.current, tokens.current.value, tokens.curTokenType);
						ObjInstAST odAST = new ObjInstAST(instTok, typeAST, numAST);
						stmts.Add(odAST);
						break;
					}
					case "ObjectInstAssigned": {
						TypeAST typeAST = new TypeAST(tokens.current);
						Token instTok = tokens.Next();
						symbols.Add(typeAST, instTok.value, null);
						tokens.Next();
						NumAST numAST = new NumAST(tokens.current, tokens.current.value, tokens.curTokenType);
						ObjInstAST odAST = new ObjInstAST(instTok, typeAST, numAST);
						stmts.Add(odAST);
						break;
					}
					default:
						ShowError("Unable to parse statement", tokens.current);
						break;
				}
			} while (tokens.Remaining() > 0);
			return new BlockAST(firstTok, stmts.ToArray());
		}


		// parse unknown amount of arguments for a function, entry on open bracket
		ArgAST[] ParseArgs() {
			List<ArgAST> args = new List<ArgAST>();
			while (tokens.Next()!=null && !tokens.IsType(TokenType.RParen)) {
				if (tokens.IsType(TokenType.Comma)) {   // end of this arg, got another tho
					tokens.Next();
					if (tokens.IsType(TokenType.EndLine) || tokens.IsType(TokenType.EndFile)) {
						ShowError("Incomplete arg list", tokens.Prev());
					}
				}
				if (tokens.IsType(TokenType.AllIdentifiers)) {   // type
					TypeAST type = new TypeAST(tokens.current);
					tokens.Next();
					if (tokens.IsType(TokenType.AllIdentifiers)) {   // arg name
						args.Add(new ArgAST(tokens.current, type));
					}
				} else {
					ShowError("Incomplete arg list", tokens.current);
				}
			}
			tokens.SkipPastEndLine();
			return args.ToArray();
		}


		AST ParseReturn() {
			Token retTok = tokens.current;
			ExprAST expAST = ParseExpr();
			return new RetCmdAST(retTok, expAST);
		}


		ExprAST ParseExpr(int precedenceOffset = 0) {   // scan the whole line and parses the expression, curToken is just before the expression
			Token firsttoken = tokens.Next();
			List<AST> bodyASTs = new List<AST>();
			List<Token> bodyToksList = new List<Token>();
			while (!tokens.IsType(TokenType.EndLine) && !tokens.IsType(TokenType.EndFile)) {
				bodyToksList.Add(tokens.current);
				tokens.Next();
			}
			Token[] bodyToks = bodyToksList.ToArray();
			int[] order = new int[bodyToks.Length];

			order = ParseExprScanToks(bodyToks, order, precedenceOffset);

			int[] astIdx = new int[bodyToks.Length];
			Array.Fill(astIdx, -1);
			int idx = IndexOfHighestPrecedence(bodyToks, order);
			if (idx < 0) {  // only a single item
				if (bodyToks[0].tokenType == TokenType.IntNum || bodyToks[0].tokenType == TokenType.FloatNum || bodyToks[0].tokenType == TokenType.DoubleNum) {
					bodyASTs.Add(new NumAST(bodyToks[0], bodyToks[0].value, bodyToks[0].tokenType));
				} else if (bodyToks[0].tokenType == TokenType.Identifier) {
					bodyASTs.Add(new IdentifierAST(bodyToks[0], bodyToks[0].value));
				}
			}
			while (idx > 0) {
				AST left, right;
				if (astIdx[idx - 1] >= 0)
					left = bodyASTs[astIdx[idx - 1]];
				else
					left = new NumAST(bodyToks[idx - 1], bodyToks[idx - 1].value, bodyToks[idx - 1].tokenType);
				if (astIdx[idx + 1] >= 0)
					right = bodyASTs[astIdx[idx + 1]];
				else
					right = new NumAST(bodyToks[idx + 1], bodyToks[idx + 1].value, bodyToks[idx + 1].tokenType);
				astIdx[idx] = astIdx[idx + 1] = astIdx[idx - 1] = bodyASTs.Count;
				order[idx] = order[idx + 1] = order[idx - 1] = -1;
				bodyASTs.Add(new BinOpAST(bodyToks[idx], left, right));
				idx = IndexOfHighestPrecedence(bodyToks, order);
			}
			ExprAST expr = new ExprAST(firsttoken, bodyASTs.ToArray());
			return expr;
		}

		int[] ParseExprScanToks(Token[] bodyToks, int[] order, int precedenceOffset) {
			for (int i = 0; i < bodyToks.Length; i++) {
				switch (bodyToks[i].tokenType) {
					case TokenType.Plus:
					case TokenType.Minus:
						order[i] = 1 + precedenceOffset;
						break;
					case TokenType.Mult:
					case TokenType.Div:
						order[i] = 2 + precedenceOffset;
						break;
					case TokenType.Pow:
						order[i] = 3 + precedenceOffset;
						break;
					case TokenType.LParen:
						int count = 0;
						order[i++] = 0;
						while (bodyToks[i + count].tokenType != TokenType.RParen && bodyToks[i + count].tokenType != TokenType.EndLine)
							count++;
						int[] newOrder = new int[count];
						Token[] newBodyToks = new Token[count];
						Array.Copy(order, i, newOrder, 0, count);
						Array.Copy(bodyToks, i, newBodyToks, 0, count);
						newOrder = ParseExprScanToks(bodyToks, newOrder, precedenceOffset + 10);
						Array.Copy(newOrder, 0, order, i, count);
						break;
					// todo add LParen, will require recursion and splitting up this function
					default:
						order[i] = 0;
						break;
				}
			}
			return order;
		}

		int IndexOfHighestPrecedence(Token[] bodyToks, int[] precedence) {
			int index = -1;
			int curPrec = -1;
			for (int i = 0; i < bodyToks.Length; i++) {
				if ((bodyToks[i].tokenType & TokenType.AllBinOp) == TokenType.AllBinOp) {
					if (precedence[i] > curPrec) {
						index = i;
						curPrec = precedence[i];
					}
				}
			}
			return index;
		}


		public void Show() {
			PrettyPrint p = new PrettyPrint();
			ConsoleEx.WriteLine("\n{0}AST:", ConsoleColor.White);
			ast.Accept(p);
			Console.WriteLine("\n");
		}







		class PrettyPrint : ASTVisitor {
			public void PositionOutput(int tabs) {
				for (int i = 0; i < tabs; i++)
					Console.Write("\t");
			}

			public object DoForAST(AST ast, object options = null) {
				ConsoleEx.WriteLine("{0}(AST)", ConsoleColor.DarkBlue);
				return options;
			}

			public object DoForBinOp(BinOpAST bo, object options = null) {
				PositionOutput(bo.token.indent);
				ConsoleEx.WriteLine("{0}(BinOpAST) {1}{2}", ConsoleColor.DarkBlue, ConsoleColor.Gray, bo.token.value);
				bo.left.Accept(this, options);
				bo.right.Accept(this, options);
				return options;
			}
			public object DoForNum(NumAST num, object options = null) {
				PositionOutput(num.token.indent);
				ConsoleEx.WriteLine("{0}(Num) {1}{2}", ConsoleColor.DarkBlue , ConsoleColor.Gray, num.token.value);
				return options;
			}
			public object DoForRetCmd(RetCmdAST rc, object options = null) {
				PositionOutput(rc.token.indent);
				ConsoleEx.WriteLine("{0}(Return) {1}{2}", ConsoleColor.DarkBlue, ConsoleColor.Gray, rc.token.value);
				rc.expression.Accept(this, options);
				return options;
			}
			public object DoForIdentifier(IdentifierAST id, object options = null) {
				PositionOutput(id.token.indent);
				ConsoleEx.WriteLine("{0}(IdentifierAST) {1}{2}", ConsoleColor.DarkBlue, ConsoleColor.Gray, id.token.value);
				return options;
			}
			public object DoForBlock(BlockAST blk, object options = null) {
				PositionOutput(blk.token.indent);
				ConsoleEx.WriteLine("{0}(Block)", ConsoleColor.DarkBlue);
				foreach (var statement in blk.stmts)
					statement.Accept(this, options);
				return options;
			}
			public object DoForFuncDef(FuncDefAST fd, object options = null) {
				PositionOutput(fd.token.indent);
				ConsoleEx.WriteLine("{0}(FuncDef) {1}{2}", ConsoleColor.DarkBlue, ConsoleColor.Gray, fd.token.value);
				fd.retType.Accept(this, options);
				foreach (var a in fd.args)
					a.Accept(this, options);
				fd.stmt.Accept(this, options);
				return options;
			}
			public object DoForObjDef(ObjDefAST od, object options = null) {
				PositionOutput(od.token.indent);
				ConsoleEx.WriteLine("{0}(ObjDef) {1}{2}", ConsoleColor.DarkBlue, ConsoleColor.Gray, od.token.value);
				od.stmt.Accept(this, options);
				return null;
			}
			public object DoForObjInst(ObjInstAST oi, object options = null) {
				PositionOutput(oi.instToken.indent);
				oi.typeAST.Accept(this, null);
				ConsoleEx.WriteLine("{0}(Inst) {1}{2}", ConsoleColor.DarkBlue, ConsoleColor.Gray, oi.name);
				if (oi.value != null)
					oi.value.Accept(this, null);
				return null;
			}
			public object DoForType(TypeAST t, object options = null) {
				PositionOutput(t.token.indent);
				ConsoleEx.WriteLine("{0}(Type) {1}{2}", ConsoleColor.DarkBlue, ConsoleColor.Gray, t.token.value);
				return options;
			}
			public object DoForArg(ArgAST arg, object options = null) {
				PositionOutput(arg.token.indent);
				ConsoleEx.WriteLine("{0}(Arg) {1}{2}", ConsoleColor.DarkBlue, ConsoleColor.Gray, arg.token.value);
				return options;
			}
			public object DoForExpr(ExprAST xpr, object options = null) {
				PositionOutput(xpr.token.indent);
				ConsoleEx.WriteLine("{0}(Expr)", ConsoleColor.DarkBlue);
				foreach (var x in xpr.body)
					x.Accept(this, options);
				return options;
			}
		}




	}
}
