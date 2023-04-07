using Converter.openFormats;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
	internal class MCLA_Drawable
	{
		public static bool ReadDrawable(MemoryStream ms, bool endian)
		{
			EndianBinaryReader br = new EndianBinaryReader(ms);
			if (endian) br.Endianness = Endian.BigEndian;
			RageResource.Drawable drawable = new RageResource.Drawable();
			drawable = ReadRageResource.Drawable(br);
			br.Position = drawable.m_pShaderGroup;
			RageResource.IV_ShaderGroup shadeGroup;
			shadeGroup = ReadRageResource.IV_ShaderGroup(br);
			if (shadeGroup.m_pTexture != 0)
				if (!TextureDictionary.ReadTextureDictionary(br, shadeGroup.m_pTexture))
				{
					ms.Close();
					br.Close();
					throw new Exception("Error while Texture Dictionary.");
				}
			RageResource.MCLAShaderFX[] fx = new RageResource.MCLAShaderFX[shadeGroup.m_pShaders.m_wCount];
			uint[] pShaderFX = new uint[shadeGroup.m_pShaders.m_wCount];
			br.Position = shadeGroup.m_pShaders.m_pList;
			for (int a = 0; a < shadeGroup.m_pShaders.m_wCount; a++) pShaderFX[a] = br.ReadOffset();
			for (int a = 0; a < shadeGroup.m_pShaders.m_wCount; a++)
			{
				br.Position = pShaderFX[a];
				fx[a] = ReadRageResource.MCLAShaderFX(br);
			}
			// модель
			uint currentModel = 0;
			uint[] pModel = new uint[200];

			for (int a = 0; a < 4; a++) // lod
			{
				if (drawable.m_pModelCollection[a] != 0)
				{
					br.Position = drawable.m_pModelCollection[a];
					RageResource.Collection сModel = br.ReadCollections();
					br.Position = сModel.m_pList;
					for (int b = 0; b < сModel.m_wCount; b++) pModel[currentModel++] = br.ReadOffset();
				}
			}
			uint modelCount = currentModel;
			Array.Resize<uint>(ref pModel, (int)modelCount);
			RageResource.Model[] model = new RageResource.Model[pModel.Length];
			for (int a = 0; a < pModel.Length; a++)
			{
				br.Position = pModel[a];
				model[a] = ReadRageResource.Model(br);
			}
			uint[] pGeometry = new uint[200];
			//uint geometryCount = 0;
			uint currentGeometry = 0;
			for (int a = 0; a < pModel.Length; a++)
			{
				br.Position = model[a].m_pGeometry.m_pList;
				for (int b = 0; b < model[a].m_pGeometry.m_wCount; b++) pGeometry[currentGeometry++] = br.ReadOffset();
			}
			uint geometryCount = currentGeometry;
			Array.Resize<uint>(ref pGeometry, (int)geometryCount);
			RageResource.Geometry[] geometry = new RageResource.Geometry[geometryCount];
			RageResource.VertexBuffer[] vertexBuffer = new RageResource.VertexBuffer[geometryCount];
			RageResource.IndexBuffer[] indexBuffer = new RageResource.IndexBuffer[geometryCount];
			RageResource.VertexDeclaration[] vertexDeclaration = new RageResource.VertexDeclaration[geometryCount];
			uint tmp;
			for (int a = 0; a < geometryCount; a++)
			{
				br.Position = pGeometry[a];
				geometry[a] = ReadRageResource.Geometry(br);
				br.Position = geometry[a].m_pVertexBuffer;
				vertexBuffer[a] = ReadRageResource.VertexBuffer(br);

				tmp = vertexBuffer[a].m_pVertexData;
				vertexBuffer[a].m_pVertexData = vertexBuffer[a].m_pDeclaration;
				vertexBuffer[a].m_pDeclaration = tmp;

				br.Position = vertexBuffer[a].m_pDeclaration;
				vertexDeclaration[a] = ReadRageResource.VertexDeclaration(br);
				br.Position = geometry[a].m_pIndexBuffer;
				indexBuffer[a] = ReadRageResource.IndexBuffer(br);
			}
			Vector4[,] vBounds = new Vector4[modelCount, 100];
			for (int a = 0; a < modelCount; a++)
			{
				br.Position = model[a].m_pBounds;
				uint boundsCount = model[a].m_pGeometry.m_wCount;
				if (model[a].m_pGeometry.m_wCount > 1) boundsCount++;
				for (int b = 0; b < boundsCount; b++) vBounds[a, b] = br.ReadVector4();
			}
			// пишем секцию ShaderGroup
			byte[] paramTypes = new byte[1];
			uint[] pParam = new uint[1];
			Vector4 vTmp = new Vector4();

			FileStream outFileMain;
			StreamWriter swOutFileMain;
			StringBuilder sbOutFileMain = new StringBuilder();
			string outFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}.odr";

			sbOutFileMain.AppendLine($"Version 110 12\nshadinggroup\n{{\n\tShaders {shadeGroup.m_pShaders.m_wCount}\n\t{{");
			if (Settings.bExportShaders)
				for (int a = 0; a < shadeGroup.m_pShaders.m_wCount; a++)
				{
					Array.Resize<byte>(ref paramTypes, fx[a].m_wParamsCount);
					br.Position = fx[a].m_pParameterTypes;
					for (int b = 0; b < fx[a].m_wParamsCount; b++) paramTypes[b] = br.ReadByte();
					Array.Resize<uint>(ref pParam, fx[a].m_wParamsCount);
					br.Position = fx[a].m_pShaderParams;
					for (int b = 0; b < fx[a].m_wParamsCount; b++) pParam[b] = br.ReadOffset();
					sbOutFileMain.Append("\t\t");
					sbOutFileMain.Append(Path.GetFileName(DataUtils.ReadStringAtOffset(fx[a].m_pSPS, br)));
					for (int b = 0; b < fx[a].m_wParamsCount; b++)
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
									if (c != 3) sbOutFileMain.Append(";");
								}
								break;
							case 8:
								sbOutFileMain.Append(" ");
								for (int c = 0; c < 6; c++)
								{
									vTmp = br.ReadVector4();
									sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
									if (c != 5) sbOutFileMain.Append(";");
								}
								break;
							case 9:
								sbOutFileMain.Append(" ");
								for (int c = 0; c < 9; c++)
								{
									vTmp = br.ReadVector4();
									sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
									if (c != 8) sbOutFileMain.Append(";");
								}
								break;
							case 14:
								sbOutFileMain.Append(" ");
								for (int c = 0; c < 14; c++)
								{
									vTmp = br.ReadVector4();
									sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
									if (c != 13) sbOutFileMain.Append(";");
								}
								break;
							case 15:
								sbOutFileMain.Append(" ");
								for (int c = 0; c < 15; c++)
								{
									vTmp = br.ReadVector4();
									sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
									if (c != 14) sbOutFileMain.Append(";");
								}
								break;
							case 16:
								sbOutFileMain.Append(" ");
								for (int c = 0; c < 16; c++)
								{
									vTmp = br.ReadVector4();
									sbOutFileMain.Append($"{vTmp.X};{vTmp.Y};{vTmp.Z};{vTmp.W}");
									if (c != 15) sbOutFileMain.Append(";");
								}
								break;
						}

					}
					sbOutFileMain.AppendLine();
				}
			else
				for (int a = 0; a < shadeGroup.m_pShaders.m_wCount; a++)
					sbOutFileMain.AppendLine("\t\tgta_default.sps null");
			sbOutFileMain.AppendLine($"\t}}");
			sbOutFileMain.AppendLine($"}}");
			if (drawable.m_pSkeleton != 0)
			{
				sbOutFileMain.AppendLine("skel");
				sbOutFileMain.AppendLine("{");
				{
					string path = $"{FileInfo.filePath}\\{FileInfo.baseFileName}";
					if (!Directory.Exists(path.Substring(0, path.Length - 1))) Directory.CreateDirectory(path);
					string skelFileName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{FileInfo.baseFileName}.skel";
					sbOutFileMain.AppendLine($"\tskel {FileInfo.baseFileName}\\{FileInfo.baseFileName}.skel");
					IV_skel.Build(br, drawable.m_pSkeleton, skelFileName, 1);
				}
				sbOutFileMain.AppendLine("}");
			}
			// строим секцию lodgroup
			sbOutFileMain.AppendLine($"lodgroup");
			sbOutFileMain.AppendLine($"{{");
			string currentLevel;
			string meshFileName;
			currentGeometry = 0;
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
				if (drawable.m_dwObjectCount[a] > -1)
				{
					sbOutFileMain.Append($"\t{currentLevel} {model.Length}");
					for (uint b = 0; b < model.Length; /*b++*/)
					{
						string MeshPath = $"{FileInfo.filePath}\\{FileInfo.baseFileName}";
						meshFileName = $"{MeshPath}\\{FileInfo.baseFileName}_{currentLevel}_{b}.mesh";
						sbOutFileMain.Append($" {FileInfo.baseFileName}\\{FileInfo.baseFileName}_{currentLevel}_{b}.mesh {model[b].m_nBoneIndex}");

						if (!Directory.Exists(MeshPath)) Directory.CreateDirectory(MeshPath);

						if (!IV_mesh.Build(model[b], br, vBounds, ref b, ref currentGeometry, indexBuffer, vertexBuffer, meshFileName, vertexDeclaration))
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
			}
			else
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
