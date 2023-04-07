using ConsoleApp1;
using Converter.utils;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Converter.RageResource;

namespace Converter.openFormats
{
	internal class IV_odd
	{
		static bool CheckTextueExists(string path)
		{
			if (File.Exists(path)) return true;

			string fileName = Path.GetFileName(path);
			string texturesFolder = Settings.sTexturesFolder;
			if (!texturesFolder.EndsWith("/") && !texturesFolder.EndsWith("\\")) texturesFolder += "\\";

			if (File.Exists(texturesFolder + fileName))
			{
				byte[] texture = File.ReadAllBytes(texturesFolder + fileName);
				File.WriteAllBytes(path, texture);
				return true;
			}

			texturesFolder = Settings.sAdditionalTexturesFolder;
			if (!texturesFolder.EndsWith("/") && !texturesFolder.EndsWith("\\")) texturesFolder += "\\";
			if (File.Exists(texturesFolder + fileName))
			{
				byte[] texture = File.ReadAllBytes(texturesFolder + fileName);
				File.WriteAllBytes(path, texture);
				return true;
			}

			return false;

		}

		public static bool Build(uint drawableCount, RDRShaderGroup[] shaderGroup, RageResource.RDRShaderFX[] shaderFX, EndianBinaryReader br ,
			/*uint m_pSkeleton,*/ RageResource.Drawable[] drawable, Collection[,] modelCollection, RageResource.Model[] model,
			Vector4[,] vBounds, RageResource.IndexBuffer[] indexBuffer, RageResource.VertexBuffer[] vertexBuffer,
			RageResource.VertexDeclaration[] vertexDeclaration, uint[] hash)
		{
			FileStream outFileMain;
			StreamWriter swOutFileMain;
			StringBuilder sbOutFileMain = new StringBuilder(); // empty file

			if (drawableCount > 0) Log.ToLog(Log.MessageType.INFO, $"Exporting to IV openFormats...");
			else { Log.ToLog(Log.MessageType.WARNING, $"Empty file. odd file will not be created, but if it contains textures, they are exported"); return true; }

			string outFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}.odd";

			uint currentModel = 0;
			uint currentGeometry = 0;
			int wddVersion = 110;
			int openIVfileVersion = 12;
			sbOutFileMain.AppendLine($"Version {wddVersion} {openIVfileVersion}");
			sbOutFileMain.AppendLine($"{{");
			//uint fileCount = 0;
			string meshFileName;
			uint currentShaderFX = 0;
			uint currentValue = 0;
			string drawableName;
			for (int a = 0; a < drawableCount; a++)
			{
				if (!RDR_FileNames.fileNames.TryGetValue(hash[a], out drawableName)) drawableName = $"0x{hash[a].ToString("X8")}";
				sbOutFileMain.AppendLine($"\tgtaDrawable {drawableName}");
				sbOutFileMain.AppendLine($"\t{{");
				sbOutFileMain.AppendLine($"\t\tshadinggroup");
				sbOutFileMain.AppendLine($"\t\t{{");
				sbOutFileMain.AppendLine($"\t\t\tShaders {shaderGroup[a].m_cShaderFX.m_wCount}");// 
				sbOutFileMain.AppendLine($"\t\t\t{{");
				Vector4 tmp4;
				string shaderName;
				if (Settings.bConvertShadersToIV)
				{
					string shaderLine;
					for (int b = 0; b < shaderGroup[a].m_cShaderFX.m_wCount; b++)
					{
						sbOutFileMain.AppendLine($"\t\t\t\t{IV_ShadingGroup.ConvertRDRShaderToIV(shaderFX[currentShaderFX], br)}");
						currentShaderFX++;
						//IV_ShadingGroup.ConvertRDRShaderToIV(shaderFX[b], br)
					}
				}
				else if (Settings.bExportShaders)
				{
					for (int b = 0; b < shaderGroup[a].m_cShaderFX.m_wCount; b++)
					{
						if (!RDR_ShaderManager.ShaderNames.shaderNames.TryGetValue(shaderFX[currentShaderFX].m_dwNameHash, out shaderName)) shaderName = $"0x{shaderFX[currentShaderFX].m_dwNameHash.ToString("X8")}";
						sbOutFileMain.Append($"\t\t\t\t{shaderName}");
						int shaderIndexInShaderManager = RDR_ShaderManager.GetShaderIndex(shaderName);
						if(shaderIndexInShaderManager== -1)
						{
							for (int c = 0; c < shaderFX[currentShaderFX].m_nParamCount; c++)
							{
								if (shaderFX[currentShaderFX].value[c].m_pValue != 0)
								{
									br.Position = shaderFX[currentShaderFX].value[c].m_pValue;
									if (shaderFX[currentShaderFX].value[c].m_nParamType == 0)// текстура
									{
										sbOutFileMain.Append($" {DataUtils.ReadStringAtOffset(ReadRageResource.RDRTextureDefinition(br).m_pName, br)}");
									}
									else if (shaderFX[currentShaderFX].value[c].m_nParamType == 9)// matrix9x4
									{
										sbOutFileMain.Append(" ");
										for (int i = 0; i < 9; i++)
										{
											tmp4 = br.ReadVector4();
											sbOutFileMain.Append($"({tmp4.X};{tmp4.Y};{tmp4.Z};{tmp4.W})");
										}
									}
									else if (shaderFX[currentShaderFX].value[c].m_nParamType == 1)// Vector4
									{
										tmp4 = br.ReadVector4();
										sbOutFileMain.Append($" {tmp4.X};{tmp4.Y};{tmp4.Z};{tmp4.W}");
									}
								}
								else sbOutFileMain.Append($" [null]");
							}
						}
						else
						{
							string[] samplerBuffer = new string[shaderFX[currentShaderFX].m_nParamCount];
							Vector4[] vector4Buffer = new Vector4[shaderFX[currentShaderFX].m_nParamCount];
							int currentPosInSamplerBuffer = 0;
							int currentPosInVector4Buffer = 0;

							for (int c = 0; c < shaderFX[currentShaderFX].m_nParamCount; c++)
							{
								br.Position = shaderFX[currentShaderFX].value[c].m_pValue;
								switch (shaderFX[currentShaderFX].value[c].m_nParamType)
								{
									case 0:
										if (shaderFX[currentShaderFX].value[c].m_pValue != 0)
											samplerBuffer[currentPosInSamplerBuffer++] = $"{DataUtils.ReadStringAtOffset(ReadRageResource.RDRTextureDefinition(br).m_pName, br)}";
										else samplerBuffer[currentPosInSamplerBuffer++] = $"[null]";
										break;
										case 1:
										if (shaderFX[currentShaderFX].value[c].m_pValue != 0)
											vector4Buffer[currentPosInVector4Buffer++] = br.ReadVector4();
										break;
								}
							}
							currentPosInSamplerBuffer = currentPosInVector4Buffer = 0;
							for (int c = 0; c < RDR_ShaderManager.ShaderParams[shaderIndexInShaderManager].shaderparams.Length; c++)
							{
								if (RDR_ShaderManager.ShaderParams[shaderIndexInShaderManager].shaderparams[c].skip) continue;
								switch (RDR_ShaderManager.ShaderParams[shaderIndexInShaderManager].shaderparams[c].type)
								{
									case "int":
										sbOutFileMain.Append($" {vector4Buffer[currentPosInVector4Buffer++].X}");
										break;
									case "float":
										sbOutFileMain.Append($" {vector4Buffer[currentPosInVector4Buffer++].X}");
										break;
									case "float2":
										sbOutFileMain.Append($" {vector4Buffer[currentPosInVector4Buffer].X};{vector4Buffer[currentPosInVector4Buffer].Y};{vector4Buffer[currentPosInVector4Buffer].Z};{vector4Buffer[currentPosInVector4Buffer++].W}");
										break;
									case "float3":
										sbOutFileMain.Append($" {vector4Buffer[currentPosInVector4Buffer].X};{vector4Buffer[currentPosInVector4Buffer].Y};{vector4Buffer[currentPosInVector4Buffer].Z};{vector4Buffer[currentPosInVector4Buffer++].W}");
										break;
									case "float4":
										sbOutFileMain.Append($" {vector4Buffer[currentPosInVector4Buffer].X};{vector4Buffer[currentPosInVector4Buffer].Y};{vector4Buffer[currentPosInVector4Buffer].Z};{vector4Buffer[currentPosInVector4Buffer++].W}");
										break;
									case "sampler":
										sbOutFileMain.Append($" {samplerBuffer[currentPosInSamplerBuffer++]}");
										break;
									case "bool":
										sbOutFileMain.Append($" {vector4Buffer[currentPosInVector4Buffer++].X}");
										break;
									case "float4x3":
										sbOutFileMain.Append($" 0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0");
										break;
									case "float4x4":
										sbOutFileMain.Append($" 0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0");
										break;
								}
							}

						}

						currentShaderFX++;
						sbOutFileMain.AppendLine("");
					}
				}
				else
				{
					for (int b = 0; b < shaderGroup[a].m_cShaderFX.m_wCount; b++)
					{
						if (!RDR_ShaderManager.ShaderNames.shaderNames.TryGetValue(shaderFX[currentShaderFX].m_dwNameHash, out shaderName)) shaderName = $"0x{shaderFX[currentShaderFX].m_dwNameHash.ToString("X8")}";
						int shaderIndexInShaderManager = RDR_ShaderManager.GetShaderIndex(shaderName);
						if (shaderIndexInShaderManager == -1)
							sbOutFileMain.AppendLine($"\t\t\t\tgta_default.sps temptxd");
						else
						{
							string[] samplerBuffer = new string[shaderFX[currentShaderFX].m_nParamCount];
							Vector4[] vector4Buffer = new Vector4[shaderFX[currentShaderFX].m_nParamCount];
							int currentPosInSamplerBuffer = 0;
							int currentPosInVector4Buffer = 0;

							for (int c = 0; c < shaderFX[currentShaderFX].m_nParamCount; c++)
							{
								br.Position = shaderFX[currentShaderFX].value[c].m_pValue;
								switch (shaderFX[currentShaderFX].value[c].m_nParamType)
								{
									case 0:
										if (shaderFX[currentShaderFX].value[c].m_pValue != 0)
											samplerBuffer[currentPosInSamplerBuffer++] = $"{DataUtils.ReadStringAtOffset(ReadRageResource.RDRTextureDefinition(br).m_pName, br)}";
										else samplerBuffer[currentPosInSamplerBuffer++] = $"[null]";
										break;
									case 1:
										if (shaderFX[currentShaderFX].value[c].m_pValue != 0)
											vector4Buffer[currentPosInVector4Buffer++] = br.ReadVector4();
										break;
								}
							}
							string diffuseValue = "temptxd";
							int diffuseValueInSamplerBuffer = RDR_ShaderManager.GetDiffuseSamplerValue(shaderIndexInShaderManager);
							if (diffuseValueInSamplerBuffer != -1)
							{
								diffuseValue = samplerBuffer[diffuseValueInSamplerBuffer];
								if (!diffuseValue.EndsWith(".dds")) diffuseValue += ".dds";
								if(CheckTextueExists($"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{diffuseValue}")) diffuseValue = $"{FileInfo.baseFileName}\\{diffuseValue}";
								else
									if (diffuseValue.EndsWith(".dds")) diffuseValue = diffuseValue.Replace(".dds", "");
							}
							sbOutFileMain.AppendLine($"\t\t\t\tgta_default.sps {diffuseValue}");
						}
						currentShaderFX++;
					}

				}
				sbOutFileMain.AppendLine($"\t\t\t}}");
				sbOutFileMain.AppendLine($"\t\t}}");
				//
				// skel
				//
				if (drawable[a].m_pSkeleton != 0)
				{
					Log.ToLog(Log.MessageType.INFO, $"Bones detected");
					sbOutFileMain.Append("\t\tskel");
					sbOutFileMain.Append("\t\t{");
					{
						string path = $"{FileInfo.filePath}\\{FileInfo.baseFileName}";
						if (!Directory.Exists(path.Substring(0, path.Length - 1))) Directory.CreateDirectory(path);
						string skelFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{drawableName}.skel";
						sbOutFileMain.AppendLine($"\t\t\tskel {FileInfo.baseFileName}\\{drawableName}.skel");
						IV_skel.Build(br, drawable[a].m_pSkeleton, skelFileName, 0);
					}
					sbOutFileMain.Append("\t\t}");
				}
				//
				//lodgroup
				//
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
					if (drawable[a].m_dwObjectCount[b] > -1)
					{
						sbOutFileMain.Append($"\t\t\t{currentLevel} {modelCollection[a, b].m_wCount}");
						for (int c = 0; c < modelCollection[a, b].m_wCount; c++)
						{
							string MeshPath = $"{FileInfo.filePath}\\{FileInfo.baseFileName}";
							meshFileName = $"{MeshPath}\\{drawableName}_{currentLevel}_{c}.mesh";
							sbOutFileMain.Append($" {FileInfo.baseFileName}\\{drawableName}_{currentLevel}_{c}.mesh {model[currentModel].m_nBoneIndex}");
							// создаем папку если ее нет
							if (!Directory.Exists(MeshPath)) Directory.CreateDirectory(MeshPath);
							// пишем mesh файл
							if (!IV_mesh.Build(model[currentModel], br, vBounds, ref currentModel, ref currentGeometry, indexBuffer, vertexBuffer, meshFileName, vertexDeclaration))
							{
								Log.ToLog(Log.MessageType.ERROR, $"Failed to write mesh file");
								return false;
							}
						}
					}
					else sbOutFileMain.Append($"\t\t\t{currentLevel} none");
					if (b == 0) sbOutFileMain.AppendLine($" {drawable[a].m_vDrawDistance.X}");
					else if (b == 1) sbOutFileMain.AppendLine($" {drawable[a].m_vDrawDistance.Y}");
					else if (b == 2) sbOutFileMain.AppendLine($" {drawable[a].m_vDrawDistance.Z}");
					else sbOutFileMain.AppendLine($" {drawable[a].m_vDrawDistance.W}");
				}
				// остальная информация из секции drawable
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
			outFileMain = System.IO.File.Create(outFileName);
			swOutFileMain = new StreamWriter(outFileMain);
			swOutFileMain.Write(sbOutFileMain.ToString());
			swOutFileMain.Close();
			sbOutFileMain.Clear();
			br.Close();
			return true;
		}
	}
}
