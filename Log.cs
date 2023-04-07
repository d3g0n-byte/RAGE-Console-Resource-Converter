using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Converter
{
	public static class Log
	{
		public enum MessageType
		{
			INFO,
			WARNING,
			ERROR
		}
		public static void ToLog(Log.MessageType messageType, string text)
		{
			switch (messageType)
			{
				case MessageType.INFO:
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine($"[INFO] {text}");
					Console.ResetColor();
					break;
				case MessageType.WARNING:
					Console.ForegroundColor = ConsoleColor.DarkYellow;
					Console.WriteLine($"[WARNING] {text}");
					Console.ResetColor();
					break;
				case MessageType.ERROR:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"[ERROR] {text}");
					Console.ResetColor();
					break;
			}
		}
	}
}
