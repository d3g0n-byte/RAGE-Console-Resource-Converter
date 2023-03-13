using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
	internal class IV_Textures
	{
		public static bool ExportTexturesFromIVRes(MemoryStream decompMem, bool endian, int version)
		{
			EndianBinaryReader br = new EndianBinaryReader(decompMem);
			if (endian) br.Endianness = Endian.BigEndian;// 0 - lit, 1 - big
			if(version == 109)
			{
				br.ReadUInt32();
				br.ReadUInt32();
				uint pointToShaderGroup = br.ReadOffset();
				if (pointToShaderGroup == 0) return true;
				br.Position = pointToShaderGroup;
				br.ReadUInt32();
				uint pointToTextures = br.ReadOffset();
				if (pointToTextures == 0) return true;
				br.Position = pointToTextures;
				if (!TextureDictionary.ReadTextureDictionary(br, pointToTextures))
				{
					throw new Exception("Error while reading IV Texture Dictionary.");
				}
			}
			return true;
		}
	}
}
