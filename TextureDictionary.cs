using ConsoleApp1;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
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
		/*public static uint GetMipsOffset( int dwTextureType, int dwWidth, int dwHeight, uint dwMipMaps, uint dwOffset, uint dwMipsOffset, string typeAsString)
		{
			if (dwMipMaps == 1) return dwOffset;
			if (dwMipMaps == 2)
			{
				dwWidth = dwWidth / 2;
				dwHeight = dwHeight / 2;
				return dwMipsOffset;
			}
			int H = 0;
			int W = 0;
			int T = 0;
			int Constant = 128;
			int dwSize = 0;
			if (dwMipMaps > 2)
			{
				for (int i = 0; i < dwMipMaps - 2; i++)
				{
					if (typeAsString == "DXT1")
					{
						W = dwWidth;
						H = dwHeight;
						if (W % Constant != 0) W = W + (Constant - W % Constant);
						if (H % Constant != 0) H = H + (Constant - H % Constant);
						dwSize = W * H / 2;
					}
					else if (typeAsString == "DXT3")
					{
						W = dwWidth;
						H = dwHeight;
						if (W % Constant != 0) W = W + (Constant - W % Constant);
						if (H % Constant != 0) H = H + (Constant - H % Constant);
						dwSize = W * H;
					}
					else if (typeAsString == "DXT5")
					{
						W = dwWidth;
						H = dwHeight;
						if (W % Constant != 0) W = W + (Constant - W % Constant);
						if (H % Constant != 0) H = H + (Constant - H % Constant);
						dwSize = W * H;
					}
					else if (typeAsString == "8")
					{
						W = dwWidth;
						H = dwHeight;
						if (W % Constant != 0) W = W + (Constant - W % Constant);
						if (H % Constant != 0) H = H + (Constant - H % Constant);
						dwSize = W * H;
					}
					else if (typeAsString == "DXT5A")
					{
						W = dwWidth;
						H = dwHeight;
						if (W % Constant != 0) W = W + (Constant - W % Constant);
						if (H % Constant != 0) H = H + (Constant - H % Constant);
						dwSize = W * H * 4;
					}
					else if (typeAsString == "8888")
					{
						W = dwWidth;
						H = dwHeight;
						if (W % Constant != 0) W = W + (Constant - W % Constant);
						if (H % Constant != 0) H = H + (Constant - H % Constant);
						dwSize = W * H * 4;
					}
					dwWidth = dwWidth / 2;
					dwHeight = dwHeight / 2;
					if (i != 0) dwMipsOffset = (uint)(dwMipsOffset + dwSize);
				}
				return dwMipsOffset;
			}
			return 0;

		}*/


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
				string textureName = DataUtils.ReadStringAtOffset(texture[a].m_pName, br);
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
					dwEndian = 2;
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
					dwEndian = 2;
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
					dwEndian = 2;
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
					dwEndian = 2;
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
					dwEndian = 2;
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
					dwEndian = 4;
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
					dwEndian = 4;
					bSwizzled = true;
				}

				Array.Resize<byte>(ref pixelData, (int)dwSize);
				br.Position = bitMap[a].m_pPixelData;
				pixelData = br.ReadBytes((int)dwSize);
				DataUtils.ReverseBytes(ref pixelData, dwEndian);
				byte[] unSwizzledBuffer = new byte[dwSize];
				int off = 0;
				for (int Y = 0; Y < H; Y++)
				{
					for (int X = 0; X < W; X++)
					{
						off = 0;
						off = XGAddress2DTiledOffset(X, Y, W, T);
						Buffer.BlockCopy(pixelData, off * T, unSwizzledBuffer, (X + Y * W) * T, T);
					}
				}
				Buffer.BlockCopy(unSwizzledBuffer, 0, pixelData, 0, unSwizzledBuffer.Length);

				string path = $"{FileInfo.filePath}\\{FileInfo.baseFileName}\\";
				if (!Directory.Exists(path.Substring(0, path.Length - 1))) Directory.CreateDirectory(path.Substring(0, path.Length - 1));

				DDS.BuildDDS(pixelData, texture[a].m_dwWidth, texture[a].m_dwHeight, (int)bitMap[a].m_dwTextureType, (uint)dwSize, 
					$"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{textureName}", texture[a].m_Lod);

			}
			return true;
		}
	}
}
