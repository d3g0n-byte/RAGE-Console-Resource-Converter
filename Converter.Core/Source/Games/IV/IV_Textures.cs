using Converter.Core.Utils;
using System;
using System.IO;

namespace Converter.Core.Games.IV
{
	public static class IV_Textures
	{
		public static bool ExportTexturesFromIVRes(MemoryStream decompMem, bool endian, int version)
		{
			using (EndianBinaryReader br = new EndianBinaryReader(decompMem))
			{
				if (endian)
				{
					br.Endianness = Endian.BigEndian;
				}

				if (version == 109)
				{
					br.ReadUInt32();
					br.ReadUInt32();

					uint pointToShaderGroup = br.ReadOffset();
					if (pointToShaderGroup == 0)
					{
						return true;
					}

					br.Position = pointToShaderGroup;
					br.ReadUInt32();

					uint pointToTextures = br.ReadOffset();
					if (pointToTextures == 0)
					{
						return true;
					}

					br.Position = pointToTextures;
					if (!TextureDictionary.ReadTextureDictionary(br, pointToTextures))
					{
						throw new Exception("Error while reading IV Texture Dictionary.");
					}
				}
			}

			return true;
		}
	}
}
