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
		RageResource.VertexDeclaration[] vertexDeclaration)
		{
			FileStream outFileMain;
			StreamWriter swOutFileMain;
			StringBuilder sbOutFileMain = new StringBuilder();
			string outFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}.odr";
			//uint currentModel = 0;
			uint currentGeometry = 0;
			sbOutFileMain.AppendLine($"Version {110} {12}");
			sbOutFileMain.AppendLine($"shadinggroup");
			sbOutFileMain.AppendLine($"{{");
			sbOutFileMain.AppendLine($"\tShaders {shaderFX.Length}");// 
			sbOutFileMain.AppendLine($"\t{{");
			Vector4 tmp4;
			if (Settings.bExportShaders)
			{
				for (int b = 0; b < shaderFX.Length; b++)
				{
					sbOutFileMain.Append($"\t\t{shaderFX[b].m_dwNameHash}");
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
					sbOutFileMain.AppendLine("");
				}
			}
			else for (int b = 0; b < shaderFX.Length; b++) sbOutFileMain.AppendLine($"\t\tgta_default.sps temptxd");
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
					IV_skel.Build(br, drawable.m_pSkeleton, skelFileName);
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
					sbOutFileMain.Append($"\t{currentLevel} {model.Length}");
					for (uint b = 0; b < model.Length; /*b++*/)
					{
						string MeshPath = $"{FileInfo.filePath}\\{FileInfo.baseFileName}";
						meshFileName = $"{MeshPath}\\{FileInfo.baseFileName}_{currentLevel}_{b}.mesh";
						sbOutFileMain.Append($" {FileInfo.baseFileName}\\{FileInfo.baseFileName}_{currentLevel}_{b}.mesh {model[b].m_nBoneIndex}");

						if (!Directory.Exists(MeshPath)) Directory.CreateDirectory(MeshPath);

						if (!IV_mesh.Build(model[b], br, vBounds,ref b, ref currentGeometry, indexBuffer, vertexBuffer, meshFileName, vertexDeclaration))
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
