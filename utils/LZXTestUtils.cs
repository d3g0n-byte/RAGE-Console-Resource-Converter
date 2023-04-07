using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Converter.utils
{
	internal class LZXTestUtils
	{
		// сжатие некоторых файлов конвертора lzx сжатием 
		public static void CompressAndWriteConverterData(byte[] data, string path)
		{
			BinaryWriter dataOut = new BinaryWriter(new FileStream(path, FileMode.Create));
			dataOut.Write(372003);
			dataOut.Write(1); // подобие версии. По ней можно понять что идет после
			dataOut.Write(data.Length);
			dataOut.Write(DataUtils.CompressLZX2(data));
			dataOut.Close();
		}
		public static void DecompressConverterData(ref byte[] data, string path)
		{
			BinaryReader inData = new BinaryReader(File.OpenRead(path));
			uint magic = inData.ReadUInt32();
			uint version = inData.ReadUInt32();
			switch (version)
			{
				case 1:
					int size = inData.ReadInt32();
					byte[] converterData = inData.ReadBytes(size);
					Array.Resize(ref data, size);
					if (DataUtils.DecompressLZX(converterData, data) != 0) throw new Exception("err");
					break;
				default: throw new Exception("unk version");
			}

		}
	}
}
