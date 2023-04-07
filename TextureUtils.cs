using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using SharpDX.Direct3D11;
using SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct3D9;
using Converter.Utils;
using static SharpDX.Toolkit.Graphics.PixelFormat;

namespace Converter
{
	internal class TextureUtils
	{
		public static int XGAddress2DTiledOffset(int x, int y, int w, int TexelPitch) // функция перенесена из консольного редактора текстур Дагерона
		{
			int alignedWidth, logBpp, Macro, Micro, Offset;
			alignedWidth = (w + 31) & ~31;
			logBpp = (TexelPitch >> 2) + ((TexelPitch >> 1) >> (TexelPitch >> 2));
			Macro = ((x >> 5) + (y >> 5) * (alignedWidth >> 5)) << (logBpp + 7);
			Micro = (((x & 7) + ((y & 6) << 2)) << logBpp);
			Offset = Macro + ((Micro & ~15) << 1) + (Micro & 15) + ((y & 8) << (3 + logBpp)) + ((y & 1) << 4);
			return (((Offset & ~511) << 3) + ((Offset & 448) << 2) + (Offset & 63) + ((y & 16) << 7) + (((((y & 8) >> 2) + (x >> 3)) & 3) << 6)) >> logBpp;
		}
		public static int CalculateTextureSize(int width, int height, int mips, string typeAsString)
		{
			int H = 0;
			int W = 0;
			int allSize = 0;
			int mainTextureSize = 0;
			int allMipsSize;
			int dwSize = 0;
			int T = 0;
			int endian = 0;
			bool swizzled = false;
			if (mips == 0) mips = 1;
			for (int a = 0; a < mips; a++)
			{
				if (width < 16 || height < 16) break;
				GetRawTextureInfo(width, height, typeAsString, ref H, ref W, ref T, ref swizzled, ref endian, ref dwSize);
				allSize += dwSize;
				width /= 2;
				height /= 2;
			}
			return allSize;
		}
		public static int CalculateFirstTextureLevelSize(int width, int height, string typeAsString, bool forXbox = true)
		{
			int H = 0;
			int W = 0;
			int T = 0;
			int endian = 0;
			bool swizzled = false;
			int dwSize = 0;
			GetRawTextureInfo(width, height, typeAsString, ref H, ref W, ref T, ref swizzled, ref endian, ref dwSize, forXbox:forXbox);
			return dwSize;
		}
		public static string GetFormat(uint dwTextureType) // from RAGE Console Texture Editor
		{
			switch (dwTextureType)
			{
				case 2: return "8";
				case 6: return "8_8_8_8";
				case 15: return "4_4_4_4";
				case 18: return "DXT1";
				case 19: return "DXT2_3";
				case 20: return "DXT4_5";
				case 32: return "16_16_16_16_FLOAT";
				case 59: return "DXT5A";
				//case 49: return "DXN";
				default: return "unknown";
			}
		}
		public static int GetByteByFormat(string typeAsString)
		{
			switch (typeAsString)
			{
				case "8": return 2;
				case "8_8_8_8": return 6;
				case "DXT1": return 18;
				case "DXT2_3": return 19;
				case "DXT4_5": return 20;
				case "16_16_16_16_FLOAT": return 32;
				case "DXT5A": return 59;
				//case 49: return "DXN";
				default: return 0;
			}
		}
		public static void ReadPixelData(EndianBinaryReader br, uint pPixelData, uint pMips, uint lodCount, ref byte[] pixelData, int width, int height, string typeAsString)
		{
			// узнаем новый размер.

			int H = 0;
			int W = 0;
			int T = 0;
			int Constant = 128;
			int dwSize = 0;
			int sizePC = 0;
			int dwEndian = 0;
			bool bSwizzled = false;

			GetRawTextureInfo(width, height, typeAsString, ref H, ref W, ref T, ref bSwizzled, ref dwEndian, ref sizePC, forXbox: false);
			GetRawTextureInfo(width, height, typeAsString, ref H, ref W, ref T, ref bSwizzled, ref dwEndian, ref dwSize);
			Array.Resize<byte>(ref pixelData, 0);

			int newSize = pixelData.Length;
			uint currentLodOffset = pPixelData;

			bool texture8 = false;
			bool texture4 = false;
			if ((width == 8 || height == 8) && !(width == 4 || height == 4)) texture8 = true;
			else if (width == 4 || height == 4) texture4 = true;

			for (int a = 0; a < lodCount; a++) // 1 это главный лод. 2 и дальше - мип-маппинг
			{
				//if (a == 0) currentLodOffset = pPixelData;
				br.Position = currentLodOffset;

				if ((width < 32 || height < 32)&&!(width>128||height>128))
				{
					if (width <= height)
					{
						byte[] smallMips = br.ReadBytes(dwSize);
						if (smallMips.Length != dwSize)
						{
							Log.ToLog(Log.MessageType.ERROR, $"{smallMips.Length}, {dwSize} | wrong size. Increased to the required, but some information may be lost");
                            Console.WriteLine("Press any key...");
							Array.Resize<byte>(ref smallMips, dwSize);
							Console.ReadKey();
                        }
						UnswizzleTexture(ref smallMips, 128, 128, typeAsString);
						byte[] mip0 = new byte[0];
						byte[] mip1 = new byte[0];
						byte[] mip2 = new byte[0];
						byte[] mip3 = new byte[0];
						byte[] mip4 = new byte[0];
						if (!(texture8 || texture4) &&a < lodCount)
						{
							Array.Resize<byte>(ref mip0, sizePC);
							sizePC /= 4;
							width /= 2;
							height /= 2;
							a++;
						}
						if (!texture4 && a < lodCount)
						{
							Array.Resize<byte>(ref mip1, sizePC);
							sizePC /= 4;
							width /= 2;
							height /= 2;
							a++;
						}
						if ((width == 4 || height == 4)&&a < lodCount)
						{
							Array.Resize<byte>(ref mip2, sizePC);
							sizePC /= 4;
							width /= 2;
							height /= 2;
							a++;
						}
						if (a < lodCount)
						{
							Array.Resize<byte>(ref mip3, sizePC);
							sizePC /= 4;
							width /= 2;
							height /= 2;
							a++;
						}
						if (a < lodCount)
						{
							Array.Resize<byte>(ref mip4, sizePC);
							sizePC /= 4;
							width /= 2;
							height /= 2;
							a++;
						}
						int currentPosInMip0 = 0;
						int currentPosInMip1 = 0;
						int currentPosInMip2 = 0;
						int currentPosInMip3 = 0;
						int currentPosInMip4 = 0;
						int currentPosInTexpDDS = 0;
						int W2 = W > 128 ? W : 128;
						for (int x = 0; x < W2; x++)
						{
							currentPosInTexpDDS += T;
							if (mip2.Length > currentPosInMip2) System.Buffer.BlockCopy(smallMips, currentPosInTexpDDS, mip2, currentPosInMip2, T);
							currentPosInMip2 += T;
							currentPosInTexpDDS += T;

							if (mip1.Length > currentPosInMip1) System.Buffer.BlockCopy(smallMips, currentPosInTexpDDS, mip1, currentPosInMip1, T * 2);
							currentPosInMip1 += (T * 2);
							currentPosInTexpDDS += (T * 2);

							if (mip0.Length > currentPosInMip0) System.Buffer.BlockCopy(smallMips, currentPosInTexpDDS, mip0, currentPosInMip0, T * 4);
							currentPosInMip0 += (T * 4);
							currentPosInTexpDDS += (T * 4);

							currentPosInTexpDDS += (T * 24);
						}
						if (mip0.Length != 0)
						{
							Array.Resize<byte>(ref pixelData, pixelData.Length + mip0.Length);
							System.Buffer.BlockCopy(mip0, 0, pixelData, pixelData.Length - mip0.Length, mip0.Length);
						}
						if (mip1.Length != 0)
						{
							Array.Resize<byte>(ref pixelData, pixelData.Length + mip1.Length);
							System.Buffer.BlockCopy(mip1, 0, pixelData, pixelData.Length - mip1.Length, mip1.Length);
						}
						if (mip2.Length != 0)
						{
							Array.Resize<byte>(ref pixelData, pixelData.Length + mip2.Length);
							System.Buffer.BlockCopy(mip2, 0, pixelData, pixelData.Length - mip2.Length, mip2.Length);
						}

						break;
					}
					else
					{
						byte[] smallMips = br.ReadBytes(dwSize);
						UnswizzleTexture(ref smallMips, 128, 128, typeAsString);
						byte[] mip0 = new byte[0];
						byte[] mip1 = new byte[0];
						byte[] mip2 = new byte[0];
						byte[] mip3 = new byte[0];
						byte[] mip4 = new byte[0];
						int mip0Height = 1;
						int mip1Height = 1;
						int mip2Height = 1;
						int mip3Height = 1;
						int mip4Height = 1;
						if (a < lodCount)
						{
							Array.Resize<byte>(ref mip0, sizePC);
							mip0Height = height;
							sizePC /= 4;
							width /= 2;
							height /= 2;
							a++;
						}
						if (a < lodCount)
						{
							Array.Resize<byte>(ref mip1, sizePC);
							mip1Height = height;
							sizePC /= 4;
							width /= 2;
							height /= 2;
							a++;
						}
						if (a < lodCount)
						{
							Array.Resize<byte>(ref mip2, sizePC);
							mip2Height = height;
							sizePC /= 4;
							width /= 2;
							height /= 2;
							a++;
						}
						if (a < lodCount)
						{
							Array.Resize<byte>(ref mip3, sizePC);
							mip3Height = height;
							sizePC /= 4;
							width /= 2;
							height /= 2;
							a++;
						}
						if (a < lodCount)
						{
							Array.Resize<byte>(ref mip4, sizePC);
							mip4Height = height;
							sizePC /= 4;
							width /= 2;
							height /= 2;
							a++;
						}

						int currentPosInMip0 = 0;
						int currentPosInMip1 = 0;
						int currentPosInMip2 = 0;
						int currentPosInMip3 = 0;
						int currentPosInMip4 = 0;
						int currentPosInTexpDDS = 0;
						int mip0GroupSize = ((mip0.Length / mip0Height) / 2) * 8;
						int mip1GroupSize = ((mip1.Length / mip1Height) / 2) * 8;
						int mip2GroupSize = ((mip2.Length / mip2Height) / 2) * 8;
						int mip3GroupSize = ((mip3.Length / mip3Height) / 2) * 8;
						int mip4GroupSize = ((mip4.Length / mip4Height) / 2) * 8;
						currentPosInTexpDDS += (0x100 / 8 * T); // пропускаем первый рядок
						int W2 = W > 128 ? W : 128;
						for (int x = 0; x < W2; x++)
						{
							if (mip2.Length > currentPosInMip2 && currentPosInTexpDDS >= (0x100 / 8 * T))
							{
								System.Buffer.BlockCopy(smallMips, currentPosInTexpDDS, mip2, currentPosInMip2, mip2GroupSize);
								currentPosInMip2 += mip2GroupSize;
								currentPosInTexpDDS += mip2GroupSize;
							}
							if (mip1.Length > currentPosInMip1 && currentPosInTexpDDS >= (0x200 / 8 * T))
							{
								System.Buffer.BlockCopy(smallMips, currentPosInTexpDDS, mip1, currentPosInMip1, mip1GroupSize);
								currentPosInMip1 += mip1GroupSize;
								currentPosInTexpDDS += mip1GroupSize;
							}
							if (mip0.Length > currentPosInMip0 && currentPosInTexpDDS >= (0x400 / 8 * T))
							{
								System.Buffer.BlockCopy(smallMips, currentPosInTexpDDS, mip0, currentPosInMip0, mip0GroupSize);
								currentPosInMip0 += mip0GroupSize;
								currentPosInTexpDDS += mip0GroupSize;
							}
							while (currentPosInTexpDDS % (32 * T) != 0) currentPosInTexpDDS++;
						}
						if (mip0.Length != 0)
						{
							Array.Resize<byte>(ref pixelData, pixelData.Length + mip0.Length);
							System.Buffer.BlockCopy(mip0, 0, pixelData, pixelData.Length - mip0.Length, mip0.Length);
						}
						if (mip1.Length != 0)
						{
							Array.Resize<byte>(ref pixelData, pixelData.Length + mip1.Length);
							System.Buffer.BlockCopy(mip1, 0, pixelData, pixelData.Length - mip1.Length, mip1.Length);
						}
						if (mip2.Length != 0)
						{
							Array.Resize<byte>(ref pixelData, pixelData.Length + mip2.Length);
							System.Buffer.BlockCopy(mip2, 0, pixelData, pixelData.Length - mip2.Length, mip2.Length);
						}

						break;

					}

				}

				byte[] tmpPixelData = br.ReadBytes(dwSize);
				DataUtils.ReverseBytes(ref tmpPixelData, dwEndian);
				if (bSwizzled)
				{
					byte[] unSwizzledBuffer = new byte[dwSize];
					int off = 0;
					for (int Y = 0; Y < H; Y++)
					{
						for (int X = 0; X < W; X++)
						{
							off = 0;
							off = TextureUtils.XGAddress2DTiledOffset(X, Y, W, T);
							System.Buffer.BlockCopy(tmpPixelData, off * T, unSwizzledBuffer, (X + Y * W) * T, T);
						}
					}
					System.Buffer.BlockCopy(unSwizzledBuffer, 0, tmpPixelData, 0, dwSize);
				}
				currentLodOffset += (uint)dwSize;
				Array.Resize<byte>(ref pixelData, pixelData.Length + sizePC);
				System.Buffer.BlockCopy(tmpPixelData, 0, pixelData, pixelData.Length- sizePC, sizePC);

				width = width / 2;
				height = height / 2;
				GetRawTextureInfo(width, height, typeAsString, ref H, ref W, ref T, ref bSwizzled, ref dwEndian, ref sizePC, forXbox: false);
				GetRawTextureInfo(width, height, typeAsString, ref H, ref W, ref T, ref bSwizzled, ref dwEndian, ref dwSize);
				if (a == 0) currentLodOffset = pMips;

			}
		}
		public static void GetRawTextureInfo(int width, int height, string typeAsString, ref int H, ref int W, ref int T, ref bool swizzled, ref int endian, ref int size, bool forXbox = true)
		{
			switch (typeAsString)
			{
				case "DXT1":
					W = width;
					H = height;
					if (forXbox)
					{
						if (W % 128 != 0) W = W + (128 - W % 128);
						if (H % 128 != 0) H = H + (128 - H % 128);
					}
					size = W * H / 2;
					W = width / 4;
					H = height / 4;
					T = 8;
					endian = 2;
					swizzled = true;
					break;
				case "DXT2_3":
					W = width;
					H = height;
					if (forXbox)
					{
						if (W % 128 != 0) W = W + (128 - W % 128);
						if (H % 128 != 0) H = H + (128 - H % 128);
					}
					size = W * H;
					W = width / 4;
					H = height / 4;
					T = 16;
					endian = 2;
					swizzled = true;
					break;
				case "DXT4_5":
					W = width;
					H = height;
					if (forXbox)
					{
						if (W % 128 != 0) W = W + (128 - W % 128);
						if (H % 128 != 0) H = H + (128 - H % 128);
					}
					size = W * H;
					W = width / 4;
					H = height / 4;
					T = 16;
					endian = 2;
					swizzled = true;
					break;
				case "DXT5A":
					W = width;
					H = height;
					if (forXbox)
					{
						if (W % 128 != 0) W = W + (128 - W % 128);
						if (H % 128 != 0) H = H + (128 - H % 128);
					}
					size = W * H;
					W = width / 4;
					H = height / 4;
					T = 8;
					endian = 2;
					swizzled = true;
					break;
				case "8":
					W = width;
					H = height;
					if (forXbox)
					{
						if (W % 128 != 0) W = W + (128 - W % 128);
						if (H % 128 != 0) H = H + (128 - H % 128);
					}
					size = W * H;
					W = width / 4;
					H = height / 4;
					T = 16;
					endian = 2;
					swizzled = true;
					break;
				case "8_8_8_8": // пофиксить
					W = width;
					H = height;
					if (forXbox)
					{
						if (W % 128 != 0) W = W + (128 - W % 128);
						if (H % 128 != 0) H = H + (128 - H % 128);
					}
					size = W * H/2;
					W = width / 4;
					H = height / 4;
					T = 8;
					endian = 2;
					swizzled = true;
					break;
				case "DXN":
					//	W = texture.m_dwWidth;
					//	H = texture.m_dwHeight;
					//	if (W % 128 != 0) W = W + (128 - W % 128);
					//	if (H % 128 != 0) H = H + (128 - H % 128);
					//	dwSize = W * H * 4;
					//	W = texture.m_dwWidth / 4;
					//	H = texture.m_dwHeight / 4;
					//	T = 4;
					//	dwEndian = 4;
					//	bSwizzled = true;
					break;
				case "16_16_16_16_FLOAT":
					W = width;
					H = height;
					if (forXbox)
					{
						if (W % 128 != 0) W = W + (128 - W % 128);
						if (H % 128 != 0) H = H + (128 - H % 128);
					}
					size = W * H * 4;
					W = width / 2;
					H = height / 2;
					T = 8;
					endian = 2;
					swizzled = true;
					break;
				case "4_4_4_4":
					W = width;
					H = height;
					if (forXbox)
					{
						if (W % 128 != 0) W = W + (128 - W % 128);
						if (H % 128 != 0) H = H + (128 - H % 128);
					}
					size = W * H / 2;
					W = width / 4;
					H = height / 4;
					T = 8;
					endian = 2;
					swizzled = true;
					break;
			}
		}

		public static void SwizzleTexture(ref byte[] pixelData, int width, int height, string typeAsString)
		{
			int H = 0;
			int W = 0;
			int T = 0;
			int dwEndian = 0;
			bool bSwizzled = false;
			int dwSize = 0;
			GetRawTextureInfo(width, height, typeAsString, ref H, ref W, ref T, ref bSwizzled, ref dwEndian, ref dwSize);
			byte[] swizzled = new byte[dwSize];
			int off = 0;

			for (int Y = 0; Y < H; Y++)
			{
				for (int X = 0; X < W; X++)
				{
					off = 0;
					off = TextureUtils.XGAddress2DTiledOffset(X, Y, W, T);
					System.Buffer.BlockCopy(pixelData, (X + Y * W) * T, swizzled, off * T, T);
				}
			}
			DataUtils.ReverseBytes(ref swizzled, dwEndian);
			Array.Resize<byte>(ref pixelData, dwSize);
			System.Buffer.BlockCopy(swizzled, 0, pixelData, 0, dwSize);
		}
		public static void UnswizzleTexture(ref byte[] pixelData, int width, int height, string typeAsString)
		{
			int H = 0;
			int W = 0;
			int T = 0;
			int dwEndian = 0;
			bool bSwizzled = false;
			int dwSize = 0;
			GetRawTextureInfo(width, height, typeAsString, ref H, ref W, ref T, ref bSwizzled, ref dwEndian, ref dwSize);
			byte[] swizzled = new byte[dwSize];
			int off = 0;
			for (int Y = 0; Y < H; Y++)
			{
				for (int X = 0; X < W; X++)
				{
					off = 0;
					off = TextureUtils.XGAddress2DTiledOffset(X, Y, W, T);
					System.Buffer.BlockCopy(pixelData, off * T, swizzled, (X + Y * W) * T, T);
				}
			}
			DataUtils.ReverseBytes(ref swizzled, dwEndian);
			//DataUtils.ReverseBytes(ref pixelData, dwEndian);
			Array.Resize<byte>(ref pixelData, dwSize);
			System.Buffer.BlockCopy(swizzled, 0, pixelData, 0, dwSize);
		}

	}
}
