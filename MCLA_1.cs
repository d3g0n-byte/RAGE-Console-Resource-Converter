using Converter.openFormats;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
	internal class MCLA_1
	{
		struct RES_1
		{
			public uint _vmt;
			public uint pEnd;
			public uint pTexture; // только одна текстура
			public uint _fc;
			public uint _f10;
			public uint _f14;
			public uint _f18;
			public uint _f1c;
		}
		struct SMALL_DRAWABLE
		{
			public uint _vmt;
			public uint _f4;
			public uint pShaderGroup;
			public uint pModelCollection;// как и xtd хедер 
		}

		public static bool Read(MemoryStream ms, bool endian)
		{
			uint pointToNextSection = 0;
			RES_1 file;
			EndianBinaryReader br = new EndianBinaryReader(ms);
			if (endian) br.Endianness = Endian.BigEndian;
			file._vmt = br.ReadUInt32();
			file.pEnd= br.ReadOffset();
			file.pTexture= br.ReadOffset();
			//file._fc = br.ReadUInt32();
			//file._f10 = br.ReadUInt32();
			//file._f14 = br.ReadUInt32();
			//file._f18 = br.ReadUInt32();
			//file._f1c = br.ReadUInt32();
			int mode = -1;
			if (file._vmt == 1690719744 || file._vmt == 2630243840)
			{
				pointToNextSection = file.pTexture;
				mode = 0;
			}
			else if (file._vmt >> 28 == 5)
			{
				pointToNextSection = file._vmt & 0x0FFFFFFF;
				mode = 0;
			}
			else if (file._vmt == 280452096)
			{
				pointToNextSection = file.pTexture;
				mode = 1;
			}
			else if (file._vmt == 1087133184)
			{
				pointToNextSection = file.pTexture;
				mode = 0;
			}
			else if (file._vmt == 2489935104)
			{
				pointToNextSection = file.pTexture;
				mode = 2;
			}
			else return false;

			switch (mode)
			{
				case 0:
					br.BaseStream.Position = pointToNextSection;
					uint tmpVmt = br.ReadUInt32();
					br.BaseStream.Position -= 4;//
					if (tmpVmt == 2492093440)
					{
						RageResource.Texture texture = ReadRageResource.Texture(br);
						br.Position = texture.m_pBitmap;
						RageResource.BitMap bitMap = ReadRageResource.BitMap(br);
						TextureDictionary.ReadTexture(texture, bitMap, br, 0);
					}
					else if (tmpVmt == 1961646080)
					{
						RageResource.Texture texture = ReadRageResource.TextureGTAIV(br);
						br.Position = texture.m_pBitmap;
						RageResource.BitMap bitMap = ReadRageResource.BitMap(br);
						TextureDictionary.ReadTexture(texture, bitMap, br, 0);
					}
					else return false;
					break;
				case 1:
					uint pSmallDrawable = br.ReadOffset(); // я не знаю как называется эта секция
					uint _f14 = br.ReadUInt32();
					uint _f18 = br.ReadOffset();
					uint _f1b = br.ReadOffset();
					RageResource.Collection pPlacement; // скорее всего. Поинтер сразу к секциям. Читается последовательно. Размер секции - 80 байтов
					// читаем текстуры из файла
					br.Position = pointToNextSection;
					TextureDictionary.ReadTextureDictionary(br, pointToNextSection);
					// читаем секцию small drawable
					SMALL_DRAWABLE smallDrawable;
					br.Position = pSmallDrawable;
					smallDrawable._vmt = br.ReadUInt32();
					smallDrawable._f4 = br.ReadUInt32();
					smallDrawable.pShaderGroup = br.ReadOffset();
					smallDrawable.pModelCollection = br.ReadOffset();

					// создаем наш odr файл. Я буду использовать именно одр ибо есть только одна секция ShaderGroup
					FileStream outFileMain;
					StreamWriter swOutFileMain;
					StringBuilder sbOutFileMain = new StringBuilder();
					string outFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}.odr";

					br.Position = smallDrawable.pShaderGroup;
					RageResource.IV_ShaderGroup shadeGroup;
					shadeGroup = ReadRageResource.IV_ShaderGroup(br);
					if (shadeGroup.m_pTexture != 0)
						if (!TextureDictionary.ReadTextureDictionary(br, shadeGroup.m_pTexture))
						{
							ms.Close();
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
					// пишем секцию ShaderGroup
					byte[] paramTypes = new byte[1];
					uint[] pParam = new uint[1];
					Vector4 vTmp = new Vector4();
					sbOutFileMain.AppendLine($"Version 110 12\nshadinggroup\n{{\n\tShaders {shadeGroup.m_pShaders.m_wCount}\n\t{{");
					if(Settings.bExportShaders)
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
					else
						for (int a = 0; a < shadeGroup.m_pShaders.m_wCount; a++)
							sbOutFileMain.AppendLine("\t\tgta_default.sps null");
					sbOutFileMain.AppendLine($"\t}}");
					sbOutFileMain.AppendLine($"}}");
					br.Position = smallDrawable.pModelCollection;
					RageResource.XTDHeader modelCollection;
					modelCollection = ReadRageResource.XTDHeader(br);

					uint tmp;
					RageResource.Model[] model = new RageResource.Model[modelCollection.m_cTexture.m_wCount];
					uint[] pModel = new uint[modelCollection.m_cTexture.m_wCount];
					br.Position = modelCollection.m_cTexture.m_pList;
					for (int a = 0; a < modelCollection.m_cTexture.m_wCount; a++) pModel[a] = br.ReadOffset();
					for (int a = 0; a < modelCollection.m_cTexture.m_wCount; a++)
					{
						br.Position = pModel[a];
						model[a] = ReadRageResource.Model(br);
					}
					uint geometryCount = 0;
					for (int a = 0; a < modelCollection.m_cTexture.m_wCount; a++) geometryCount += model[a].m_pGeometry.m_wCount;
					uint[] pGeometry = new uint[geometryCount];
					RageResource.Geometry[] geometry = new RageResource.Geometry[geometryCount];
					RageResource.VertexBuffer[] vertexBuffer = new RageResource.VertexBuffer[geometryCount];
					RageResource.IndexBuffer[] indexBuffer = new RageResource.IndexBuffer[geometryCount];
					RageResource.VertexDeclaration[] vertexDeclaration = new RageResource.VertexDeclaration[geometryCount];
					uint currentGeometry = 0;
					for (int a = 0; a < modelCollection.m_cTexture.m_wCount; a++)
					{
						br.Position = model[a].m_pGeometry.m_pList;
						for (int b = 0; b < model[a].m_pGeometry.m_wCount; b++)
							pGeometry[currentGeometry++] = br.ReadOffset();
					}
					for (int a = 0; a < geometryCount; a++)
					{
						br.Position = pGeometry[a];
						geometry[a] = ReadRageResource.Geometry(br);
						br.Position = geometry[a].m_pVertexBuffer;
						vertexBuffer[a] = ReadRageResource.VertexBuffer(br);
						tmp = vertexBuffer[a].m_pVertexData;
						vertexBuffer[a].m_pVertexData = vertexBuffer[a].m_pDeclaration;
						vertexBuffer[a].m_pDeclaration = tmp;
						br.Position = geometry[a].m_pIndexBuffer;
						indexBuffer[a] = ReadRageResource.IndexBuffer(br);
						br.Position = vertexBuffer[a].m_pDeclaration;
						vertexDeclaration[a] = ReadRageResource.VertexDeclaration(br);
					}
					Vector4[,] vBounds = new Vector4[modelCollection.m_cTexture.m_wCount, 100];
					for (int a = 0; a < modelCollection.m_cTexture.m_wCount; a++)
					{
						br.Position = model[a].m_pBounds;
						uint boundsCount = model[a].m_pGeometry.m_wCount;
						if (model[a].m_pGeometry.m_wCount > 1) boundsCount++;
						for (int b = 0; b < boundsCount; b++) vBounds[a, b] = br.ReadVector4();
					}
					// пишем секцию lodGroup в odr файл
					string meshFileName;
					sbOutFileMain.AppendLine("lodgroup");
					sbOutFileMain.AppendLine("{");
					sbOutFileMain.Append($"\t{"high"} {model.Length}");
					currentGeometry = 0;
					for (uint b = 0; b < model.Length; /*b++*/)
					{
						string MeshPath = $"{FileInfo.filePath}\\{FileInfo.baseFileName}";
						meshFileName = $"{MeshPath}\\{FileInfo.baseFileName}_{b}.mesh";
						sbOutFileMain.Append($" {FileInfo.baseFileName}\\{FileInfo.baseFileName}_{b}.mesh {model[b].m_nBoneIndex}");
						if (!Directory.Exists(MeshPath)) Directory.CreateDirectory(MeshPath);
						if (!IV_mesh.Build(model[b], br, vBounds, ref b, ref currentGeometry, indexBuffer, vertexBuffer, meshFileName, vertexDeclaration))
							throw new Exception("failed to write mesh file");
					}
					sbOutFileMain.AppendLine($" {9999.0}");
					sbOutFileMain.AppendLine($"\tmed none {9999.0}");
					sbOutFileMain.AppendLine($"\tlow none {9999.0}");
					sbOutFileMain.AppendLine($"\tvlow none {9999.0}");
					sbOutFileMain.AppendLine($"\tcenter {0} {0} {0}");
					sbOutFileMain.AppendLine($"\tAABBMin {-10f} {-10f} {-10f}");
					sbOutFileMain.AppendLine($"\tAABBMax {10f} {10f} {10f}");
					sbOutFileMain.AppendLine($"\tradius {10f}");
					sbOutFileMain.AppendLine("}");

					outFileMain = System.IO.File.Create(outFileName);
					swOutFileMain = new StreamWriter(outFileMain);
					swOutFileMain.Write(sbOutFileMain.ToString());
					swOutFileMain.Close();
					sbOutFileMain.Clear();
					br.Close();


					break;
				case 2:
					//br.BaseStream.Position = pointToNextSection;
					file._fc = br.ReadUInt32();
					file._f10 = br.ReadOffset();
					file._f14 = br.ReadUInt16();
					br.ReadUInt16();
					file._f18 = br.ReadOffset();
					file._f1c = br.ReadUInt16();
					br.ReadUInt16();
					//file._f1c = br.ReadUInt32();
					//uint hash
					uint[] pTexture = new uint[file._f1c];
					br.Position = file._f18;
					for (int a = 0; a < file._f1c; a++) pTexture[a] = br.ReadOffset();
					for (int a = 0; a < file._f1c; a++)
					{
						br.Position = pTexture[a];
						uint tmpVmt2 = br.ReadUInt32();
						br.BaseStream.Position -= 4;//
						if (tmpVmt2 == 1955882240)
						{
							RageResource.Texture texture = ReadRageResource.Texture(br);
							br.Position = texture.m_pBitmap;
							RageResource.BitMap bitMap = ReadRageResource.BitMap(br);
							TextureDictionary.ReadTexture(texture, bitMap, br, 0);
						}
						//else if (tmpVmt2 == 1961646080)
						//{
						//	RageResource.Texture texture = ReadRageResource.TextureGTAIV(br);
						//	br.Position = texture.m_pBitmap;
						//	RageResource.BitMap bitMap = ReadRageResource.BitMap(br);
						//	TextureDictionary.ReadTexture(texture, bitMap, br, true);
						//}
						else return false;
					}
					break;

			}
			return true;
		}
	}
}
