using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
	struct RES_107
	{
		public uint _vmt;
		public uint pBlockEnd;
		public uint pTextureDictionary;
		public uint _fc;
		public uint pDrawableCollecion;// xtd header
		public uint _f14; // поинтер
		public uint _f18;
		public uint _f1с; // поинтер
		public uint _f20; // поинтер
		public uint _f24;
	}
	internal class MCLA_107
	{
		public static void Read(MemoryStream ms, bool endian)
		{
			RES_107 file;

		}
		
	}
}
