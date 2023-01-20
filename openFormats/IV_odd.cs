using ConsoleApp1;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Converter.RageResource;

namespace Converter.openFormats
{
	internal class IV_odd
	{
		public static bool Build(uint drawableCount, uint[] shaderFXCount2, RageResource.RDRShaderFX[] shaderFX, EndianBinaryReader br ,
			/*uint m_pSkeleton,*/ RageResource.Drawable[] drawable, uint[,] modelCount2, RageResource.Model[] model,
			Vector4[,] vBounds, RageResource.IndexBuffer[] indexBuffer, RageResource.VertexBuffer[] vertexBuffer,
			RageResource.VertexDeclaration[] vertexDeclaration)
		{
			FileStream outFileMain;
			StreamWriter swOutFileMain;
			StringBuilder sbOutFileMain = new StringBuilder();

			string outFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}.odd";

			uint currentModel = 0;
			uint currentGeometry = 0;
			int wddVersion = 110;
			int openIVfileVersion = 12;
			sbOutFileMain.AppendLine($"Version {wddVersion} {openIVfileVersion}");
			sbOutFileMain.AppendLine($"{{");
			uint fileCount = 0;
			string meshFileName;
			uint currentShaderFX = 0;
			uint currentValue = 0;
			for (int a = 0; a < drawableCount; a++)
			{
				sbOutFileMain.AppendLine($"\tgtaDrawable {fileCount++}");
				sbOutFileMain.AppendLine($"\t{{");
				sbOutFileMain.AppendLine($"\t\tshadinggroup");
				sbOutFileMain.AppendLine($"\t\t{{");
				sbOutFileMain.AppendLine($"\t\t\tShaders {shaderFXCount2[a]}");// 
				sbOutFileMain.AppendLine($"\t\t\t{{");
				Vector4 tmp4;
				if (Settings.bExportShaders)
				{
					for (int b = 0; b < shaderFXCount2[a]; b++)
					{
						sbOutFileMain.Append($"\t\t\t\t{shaderFX[currentShaderFX].m_dwNameHash}");
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
						currentShaderFX++;
						sbOutFileMain.AppendLine("");
					}
				}
				else for (int b = 0; b < shaderFXCount2[a]; b++) sbOutFileMain.AppendLine($"\t\t\t\tgta_default.sps temptxd");
				sbOutFileMain.AppendLine($"\t\t\t}}");
				sbOutFileMain.AppendLine($"\t\t}}");
				//
				// skel
				//
				if (drawable[a].m_pSkeleton != 0)
				{
					sbOutFileMain.Append("\t\tskel");
					sbOutFileMain.Append("\t\t{");
					{
						string path = $"{FileInfo.filePath}\\{FileInfo.baseFileName}";
						if (!Directory.Exists(path.Substring(0, path.Length - 1))) Directory.CreateDirectory(path);
						string skelFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{FileInfo.baseFileName}.skel";
						sbOutFileMain.AppendLine($"\t\t\tskel {FileInfo.baseFileName}\\{FileInfo.baseFileName}.skel");
						IV_skel.Build(br, drawable[a].m_pSkeleton, skelFileName);
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
						sbOutFileMain.Append($"\t\t\t{currentLevel} {modelCount2[a, b]}");
						for (int c = 0; c < modelCount2[a, b]; c++)
						{
							string MeshPath = $"{FileInfo.filePath}\\{FileInfo.baseFileName}";
							meshFileName = $"{MeshPath}\\{FileInfo.baseFileName}_{a}_{currentLevel}_{c}.mesh";
							sbOutFileMain.Append($" {FileInfo.baseFileName}\\{FileInfo.baseFileName}_{a}_{currentLevel}_{c}.mesh {model[currentModel].m_nBoneIndex}");
							// создаем папку если ее нет
							if (!Directory.Exists(MeshPath)) Directory.CreateDirectory(MeshPath);
							// пишем mesh файл
							if (!IV_mesh.Build(model[currentModel], br, vBounds, ref currentModel, ref currentGeometry, indexBuffer, vertexBuffer, meshFileName, vertexDeclaration))
							{
								Console.WriteLine("failed to write mesh file");
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
