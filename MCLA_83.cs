using Converter.openFormats;
using Converter.Utils;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Converter.RageResource;

namespace Converter
{
	struct RES_83
	{
		public uint _vmt;
		public uint end;
		public uint _f8; // нули?
		public uint m_pShaderGroup;
		public uint _f10;
		public uint _f14;
		public uint _f18;
		public uint _f1c;
		// это не все...
	}
	internal class MCLA_83
	{
		public static bool Read(MemoryStream mem, bool endian)
		{
			FileStream outFileMain;
			StreamWriter swOutFileMain;
			StringBuilder sbOutFileMain = new StringBuilder();
			string outFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}.odr";


			RES_83 file = new RES_83();
			EndianBinaryReader br = new EndianBinaryReader(mem);
			if (endian) br.Endianness = Endian.BigEndian;
			file._vmt = br.ReadUInt32();
			file.end = br.ReadOffset();
			file._f8 = br.ReadUInt32();
			file.m_pShaderGroup = br.ReadOffset();
			file._f10 = br.ReadOffset();
			file._f14 = br.ReadOffset();
			file._f18 = br.ReadOffset();
			file._f1c = br.ReadOffset();

			if (file._f14 != 0)
			{
				br.Position = file._f14;
				RageResource.Texture zoneTexture = ReadRageResource.Texture(br);
				br.Position = zoneTexture.m_pBitmap;
				RageResource.BitMap zoneBitMap = ReadRageResource.BitMap(br);
				TextureDictionary.ReadTexture(zoneTexture, zoneBitMap, br, 0);
			}
			if (file._f18 != 0)
			{
				br.Position = file._f18;
				RageResource.Texture maxDamageTexture = ReadRageResource.Texture(br);
				br.Position = maxDamageTexture.m_pBitmap;
				RageResource.BitMap maxDamageBitMap = ReadRageResource.BitMap(br);
				TextureDictionary.ReadTexture(maxDamageTexture, maxDamageBitMap, br, 0);
			}
			if (file._f1c != 0)
			{
				br.Position = file._f1c;
				RageResource.Texture scratchTexture = ReadRageResource.Texture(br);
				br.Position = scratchTexture.m_pBitmap;
				RageResource.BitMap scratchBitMap = ReadRageResource.BitMap(br);
				TextureDictionary.ReadTexture(scratchTexture, scratchBitMap, br, 0);
			}

			br.Position = file.m_pShaderGroup;
			RageResource.IV_ShaderGroup shadeGroup;
			shadeGroup = ReadRageResource.IV_ShaderGroup(br);

			if (!TextureDictionary.ReadTextureDictionary(br, shadeGroup.m_pTexture))
			{
				mem.Close();
				br.Close();
				throw new Exception("Error while Texture Dictionary.");
			}

			RageResource.MCLAShaderFX[] fx = new RageResource.MCLAShaderFX[shadeGroup.m_pShaders.m_wCount];
			uint[] pShaderFX = new uint[shadeGroup.m_pShaders.m_wCount];
			br.Position = shadeGroup.m_pShaders.m_pList;
			for (int a = 0; a < shadeGroup.m_pShaders.m_wCount; a++) pShaderFX[a] = br.ReadOffset();
			for (int a = 0; a < shadeGroup.m_pShaders.m_wCount; a++)
			{
				br.Position = pShaderFX[a];
				fx[a] = ReadRageResource.MCLAShaderFX(br);
			}
			byte[] paramTypes = new byte[1];
			uint[] pParam = new uint[1];
			Vector4 vTmp= new Vector4();
			// пишем
			sbOutFileMain.AppendLine($"Version 110 12\nshadinggroup\n{{\n\tShaders {shadeGroup.m_pShaders.m_wCount}\n\t{{");
			for (int a = 0; a < shadeGroup.m_pShaders.m_wCount; a++)
			{
				Array.Resize<byte>(ref paramTypes, fx[a].m_wParamsCount);
				br.Position = fx[a].m_pParameterTypes;
				for (int b = 0; b < fx[a].m_wParamsCount; b++) paramTypes[b] = br.ReadByte();
				Array.Resize<uint>(ref pParam, fx[a].m_wParamsCount);
				br.Position = fx[a].m_pShaderParams;
				for (int b = 0; b < fx[a].m_wParamsCount; b++) pParam[b] = br.ReadOffset();
				sbOutFileMain.Append("\t\t");
				sbOutFileMain.Append(Path.GetFileName(DataUtils.ReadStringAtOffset(fx[a].m_pSPS, br)));
				for (int b = 0; b < fx[a].m_wParamsCount; b++)
				{
					if (pParam[b] == 0)
					{
						sbOutFileMain.Append(" null");
						continue;
					}
					br.Position = pParam[b];
					switch (paramTypes[b])
					{
						case 0:
							br.Position += 24;
							sbOutFileMain.Append($" {DataUtils.ReadStringAtOffset(br.ReadOffset(), br)}");
							break;
							case 1:
							vTmp = br.ReadVector4();
							sbOutFileMain.Append($" {vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
							break;
						case 4:
							sbOutFileMain.Append(" ");
							for (int c = 0; c < 4; c++)
							{
								vTmp = br.ReadVector4();
								sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
								if (c != 3) sbOutFileMain.Append(";");
							}
							break;
						case 8:
							sbOutFileMain.Append(" ");
							for (int c = 0; c < 6; c++)
							{
								vTmp = br.ReadVector4();
								sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
								if (c != 5) sbOutFileMain.Append(";");
							}
							break;
						case 9:
							sbOutFileMain.Append(" ");
							for (int c = 0; c < 9; c++)
							{
								vTmp = br.ReadVector4();
								sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
								if (c != 8) sbOutFileMain.Append(";");
							}
							break;
						case 14:
							sbOutFileMain.Append(" ");
							for (int c = 0; c < 14; c++)
							{
								vTmp = br.ReadVector4();
								sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
								if (c != 13) sbOutFileMain.Append(";");
							}
							break;
						case 15:
							sbOutFileMain.Append(" ");
							for (int c = 0; c < 15; c++)
							{
								vTmp = br.ReadVector4();
								sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
								if (c != 14) sbOutFileMain.Append(";");
							}
							break;
						case 16:
							sbOutFileMain.Append(" ");
							for (int c = 0; c < 16; c++)
							{
								vTmp = br.ReadVector4();
								sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
								if (c != 15) sbOutFileMain.Append(";");
							}
							break;
					}

				}
				sbOutFileMain.AppendLine();
			}
			sbOutFileMain.AppendLine($"\t}}");
			sbOutFileMain.AppendLine($"}}");

			sbOutFileMain.AppendLine("lodgroup");
			sbOutFileMain.AppendLine("{");
			sbOutFileMain.AppendLine($"\thigh none {9999.0}");
			sbOutFileMain.AppendLine($"\tmed none {9999.0}");
			sbOutFileMain.AppendLine($"\tlow none {9999.0}");
			sbOutFileMain.AppendLine($"\tvlow none {9999.0}");
			sbOutFileMain.AppendLine($"\tcenter {0} {0} {0}");
			sbOutFileMain.AppendLine($"\tAABBMin {0} {0} {0}");
			sbOutFileMain.AppendLine($"\tAABBMax {0} {0} {0}");
			sbOutFileMain.AppendLine($"\tradius {0}");
			sbOutFileMain.AppendLine("}");


			outFileMain = System.IO.File.Create(outFileName);
			swOutFileMain = new StreamWriter(outFileMain);
			swOutFileMain.Write(sbOutFileMain.ToString());
			swOutFileMain.Close();
			sbOutFileMain.Clear();

			mem.Close();
			br.Close();
			return true;
		}
	}
}
