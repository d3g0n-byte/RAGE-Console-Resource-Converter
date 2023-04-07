using Converter.utils;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Converter.openFormats
{
	internal class IV_odr
	{

		public static bool Build(RageResource.RDRShaderFX[] shaderFX, EndianBinaryReader br,
		RageResource.Drawable drawable, RageResource.Model[] model,
		Vector4[,] vBounds, RageResource.IndexBuffer[] indexBuffer, RageResource.VertexBuffer[] vertexBuffer,
		RageResource.VertexDeclaration[] vertexDeclaration, RageResource.Collection[] сModel)
		{
			if (shaderFX.Length < 0|| drawable.m_pSkeleton != 0|| model.Length>0) Log.ToLog(Log.MessageType.INFO, $"Exporting to IV openFormats...");
			else { Log.ToLog(Log.MessageType.WARNING, $"Empty file. odr file will not be created, but if it contains textures, they are exported"); return true; }


			FileStream outFileMain;
			StreamWriter swOutFileMain;
			StringBuilder sbOutFileMain = new StringBuilder();
			string outFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}.odr";
			uint currentGeometry = 0;
			sbOutFileMain.AppendLine($"Version {110} {12}");
			sbOutFileMain.AppendLine($"shadinggroup");
			sbOutFileMain.AppendLine($"{{");
			sbOutFileMain.AppendLine($"\tShaders {shaderFX.Length}");// 
			sbOutFileMain.AppendLine($"\t{{");
			Vector4 tmp4;
			string shaderName;
			if (Settings.bExportShaders)
			{
				for (int b = 0; b < shaderFX.Length; b++)
				{
					if (!RDR_ShaderManager.ShaderNames.shaderNames.TryGetValue(shaderFX[b].m_dwNameHash, out shaderName)) shaderName = $"0x{shaderFX[b].m_dwNameHash.ToString("X8")}";
					sbOutFileMain.Append($"\t\t{shaderName}");
					int shaderIndexInShaderManager = RDR_ShaderManager.GetShaderIndex(shaderName);
					if (shaderIndexInShaderManager == -1)
					{
						for (int c = 0; c < shaderFX[b].m_nParamCount; c++)
						{
							if (shaderFX[b].value[c].m_pValue != 0)
							{
								br.Position = shaderFX[b].value[c].m_pValue;
								if (shaderFX[b].value[c].m_nParamType == 0)// текстура
								{
									sbOutFileMain.Append($" {DataUtils.ReadStringAtOffset(ReadRageResource.RDRTextureDefinition(br).m_pName, br)}");
								}
								else if (shaderFX[b].value[c].m_nParamType == 9)// matrix9x4
								{
									sbOutFileMain.Append(" ");
									for (int i = 0; i < 9; i++)
									{
										tmp4 = br.ReadVector4();
										sbOutFileMain.Append($"({tmp4.X};{tmp4.Y};{tmp4.Z};{tmp4.W})");
									}
								}
								else if (shaderFX[b].value[c].m_nParamType == 1)// Vector4
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
										samplerBuffer[currentPosInSamplerBuffer++] = $"{DataUtils.ReadStringAtOffset(ReadRageResource.RDRTextureDefinition(br).m_pName, br)}";
									else samplerBuffer[currentPosInSamplerBuffer++] = $"[null]";
									break;
								case 1:
									if (shaderFX[b].value[c].m_pValue != 0)
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
					sbOutFileMain.AppendLine("");
				}
			}
			else
			{
				for (int b = 0; b < shaderFX.Length; b++)
				{
					if (!RDR_ShaderManager.ShaderNames.shaderNames.TryGetValue(shaderFX[b].m_dwNameHash, out shaderName)) shaderName = $"0x{shaderFX[b].m_dwNameHash.ToString("X8")}";
					int shaderIndexInShaderManager = RDR_ShaderManager.GetShaderIndex(shaderName);
					if (shaderIndexInShaderManager == -1)
						sbOutFileMain.AppendLine($"\t\tgta_default.sps temptxd");
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
										samplerBuffer[currentPosInSamplerBuffer++] = $"{DataUtils.ReadStringAtOffset(ReadRageResource.RDRTextureDefinition(br).m_pName, br)}";
									else samplerBuffer[currentPosInSamplerBuffer++] = $"[null]";
									break;
								case 1:
									if (shaderFX[b].value[c].m_pValue != 0)
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
							diffuseValue = $"{FileInfo.baseFileName}\\{diffuseValue}";
						}
						sbOutFileMain.AppendLine($"\t\tgta_default.sps {diffuseValue}");
					}
				}
			}
			sbOutFileMain.AppendLine("\t}");
			sbOutFileMain.AppendLine("}");

			if (drawable.m_pSkeleton != 0)
			{
				sbOutFileMain.AppendLine("skel");
				sbOutFileMain.AppendLine("{");
				{
					string path = $"{FileInfo.filePath}\\{FileInfo.baseFileName}";
					if (!Directory.Exists(path.Substring(0, path.Length - 1))) Directory.CreateDirectory(path);
					string skelFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{FileInfo.baseFileName}.skel";
					sbOutFileMain.AppendLine($"\tskel {FileInfo.baseFileName}\\{FileInfo.baseFileName}.skel");
					IV_skel.Build(br, drawable.m_pSkeleton, skelFileName, 0);
				}
				sbOutFileMain.AppendLine("}");
			}
			//
			//lodgroup
			//
			sbOutFileMain.AppendLine($"lodgroup");
			sbOutFileMain.AppendLine($"{{");
			string currentLevel;
			string meshFileName;
			uint currentModel = 0;
			for (int a = 0;a < 4; a++)
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
				if (drawable.m_dwObjectCount[a] > -1)
				{
					sbOutFileMain.Append($"\t{currentLevel} {сModel[a].m_wCount}");
					for (uint b = 0; b < сModel[a].m_wCount; /*b++*/)
					{
						string MeshPath = $"{FileInfo.filePath}\\{FileInfo.baseFileName}";
						meshFileName = $"{MeshPath}\\{FileInfo.baseFileName}_{currentLevel}_{b}.mesh";
						sbOutFileMain.Append($" {FileInfo.baseFileName}\\{FileInfo.baseFileName}_{currentLevel}_{b}.mesh {model[currentModel].m_nBoneIndex}");

						if (!Directory.Exists(MeshPath)) Directory.CreateDirectory(MeshPath);

						if (!IV_mesh.Build(model[currentModel++], br, vBounds,ref b, ref currentGeometry, indexBuffer, vertexBuffer, meshFileName, vertexDeclaration))
						{
							Console.WriteLine("failed to write mesh file");
							return false;
						}
					}
				}
				else sbOutFileMain.Append($"\t{currentLevel} none");
				if (a == 0) sbOutFileMain.AppendLine($" {drawable.m_vDrawDistance.X}");
				else if (a == 1) sbOutFileMain.AppendLine($" {drawable.m_vDrawDistance.Y}");
				else if (a == 2) sbOutFileMain.AppendLine($" {drawable.m_vDrawDistance.Z}");
				else sbOutFileMain.AppendLine($" {drawable.m_vDrawDistance.W}");
			}
			// остальная информация из секции drawable
			if (!Settings.bSwapYAndZ)
			{
				sbOutFileMain.AppendLine($"\tcenter {drawable.m_vCenter.X} {drawable.m_vCenter.Y} {drawable.m_vCenter.Z}");
				sbOutFileMain.AppendLine($"\tAABBMin {drawable.m_vAabbMin.X} {drawable.m_vAabbMin.Y} {drawable.m_vAabbMin.Z}");
				sbOutFileMain.AppendLine($"\tAABBMax {drawable.m_vAabbMax.X} {drawable.m_vAabbMax.Y} {drawable.m_vAabbMax.Z}");
			}else
			{
				sbOutFileMain.AppendLine($"\tcenter {drawable.m_vCenter.X} {drawable.m_vCenter.Z} {drawable.m_vCenter.Y}");
				sbOutFileMain.AppendLine($"\tAABBMin {drawable.m_vAabbMin.X} {drawable.m_vAabbMin.Z} {drawable.m_vAabbMin.Y}");
				sbOutFileMain.AppendLine($"\tAABBMax {drawable.m_vAabbMax.X} {drawable.m_vAabbMax.Z} {drawable.m_vAabbMax.Y}");
			}
			sbOutFileMain.AppendLine($"\tradius {drawable.m_vRadius.X}");
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
