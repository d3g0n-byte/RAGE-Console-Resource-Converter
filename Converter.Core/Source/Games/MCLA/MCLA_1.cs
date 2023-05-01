using Converter.Core.Utils.openFormats;
using Converter.Core.Utils;
using System;
using System.IO;
using System.Numerics;
using System.Text;
using Converter.Core.ResourceTypes;

namespace Converter.Core.Games.MCLA
{
	public static class MCLA_1
	{
		public struct RES_1
		{
			public uint _vmt;
			public uint pEnd;
			public uint pTexture; // only one texture
			public uint _fc;
			public uint _f10;
			public uint _f14;
			public uint _f18;
			public uint _f1c;
		}

		public struct SMALL_DRAWABLE
		{
			public uint _vmt;
			public uint _f4;
			public uint pShaderGroup;
			public uint pModelCollection; // like as xtd header
		}

		public static bool Read(MemoryStream ms, bool endian)
		{
			using (EndianBinaryReader br = new EndianBinaryReader(ms))
			{
				if (endian)
				{
					br.Endianness = Endian.BigEndian;
				}

				uint pointToNextSection = 0;
				RES_1 file = new RES_1
				{
					_vmt = br.ReadUInt32(),
					pEnd = br.ReadOffset(),
					pTexture = br.ReadOffset()
				};

				int mode = -1;
				if (file._vmt == 1690719744)
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
				else if (file._vmt == 2630243840)
				{
					file._fc = br.ReadOffset();
					mode = 3;
				}
				else
				{
					return false;
				}

				switch (mode)
				{
					case 0:
						{
							br.BaseStream.Position = pointToNextSection;
							uint tmpVmt = br.ReadUInt32();
							br.BaseStream.Position -= 4;

							if (tmpVmt == 2492093440)
							{
								TextureDictionary.UniversalTexture texture = TextureDictionary.UniversalTexture.ConvertToUniversalTexture(IV_Texture.Read(br));
								br.Position = texture.m_pBitmap;
								BitMap bitMap = BitMap.Read(br);
								TextureDictionary.ReadTexture(texture, bitMap, br, 0);
							}
							else if (tmpVmt == 1961646080)
							{
								TextureDictionary.UniversalTexture texture = TextureDictionary.UniversalTexture.ConvertToUniversalTexture(RDR_Texture.Read(br));
								br.Position = texture.m_pBitmap;
								BitMap bitMap = BitMap.Read(br);
								TextureDictionary.ReadTexture(texture, bitMap, br, 0);
							}
							else
							{
								return false;
							}

							break;
						}

					case 1:
						{
							uint pSmallDrawable = br.ReadOffset(); // I don't know how this section is named
							uint _f14 = br.ReadUInt32();
							uint _f18 = br.ReadOffset();
							uint _f1b = br.ReadOffset();
							Collection pPlacement; // most likely. This is a pointer directly to the sections which reads subsequentially. The size of the section is 80 bytes.

							// read textures from file
							br.Position = pointToNextSection;
							TextureDictionary.ReadTextureDictionary(br, pointToNextSection);

							// read small drawable section
							br.Position = pSmallDrawable;
							SMALL_DRAWABLE smallDrawable = new SMALL_DRAWABLE
							{
								_vmt = br.ReadUInt32(),
								_f4 = br.ReadUInt32(),
								pShaderGroup = br.ReadOffset(),
								pModelCollection = br.ReadOffset()
							};

							// create the odr file. I will use odr because there is only one section - SectionGroup
							StringBuilder sbOutFileMain = new StringBuilder();
							// 
							string outFileName = Path.Combine(Path.GetDirectoryName(Main.inputPath), $"{Path.GetFileNameWithoutExtension(Main.inputPath)}.odr");

							br.Position = smallDrawable.pShaderGroup;
							IV_ShaderGroup shadeGroup = IV_ShaderGroup.Read(br);

							if (shadeGroup.m_pTexture != 0)
							{
								if (!TextureDictionary.ReadTextureDictionary(br, shadeGroup.m_pTexture))
								{
									throw new Exception("Error while reading Texture Dictionary.");
								}
							}

							MCLA_ShaderFX[] fx = new MCLA_ShaderFX[shadeGroup.m_pShaders.m_nCount];
							uint[] pShaderFX = new uint[shadeGroup.m_pShaders.m_nCount];
							br.Position = shadeGroup.m_pShaders.m_pList;

							for (int a = 0; a < shadeGroup.m_pShaders.m_nCount; a++)
							{
								pShaderFX[a] = br.ReadOffset();
							}

							for (int a = 0; a < shadeGroup.m_pShaders.m_nCount; a++)
							{
								br.Position = pShaderFX[a];
								fx[a] = MCLA_ShaderFX.Read(br);
							}

							// write ShaderGroup section
							byte[] paramTypes = new byte[1];
							uint[] pParam = new uint[1];
							Vector4 vTmp = new Vector4();
							sbOutFileMain.AppendLine($"Version 110 12\nshadinggroup\n{{\n\tShaders {shadeGroup.m_pShaders.m_nCount}\n\t{{");

							if (Settings.bExportShaders)
							{
								for (int a = 0; a < shadeGroup.m_pShaders.m_nCount; a++)
								{
									Array.Resize(ref paramTypes, fx[a].m_nParamsCount);
									br.Position = fx[a].m_pParameterTypes;

									for (int b = 0; b < fx[a].m_nParamsCount; b++)
									{
										paramTypes[b] = br.ReadByte();
									}

									Array.Resize(ref pParam, fx[a].m_nParamsCount);
									br.Position = fx[a].m_pShaderParams;

									for (int b = 0; b < fx[a].m_nParamsCount; b++)
									{
										pParam[b] = br.ReadOffset();
									}

									sbOutFileMain.Append("\t\t");
									sbOutFileMain.Append(Path.GetFileName(DataUtils.ReadStringAtOffset(fx[a].m_pSPS, br)));

									for (int b = 0; b < fx[a].m_nParamsCount; b++)
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
							}
							else
							{
								for (int a = 0; a < shadeGroup.m_pShaders.m_nCount; a++)
								{
									sbOutFileMain.AppendLine("\t\tgta_default.sps null");
								}
							}

							sbOutFileMain.AppendLine($"\t}}");
							sbOutFileMain.AppendLine($"}}");
							br.Position = smallDrawable.pModelCollection;
							Dictionary modelCollection = Dictionary.Read(br);

							uint tmp;
							Model[] model = new Model[modelCollection.m_cTexture.m_nCount];
							uint[] pModel = new uint[modelCollection.m_cTexture.m_nCount];
							br.Position = modelCollection.m_cTexture.m_pList;

							for (int a = 0; a < modelCollection.m_cTexture.m_nCount; a++)
							{
								pModel[a] = br.ReadOffset();
							}

							for (int a = 0; a < modelCollection.m_cTexture.m_nCount; a++)
							{
								br.Position = pModel[a];
								model[a] = Model.Read(br);
							}

							uint geometryCount = 0;
							for (int a = 0; a < modelCollection.m_cTexture.m_nCount; a++)
							{
								geometryCount += model[a].m_cGeometry.m_nCount;
							}

							uint[] pGeometry = new uint[geometryCount];
							Geometry[] geometry = new Geometry[geometryCount];
							VertexBuffer[] vertexBuffer = new VertexBuffer[geometryCount];
							IndexBuffer[] indexBuffer = new IndexBuffer[geometryCount];
							VertexDeclaration[] vertexDeclaration = new VertexDeclaration[geometryCount];

							uint currentGeometry = 0;
							for (int a = 0; a < modelCollection.m_cTexture.m_nCount; a++)
							{
								br.Position = model[a].m_cGeometry.m_nCount;
								for (int b = 0; b < model[a].m_cGeometry.m_nCount; b++)
								{
									pGeometry[currentGeometry++] = br.ReadOffset();
								}
							}

							for (int a = 0; a < geometryCount; a++)
							{
								br.Position = pGeometry[a];
								geometry[a] = Geometry.Read(br);

								br.Position = geometry[a].m_pVertexBuffer;
								vertexBuffer[a] = VertexBuffer.Read(br);

								tmp = vertexBuffer[a].m_pVertexData;
								vertexBuffer[a].m_pVertexData = vertexBuffer[a].m_pDeclaration;
								vertexBuffer[a].m_pDeclaration = tmp;

								br.Position = geometry[a].m_pIndexBuffer;
								indexBuffer[a] = IndexBuffer.Read(br);

								br.Position = vertexBuffer[a].m_pDeclaration;
								vertexDeclaration[a] = VertexDeclaration.Read(br);
							}

							Vector4[,] vBounds = new Vector4[modelCollection.m_cTexture.m_nCount, 100];
							for (int a = 0; a < modelCollection.m_cTexture.m_nCount; a++)
							{
								br.Position = model[a].m_pBounds;
								uint boundsCount = model[a].m_cGeometry.m_nCount;

								if (model[a].m_cGeometry.m_nCount > 1)
								{
									boundsCount++;
								}

								for (int b = 0; b < boundsCount; b++)
								{
									vBounds[a, b] = br.ReadVector4();
								}
							}

							// write lodGroup section in the odr file
							string meshFileName;
							sbOutFileMain.AppendLine("lodgroup");
							sbOutFileMain.AppendLine("{");
							sbOutFileMain.Append($"\t{"high"} {model.Length}");

							currentGeometry = 0;
							for (uint b = 0; b < model.Length; /*b++*/)
							{
								string MeshPath = Path.Combine(Path.GetDirectoryName(Main.inputPath), Path.GetFileNameWithoutExtension(Main.inputPath));
								meshFileName = $"{MeshPath}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}_{b}.mesh";
								sbOutFileMain.Append($" {Path.Combine(Path.GetFileNameWithoutExtension(Main.inputPath), $"{Path.GetFileNameWithoutExtension(Main.inputPath)}_{b}.mesh")} {model[b].m_nBoneIndex}");

								if (!Directory.Exists(MeshPath))
								{
									Directory.CreateDirectory(MeshPath);
								}

								if (!IV_mesh.Build(model[b], br, vBounds, ref b, ref currentGeometry, indexBuffer, vertexBuffer, meshFileName, 0, vertexDeclaration))
								{
									throw new Exception("Failed to write .mesh file.");
								}
							}

							sbOutFileMain.AppendLine(" 9999.0");
							sbOutFileMain.AppendLine("\tmed none 9999.0");
							sbOutFileMain.AppendLine("\tlow none 9999.0");
							sbOutFileMain.AppendLine("\tvlow none 9999.0");
							sbOutFileMain.AppendLine("\tcenter 0 0 0");
							sbOutFileMain.AppendLine("\tAABBMin -10.0 -10.0 -10.0");
							sbOutFileMain.AppendLine("\tAABBMax 10.0 10.0 10.0");
							sbOutFileMain.AppendLine("\tradius 10.0");
							sbOutFileMain.AppendLine("}");

							using (FileStream outFileMain = File.Create(outFileName))
							{
								using (StreamWriter swOutFileMain = new StreamWriter(outFileMain))
								{
									swOutFileMain.Write(sbOutFileMain.ToString());
								}
							}

							sbOutFileMain.Clear();

							break;
						}

					case 2:
						{
							file._fc = br.ReadUInt32();
							file._f10 = br.ReadOffset();
							file._f14 = br.ReadUInt16();
							br.ReadUInt16();
							file._f18 = br.ReadOffset();
							file._f1c = br.ReadUInt16();
							br.ReadUInt16();

							uint[] pTexture = new uint[file._f1c];
							br.Position = file._f18;

							for (int a = 0; a < file._f1c; a++)
							{
								pTexture[a] = br.ReadOffset();
							}

							for (int a = 0; a < file._f1c; a++)
							{
								br.Position = pTexture[a];
								uint tmpVmt2 = br.ReadUInt32();
								br.BaseStream.Position -= 4;

								if (tmpVmt2 == 1955882240)
								{
									TextureDictionary.UniversalTexture texture = TextureDictionary.UniversalTexture.ConvertToUniversalTexture(RDR_Texture.Read(br));
									br.Position = texture.m_pBitmap;
									BitMap bitMap = BitMap.Read(br);
									TextureDictionary.ReadTexture(texture, bitMap, br, 0);
								}
								else
								{
									return false;
								}
							}

							break;
						}

					case 3:
						{
							br.Position = file.pTexture;
							RDR_Texture texture1 = RDR_Texture.Read(br);
							
							br.Position = texture1.m_pBitmap;
							BitMap bitMap1 = BitMap.Read(br);

							br.Position = file._fc;
							RDR_Texture texture2 = RDR_Texture.Read(br);
							BitMap bitMap2 = BitMap.Read(br);
							
							TextureDictionary.ReadTexture(TextureDictionary.UniversalTexture.ConvertToUniversalTexture(texture1), bitMap1, br, 0);
							TextureDictionary.ReadTexture(TextureDictionary.UniversalTexture.ConvertToUniversalTexture(texture2), bitMap2, br, 1);

							break;
						}
				}
			}

			return true;
		}
	}
}
