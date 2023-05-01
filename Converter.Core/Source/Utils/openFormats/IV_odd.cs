using System;
using System.Numerics;
using System.Text;
using System.IO;
using Converter.Core.Games.RDR;
using Converter.Core.ResourceTypes;

namespace Converter.Core.Utils.openFormats
{
	public static class IV_odd
	{
		public class UniversalShaderFX
		{
			public static string[] ShaderFXToShaderLine(EndianBinaryReader br, RDR_ShaderFX[] shaderFX)
			{
				Vector4 tmp4;
				string shaderName;
				string[] shaderLine = new string[shaderFX.Length];

				if (Settings.bConvertShadersToIV)
				{
					for (int b = 0; b < shaderFX.Length; b++)
					{
						shaderLine[b] = $"{IV_ShadingGroup.ConvertRDRShaderToIV(shaderFX[b], br)}";
					}
				}
				
				else if (Settings.bExportShaders)
				{
					for (int b = 0; b < shaderFX.Length; b++)
					{
						if (!RDR_ShaderManager.ShaderNames.shaderNames.TryGetValue(shaderFX[b].m_nNameHash, out shaderName))
						{
							shaderName = $"0x{shaderFX[b].m_nNameHash:X8}";
						}

						shaderLine[b] = ($"{shaderName}");
						int shaderIndexInShaderManager = RDR_ShaderManager.GetShaderIndex(shaderName);
						if (shaderIndexInShaderManager == -1)
						{
							for (int c = 0; c < shaderFX[b].m_nParamCount; c++)
							{
								if (shaderFX[b].value[c].m_pValue != 0)
								{
									br.Position = shaderFX[b].value[c].m_pValue;
									if (shaderFX[b].value[c].m_nParamType == 0) // texture
									{
										shaderLine[b] += $" {DataUtils.ReadStringAtOffset(RDR_TextureDefinition.Read(br).m_pName, br)}";
									}
									else if (shaderFX[b].value[c].m_nParamType == 9) // matrix9x4
									{
										shaderLine[b] += " ";
										for (int i = 0; i < 9; i++)
										{
											tmp4 = br.ReadVector4();
											shaderLine[b] += $"({tmp4.X};{tmp4.Y};{tmp4.Z};{tmp4.W})";
										}
									}
									else if (shaderFX[b].value[c].m_nParamType == 1) // Vector4
									{
										tmp4 = br.ReadVector4();
										shaderLine[b] += $" {tmp4.X};{tmp4.Y};{tmp4.Z};{tmp4.W}";
									}
								}
								else
								{
									shaderLine[b] += " [null]";
								}
							}
						}
						else
						{
							string[] samplerBuffer = new string[shaderFX[b].m_nParamCount];
							Vector4[] vector4Buffer = new Vector4[shaderFX[b].m_nParamCount];
							int currentPosInSamplerBuffer = 0;
							int currentPosInVector4Buffer = 0;

							for (int c = 0; c < shaderFX[b].m_nParamCount; c++)
							{
								br.Position = shaderFX[b].value[c].m_pValue;
								switch (shaderFX[b].value[c].m_nParamType)
								{
									case 0:
										if (shaderFX[b].value[c].m_pValue != 0)
										{
											samplerBuffer[currentPosInSamplerBuffer++] = $"{DataUtils.ReadStringAtOffset(RDR_TextureDefinition.Read(br).m_pName, br)}";
										}
										else
										{
											samplerBuffer[currentPosInSamplerBuffer++] = $"[null]";
										}
										break;

									case 1:
										if (shaderFX[b].value[c].m_pValue != 0)
										{
											vector4Buffer[currentPosInVector4Buffer++] = br.ReadVector4();
										}
										break;
								}
							}

							currentPosInSamplerBuffer = currentPosInVector4Buffer = 0;
							for (int c = 0; c < RDR_ShaderManager.ShaderParams[shaderIndexInShaderManager].shaderparams.Length; c++)
							{
								if (RDR_ShaderManager.ShaderParams[shaderIndexInShaderManager].shaderparams[c].skip)
								{
									continue;
								}

								switch (RDR_ShaderManager.ShaderParams[shaderIndexInShaderManager].shaderparams[c].type)
								{
									case "int":
										shaderLine[b] += $" {vector4Buffer[currentPosInVector4Buffer++].X}";
										break;

									case "float":
										shaderLine[b] += $" {vector4Buffer[currentPosInVector4Buffer++].X}";
										break;

									case "float2":
										shaderLine[b] += $" {vector4Buffer[currentPosInVector4Buffer].X};{vector4Buffer[currentPosInVector4Buffer].Y};{vector4Buffer[currentPosInVector4Buffer].Z};{vector4Buffer[currentPosInVector4Buffer++].W}";
										break;

									case "float3":
										shaderLine[b] += $" {vector4Buffer[currentPosInVector4Buffer].X};{vector4Buffer[currentPosInVector4Buffer].Y};{vector4Buffer[currentPosInVector4Buffer].Z};{vector4Buffer[currentPosInVector4Buffer++].W}";
										break;

									case "float4":
										shaderLine[b] += $" {vector4Buffer[currentPosInVector4Buffer].X};{vector4Buffer[currentPosInVector4Buffer].Y};{vector4Buffer[currentPosInVector4Buffer].Z};{vector4Buffer[currentPosInVector4Buffer++].W}";
										break;

									case "sampler":
										shaderLine[b] += $" {samplerBuffer[currentPosInSamplerBuffer++]}";
										break;

									case "bool":
										shaderLine[b] += $" {vector4Buffer[currentPosInVector4Buffer++].X}";
										break;

									case "float4x3":
										shaderLine[b] += $" 0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0";
										break;

									case "float4x4":
										shaderLine[b] += $" 0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0";
										break;
								}
							}
						}
					}
				}
				else
				{
					for (int b = 0; b < shaderFX.Length; b++)
					{
						if (!RDR_ShaderManager.ShaderNames.shaderNames.TryGetValue(shaderFX[b].m_nNameHash, out shaderName))
						{
							shaderName = $"0x{shaderFX[b].m_nNameHash:X8}";
						}

						int shaderIndexInShaderManager = RDR_ShaderManager.GetShaderIndex(shaderName);
						if (shaderIndexInShaderManager == -1)
						{
							shaderLine[b] = $"gta_default.sps temptxd";
						}
						else
						{
							string[] samplerBuffer = new string[shaderFX[b].m_nParamCount];
							Vector4[] vector4Buffer = new Vector4[shaderFX[b].m_nParamCount];
							int currentPosInSamplerBuffer = 0;
							int currentPosInVector4Buffer = 0;

							for (int c = 0; c < shaderFX[b].m_nParamCount; c++)
							{
								br.Position = shaderFX[b].value[c].m_pValue;
								switch (shaderFX[b].value[c].m_nParamType)
								{
									case 0:
										if (shaderFX[b].value[c].m_pValue != 0)
										{
											samplerBuffer[currentPosInSamplerBuffer++] = $"{DataUtils.ReadStringAtOffset(RDR_TextureDefinition.Read(br).m_pName, br)}";
										}
										else
										{
											samplerBuffer[currentPosInSamplerBuffer++] = $"[null]";
										}
										break;

									case 1:
										if (shaderFX[b].value[c].m_pValue != 0)
										{
											vector4Buffer[currentPosInVector4Buffer++] = br.ReadVector4();
										}
										break;
								}
							}

							string diffuseValue = "temptxd";
							int diffuseValueInSamplerBuffer = RDR_ShaderManager.GetDiffuseSamplerValue(shaderIndexInShaderManager);

							if (diffuseValueInSamplerBuffer != -1)
							{
								diffuseValue = samplerBuffer[diffuseValueInSamplerBuffer];

								if (!diffuseValue.EndsWith(".dds"))
								{
									diffuseValue += ".dds";
								}

								if (CheckTextureExists($"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}\\{diffuseValue}"))
								{
									diffuseValue = $"{Path.GetFileNameWithoutExtension(Main.inputPath)}\\{diffuseValue}";
								}
								else
								{
									if (diffuseValue.EndsWith(".dds"))
									{
										diffuseValue = diffuseValue.Replace(".dds", "");
									}
								}
							}

							shaderLine[b] = $"\t\t\t\tgta_default.sps {diffuseValue}";
						}
					}
				}
				return shaderLine;
			}

			public static string[] ShaderFXToShaderLine(EndianBinaryReader br, MCLA_ShaderFX[] shaderFX)
			{
				string[] shaderLine = new string[shaderFX.Length];
				byte[] paramTypes = new byte[1];
				uint[] pParam = new uint[1];
				Vector4 vTmp = new Vector4();

				for (int a = 0; a < shaderFX.Length; a++)
				{
					Array.Resize(ref paramTypes, shaderFX[a].m_nParamsCount);
					br.Position = shaderFX[a].m_pParameterTypes;

					for (int b = 0; b < shaderFX[a].m_nParamsCount; b++)
					{
						paramTypes[b] = br.ReadByte();
					}

					Array.Resize(ref pParam, shaderFX[a].m_nParamsCount);
					br.Position = shaderFX[a].m_pShaderParams;

					for (int b = 0; b < shaderFX[a].m_nParamsCount; b++)
					{
						pParam[b] = br.ReadOffset();
					}

					shaderLine[a] = Path.GetFileName(DataUtils.ReadStringAtOffset(shaderFX[a].m_pSPS, br));

					for (int b = 0; b < shaderFX[a].m_nParamsCount; b++)
					{
						if (pParam[b] == 0)
						{
							shaderLine[a] +=" null";
							continue;
						}

						br.Position = pParam[b];
						
						switch (paramTypes[b])
						{
							case 0:
								br.Position += 24;
								shaderLine[a]+=$" {DataUtils.ReadStringAtOffset(br.ReadOffset(), br)}";
								break;

							case 1:
								vTmp = br.ReadVector4();
								shaderLine[a] += $" {vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}";
								break;

							case 4:
								shaderLine[a] += " ";
								for (int c = 0; c < 4; c++)
								{
									vTmp = br.ReadVector4();
									shaderLine[a] += $"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}";
									if (c != 3)
									{
										shaderLine[a] += ";";
									}
								}
								break;

							case 8:
								shaderLine[a] += " ";
								for (int c = 0; c < 6; c++)
								{
									vTmp = br.ReadVector4();
									shaderLine[a] += $"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}";
									if (c != 5)
									{
										shaderLine[a] += ";";
									}
								}
								break;

							case 9:
								shaderLine[a] += " ";
								for (int c = 0; c < 9; c++)
								{
									vTmp = br.ReadVector4();
									shaderLine[a] += $"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}";
									if (c != 8)
									{
										shaderLine[a] += ";";
									}
								}
								break;

							case 14:
								shaderLine[a] += " ";
								for (int c = 0; c < 14; c++)
								{
									vTmp = br.ReadVector4();
									shaderLine[a] += $"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}";
									if (c != 13)
									{
										shaderLine[a] += ";";
									}
								}
								break;

							case 15:
								shaderLine[a] += " ";
								for (int c = 0; c < 15; c++)
								{
									vTmp = br.ReadVector4();
									shaderLine[a] += $"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}";
									if (c != 14)
									{
										shaderLine[a] += ";";
									}
								}
								break;

							case 16:
								shaderLine[a] += " ";
								for (int c = 0; c < 16; c++)
								{
									vTmp = br.ReadVector4();
									shaderLine[a] += $"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}";
									if (c != 15)
									{
										shaderLine[a] += ";";
									}
								}
								break;
						}
					}
				}
				return shaderLine;
			}
		}
		
		public class UniversalShaderGroup
		{
			public uint _vmt;
			public uint m_pTexture;
			public Collection m_cShaderFX;
			
			public static UniversalShaderGroup ConvertToUniversalShaderGroup(RDR_ShaderGroup shaderGroup)
			{
				return new UniversalShaderGroup
				{
					_vmt = shaderGroup._vmt,
					m_pTexture = shaderGroup.m_pTextureDictionary,
					m_cShaderFX = shaderGroup.m_cShaderFX
				};
			}
		}

		public static bool CheckTextureExists(string path)
		{
			if (File.Exists(path))
			{
				return true;
			}

			string fileName = Path.GetFileName(path);
			string texturesFolder = Settings.sTexturesFolder;
			if (!texturesFolder.EndsWith("/") && !texturesFolder.EndsWith("\\"))
			{
				texturesFolder += "\\";
			}

			if (File.Exists(texturesFolder + fileName))
			{
				byte[] texture = File.ReadAllBytes(texturesFolder + fileName);
				File.WriteAllBytes(path, texture);
				return true;
			}

			texturesFolder = Settings.sAdditionalTexturesFolder;
			if (!texturesFolder.EndsWith("/") && !texturesFolder.EndsWith("\\"))
			{
				texturesFolder += "\\";
			}

			if (File.Exists(texturesFolder + fileName))
			{
				byte[] texture = File.ReadAllBytes(texturesFolder + fileName);
				File.WriteAllBytes(path, texture);

				return true;
			}

			return false;
		}

		public static bool Build(uint drawableCount, UniversalShaderGroup[] shaderGroup, EndianBinaryReader br,
			Drawable[] drawable, Collection[,] modelCollection, Model[] model,
			Vector4[,] vBounds, IndexBuffer[] indexBuffer, VertexBuffer[] vertexBuffer,
			VertexDeclaration[] vertexDeclaration, uint[] hash, IV_skel.UniversalSkeletonData[] skelData,
			string[] shaderLine)
		{
			StringBuilder sbOutFileMain = new StringBuilder();

			string outFileName = Path.Combine(Path.GetDirectoryName(Main.inputPath), $"{Path.GetFileNameWithoutExtension(Main.inputPath)}.odd");

			if (drawableCount > 0)
			{
				Console.WriteLine($"[INFO] Writing {Path.GetFileNameWithoutExtension(Main.inputPath)}.odd");
			}
			else
			{
				Console.WriteLine("[WARNING] Empty file. ODD file will not be created, but if it contains textures, they are exported");
				return true;
			}

			uint currentModel = 0;
			uint currentGeometry = 0;
			sbOutFileMain.AppendLine($"Version 110 12");
			sbOutFileMain.AppendLine($"{{");
			string meshFileName;
			uint currentShaderFX = 0;
			uint currentSkeletonData = 0;

			for (int a = 0; a < drawableCount; a++)
			{
				if (!RDR_FileNames.fileNames.TryGetValue(hash[a], out string drawableName))
				{
					drawableName = $"0x{hash[a]:X8}";
				}

				sbOutFileMain.AppendLine($"\tgtaDrawable {drawableName}");
				sbOutFileMain.AppendLine($"\t{{");
				sbOutFileMain.AppendLine($"\t\tshadinggroup");
				sbOutFileMain.AppendLine($"\t\t{{");
				sbOutFileMain.AppendLine($"\t\t\tShaders {shaderGroup[a].m_cShaderFX.m_nCount}");
				sbOutFileMain.AppendLine($"\t\t\t{{");

				for (int b = 0; b < shaderGroup[a].m_cShaderFX.m_nCount; b++)
				{
					sbOutFileMain.AppendLine($"\t\t\t\t{shaderLine[currentShaderFX++]}");
				}

				sbOutFileMain.AppendLine($"\t\t\t}}");
				sbOutFileMain.AppendLine($"\t\t}}");

				// skel
				ushort boneCount = 0;
				if (drawable[a].m_pSkeleton != 0)
				{
					boneCount = skelData[currentSkeletonData].m_nBoneCount;
					if (Main.useVerboseMode || Main.useVeryVerboseMode)
					{
						Console.WriteLine("[INFO] Bones detected");
					}
					
					sbOutFileMain.Append("\t\tskel");
					sbOutFileMain.Append("\t\t{");

					string path = $"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileName(Main.inputPath).Replace(".", "_")}";

					if (!Directory.Exists(path.Substring(0, path.Length - 1)))
					{
						Directory.CreateDirectory(path);
					}
					string skelFileName = $"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileName(Main.inputPath).Replace(".", "_")}\\{drawableName}.skel";
					sbOutFileMain.AppendLine($"\t\t\tskel {Path.GetFileName(Main.inputPath).Replace(".", "_")}\\{drawableName}.skel");
					IV_skel.Build(br, drawable[a].m_pSkeleton, skelFileName, skelData[currentSkeletonData++]);
					sbOutFileMain.Append("\t\t}");
				}

				//lodgroup
				sbOutFileMain.AppendLine($"\t\tlodgroup");
				sbOutFileMain.AppendLine($"\t\t{{");
				string currentLevel;

				for (int b = 0; b < 4; b++)
				{
					switch (b)
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

					if (drawable[a].m_nObjectCount[b] > -1)
					{
						sbOutFileMain.Append($"\t\t\t{currentLevel} {modelCollection[a, b].m_nCount}");

						for (int c = 0; c < modelCollection[a, b].m_nCount; c++)
						{
							string MeshPath = $"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileName(Main.inputPath).Replace(".", "_")}";
							meshFileName = $"{MeshPath}\\{drawableName}_{currentLevel}_{c}.mesh";
							sbOutFileMain.Append($" {Path.GetFileName(Main.inputPath).Replace(".", "_")}\\{drawableName}_{currentLevel}_{c}.mesh {model[currentModel].m_nBoneIndex}");

							if (!Directory.Exists(MeshPath))
							{
								Directory.CreateDirectory(MeshPath);
							}

							if (!IV_mesh.Build(model[currentModel], br, vBounds, ref currentModel, ref currentGeometry, indexBuffer, vertexBuffer, meshFileName, boneCount, vertexDeclaration))
							{
								Console.WriteLine("[ERROR] Failed to write mesh file");
								return false;
							}
						}
					}
					else
					{
						sbOutFileMain.Append($"\t\t\t{currentLevel} none");
					}

					if (b == 0) sbOutFileMain.AppendLine($" {drawable[a].m_vDrawDistance.X}");
					else if (b == 1) sbOutFileMain.AppendLine($" {drawable[a].m_vDrawDistance.Y}");
					else if (b == 2) sbOutFileMain.AppendLine($" {drawable[a].m_vDrawDistance.Z}");
					else sbOutFileMain.AppendLine($" {drawable[a].m_vDrawDistance.W}");
				}

				// other data from the drawable section
				if (!Settings.bSwapYAndZ)
				{
					sbOutFileMain.AppendLine($"\t\t\tcenter {drawable[a].m_vCenter.X} {drawable[a].m_vCenter.Y} {drawable[a].m_vCenter.Z}");
					sbOutFileMain.AppendLine($"\t\t\tAABBMin {drawable[a].m_vAabbMin.X} {drawable[a].m_vAabbMin.Y} {drawable[a].m_vAabbMin.Z}");
					sbOutFileMain.AppendLine($"\t\t\tAABBMax {drawable[a].m_vAabbMax.X} {drawable[a].m_vAabbMax.Y} {drawable[a].m_vAabbMax.Z}");
				}
				else
				{
					sbOutFileMain.AppendLine($"\t\t\tcenter {drawable[a].m_vCenter.X} {drawable[a].m_vCenter.Z} {drawable[a].m_vCenter.Y}");
					sbOutFileMain.AppendLine($"\t\t\tAABBMin {drawable[a].m_vAabbMin.X} {drawable[a].m_vAabbMin.Z} {drawable[a].m_vAabbMin.Y}");
					sbOutFileMain.AppendLine($"\t\t\tAABBMax {drawable[a].m_vAabbMax.X} {drawable[a].m_vAabbMax.Z} {drawable[a].m_vAabbMax.Y}");
				}

				sbOutFileMain.AppendLine($"\t\t\tradius {drawable[a].m_vRadius.X}");
				sbOutFileMain.AppendLine($"\t\t}}");
				sbOutFileMain.AppendLine($"\t}}");
			}

			sbOutFileMain.AppendLine($"}}");
			using (FileStream outFileMain = File.Create(outFileName))
			{
				using (StreamWriter swOutFileMain = new StreamWriter(outFileMain))
				{
					swOutFileMain.Write(sbOutFileMain.ToString());
				}
			}

			sbOutFileMain.Clear();

			return true;
		}
	}
}
