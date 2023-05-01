using Converter.Core.ResourceTypes;
using Converter.Core.Utils;
using System;
using System.IO;

namespace Converter.Core
{
	public static class TextureDictionary
	{
		public class UniversalTexture
		{
			public uint _vmt;
			public uint m_nSize;
			public uint m_pName;
			public uint m_pBitmap;
			public ushort m_nWidth;
			public ushort m_nHeight;
			public uint m_nLodCount;

			public static UniversalTexture ConvertToUniversalTexture(RDR_Texture texture)
			{
				return new UniversalTexture
				{
					_vmt = texture._vmt,
					m_nSize = texture.m_nSize,
					m_pName = texture.m_pName,
					m_pBitmap = texture.m_pBitmap,
					m_nWidth = texture.m_nWidth,
					m_nHeight = texture.m_nHeight,
					m_nLodCount = texture.m_nLodCount
				};
			}

			public static UniversalTexture ConvertToUniversalTexture(IV_Texture texture)
			{
				return new UniversalTexture
				{
					_vmt = texture._vmt,
					m_nSize = 0, // absent in this section
					m_pName = texture.m_pName,
					m_pBitmap = texture.m_pBitmap,
					m_nWidth = texture.m_nWidth,
					m_nHeight = texture.m_nHeight,
					m_nLodCount = texture.m_nLodCount
				};
			}
		}

		public static bool ReadTexture(UniversalTexture texture, BitMap bitMap, EndianBinaryReader br, uint hash, bool overwriteTexture = true)
		{
			string typeAsString = TextureUtils.GetFormat(bitMap.DataFormat);
			
			if (typeAsString == "unknown")
			{
				Console.WriteLine($"[WARNING] Unknown texture type {bitMap.DataFormat}.");
				return true;
			}
			
			string textureName;
			if (texture.m_pName != 0)
			{
				textureName = Path.GetFileName(DataUtils.ReadStringAtOffset(texture.m_pName, br).Replace(':', '\\'));

				// force add texture extension
				if (!textureName.EndsWith(".dds"))
				{
					textureName += ".dds";
				}
			}
			else
			{
				textureName = $"0x{hash:x8}.dds";
			}

			// TODO: change pixelData argument to return value
			byte[] pixelData = new byte[1];
			TextureUtils.ReadPixelData(br, bitMap.BaseAddress, bitMap.MipAddress, texture.m_nLodCount, ref pixelData, texture.m_nWidth, texture.m_nHeight, typeAsString);

			// define basic data in dds file header
			DDS_HEADER hdr = new DDS_HEADER
			{
				dwHeight = texture.m_nHeight,
				dwWidth = texture.m_nWidth,
				dwDepth = 1,
				dwMipMapCount = texture.m_nLodCount,
				dwCaps = (uint)(DDSCAPS.TEXTURE | DDSCAPS.MIPMAP | DDSCAPS.COMPLEX),
			};

			// add pixelformat-related data to the header
			switch (typeAsString)
			{
				case "DXT1": // BC1
					hdr.dwFlags = (uint)(DDSD.CAPS | DDSD.HEIGHT | DDSD.WIDTH | DDSD.PIXELFORMAT | DDSD.MIPMAPCOUNT | DDSD.LINEARSIZE);
					hdr.dwPitchOrLinearSize = (uint)(Math.Max(1, (texture.m_nWidth + 3) / 4) * 8);
					hdr.ddspf = new DDS_PIXELFORMAT
					{
						dwFlags = (uint)DDPF.FOURCC,
						dwFourCC = hdr.ddspf.DXT1
					};
					// don't set unneccessary fields like bitcount or bitmask, because they will be equal to 0 when writing
					break;

				case "DXT2_3":
					hdr.dwFlags = (uint)(DDSD.CAPS | DDSD.HEIGHT | DDSD.WIDTH | DDSD.PIXELFORMAT | DDSD.MIPMAPCOUNT | DDSD.LINEARSIZE);
					hdr.dwPitchOrLinearSize = (uint)(Math.Max(1, (texture.m_nWidth + 3) / 4) * 16);
					hdr.ddspf = new DDS_PIXELFORMAT
					{
						dwFlags = (uint)DDPF.FOURCC,
						dwFourCC = hdr.ddspf.DXT3
					};
					break;

				case "DXT4_5":
				case "DXT5A":
					hdr.dwFlags = (uint)(DDSD.CAPS | DDSD.HEIGHT | DDSD.WIDTH | DDSD.PIXELFORMAT | DDSD.MIPMAPCOUNT | DDSD.LINEARSIZE);
					hdr.dwPitchOrLinearSize = (uint)(Math.Max(1, (texture.m_nWidth + 3) / 4) * 16);
					hdr.ddspf = new DDS_PIXELFORMAT
					{
						dwFlags = (uint)DDPF.FOURCC,
						dwFourCC = hdr.ddspf.DXT5
					};
					break;

				case "8":
					hdr.dwFlags = (uint)(DDSD.CAPS | DDSD.HEIGHT | DDSD.WIDTH | DDSD.PIXELFORMAT | DDSD.MIPMAPCOUNT | DDSD.PITCH);
					hdr.dwPitchOrLinearSize = (uint)((texture.m_nWidth * 8 + 7) / 8);
					hdr.ddspf = new DDS_PIXELFORMAT
					{
						dwFlags = (uint)DDPF.LUMINANCE,
						dwRGBBitCount = 8,
						dwRBitMask = 0xFF
					};
					break;

				case "8_8_8_8":
					hdr.dwFlags = (uint)(DDSD.CAPS | DDSD.HEIGHT | DDSD.WIDTH | DDSD.PIXELFORMAT | DDSD.MIPMAPCOUNT | DDSD.PITCH);
					hdr.dwPitchOrLinearSize = (uint)((texture.m_nWidth * 32 + 7) / 8);
					hdr.ddspf = new DDS_PIXELFORMAT
					{
						dwFlags = (uint)(DDPF.RGB | DDPF.ALPHAPIXELS),
						dwRGBBitCount = 32,
						dwRBitMask = 0xFF0000,
						dwGBitMask = 0xFF00,
						dwBBitMask = 0xFF,
						dwABitMask = 0xFF000000
					};
					break;

				case "4_4_4_4":
					hdr.dwFlags = (uint)(DDSD.CAPS | DDSD.HEIGHT | DDSD.WIDTH | DDSD.PIXELFORMAT | DDSD.MIPMAPCOUNT | DDSD.PITCH);
					hdr.dwPitchOrLinearSize = (uint)((texture.m_nWidth * 16 + 7) / 8);
					hdr.ddspf = new DDS_PIXELFORMAT
					{
						dwFlags = (uint)(DDPF.RGB | DDPF.ALPHAPIXELS),
						dwRGBBitCount = 16,
						dwRBitMask = 0xF00,
						dwGBitMask = 0xF0,
						dwBBitMask = 0xF,
						dwABitMask = 0xF000
					};
					break;

				case "16_16_16_16_FLOAT":
					hdr.dwFlags = (uint)(DDSD.CAPS | DDSD.HEIGHT | DDSD.WIDTH | DDSD.PIXELFORMAT | DDSD.MIPMAPCOUNT | DDSD.PITCH);
					hdr.dwPitchOrLinearSize = (uint)((texture.m_nWidth * 64 + 7) / 8);
					hdr.ddspf = new DDS_PIXELFORMAT
					{
						dwFlags = (uint)DDPF.FOURCC,
						dwFourCC = 113
					};
					break;
			}

			// prepare the output path and create a subdirectory for textures if it's not exist
			string finalPath = $"{Path.Combine(Main.outputPath, $"{Path.GetFileName(Main.inputPath).Replace(".", "_")}")}";
			if (!Directory.Exists(finalPath))
			{
				Directory.CreateDirectory(finalPath);
			}

			// save the file
			hdr.Write(Path.Combine(finalPath, textureName), pixelData, overwriteTexture);
			return true;
		}

		public static bool ReadTextureDictionary(EndianBinaryReader br, uint pointer, bool overwriteTexture = true)
		{
			br.Position = pointer;
			Dictionary header = Dictionary.Read(br);
			uint[] hash = new uint[header.m_cHash.m_nCount];
			br.Position = header.m_cHash.m_pList;
			
			for (int i = 0; i < header.m_cHash.m_nCount; i++)
			{
				hash[i] = br.ReadUInt32();
			}
			
			uint[] pTexture = new uint[header.m_cTexture.m_nCount];
			br.Position = header.m_cTexture.m_pList;
			
			for (int i = 0; i < header.m_cTexture.m_nCount; i++)
			{
				pTexture[i] = br.ReadOffset();
			}

			UniversalTexture[] texture = new UniversalTexture[header.m_cTexture.m_nCount];
			BitMap[] bitMap = new BitMap[header.m_cTexture.m_nCount];

			if (Main.useVerboseMode || Main.useVeryVerboseMode)
			{
				Console.WriteLine($"[INFO] Textures count: {header.m_cTexture.m_nCount}");
			}			
			
			for (int i = 0; i < header.m_cTexture.m_nCount; i++)
			{
				br.Position = pTexture[i];

				if (ResourceInfo.Version == 109)
				{
					texture[i] = UniversalTexture.ConvertToUniversalTexture(IV_Texture.Read(br));
				}
				else
				{
					texture[i] = UniversalTexture.ConvertToUniversalTexture(RDR_Texture.Read(br));
				}
				
				br.Position = texture[i].m_pBitmap;
				bitMap[i] = BitMap.Read(br);
			}

			for (int i = 0; i < header.m_cTexture.m_nCount; i++)
			{
				if (Main.useVeryVerboseMode)
				{
					string tmpName = texture[i].m_pName != 0 ? DataUtils.ReadStringAtOffset(texture[i].m_pName, br) : $"0x{hash[i]}";
					Console.WriteLine($"[INFO] Name: {tmpName}, Size: {texture[i].m_nHeight}x{texture[i].m_nWidth}, Type as byte: {bitMap[i].DataFormat}");
				}

				ReadTexture(texture[i], bitMap[i], br, hash[i], overwriteTexture);
			}

			return true;
		}
	}
}
