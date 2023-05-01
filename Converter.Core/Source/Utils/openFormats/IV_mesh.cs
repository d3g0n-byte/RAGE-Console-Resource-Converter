using System;
using System.Numerics;
using System.Text;
using System.IO;
using Converter.Core.ResourceTypes;

namespace Converter.Core.Utils.openFormats
{
	public static class IV_mesh
	{
		public static bool Build(Model model, EndianBinaryReader br, Vector4[,] vBouds, ref uint currentModel, ref uint currentGeometry,
			IndexBuffer[] indexBuffer, VertexBuffer[] vertexBuffer, string meshFileName, ushort boneCount,
			VertexDeclaration[] vertexDeclaration)
		{
			StringBuilder sbOutFileMesh = new StringBuilder();

			if (Main.useVeryVerboseMode)
			{
				Console.WriteLine($"[INFO] Model{currentModel}:");
			}

			sbOutFileMesh.AppendLine($"Version {11} {13}");
			sbOutFileMesh.AppendLine($"{{");
			sbOutFileMesh.AppendLine($"\tSkinned {Convert.ToInt32(model.m_bSkinned)}");

			if (!model.m_bSkinned) //skinned
			{
				// write a bounds which only present in non-skinned models
				uint boundsCount = model.m_cGeometry.m_nCount > 1 ? (uint)model.m_cGeometry.m_nCount + 1 : model.m_cGeometry.m_nCount;
				if (Main.useVeryVerboseMode)
				{
					Console.WriteLine($"    Bounds: {boundsCount}");
				}

				//if (model.m_pGeometry.m_wCount > 1)
				//{
				//boundsCount++; // I've lost approx. a hour because of that
				//}

				sbOutFileMesh.AppendLine($"\tBounds {boundsCount}");
				sbOutFileMesh.AppendLine($"\t{{");

				if (!Settings.bSwapYAndZ)
				{
					for (int d = 0; d < boundsCount; d++)
					{
						sbOutFileMesh.AppendLine($"\t\t{vBouds[currentModel, d].X} {vBouds[currentModel, d].Y} {vBouds[currentModel, d].Z} {vBouds[currentModel, d].W}");
					}
				}
				else
				{
					for (int d = 0; d < boundsCount; d++)
					{
						sbOutFileMesh.AppendLine($"\t\t{vBouds[currentModel, d].X} {vBouds[currentModel, d].Z} {vBouds[currentModel, d].Y} {vBouds[currentModel, d].W}");
					}
				}

				sbOutFileMesh.AppendLine($"\t}}");
			}

			// MaterialMapping for this model
			ushort[] wMaterialMapping = new ushort[100];

			br.Position = model.m_pShaderMapping;
			for (int d = 0; d < model.m_cGeometry.m_nCount; d++)
			{
				wMaterialMapping[d] = br.ReadUInt16();
			}

			// the geometry section is one material in the mesh file
			if (Main.useVeryVerboseMode)
			{
				Console.WriteLine($"    Geometries: {model.m_cGeometry.m_nCount}");
			}

			for (int d = 0; d < model.m_cGeometry.m_nCount; d++)
			{
				if (Main.useVeryVerboseMode)
				{
					Console.WriteLine($"[INFO] Geometry{d}:");
					Console.WriteLine($"[INFO] Material ID: {wMaterialMapping[d]}");
				}

				sbOutFileMesh.AppendLine($"\tMtl {wMaterialMapping[d]}");
				sbOutFileMesh.AppendLine($"\t{{");
				sbOutFileMesh.AppendLine($"\t\tPrim {0}");
				sbOutFileMesh.AppendLine($"\t\t{{");

				if (Main.useVeryVerboseMode)
				{
					Console.WriteLine($"[INFO] Indices: {indexBuffer[currentGeometry].m_nIndexCount}");
				}

				sbOutFileMesh.AppendLine($"\t\t\tIdx {indexBuffer[currentGeometry].m_nIndexCount}");
				sbOutFileMesh.AppendLine($"\t\t\t{{");

				if (!GFX.ExportIndexBuffer(br, sbOutFileMesh, indexBuffer[currentGeometry].m_pIndexData, indexBuffer[currentGeometry].m_nIndexCount))
				{
					Console.WriteLine($"[ERROR] Failed to export indices");
					return false;
				}

				sbOutFileMesh.AppendLine($"\t\t\t}}");

				if (Main.useVeryVerboseMode)
				{
					Console.WriteLine($"[INFO] Vertices: {vertexBuffer[currentGeometry].m_nVertexCount}");
				}

				sbOutFileMesh.AppendLine($"\t\t\tVerts {vertexBuffer[currentGeometry].m_nVertexCount}");
				sbOutFileMesh.AppendLine($"\t\t\t{{");

				if (!GFX.ExportVertexBuffer(br, sbOutFileMesh, vertexDeclaration[currentGeometry], vertexBuffer[currentGeometry].m_pVertexData, vertexBuffer[currentGeometry].m_nVertexCount, model.m_bSkinned, boneCount))
				{
					Console.WriteLine($"[ERROR] Failed to export vertices");
					return false;
				}

				sbOutFileMesh.AppendLine($"\t\t\t}}");
				sbOutFileMesh.AppendLine($"\t\t}}");
				sbOutFileMesh.AppendLine($"\t}}");

				currentGeometry++;
			}

			sbOutFileMesh.AppendLine($"}}");

			using (FileStream outFileMesh = File.Create(meshFileName))
			{
				using (StreamWriter swOutFileMesh = new StreamWriter(outFileMesh))
				{
					swOutFileMesh.Write(sbOutFileMesh.ToString());
				}
			}

			sbOutFileMesh.Clear();
			currentModel++;

			return true;
		}
	}
}
