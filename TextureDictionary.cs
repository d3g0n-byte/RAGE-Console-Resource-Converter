using ConsoleApp1;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
	internal class TextureDictionary
	{
		private static string GetFormat(uint dwTextureType) // from RAGE Console Texture Editor
		{
			switch (dwTextureType)
			{
				case 6: return "8888";
				case 18: return "DXT1";
				case 19: return "DXT3";
				case 20: return "DXT5";
				case 59: return "DXT5A";
				case 49: return "DXN";
				case 2: return "8";
				default: return "unknown";
			}
		}
		private static int XGAddress2DTiledOffset(int x, int y, int w, int TexelPitch) // from RAGE Console Texture Editor
		{
			int alignedWidth, logBpp, Macro, Micro, Offset;
			alignedWidth = (w + 31) & ~31;
			logBpp = (TexelPitch >> 2) + ((TexelPitch >> 1) >> (TexelPitch >> 2));
			Macro = ((x >> 5) + (y >> 5) * (alignedWidth >> 5)) << (logBpp + 7);
			Micro = (((x & 7) + ((y & 6) << 2)) << logBpp);
			Offset = Macro + ((Micro & ~15) << 1) + (Micro & 15) + ((y & 8) << (3 + logBpp)) + ((y & 1) << 4);
			return (((Offset & ~511) << 3) + ((Offset & 448) << 2) + (Offset & 63) + ((y & 16) << 7) + (((((y & 8) >> 2) + (x >> 3)) & 3) << 6)) >> logBpp;
		}
		public static bool ReadTextureDictionary(EndianBinaryReader br, /*bool endian, */uint pointer)
		{

			br.Position = pointer;
			RageResource.XTDHeader header = new RageResource.XTDHeader();
			header = ReadRageResource.XTDHeader(br);
			uint[] pTexture = new uint[header.m_cTexture.m_wCount];// 
			br.Position = header.m_cTexture.m_pList;
			for (int a = 0; a < header.m_cTexture.m_wCount; a++) pTexture[a] = br.ReadOffset();
			RageResource.Texture[] texture = new RageResource.Texture[header.m_cTexture.m_wCount];
			RageResource.BitMap[] bitMap = new RageResource.BitMap[header.m_cTexture.m_wCount];
			for (int a = 0; a < header.m_cTexture.m_wCount; a++)
			{
				br.Position = pTexture[a];
				texture[a] = ReadRageResource.Texture(br);
				br.Position = texture[a].m_pBitmap;
				bitMap[a] = ReadRageResource.BitMap(br);
			}

			string typeAsString;
			uint endian;
			byte[] pixelData = new byte[1];
			for (int a = 0; a < header.m_cTexture.m_wCount; a++)
			{
				typeAsString = GetFormat(bitMap[a].m_dwTextureType);
				if (typeAsString == "unknown") continue;
//				uint textureSize = 0;
				string textureName = Other.ReadStringAtOffset(texture[a].m_pName, br);
				textureName = Path.GetFileName(textureName.Replace(':', '\\'));

				// this code was moved from Rage Console Texture Editor

				int H = 0;
				int W = 0;
				int T = 0;
				int dwEndian = 0;
				bool bSwizzled;
				int dwSize = 0;
				if (typeAsString == "DXT1")
				{
					W = texture[a].m_dwWidth;
					H = texture[a].m_dwHeight;
					if (W % 128 != 0) W = W + (128 - W % 128);
					if (H % 128 != 0) H = H + (128 - H % 128);
					dwSize = W * H / 2;
					W = texture[a].m_dwWidth  / 4;
					H = texture[a].m_dwHeight / 4;
					T = 8;
					dwEndian = 1;
					bSwizzled = true;
				}
				else if (typeAsString == "DXT3")
				{
					W = texture[a].m_dwWidth;
					H = texture[a].m_dwHeight;
					if (W % 128 != 0) W = W + (128 - W % 128);
					if (H % 128 != 0) H = H + (128 - H % 128);
					dwSize = W * H;
					W = texture[a].m_dwWidth / 4;
					H = texture[a].m_dwHeight / 4;
					T = 16;
					dwEndian = 1;
					bSwizzled = true;
				}
				else if (typeAsString == "DXT5")
				{
					W = texture[a].m_dwWidth;
					H = texture[a].m_dwHeight;
					if (W % 128 != 0) W = W + (128 - W % 128);
					if (H % 128 != 0) H = H + (128 - H % 128);
					dwSize = W * H;
					W = texture[a].m_dwWidth / 4;
					H = texture[a].m_dwHeight / 4;
					T = 16;
					dwEndian = 1;
					bSwizzled = true;
				}
				else if (typeAsString == "DXT5A")
				{
					W = texture[a].m_dwWidth;
					H = texture[a].m_dwHeight;
					if (W % 128 != 0) W = W + (128 - W % 128);
					if (H % 128 != 0) H = H + (128 - H % 128);
					dwSize = W * H;
					W = texture[a].m_dwWidth / 4;
					H = texture[a].m_dwHeight / 4;
					T = 8;
					dwEndian = 1;
					bSwizzled = true;
				}
				else if (typeAsString == "8")
				{
					W = texture[a].m_dwWidth;
					H = texture[a].m_dwHeight;
					if (W % 128 != 0) W = W + (128 - W % 128);
					if (H % 128 != 0) H = H + (128 - H % 128);
					dwSize = W * H;
					W = texture[a].m_dwWidth;
					H = texture[a].m_dwHeight;
					T = 1;
					dwEndian = 1;
					bSwizzled = true;
				}
				else if (typeAsString == "8888")
				{
					W = texture[a].m_dwWidth;
					H = texture[a].m_dwHeight;
					if (W % 128 != 0) W = W + (128 - W % 128);
					if (H % 128 != 0) H = H + (128 - H % 128);
					dwSize = W * H*4;
					W = texture[a].m_dwWidth / 4;
					H = texture[a].m_dwHeight / 4;
					T = 4;
					dwEndian = 2;
					bSwizzled = true;
				}
				else if (typeAsString == "DXN")
				{
					W = texture[a].m_dwWidth;
					H = texture[a].m_dwHeight;
					if (W % 128 != 0) W = W + (128 - W % 128);
					if (H % 128 != 0) H = H + (128 - H % 128);
					dwSize = W * H * 4;
					W = texture[a].m_dwWidth / 4;
					H = texture[a].m_dwHeight / 4;
					T = 4;
					dwEndian = 2;
					bSwizzled = true;
				}

				Array.Resize<byte>(ref pixelData, (int)dwSize);
				br.Position = bitMap[a].m_pPixelData;
				pixelData = br.ReadBytes((int)dwSize);

				byte[] changedEndianBuffer = new byte[dwSize];
				uint of = 0;
				if (dwEndian == 1)
				{
					/// swap two bytes in each value
					for (int i = 0; i < dwSize / 2; i++)
					{
						for (int x = 0; x < 2; x++)changedEndianBuffer[(1 - x) + of] = pixelData[x + of];
						of += 2;
					}
				}
				else if (dwEndian == 2)
				{
					/// swap four bytes in each value
					for (int i = 0; i < dwSize / 4; i++)
					{
						for (int x = 0; x < 4; x++)changedEndianBuffer[(3 - x) + of] = pixelData[x + of];
						of += 4;
					}
				}
				else changedEndianBuffer = pixelData;
				byte[] unSwizzledBuffer = new byte[dwSize];
				int off = 0;
				for (int Y = 0; Y < H; Y++)
				{
					for (int X = 0; X < W; X++)
					{
						off = 0;
						off = XGAddress2DTiledOffset(X, Y, W, T);
						Console.WriteLine($"{Y} {X}");
						Buffer.BlockCopy(changedEndianBuffer, off * T, unSwizzledBuffer, (X + Y * W) * T, T);
					}
				}

				string fileName = Path.GetFileName(Program.fileName.Substring(0, Program.fileName.Length - Program.fileMask.Length));
				string path = $"{Path.GetDirectoryName(Program.fileName.Substring(0, Program.fileName.Length - Program.fileMask.Length))}\\{fileName}\\";

				if (!Directory.Exists(path.Substring(0, path.Length - 1))) Directory.CreateDirectory(path.Substring(0, path.Length - 1));

				DDS.BuildDDS(unSwizzledBuffer, texture[a].m_dwWidth, texture[a].m_dwHeight, (int)bitMap[a].m_dwTextureType, (uint)dwSize, 
					$"{Path.GetDirectoryName(Program.fileName.Substring(0, Program.fileName.Length - Program.fileMask.Length))}\\{fileName}\\{textureName}");

			}
			return true;
		}
	}
}