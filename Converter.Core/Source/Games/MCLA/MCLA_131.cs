using Converter.Core.Utils.openFormats;
using Converter.Core.Utils;
using Converter.Core.ResourceTypes;
using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace Converter.Core.Games.MCLA
{
	public static class MCLA_131
	{
		public static bool Read(MemoryStream ms, bool endian)
		{
			// we will use .odd file to perform a batch import
			StringBuilder sbOutFileMain = new StringBuilder();
			string outFileName = $"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}.odd";
			sbOutFileMain.AppendLine("Version 110 12\n{");

			using (EndianBinaryReader br = new EndianBinaryReader(ms))
			{
				if (endian)
				{
					br.Endianness = Endian.BigEndian;
				}

				// ищем секции drawable...
				uint _vmt = br.ReadUInt32();
				uint pBlockEnd = br.ReadOffset();
				uint _f8 = br.ReadOffset();
				Collection _fc = Collection.Read(br);
				uint[] pUnk = new uint[_fc.m_nCount];
				br.Position = _fc.m_pList;

				for (int a = 0; a < _fc.m_nCount; a++)
				{
					pUnk[a] = br.ReadOffset();
				}

				string fragName;
				uint pDrawable;

				for (int x = 0; x < _fc.m_nCount; x++)
				{
					br.Position = pUnk[x] + 0x5c;
					br.Position = br.ReadOffset() + 0xb0;
					fragName = DataUtils.ReadStringAtOffset(br.ReadOffset(), br);
					pDrawable = br.ReadOffset();
					sbOutFileMain.AppendLine($"gtaDrawable {fragName}\n{{");
					br.Position = pDrawable;

					Drawable drawable = Drawable.Read(br);
					IV_ShaderGroup shadeGroup = new IV_ShaderGroup();
					MCLA_ShaderFX[] fx = new MCLA_ShaderFX[0];

					if (drawable.m_pShaderGroup != 0)
					{
						br.Position = drawable.m_pShaderGroup;
						shadeGroup = IV_ShaderGroup.Read(br);

						if (shadeGroup.m_pTexture != 0)
						{
							if (!TextureDictionary.ReadTextureDictionary(br, shadeGroup.m_pTexture))
							{
								ms.Close();
								br.Close();
								throw new Exception("Error while reading Texture Dictionary.");
							}
						}

						fx = new MCLA_ShaderFX[shadeGroup.m_pShaders.m_nCount];
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
					}

					// модель
					uint currentModel = 0;
					uint[] pModel = new uint[200];

					if (x == 3944)
					{
						Console.WriteLine("");
					}

					Collection[] сModel = new Collection[4];

					for (int a = 0; a < 4; a++) // lod
					{
						if (drawable.m_pModelCollection[a] != 0)
						{
							br.Position = drawable.m_pModelCollection[a];
							сModel[a] = Collection.Read(br);
							br.Position = сModel[a].m_pList;

							for (int b = 0; b < сModel[a].m_nCount; b++)
							{
								pModel[currentModel++] = br.ReadOffset();
							}
						}
					}

					uint modelCount = currentModel;
					Array.Resize(ref pModel, (int)modelCount);
					Model[] model = new Model[pModel.Length];

					for (int a = 0; a < pModel.Length; a++)
					{
						br.Position = pModel[a];
						model[a] = Model.Read(br);
					}

					uint[] pGeometry = new uint[200];
					//uint geometryCount = 0;
					uint currentGeometry = 0;

					for (int a = 0; a < pModel.Length; a++)
					{
						br.Position = model[a].m_cGeometry.m_pList;

						for (int b = 0; b < model[a].m_cGeometry.m_nCount; b++)
						{
							pGeometry[currentGeometry++] = br.ReadOffset();
						}
					}

					uint geometryCount = currentGeometry;
					Array.Resize(ref pGeometry, (int)geometryCount);
					Geometry[] geometry = new Geometry[geometryCount];
					VertexBuffer[] vertexBuffer = new VertexBuffer[geometryCount];
					IndexBuffer[] indexBuffer = new IndexBuffer[geometryCount];
					VertexDeclaration[] vertexDeclaration = new VertexDeclaration[geometryCount];
					uint tmp;

					for (int a = 0; a < geometryCount; a++)
					{
						br.Position = pGeometry[a];
						geometry[a] = Geometry.Read(br);

						br.Position = geometry[a].m_pVertexBuffer;
						vertexBuffer[a] = VertexBuffer.Read(br);

						tmp = vertexBuffer[a].m_pVertexData;
						vertexBuffer[a].m_pVertexData = vertexBuffer[a].m_pDeclaration;
						vertexBuffer[a].m_pDeclaration = tmp;

						br.Position = vertexBuffer[a].m_pDeclaration;
						vertexDeclaration[a] = VertexDeclaration.Read(br);

						br.Position = geometry[a].m_pIndexBuffer;
						indexBuffer[a] = IndexBuffer.Read(br);
					}

					Vector4[,] vBounds = new Vector4[modelCount, 100];

					for (int a = 0; a < modelCount; a++)
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

					//
					// write down ShaderGroup section
					byte[] paramTypes = new byte[1];
					uint[] pParam = new uint[1];
					Vector4 vTmp = new Vector4();

					if (shadeGroup.m_pShaders.m_nCount != 0)
					{
						sbOutFileMain.AppendLine($"shadinggroup\n{{\n\tShaders {shadeGroup.m_pShaders.m_nCount}\n\t{{");

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
												if (c != 3)
												{
													sbOutFileMain.Append(";");
												}
											}
											break;

										case 8:
											sbOutFileMain.Append(" ");
											for (int c = 0; c < 6; c++)
											{
												vTmp = br.ReadVector4();
												sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
												if (c != 5)
												{
													sbOutFileMain.Append(";");
												}
											}
											break;

										case 9:
											sbOutFileMain.Append(" ");
											for (int c = 0; c < 9; c++)
											{
												vTmp = br.ReadVector4();
												sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
												if (c != 8)
												{
													sbOutFileMain.Append(";");
												}
											}
											break;

										case 14:
											sbOutFileMain.Append(" ");
											for (int c = 0; c < 14; c++)
											{
												vTmp = br.ReadVector4();
												sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
												if (c != 13)
												{
													sbOutFileMain.Append(";");
												}
											}
											break;

										case 15:
											sbOutFileMain.Append(" ");
											for (int c = 0; c < 15; c++)
											{
												vTmp = br.ReadVector4();
												sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
												if (c != 14)
												{
													sbOutFileMain.Append(";");
												}
											}
											break;

										case 16:
											sbOutFileMain.Append(" ");
											for (int c = 0; c < 16; c++)
											{
												vTmp = br.ReadVector4();
												sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
												if (c != 15)
												{
													sbOutFileMain.Append(";");
												}
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
					}
					else
					{
						sbOutFileMain.AppendLine($"shadinggroup\n{{\n\tShaders {1}\n\t{{");
						sbOutFileMain.AppendLine("\t\tgta_default.sps null");
					}

					sbOutFileMain.AppendLine($"\t}}");
					sbOutFileMain.AppendLine($"}}");

					if (drawable.m_pSkeleton != 0)
					{
						sbOutFileMain.AppendLine("skel");
						sbOutFileMain.AppendLine("{");
						string path = $"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}";

						if (!Directory.Exists(path.Substring(0, path.Length - 1)))
						{
							Directory.CreateDirectory(path);
						}

						string skelFileName = $"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}\\{fragName}.skel";
						sbOutFileMain.AppendLine($"\tskel {Path.GetFileNameWithoutExtension(Main.inputPath)}\\{fragName}.skel");
						br.Position = drawable.m_pSkeleton;
						IV_skel.Build(br, drawable.m_pSkeleton, skelFileName, IV_skel.UniversalSkeletonData.ConvertToUniversalSkeletonData(IV_SkeletonData.Read(br)));
						sbOutFileMain.AppendLine("}");
					}

					// build the lodgroup section
					sbOutFileMain.AppendLine($"lodgroup");
					sbOutFileMain.AppendLine($"{{");
					string currentLevel;
					string meshFileName;
					currentGeometry = 0;
					currentModel = 0;

					//if (x == 3944)
					//{
					//	Console.WriteLine("");
					//}

					for (int a = 0; a < 4; a++)
					{
						switch (a)
						{
							case 0:
								currentLevel = "high";
								break;
							case 1:
								currentLevel = "med";
								break;
							case 2:
								currentLevel = "low";
								break;
							default:
								currentLevel = "vlow";
								break;
						}

						if (drawable.m_nObjectCount[a] > -1)
						{
							sbOutFileMain.Append($"\t{currentLevel} {сModel[a].m_nCount}");

							for (uint b = 0; b < сModel[a].m_nCount; /*b++*/)
							{
								string MeshPath = $"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}";
								meshFileName = $"{MeshPath}\\{fragName}_{currentLevel}_{b}.mesh";
								sbOutFileMain.Append($" {Path.GetFileNameWithoutExtension(Main.inputPath)}\\{fragName}_{currentLevel}_{b}.mesh {model[currentModel].m_nBoneIndex}");

								if (!Directory.Exists(MeshPath))
								{
									Directory.CreateDirectory(MeshPath);
								}

								if (!IV_mesh.Build(model[currentModel++], br, vBounds, ref b, ref currentGeometry, indexBuffer, vertexBuffer, meshFileName, 0, vertexDeclaration))
								{
									Console.WriteLine("failed to write mesh file");
									return false;
								}
							}
						}
						else
						{
							sbOutFileMain.Append($"\t{currentLevel} none");
						}

						switch (a)
						{
							case 0:
								sbOutFileMain.AppendLine($" {drawable.m_vDrawDistance.X}");
								break;

							case 1:
								sbOutFileMain.AppendLine($" {drawable.m_vDrawDistance.Y}");
								break;

							case 2:
								sbOutFileMain.AppendLine($" {drawable.m_vDrawDistance.Z}");
								break;

							default:
								sbOutFileMain.AppendLine($" {drawable.m_vDrawDistance.W}");
								break;
						}
					}

					// other data from the drawable section
					if (!Settings.bSwapYAndZ)
					{
						sbOutFileMain.AppendLine($"\tcenter {drawable.m_vCenter.X} {drawable.m_vCenter.Y} {drawable.m_vCenter.Z}");
						sbOutFileMain.AppendLine($"\tAABBMin {drawable.m_vAabbMin.X} {drawable.m_vAabbMin.Y} {drawable.m_vAabbMin.Z}");
						sbOutFileMain.AppendLine($"\tAABBMax {drawable.m_vAabbMax.X} {drawable.m_vAabbMax.Y} {drawable.m_vAabbMax.Z}");
					}
					else
					{
						sbOutFileMain.AppendLine($"\tcenter {drawable.m_vCenter.X} {drawable.m_vCenter.Z} {drawable.m_vCenter.Y}");
						sbOutFileMain.AppendLine($"\tAABBMin {drawable.m_vAabbMin.X} {drawable.m_vAabbMin.Z} {drawable.m_vAabbMin.Y}");
						sbOutFileMain.AppendLine($"\tAABBMax {drawable.m_vAabbMax.X} {drawable.m_vAabbMax.Z} {drawable.m_vAabbMax.Y}");
					}

					sbOutFileMain.AppendLine($"\tradius {drawable.m_vRadius.X}");
					sbOutFileMain.AppendLine($"}}");
					sbOutFileMain.AppendLine($"}}");

					if (Main.useVeryVerboseMode)
					{
						Console.WriteLine($"{x}/{_fc.m_nCount + 1}");
					}
				}

				sbOutFileMain.AppendLine("}");

				using (FileStream outFileMain = File.Create(outFileName))
				{
					using (StreamWriter swOutFileMain = new StreamWriter(outFileMain))
					{
						swOutFileMain.Write(sbOutFileMain.ToString());
					}
				}

				sbOutFileMain.Clear();
			}

			return true;
		}
	}
}
