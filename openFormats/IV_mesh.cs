using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Converter.openFormats
{
	internal class IV_mesh
	{
		public static unsafe void Add(ref int aa)
		{
			int a = 100;
			//			memmove()
			aa++;
		}
		public static unsafe bool Build(RageResource.Model model, EndianBinaryReader br, Vector4[,] vBouds, ref uint currentModel, ref uint currentGeometry,
			RageResource.IndexBuffer[] indexBuffer, RageResource.VertexBuffer[] vertexBuffer, string meshFileName,
			RageResource.VertexDeclaration[] vertexDeclaration)
		{

			FileStream outFileMesh;
			StreamWriter swOutFileMesh;
			StringBuilder sbOutFileMesh = new StringBuilder();

			sbOutFileMesh.AppendLine($"Version {11} {13}");
			sbOutFileMesh.AppendLine($"{{");
			sbOutFileMesh.AppendLine($"\tSkinned {Convert.ToInt32(model.m_bSkinned)}");
			if (!model.m_bSkinned)//skinned
			{
				// пишем границы, которые есть только в non skinned модели
				uint boundsCount = model.m_pGeometry.m_wCount;
				if (model.m_pGeometry.m_wCount > 1) boundsCount++;
				sbOutFileMesh.AppendLine($"\tBounds {boundsCount}");
				sbOutFileMesh.AppendLine($"\t{{");
				if (!Settings.bSwapYAndZ)
					for (int d = 0; d < boundsCount; d++)
						sbOutFileMesh.AppendLine($"\t\t{vBouds[currentModel, d].X} {vBouds[currentModel, d].Y} {vBouds[currentModel, d].Z} {vBouds[currentModel, d].W}");
				else
					for (int d = 0; d < boundsCount; d++)
						sbOutFileMesh.AppendLine($"\t\t{vBouds[currentModel, d].X} {vBouds[currentModel, d].Z} {vBouds[currentModel, d].Y} {vBouds[currentModel, d].W}");

				sbOutFileMesh.AppendLine($"\t}}");

			}
			// MaterialMapping for this model
			ushort[] wMaterialMapping = new ushort[100];

			br.Position = model.m_pShaderMapping;
			for (int d = 0; d < model.m_pGeometry.m_wCount; d++) wMaterialMapping[d] = br.ReadUInt16();
			// the geometry section is one material in the mesh file
			for (int d = 0; d < model.m_pGeometry.m_wCount; d++)
			{
				sbOutFileMesh.AppendLine($"\tMtl {wMaterialMapping[d]}");
				sbOutFileMesh.AppendLine($"\t{{");
				sbOutFileMesh.AppendLine($"\t\tPrim {0}");
				sbOutFileMesh.AppendLine($"\t\t{{");
				sbOutFileMesh.AppendLine($"\t\t\tIdx {indexBuffer[currentGeometry].m_dwIndexCount}");
				sbOutFileMesh.AppendLine($"\t\t\t{{");
				if (!GFX.Indices(br, sbOutFileMesh, indexBuffer[currentGeometry].m_pIndexData, indexBuffer[currentGeometry].m_dwIndexCount))
				{
					Console.WriteLine("Failed to export indices");
					return false;
				}
				sbOutFileMesh.AppendLine($"\t\t\t}}");
				sbOutFileMesh.AppendLine($"\t\t\tVerts {vertexBuffer[currentGeometry].m_wVertexCount}");
				sbOutFileMesh.AppendLine($"\t\t\t{{");
				if (!GFX.Vertex(br, sbOutFileMesh, vertexDeclaration[currentGeometry], vertexBuffer[currentGeometry].m_pVertexData, vertexBuffer[currentGeometry].m_wVertexCount))
				{
					Console.WriteLine("Failed to export vertex");
					return false;
				}
				sbOutFileMesh.AppendLine($"\t\t\t}}");
				sbOutFileMesh.AppendLine($"\t\t}}");
				sbOutFileMesh.AppendLine($"\t}}");
				currentGeometry++;
			}
			sbOutFileMesh.AppendLine($"}}");

			outFileMesh = System.IO.File.Create(meshFileName);
			swOutFileMesh = new StreamWriter(outFileMesh);
			swOutFileMesh.Write(sbOutFileMesh.ToString());
			swOutFileMesh.Close();
			sbOutFileMesh.Clear();
			currentModel++;
			return true;
		}
	}
}
