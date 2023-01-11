using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
	internal class Other
	{
		public static bool GetBit(ushort b, int i)
		{
			return (b & (1 << i)) != 0;
		}
		internal static bool GetRealBit(byte b, int i)
		{
			return (b & (1 << i)) != 0;
		}

		public static string GetFileMask(string fileName)
		{
			return System.IO.Path.GetExtension(fileName);
		}
		public static string ReadStringAtOffset(uint p, EndianBinaryReader br)
		{
			br.Position = p;
			string tempstring = "";
			char tmp;
			for (int a = 0; a < 1; )
			{
				tmp = br.ReadChar();
				if (tmp != 0) tempstring += tmp;
				else break;
			}
			return tempstring;
		}
	}
}
