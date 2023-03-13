using Converter.openFormats;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Converter.RageResource;

namespace Converter
{
	internal class MCLA_63
	{
		struct RES_63
		{
			public uint _vmt;
			public uint end;
			public uint m_pModelCollection;
			public uint _fc; // нули?
			public uint _f10; // нули?
			public uint m_pName;
			public uint _f18; // нули?
			public uint _f1c; // нули?
			public ushort _f20; //
			public ushort _f22; //
			public uint m_pSkelData;
			// это не все...
		}

		public static bool Read(MemoryStream mem, bool endian)
		{
			uint tmp;
			EndianBinaryReader br = new EndianBinaryReader(mem);
			if (endian) br.Endianness = Endian.BigEndian;
			RES_63 file;
			file._vmt = br.ReadUInt32();
			file.end = br.ReadOffset();
			file.m_pModelCollection = br.ReadOffset();
			file._fc = br.ReadUInt32();
			file._f10 = br.ReadUInt32();
			file.m_pName = br.ReadOffset();
			file._f18 = br.ReadUInt32();
			file._f1c = br.ReadUInt32();
			file._f20 = br.ReadUInt16();
			file._f22 = br.ReadUInt16();
			file.m_pSkelData = br.ReadOffset();

			br.Position = file.m_pModelCollection;
			RageResource.Collection modelCollection = br.ReadCollections();
			RageResource.Model[] model = new RageResource.Model[modelCollection.m_wCount];
			uint[] pModel = new uint[modelCollection.m_wCount];
			br.Position = modelCollection.m_pList;
			for (int a = 0; a < modelCollection.m_wCount; a++) pModel[a] = br.ReadOffset();
			for (int a = 0; a < modelCollection.m_wCount; a++)
			{
				br.Position = pModel[a];
				model[a] = ReadRageResource.Model(br);
			}
			uint geometryCount = 0;
			for (int a = 0; a < modelCollection.m_wCount; a++) geometryCount += model[a].m_pGeometry.m_wCount;
			uint[] pGeometry = new uint[geometryCount];
			RageResource.Geometry[] geometry = new RageResource.Geometry[geometryCount];
			RageResource.VertexBuffer[] vertexBuffer = new RageResource.VertexBuffer[geometryCount];
			RageResource.IndexBuffer[] indexBuffer = new RageResource.IndexBuffer[geometryCount];
			RageResource.VertexDeclaration[] vertexDeclaration = new RageResource.VertexDeclaration[geometryCount];
			uint currentGeometry = 0;
			for (int a = 0; a < modelCollection.m_wCount; a++)
			{
				br.Position = model[a].m_pGeometry.m_pList;
				for (int b = 0; b < model[a].m_pGeometry.m_wCount; b++)
					pGeometry[currentGeometry++] = br.ReadOffset();
			}
			for (int a = 0; a < geometryCount; a++)
			{
				br.Position = pGeometry[a];
				geometry[a] = ReadRageResource.Geometry(br);
				br.Position = geometry[a].m_pVertexBuffer;
				vertexBuffer[a] = ReadRageResource.VertexBuffer(br);
				tmp = vertexBuffer[a].m_pVertexData;
				vertexBuffer[a].m_pVertexData = vertexBuffer[a].m_pDeclaration;
				vertexBuffer[a].m_pDeclaration = tmp;
				br.Position = geometry[a].m_pIndexBuffer;
				indexBuffer[a] = ReadRageResource.IndexBuffer(br);
				br.Position = vertexBuffer[a].m_pDeclaration;
				vertexDeclaration[a] = ReadRageResource.VertexDeclaration(br);
			}
			Vector4[,] vBounds = new Vector4[modelCollection.m_wCount, 100];
			for (int a = 0; a < modelCollection.m_wCount; a++)
			{
				br.Position = model[a].m_pBounds;
				uint boundsCount = model[a].m_pGeometry.m_wCount;
				if (model[a].m_pGeometry.m_wCount > 1) boundsCount++;
				for (int b = 0; b < boundsCount; b++) vBounds[a, b] = br.ReadVector4();
			}

			// содаем часть odr файла...
			FileStream outFileMain;
			StreamWriter swOutFileMain;
			StringBuilder sbOutFileMain = new StringBuilder();
			string outFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}.odr";
			sbOutFileMain.AppendLine($"Version 110 12\nshadinggroup\n{{\n\tShaders {Settings.nTempShadersCount}\n\t{{");
			for (int a = 0; a < Settings.nTempShadersCount; a++) sbOutFileMain.AppendLine("\t\tgta_default.sps null");
			sbOutFileMain.AppendLine($"\t}}");
			sbOutFileMain.AppendLine($"}}");
			if (file.m_pSkelData != 0)
			{
				sbOutFileMain.AppendLine("skel");
				sbOutFileMain.AppendLine("{");
				{
					string path = $"{FileInfo.filePath}\\{FileInfo.baseFileName}";
					if (!Directory.Exists(path.Substring(0, path.Length - 1))) Directory.CreateDirectory(path);
					string skelFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{FileInfo.baseFileName}.skel";
					sbOutFileMain.AppendLine($"\tskel {FileInfo.baseFileName}\\{FileInfo.baseFileName}.skel");
					IV_skel.Build(br, file.m_pSkelData, skelFileName, 1);
				}
				sbOutFileMain.AppendLine("}");
			}

			string meshFileName;
			sbOutFileMain.AppendLine("lodgroup");
			sbOutFileMain.AppendLine("{");
			sbOutFileMain.Append($"\t{"high"} {model.Length}");
			currentGeometry = 0;
			for (uint b = 0; b < model.Length; /*b++*/)
			{
				string MeshPath = $"{FileInfo.filePath}\\{FileInfo.baseFileName}";
				meshFileName = $"{MeshPath}\\{FileInfo.baseFileName}_{b}.mesh";
				sbOutFileMain.Append($" {FileInfo.baseFileName}\\{FileInfo.baseFileName}_{b}.mesh {model[b].m_nBoneIndex}");
				if (!Directory.Exists(MeshPath)) Directory.CreateDirectory(MeshPath);
				if (!IV_mesh.Build(model[b], br, vBounds, ref b, ref currentGeometry, indexBuffer, vertexBuffer, meshFileName, vertexDeclaration))
					throw new Exception("failed to write mesh file");
			}
			sbOutFileMain.AppendLine($" {9999.0}");
			sbOutFileMain.AppendLine($"\tmed none {9999.0}");
			sbOutFileMain.AppendLine($"\tlow none {9999.0}");
			sbOutFileMain.AppendLine($"\tvlow none {9999.0}");
			sbOutFileMain.AppendLine($"\tcenter {0} {0} {0}");
			sbOutFileMain.AppendLine($"\tAABBMin {-10f} {-10f} {-10f}");
			sbOutFileMain.AppendLine($"\tAABBMax {10f} {10f} {10f}");
			sbOutFileMain.AppendLine($"\tradius {10f}");
			sbOutFileMain.AppendLine("}");

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
