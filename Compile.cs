using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Xml.Serialization;

namespace SimpleC {


	class Compiler {
		CompilerVisitor compilerVisitor;
		public Compiler() {
			ConsoleEx.WriteLine("{0}Compiling:", ConsoleColor.White);
			compilerVisitor = new CompilerVisitor();
		}

		public string Compile(BlockAST blockAST) {
			return compilerVisitor.Compile(blockAST);
		}




		class CompilerVisitor:ASTVisitor {
			StringBuilder code;

			public class Symbol {
				public Symbol parent;
				public string objName;
				public bool functional;
				public ObjType llType;
				public string llName;
				public ObjArg[] args;
			}

			public class ObjType {
				public string llName;	// i32, i8 etc
				public int size;
				public int align;
			}

			public class ObjArg {
				public Symbol parent;
				public string llName;  // !0, !1 something like this usually
				public string name;
				public ObjType type;
			}

			public class ObjExpr {
				public string val;
				public ObjType objType;
				public bool isSimplifiedToNum;
				public bool isInt;
			}

			List<Symbol> symTbl;
			Symbol curParent;
			public string irCode;

			public CompilerVisitor() {
				curParent = null;
				code = new StringBuilder();
				symTbl = new List<Symbol>();
			}

			public string Compile(BlockAST start) {
				code.Append("; ModuleID = '" + start.token.filename + "'\nsource_filename = \"" + start.token.filename +
					"\"\ntarget datalayout = \"e-m:w-p270:32:32-p271:32:32-p272:64:64-i64:64-f80:128-n8:16:32:64-S128\"\n" +
					/* windows: "target triple = \"x86_64-pc-windows-msvc19.26.28806\"\n\n");*/
					"target triple = \"x86_64-pc-linux-gnu\"\n\n");

				start.Accept(this, null);
				code.Append("attributes #0 = { noinline nounwind optnone uwtable \"correctly-rounded-divide-sqrt-fp-math\"=\"false\"" +
					" \"disable-tail-calls\"=\"false\" \"frame-pointer\"=\"none\" \"less-precise-fpmad\"=\"false\" \"min-legal-vector-width\"=\"0\"" +
					" \"no-infs-fp-math\"=\"false\" \"no-jump-tables\"=\"false\" \"no-nans-fp-math\"=\"false\" \"no-signed-zeros-fp-math\"=\"false\"" +
					" \"no-trapping-math\"=\"false\" \"stack-protector-buffer-size\"=\"8\" \"target-cpu\"=\"x86-64\" \"target-features\"=\"+cx8,+fxsr,+mmx,+sse,+sse2,+x87\"" +
					" \"unsafe-fp-math\"=\"false\" \"use-soft-float\"=\"false\" }\n\n" +
					"!llvm.module.flags = !{ !0, !1}\n" +
					"!llvm.ident = !{ !2}\n\n" +
					"!0 = !{ i32 1, !\"wchar_size\", i32 2}\n" +
					"!1 = !{ i32 7, !\"PIC Level\", i32 2}\n" +
					"!2 = !{ !\"simplec version 0.0.1\"}\n");
				irCode = code.ToString();
				return irCode;
			}


			int intLabel = 0;
			string NextLabel() {
				string label = $"%{intLabel}";
				intLabel++;
				return label;
			}

			public object DoForAST(AST ast, object options = null) {
				Console.WriteLine("(AST)");
				return options;
			}


			// 2 * 5 +  4 * 3
			// binop: * 2 5, binop: * 4 3, binop: binop + binop
			public object DoForBinOp(BinOpAST bo, object lhsCarried = null) {
				ObjExpr lhsc = (ObjExpr)lhsCarried;
				ObjExpr rhs = (ObjExpr)bo.right.Accept(this, null);
				if (lhsCarried != null) {
					if (lhsc.isSimplifiedToNum && rhs.isSimplifiedToNum) {
						return BinOpTwoLiterals(lhsc, rhs, bo.op);
					} else {

					}
				} else {
					ObjExpr lhs = (ObjExpr)bo.left.Accept(this, null);
					if (lhs.isSimplifiedToNum && rhs.isSimplifiedToNum) {
						return BinOpTwoLiterals(lhs, rhs, bo.op);
					} else {

					}
				}
				Console.WriteLine("(BinOpAST)" + bo.token.value);
				return null;
			}

			ObjExpr BinOpTwoLiterals(ObjExpr lhs, ObjExpr rhs, string op) {
				ObjExpr objExpr = new ObjExpr();
				if (lhs.isInt) {
					Int64 lhsVal = Int64.Parse(lhs.val);
					if (rhs.isInt) {
						Int64 rhsVal = Int64.Parse(rhs.val);
						Int64 tot;
						switch (op) {
							case "+":
								tot = lhsVal + rhsVal;
								break;
							case "-":
								tot = lhsVal - rhsVal;
								break;
							case "*":
								tot = lhsVal * rhsVal;
								break;
							case "/":
								tot = lhsVal / rhsVal;
								break;
							default:
								tot = 42;
								break;
						}
						objExpr.val = tot.ToString();
						objExpr.isInt = true;
						objExpr.objType = GetTypeFromName("int64");
					} else {
						double rhsVal = double.Parse(rhs.val);
						double tot;
						switch (op) {
							case "+":
								tot = lhsVal + rhsVal;
								break;
							case "-":
								tot = lhsVal - rhsVal;
								break;
							case "*":
								tot = lhsVal * rhsVal;
								break;
							case "/":
								tot = lhsVal / rhsVal;
								break;
							default:
								tot = 42.0;
								break;
						}
						objExpr.val = tot.ToString();
						objExpr.isInt = false;
						objExpr.objType = GetTypeFromName("float");
					}
				} else {
					double lhsVal = double.Parse(lhs.val);
					if (rhs.isInt) {
						Int64 rhsVal = Int64.Parse(rhs.val);
						double tot;
						switch (op) {
							case "+":
								tot = lhsVal + rhsVal;
								break;
							case "-":
								tot = lhsVal - rhsVal;
								break;
							case "*":
								tot = lhsVal * rhsVal;
								break;
							case "/":
								tot = lhsVal / rhsVal;
								break;
							default:
								tot = 42.0;
								break;
						}
						objExpr.val = tot.ToString();
						objExpr.isInt = false;
						objExpr.objType = GetTypeFromName("float");
					} else {
						double rhsVal = double.Parse(rhs.val);
						double tot;
						switch (op) {
							case "+":
								tot = lhsVal + rhsVal;
								break;
							case "-":
								tot = lhsVal - rhsVal;
								break;
							case "*":
								tot = lhsVal * rhsVal;
								break;
							case "/":
								tot = lhsVal / rhsVal;
								break;
							default:
								tot = 42.0;
								break;
						}
						objExpr.val = tot.ToString();
						objExpr.isInt = false;
						objExpr.objType = GetTypeFromName("float");
					}
				}
				objExpr.isSimplifiedToNum = true;
				return objExpr;
			}

			public object DoForNum(NumAST num, object options = null) {
				ObjType ot = new ObjType();
				switch (num.token.tokenType) {
					case TokenType.IntNum:
						ot = GetTypeFromName("int64");
						break;
					case TokenType.FloatNum:
						ot = GetTypeFromName("float64");
						break;
				}
				ObjExpr oe = new ObjExpr() {
					val = num.value,
					isSimplifiedToNum = true,
					isInt = (num.token.tokenType == TokenType.IntNum),
					objType = ot
				};
				return oe;
			}

			public object DoForRetCmd(RetCmdAST rc, object options = null) {
				ObjExpr oe = (ObjExpr)rc.expression.Accept(this, options);
				code.Append($"  ret {curParent.llType.llName} ");   // we're returning from the curParent (probably)
																	// todo set new curParent
				code.Append(oe.val);
				return options;
			}
			public object DoForIdentifier(IdentifierAST id, object options = null) {
				Console.WriteLine("(IdentifierAST)" + id.token.value);
				return options;
			}
			public object DoForBlock(BlockAST blk, object options = null) {
				foreach (var statement in blk.stmts) {
					statement.Accept(this, options);
					code.Append("\n");
				}
				return options;
			}
			public object DoForFuncDef(FuncDefAST fd, object options = null) {
				Symbol sym = new Symbol();
				sym.parent = curParent;
				curParent = sym;
				sym.llType = (ObjType)fd.retType.Accept(this, null);
				sym.functional = true;
				sym.objName = fd.name;
				if (fd.name.ToLower() == "main")
					sym.llName = "@main";
				else
					sym.llName = "@" + fd.name;
				List<ObjArg> oal = new List<ObjArg>();
				for (var i = 0; i < fd.args.Length; i+=2) {
					ObjType t = (ObjType)fd.args[i].Accept(this, null);
					ObjArg a = (ObjArg)fd.args[i].Accept(this, t);
					oal.Add(a);
				}
				code.Append($"define dso_local {sym.llType.llName} {sym.llName}() #0 {{\n");
				sym.args = oal.ToArray();
				symTbl.Add(sym);
				fd.stmt.Accept(this, sym);
				code.Append("}\n\n");
				return null;
			}

			public object DoForObjDef(ObjDefAST od, object options = null) {
				Console.WriteLine($"(ObjDef){od.token.value}");
				od.stmt.Accept(this, options);
				return options;
			}


			public object DoForType(TypeAST t, object options = null) {
				ObjType ot = GetTypeFromName(t.token.value);
				return ot;
			}

			public object DoForArg(ArgAST arg, object options = null) {
				ObjArg oa = new ObjArg();
				oa.parent = curParent;
				oa.name = arg.name;
				oa.llName = NextLabel();
				oa.type = (ObjType)arg.typeAST.Accept(this, options);
				return oa;
			}

			public object DoForExpr(ExprAST exprAST, object options = null) {
				ObjExpr objExpr = null;
				foreach (var expr in exprAST.body) {
					objExpr = (ObjExpr)expr.Accept(this, objExpr);
				}
				return objExpr;
			}

			ObjType GetTypeFromName(string name) {
				ObjType ot = new ObjType();
				switch (name) {
					case "int32":
					case "int":
						ot.llName = "i32";
						ot.size = 32;   // in bits
						ot.align = 4;   // in bytes
						break;
					case "int8":
						ot.llName = "i8";
						ot.size = 8;    // in bits
						ot.align = 1;   // in bytes
						break;
					case "int16":
						ot.llName = "i16";
						ot.size = 16;    // in bits
						ot.align = 2;   // in bytes
						break;
					case "int64":
						ot.llName = "i64";
						ot.size = 64;    // in bits
						ot.align = 8;   // in bytes
						break;
					case "float16":
						ot.llName = "half";
						ot.size = 16;    // in bits
						ot.align = 2;   // in bytes
						break;
					case "float32":
						ot.llName = "float";
						ot.size = 32;    // in bits
						ot.align = 4;   // in bytes
						break;
					case "float":
					case "float64":
						ot.llName = "double";
						ot.size = 64;    // in bits
						ot.align = 8;   // in bytes
						break;
					case "float128":
						ot.llName = "fp128";
						ot.size = 128;    // in bits
						ot.align = 16;   // in bytes
						break;
					default:
						ot.llName = name;
						ot.size = 32;    // in bits
						ot.align = 4;   // in bytes
						break;
				}
				return ot;
			}

		} // CompilerVisitor

	}	// Compiler


}
