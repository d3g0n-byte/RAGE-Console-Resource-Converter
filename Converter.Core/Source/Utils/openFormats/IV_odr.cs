using Converter.Core.ResourceTypes;
using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace Converter.Core.Utils.openFormats
{
	public static class IV_odr
	{
		public static bool Build(
			EndianBinaryReader br, Drawable drawable,
			Model[] model, Vector4[,] vBounds, IndexBuffer[] indexBuffer,
			VertexBuffer[] vertexBuffer, VertexDeclaration[] vertexDeclaration, Collection[] сModel,
			string[] shaderLine, IV_skel.UniversalSkeletonData skeletonData
		)
		{
			string outFileName = Path.Combine(Path.GetDirectoryName(Main.inputPath), $"{Path.GetFileNameWithoutExtension(Main.inputPath)}.odr");

			if (shaderLine.Length < 0 || drawable.m_pSkeleton != 0 || model.Length > 0)
			{
				Console.WriteLine($"[INFO] Writing {Path.GetFileNameWithoutExtension(Main.inputPath)}.odr");
			}
			else
			{
				Console.WriteLine("[WARNING] Empty file. ODR file will not be created, but if it contains textures, they are exported");
				return true;
			}

			StringBuilder sbOutFileMain = new StringBuilder();

			uint currentGeometry = 0;
			sbOutFileMain.AppendLine($"Version {110} {12}");
			sbOutFileMain.AppendLine($"shadinggroup");
			sbOutFileMain.AppendLine($"{{");
			sbOutFileMain.AppendLine($"\tShaders {shaderLine.Length}");
			sbOutFileMain.AppendLine($"\t{{");

			for (int b = 0; b < shaderLine.Length; b++)
			{
				sbOutFileMain.AppendLine($"\t\t{shaderLine[b]}");
			}

			sbOutFileMain.AppendLine("\t}");
			sbOutFileMain.AppendLine("}");

			if (drawable.m_pSkeleton != 0)
			{
				sbOutFileMain.AppendLine("skel");
				sbOutFileMain.AppendLine("{");

				string path = Path.Combine(Path.GetDirectoryName(Main.inputPath), Path.GetFileName(Main.inputPath).Replace(".", "_"));

				if (!Directory.Exists(path.Substring(0, path.Length - 1)))
				{
					Directory.CreateDirectory(path);
				}

				string skelFileName = $"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileName(Main.inputPath).Replace(".", "_")}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}.skel";

				sbOutFileMain.AppendLine($"\tskel {Path.GetFileName(Main.inputPath).Replace(".", "_")}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}.skel");

				IV_skel.Build(br, drawable.m_pSkeleton, skelFileName, skeletonData);
				
				sbOutFileMain.AppendLine("}");
			}

			// lodgroup
			sbOutFileMain.AppendLine($"lodgroup");
			sbOutFileMain.AppendLine($"{{");
			string currentLevel;
			string meshFileName;
			uint currentModel = 0;

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

					for (uint currentModelIndex = 0; currentModelIndex < сModel[a].m_nCount; /*b++*/)
					{
						string MeshPath = $"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileName(Main.inputPath).Replace(".", "_")}";

						meshFileName = $"{MeshPath}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}_{currentLevel}_{currentModelIndex}.mesh";
						
						sbOutFileMain.Append($" {Path.GetFileName(Main.inputPath).Replace(".", "_")}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}_{currentLevel}_{currentModelIndex}.mesh {model[currentModel].m_nBoneIndex}");

						if (!Directory.Exists(MeshPath))
						{
							Directory.CreateDirectory(MeshPath);
						}

						if (!IV_mesh.Build(model[currentModel++], br, vBounds, ref currentModelIndex, ref currentGeometry, indexBuffer, vertexBuffer, meshFileName, skeletonData.m_nBoneCount, vertexDeclaration))
						{
							Console.WriteLine("[ERROR] Failed to write mesh file");
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

			// other data from drawable section
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
