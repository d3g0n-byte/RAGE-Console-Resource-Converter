using Converter.Utils;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharpDX.Toolkit.Graphics.PixelFormat;

namespace Converter.Test
{
	internal class EditorFunc
	{
		public static void WriteNewTexturesToBuffer(uint pTexture, ref RageResource.Texture texture, ref RageResource.BitMap bitMap, ref byte[] sysBuffer)
		{

			RDR_XTD_CREATOR.currentPosInSysBuffer = pTexture;
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._vmt.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._f4.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._f8, ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._f9, ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._fA.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._fC.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._f10.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._f14.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, (texture.m_pName + 0x50000000).Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, (texture.m_pBitmap + 0x50000000).Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture.m_dwWidth.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture.m_dwHeight.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture.m_Lod.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._f28.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._f2C.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._f30.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._f34.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._f38.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, texture._f3C.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);

			RDR_XTD_CREATOR.currentPosInSysBuffer = texture.m_pBitmap;
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._f0.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._f4.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._f8.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._fC.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._f10.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._f14.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._f18.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._f1C.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, (bitMap.m_pPixelData + 0x60000000).Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._f24.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._f26.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._f28.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._f2C.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);
			RDR_XTD_CREATOR.WriteValueToByteArray(ref sysBuffer, bitMap._f30.Reverse(), ref RDR_XTD_CREATOR.currentPosInSysBuffer);

		}
		public static void WriteNewTextures(RDR_XTD_CREATOR.XmlInfo xmlInfo, ref RageResource.Texture texture, ref RageResource.BitMap bitMap, ref byte[] gfxBuffer)
		{
			EndianBinaryReader br2 = new EndianBinaryReader(File.OpenRead(xmlInfo.path));
			br2.Position = 0x1c;
			uint ddsMipsCount = br2.ReadUInt32();
			if (ddsMipsCount == 0) ddsMipsCount = 1;
			texture.m_dwWidth = (ushort)xmlInfo.width;
			texture.m_dwHeight = (ushort)xmlInfo.height;
			texture.m_Lod = (uint)xmlInfo.mipCount> ddsMipsCount? ddsMipsCount: (uint)xmlInfo.mipCount;
			texture._f14 = TextureUtils.CalculateTextureSize(texture.m_dwWidth, texture.m_dwHeight, (int)texture.m_Lod, xmlInfo.typeAsString);
			int textureSize = TextureUtils.CalculateFirstTextureLevelSize(texture.m_dwWidth, texture.m_dwHeight, xmlInfo.typeAsString);
			int textureSizeToRead = TextureUtils.CalculateFirstTextureLevelSize(texture.m_dwWidth, texture.m_dwHeight, xmlInfo.typeAsString, forXbox: false);
			byte[] pixelDataOrig = new byte[textureSizeToRead];
			br2.Position = 0x80;
			pixelDataOrig = br2.ReadBytes(textureSizeToRead);
			TextureUtils.SwizzleTexture(ref pixelDataOrig, texture.m_dwWidth, texture.m_dwHeight, xmlInfo.typeAsString);
			bitMap._f1C = (bitMap._f1C & 0x00ffffff);
			bool textureIsCorrect = false;
			for (int b = 2; b < 12; b++) if (texture.m_dwWidth == Math.Pow(2, b) || texture.m_dwHeight == Math.Pow(2, b)) textureIsCorrect = true;
			if (textureIsCorrect) bitMap._f1C += 2147483648;

			if (texture.m_dwWidth > 4096/*|| texture[a].m_dwHeight> 4096*/) bitMap._f1C += 1073741824; // 0x40
			else if (texture.m_dwWidth > 2048/*|| texture[a].m_dwHeight>2048*/) bitMap._f1C += 536870912; // 0x20
			else if (texture.m_dwWidth > 1024/*|| texture[a].m_dwHeight>1024*/) bitMap._f1C += 268435456; // 0x10
			else if (texture.m_dwWidth > 512/* || texture[a].m_dwHeight > 512*/) bitMap._f1C += 134217728; // 8
			else if (texture.m_dwWidth > 256/*|| texture[a].m_dwHeight>256*/) bitMap._f1C += 67108864; // 4
			else if (texture.m_dwWidth > 128/*|| texture[a].m_dwHeight>128*/) bitMap._f1C += 33554432; // 2
			else bitMap._f1C += 33554432; // 1
			bitMap._f24 = (ushort)((texture.m_dwHeight / 8) - 1);
			bitMap._f26 = (ushort)((texture.m_dwWidth - 1) + 0xE000);

			bitMap._f2C = 0;
			if (textureIsCorrect)
			{
				int tmpSize = 8;
				while (tmpSize <= texture.m_dwHeight)
				{
					bitMap._f2C += 0x40;
					tmpSize *= 2;
				}
			}
			if((texture.m_dwWidth < 32 || texture.m_dwHeight < 32) && !(texture.m_dwWidth > 128 || texture.m_dwHeight > 128)|| texture.m_Lod<2)
				bitMap._f30 = 512;// 0x00000200
			else
			{
				bitMap._f30 = (bitMap._f30 & 0xF0000FFF);
				bitMap._f30 += RDR_XTD_CREATOR.currentPosInGfxBuffer;
			}

			if (texture.m_Lod > 1)
			{
				int currentMipPos = (int)RDR_XTD_CREATOR.currentPosInGfxBuffer;
				int currentMipSize = textureSizeToRead;
				int currentWidth = texture.m_dwWidth;
				int currentHeight = texture.m_dwHeight;
				for (int b = 1; b < texture.m_Lod; b++) // нужно проверить
				{
					currentMipSize /= 4;
					currentWidth /= 2;
					currentHeight /= 2;
					if (currentWidth < 32 || currentHeight < 32)
					{
						if (currentWidth <= currentHeight)
						{
							byte[] mip0 = new byte[1];
							byte[] mip1 = new byte[1];
							byte[] mip2 = new byte[1];
							byte[] mip3 = new byte[1];
							byte[] mip4 = new byte[1];

							if (b < texture.m_Lod)
							{
								Array.Resize<byte>(ref mip0, currentMipSize);
								mip0 = br2.ReadBytes(currentMipSize);
								currentMipSize /= 4;
								currentWidth /= 2;
								currentHeight /= 2;
								b++;
							}
							if (b < texture.m_Lod)
							{
								Array.Resize<byte>(ref mip1, currentMipSize);
								mip1 = br2.ReadBytes(currentMipSize);
								currentMipSize /= 4;
								currentWidth /= 2;
								currentHeight /= 2;
								b++;
							}
							if (b < texture.m_Lod)
							{
								Array.Resize<byte>(ref mip2, currentMipSize);
								mip2 = br2.ReadBytes(currentMipSize);
								currentMipSize /= 4;
								currentWidth /= 2;
								currentHeight /= 2;
								b++;
							}
							if (b < texture.m_Lod)
							{
								Array.Resize<byte>(ref mip3, currentMipSize);
								mip3 = br2.ReadBytes(currentMipSize);
								currentMipSize /= 4;
								currentWidth /= 2;
								currentHeight /= 2;
								b++;
							}
							if (b < texture.m_Lod)
							{
								Array.Resize<byte>(ref mip4, currentMipSize);
								mip4 = br2.ReadBytes(currentMipSize);
								currentMipSize /= 4;
								currentWidth /= 2;
								currentHeight /= 2;
								b++;
							}
							int T = 0;
							int H = 0;
							int W = 0;
							bool Swizzled = false;
							int Endian = 0;
							int size = 0;
							TextureUtils.GetRawTextureInfo(128, 128, xmlInfo.typeAsString, ref H, ref W, ref T, ref Swizzled, ref Endian, ref size, forXbox: false);
							int currentPosInMip0 = 0;
							int currentPosInMip1 = 0;
							int currentPosInMip2 = 0;
							int currentPosInMip3 = 0;
							int currentPosInMip4 = 0;

							byte[] tempDDS = new byte[size];
							int currentPosInTexpDDS = 0;
							int W2 = W > 128 ? W : 128;
							for (int x = 0; x < W2; x++)
							{
								if (mip2.Length > currentPosInMip2)
								{
									Buffer.BlockCopy(mip2, currentPosInMip2, tempDDS, currentPosInTexpDDS, T);
									currentPosInTexpDDS += T; // tmp
									Buffer.BlockCopy(mip2, currentPosInMip2, tempDDS, currentPosInTexpDDS, T);
								}
								else currentPosInTexpDDS += T; // tmp
								currentPosInMip2 += T;
								currentPosInTexpDDS += T;

								if (mip1.Length > currentPosInMip1) Buffer.BlockCopy(mip1, currentPosInMip1, tempDDS, currentPosInTexpDDS, T * 2);
								currentPosInMip1 += (T * 2);
								currentPosInTexpDDS += (T * 2);

								if (mip0.Length > currentPosInMip0) Buffer.BlockCopy(mip0, currentPosInMip0, tempDDS, currentPosInTexpDDS, T * 4);
								currentPosInMip0 += (T * 4);
								currentPosInTexpDDS += (T * 4);

								currentPosInTexpDDS += (T * 24);
							}
							TextureUtils.SwizzleTexture(ref tempDDS, 128, 128, xmlInfo.typeAsString);
							RDR_XTD_CREATOR.WriteToByteArray(ref gfxBuffer, tempDDS, ref RDR_XTD_CREATOR.currentPosInGfxBuffer);
							break;
						}
						else
						{
							byte[] mip0 = new byte[1];
							byte[] mip1 = new byte[1];
							byte[] mip2 = new byte[1];
							byte[] mip3 = new byte[1];
							byte[] mip4 = new byte[1];
							int mip0Height = -1;
							int mip1Height = -1;
							int mip2Height = -1;
							int mip3Height = -1;
							int mip4Height = -1;
							if (b < texture.m_Lod)
							{
								Array.Resize<byte>(ref mip0, currentMipSize);
								mip0 = br2.ReadBytes(currentMipSize);
								mip0Height = currentHeight;
								currentMipSize /= 4;
								currentWidth /= 2;
								currentHeight /= 2;
								b++;
							}
							if (b < texture.m_Lod)
							{
								Array.Resize<byte>(ref mip1, currentMipSize);
								mip1Height = currentHeight;
								mip1 = br2.ReadBytes(currentMipSize);
								currentMipSize /= 4;
								currentWidth /= 2;
								currentHeight /= 2;
								b++;
							}
							if (b < texture.m_Lod)
							{
								Array.Resize<byte>(ref mip2, currentMipSize);
								mip2 = br2.ReadBytes(currentMipSize);
								mip2Height = currentHeight;
								currentMipSize /= 4;
								currentWidth /= 2;
								currentHeight /= 2;
								b++;
							}
							if (b < texture.m_Lod)
							{
								Array.Resize<byte>(ref mip3, currentMipSize);
								mip3 = br2.ReadBytes(currentMipSize);
								mip3Height = currentHeight;
								currentMipSize /= 4;
								currentWidth /= 2;
								currentHeight /= 2;
								b++;
							}
							if (b < texture.m_Lod)
							{
								Array.Resize<byte>(ref mip4, currentMipSize);
								mip4 = br2.ReadBytes(currentMipSize);
								mip4Height = currentHeight;
								currentMipSize /= 4;
								currentWidth /= 2;
								currentHeight /= 2;
								b++;
							}
							int T = 0;
							int H = 0;
							int W = 0;
							bool Swizzled = false;
							int Endian = 0;
							int size = 0;
							TextureUtils.GetRawTextureInfo(128, 128, xmlInfo.typeAsString, ref H, ref W, ref T, ref Swizzled, ref Endian, ref size, forXbox: false);
							int currentPosInMip0 = 0;
							int currentPosInMip1 = 0;
							int currentPosInMip2 = 0;
							int currentPosInMip3 = 0;
							int currentPosInMip4 = 0;
							byte[] tempDDS = new byte[size];
							int currentPosInTexpDDS = 0;
							int mip0GroupSize = ((mip0.Length / mip0Height) / 2) * 8;
							int mip1GroupSize = ((mip1.Length / mip1Height) / 2) * 8;
							int mip2GroupSize = ((mip2.Length / mip2Height) / 2) * 8;
							int mip3GroupSize = ((mip3.Length / mip3Height) / 2) * 8;
							int mip4GroupSize = ((mip4.Length / mip4Height) / 2) * 8;
							currentPosInTexpDDS += (0x100 / 8 * T); // пропускаем первый рядок
							int H2 = H > 128 ? H : 128;
							for (int x = 0; x < H2; x++)
							{
								if (mip2.Length > currentPosInMip2 && currentPosInTexpDDS >= (0x100 / 8 * T))
								{
									Buffer.BlockCopy(mip2, currentPosInMip2, tempDDS, currentPosInTexpDDS, mip2GroupSize);
									currentPosInMip2 += mip2GroupSize;
									currentPosInTexpDDS += mip2GroupSize;
								}
								if (mip1.Length > currentPosInMip1 && currentPosInTexpDDS >= (0x200 / 8 * T))
								{
									Buffer.BlockCopy(mip1, currentPosInMip1, tempDDS, currentPosInTexpDDS, mip1GroupSize);
									currentPosInMip1 += mip1GroupSize;
									currentPosInTexpDDS += mip1GroupSize;
								}
								if (mip0.Length > currentPosInMip0 && currentPosInTexpDDS >= (0x400 / 8 * T))
								{
									Buffer.BlockCopy(mip0, currentPosInMip0, tempDDS, currentPosInTexpDDS, mip0GroupSize);
									currentPosInMip0 += mip0GroupSize;
									currentPosInTexpDDS += mip0GroupSize;
								}
								while (currentPosInTexpDDS % (32 * T) != 0) currentPosInTexpDDS++;
							}
							TextureUtils.SwizzleTexture(ref tempDDS, 128, 128, xmlInfo.typeAsString);
							RDR_XTD_CREATOR.WriteToByteArray(ref gfxBuffer, tempDDS, ref RDR_XTD_CREATOR.currentPosInGfxBuffer);
							break;


						}
						//else { texture[a].m_Lod = (uint)b; break; } // временный фикс малого размера
					}
					byte[] mipLevel = br2.ReadBytes(currentMipSize);
					//br2.ReadBytes(currentMipSize);
					TextureUtils.SwizzleTexture(ref mipLevel, currentWidth, currentHeight, xmlInfo.typeAsString);
					RDR_XTD_CREATOR.WriteToByteArray(ref gfxBuffer, mipLevel, ref RDR_XTD_CREATOR.currentPosInGfxBuffer);
				}
			}
			br2.Close();
			RDR_XTD_CREATOR.AlignGFX(0x00000100);
			// меняем тип текстуры 
			int newType = TextureUtils.GetByteByFormat(xmlInfo.typeAsString);
			if (newType == 0) throw new Exception($"unk type {xmlInfo.typeAsString}");
			uint oldPointer = bitMap.originalPoint;
			oldPointer = oldPointer >> 6; // удаляем старый тип
			oldPointer = oldPointer << 6;
			oldPointer += (uint)newType; // добавляем новый тип
			bitMap.originalPoint = oldPointer;
			bitMap.m_pPixelData = bitMap.originalPoint & 0x000000FF;// оставляем тип текстуры
			bitMap.m_pPixelData += RDR_XTD_CREATOR.currentPosInGfxBuffer;
			RDR_XTD_CREATOR.WriteToByteArray(ref gfxBuffer, pixelDataOrig, ref RDR_XTD_CREATOR.currentPosInGfxBuffer);
			RDR_XTD_CREATOR.AlignGFX(0x00010000); // выравниваем для следующего уровня
		}
	}
}
