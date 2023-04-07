using ConsoleApp1;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Converter.RageResource;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;
using SharpDX.IO;
using SharpDX;
using SharpDX.Direct3D11;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using SharpDX.Direct3D9;
using System.Reflection.Emit;
using static System.Net.Mime.MediaTypeNames;

namespace Converter
{
	internal class TextureDictionary
	{
		public static bool ReadTexture(RageResource.Texture texture, RageResource.BitMap bitMap, EndianBinaryReader br, uint hash, bool overwriteTexture = true)
		{
			string  typeAsString = TextureUtils.GetFormat(bitMap.m_dwTextureType);
			if (typeAsString == "unknown")
			{
				Log.ToLog(Log.MessageType.WARNING, $"Unknown texture type {bitMap.m_dwTextureType}. Send this file to the author d3g0n#8929 or on gtaforums so that he can add its support. Press any key to continue");
				Console.ReadKey();
				return true; }
			string textureName;
			if (texture.m_pName != 0)
			{
				textureName = DataUtils.ReadStringAtOffset(texture.m_pName, br);
				textureName = Path.GetFileName(textureName.Replace(':', '\\'));
				if (!textureName.EndsWith(".dds"))
				{
					Log.ToLog(Log.MessageType.WARNING, $"Texture name {textureName} does not contain in end \".dds\". Would you like to add(y or n)?");
					while (true)
					{
						ConsoleKey key = Console.ReadKey().Key;
						if (key == ConsoleKey.Y|| key == ConsoleKey.N)
						{
							if(key == ConsoleKey.Y) textureName += ".dds";
							Console.WriteLine();
							break;
						}
						Console.WriteLine();
					}
				}
			}
			else textureName = $"{hash.ToString("X8").ToLower()}.dds";
			uint endian;
			byte[] pixelData = new byte[1];
			TextureUtils.ReadPixelData(br, bitMap.m_pPixelData, bitMap.m_pMipsOffset, texture.m_Lod, ref pixelData, texture.m_dwWidth, texture.m_dwHeight, typeAsString);
			string path = $"{FileInfo.filePath}\\{FileInfo.baseFileName}\\";
			if (!Directory.Exists(path.Substring(0, path.Length - 1))) Directory.CreateDirectory(path.Substring(0, path.Length - 1));
			SharpDX.Toolkit.Graphics.ImageDescription description = new SharpDX.Toolkit.Graphics.ImageDescription();
			description.MipLevels = (int)texture.m_Lod;
			description.Width = texture.m_dwWidth;
			description.Height = texture.m_dwHeight;
			description.Format = typeAsString switch
			{
				"DXT1" => SharpDX.DXGI.Format.BC1_UNorm,
				"DXT2_3" => SharpDX.DXGI.Format.BC2_UNorm,
				"DXT4_5" => SharpDX.DXGI.Format.BC3_UNorm,
				"DXT5A" => SharpDX.DXGI.Format.B8G8R8A8_UNorm,
				"8" => SharpDX.DXGI.Format.R8_UNorm,
				"8_8_8_8" => SharpDX.DXGI.Format.R8G8B8A8_SNorm,
				"4_4_4_4" => SharpDX.DXGI.Format.B4G4R4A4_UNorm,
				"16_16_16_16_FLOAT" => SharpDX.DXGI.Format.R16G16B16A16_Float
			};
			description.Depth = 1;
			description.ArraySize = 1;
			description.Dimension = SharpDX.Toolkit.Graphics.TextureDimension.Texture2D;
			DataStream s = DataStream.Create(pixelData, true, true);
			SharpDX.Toolkit.Graphics.Image textureDDS = SharpDX.Toolkit.Graphics.Image.New(description, s.DataPointer);
            //Console.WriteLine($"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{textureName}");
			s.Close();
			if (!overwriteTexture)
			{
				if (File.Exists($"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{textureName}"))
				{
					int size = DataUtils.GetFileSize($"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{textureName}");
                     //Console.ReadKey();
                    if (size < (textureDDS.TotalSizeInBytes + 128)) // пофиксить
					{
						textureDDS.Save($"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{textureName}");
						Log.ToLog(Log.MessageType.WARNING, $"Texture with the same name was found, but because its size is smaller, it has been replaced with a new one");
					}
					else Log.ToLog(Log.MessageType.WARNING, $"Texture with the same name was found, but its size is greater than or equal to the new texture so it will not be replaced");

				}
				else textureDDS.Save($"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{textureName}");
			}
			else textureDDS.Save($"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{textureName}");
			textureDDS.Dispose();
			return true;
		}
		public static bool ReadTextureDictionary(EndianBinaryReader br, /*bool endian, */uint pointer, bool overwriteTexture = true)
		{

			br.Position = pointer;
			RageResource.XTDHeader header = new RageResource.XTDHeader();
			header = ReadRageResource.XTDHeader(br);
			uint[] hash = new uint[header.m_cHash.m_wCount];
			br.Position = header.m_cHash.m_pList;
			for (int a = 0; a < header.m_cHash.m_wCount; a++) hash[a] = br.ReadUInt32();
			uint[] pTexture = new uint[header.m_cTexture.m_wCount];// 
			br.Position = header.m_cTexture.m_pList;
			for (int a = 0; a < header.m_cTexture.m_wCount; a++) pTexture[a] = br.ReadOffset();
			RageResource.Texture[] texture = new RageResource.Texture[header.m_cTexture.m_wCount];
			RageResource.BitMap[] bitMap = new RageResource.BitMap[header.m_cTexture.m_wCount];

			Log.ToLog(Log.MessageType.INFO, $"Textures count: {header.m_cTexture.m_wCount}");
			for (int a = 0; a < header.m_cTexture.m_wCount; a++)
			{
				br.Position = pTexture[a];
				if (ResourceUtils.ResourceInfo.version == 109)
				{
					texture[a] = ReadRageResource.TextureGTAIV(br);
				}
				else
				{
					texture[a] = ReadRageResource.Texture(br);
				}
				br.Position = texture[a].m_pBitmap;
				bitMap[a] = ReadRageResource.BitMap(br);
			}
			string tmpName;
			for (int a = 0; a < header.m_cTexture.m_wCount; a++)
			{
				tmpName = texture[a].m_pName != 0 ? DataUtils.ReadStringAtOffset(texture[a].m_pName, br) : $"0x{hash[a]}";
				Log.ToLog(Log.MessageType.INFO, $"Name: {tmpName}, Size: {texture[a].m_dwHeight}x{texture[a].m_dwWidth}, Type as byte: {bitMap[a].m_dwTextureType}");

				ReadTexture(texture[a], bitMap[a], br, hash[a], overwriteTexture:overwriteTexture);
			}
			return true;
		}
	}
}