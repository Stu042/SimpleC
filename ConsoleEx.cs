using System;

namespace SimpleC {

	public static class ConsoleEx {
		public static void WriteLine(String message, params object[] args) {
			if (args.Length > 0) {
				var oldColor = Console.ForegroundColor;
				String msg = message;
				foreach (var arg in args) {
					char[] c = msg.ToCharArray();
					int idx = 0;
					while (c[idx] != '{')
						idx++;
					if (Object.ReferenceEquals(arg.GetType(), oldColor.GetType())) {    // next arg is a color
						Console.Write(msg.Substring(0, idx));
						Console.ForegroundColor = (ConsoleColor)arg;
					} else {
						Console.Write(msg.Substring(0, idx) + arg.ToString());
					}
					while (c[idx] != '}')
						idx++;
					idx++;
					msg = msg.Substring(idx);
				}
				Console.WriteLine(msg);
				Console.ForegroundColor = oldColor;
			} else {
				Console.WriteLine(message);
			}
		}


		public static void Write(String message, params object[] args) {
			if (args.Length > 0) {
				var oldColor = Console.ForegroundColor;
				String msg = message;
				foreach (var arg in args) {
					char[] c = msg.ToCharArray();
					int idx = 0;
					while (c[idx] != '{')
						idx++;
					if (Object.ReferenceEquals(arg.GetType(), oldColor.GetType())) {    // next arg is a color
						Console.Write(msg.Substring(0, idx));
						Console.ForegroundColor = (ConsoleColor)arg;
					} else {
						Console.Write(msg.Substring(0, idx) + arg.ToString());
					}
					while (c[idx] != '}')
						idx++;
					idx++;
					msg = msg.Substring(idx);
				}
				Console.Write(msg);
				Console.ForegroundColor = oldColor;
			} else {
				Console.Write(message);
			}
		}

	

}



}
