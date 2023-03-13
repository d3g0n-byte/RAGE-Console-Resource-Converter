using Converter;
using Converter.openFormats;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Converter.RageResource;
using static System.Net.WebRequestMethods;

namespace ConsoleApp1
{
	internal class RDR_VolumeData
	{
		public static bool ReadVolumeData(MemoryStream decompMem, bool endian)
		{
			EndianBinaryReader br = new EndianBinaryReader(decompMem);
			if (endian)br.Endianness = Endian.BigEndian;// 0 - lit, 1 - big
			// узнаем позицию начальной страницы в файле
			br.Position = ResourceUtils.FlagInfo.RSC85_ObjectStart;
			RageResource.RDRVolumeData volumeData;
			RageResource.Drawable[] drawable = new RageResource.Drawable[1];

			volumeData = ReadRageResource.RDRVolumeData(br);
			// xtd 
			if (volumeData.pTexture != 0) TextureDictionary.ReadTextureDictionary(br, volumeData.pTexture);

			// offset to drawable sections
			br.Position = volumeData.cDrawable.m_pList;
			uint[] pDrawable = new uint[volumeData.cDrawable.m_wCount];
			for (int a = 0; a  < volumeData.cDrawable.m_wCount; a ++) pDrawable[a] = br.ReadOffset();

			Array.Resize<RageResource.Drawable>(ref drawable, volumeData.cDrawable.m_wCount);
			// read drawable sections
			for (ushort a = 0; a < volumeData.cDrawable.m_wCount; a++)
			{
				br.Position = pDrawable[a];
				drawable[a] = ReadRageResource.Drawable(br);
			}
			RageResource.RDRShaderGroup[] shaderGroup = new RageResource.RDRShaderGroup[volumeData.cDrawable.m_wCount];
			// offset to shadergroup section
			uint[] pShaderGroup = new uint[volumeData.cDrawable.m_wCount];
			for (int a = 0; a < volumeData.cDrawable.m_wCount; a++)
			{
				pShaderGroup[a] = drawable[a].m_pShaderGroup;
			}
			// read shadergroup sections
			for (int a = 0; a < volumeData.cDrawable.m_wCount; a++)
			{
				br.Position = pShaderGroup[a];
				shaderGroup[a] = ReadRageResource.RDRShaderGroup(br);
			}
			// shaderFX sections count
			uint shaderFXCount = 0;
			uint[] shaderFXCount2 = new uint[100];
			for (int a = 0; a < volumeData.cDrawable.m_wCount; a++)
			{
				br.Position = drawable[a].m_pShaderGroup+12;
				ushort fxcount = br.ReadUInt16();
				shaderFXCount += fxcount;
				shaderFXCount2[a] = fxcount;
			}
			RageResource.RDRShaderFX[] shaderFX = new RageResource.RDRShaderFX[shaderFXCount];
			// offset
			uint currentShaderFX = 0;
			uint[] pShaderFX = new uint[shaderFXCount];
			for (int a = 0; a < volumeData.cDrawable.m_wCount; a++)
			{
				br.Position = drawable[a].m_pShaderGroup + 8;
				br.Position = br.ReadOffset();
				for (int b = 0; b < shaderFXCount2[a]; b++)pShaderFX[currentShaderFX++] = br.ReadOffset();
			}
			// read shaderFX sections
			for (int a = 0; a < shaderFXCount; a++)
			{
				br.Position = pShaderFX[a];
				shaderFX[a] = ReadRageResource.RDRShaderFX(br);
			}

			// model sections count
			uint modelCount = 0;
			uint[,] modelCount2 = new uint[volumeData.cDrawable.m_wCount, 4];
			for (ushort a = 0; a < volumeData.cDrawable.m_wCount; a++)
			{
				for (int b = 0; b < 4; b++)
				{
					if (drawable[a].m_pModelCollection[b]!=0)
					{
						br.Position = drawable[a].m_pModelCollection[b];
						br.ReadUInt32();
						ushort count = br.ReadUInt16();
						modelCount2[a,b] = count;
						modelCount += count;
					}
				}
			}
			RageResource.Model[] model = new RageResource.Model[modelCount];
			int currentModel = 0;
			// read model sections
			for (int a = 0; a < drawable.Length; a++)
			{
				for (int b = 0; b < 4; b++) // lod
				{
					if (drawable[a].m_pModelCollection[b] != 0)
					{
						br.Position = drawable[a].m_pModelCollection[b];
						uint pModel = br.ReadOffset();
						uint wModelCount = br.ReadUInt16();
						uint wModelSize = br.ReadUInt16();

						br.Position = pModel;
						uint[] pModel2 = new uint[wModelCount];
						for (int c = 0; c < wModelCount; c++) pModel2[c] = br.ReadOffset();
						for (int c = 0; c < wModelCount; c++)
						{
							br.Position = pModel2[c];
							model[currentModel++] = ReadRageResource.Model(br);
						}
					}
				}
			}
			// 
			uint geometryCount = 0;
			for (int a = 0; a < modelCount; a++)geometryCount += model[a].m_pGeometry.m_wCount;
			RageResource.Geometry[] geometry = new RageResource.Geometry[geometryCount];
			// offset to geometry
			uint[] pGeometry = new uint[geometryCount];
			uint currentGeometry = 0;
			for (int a = 0; a < modelCount; a++)
			{
				br.Position = model[a].m_pGeometry.m_pList;
				for (int b = 0; b < model[a].m_pGeometry.m_wCount; b++)pGeometry[currentGeometry++] = br.ReadOffset(); // нужно проверить
			}
			// reading geometry sections
			for (uint a = 0; a < geometryCount; a++)
			{
				br.Position = pGeometry[a];
				geometry[a] = ReadRageResource.Geometry(br);
			}
			RageResource.VertexBuffer[] vertexBuffer = new RageResource.VertexBuffer[geometryCount];
			RageResource.IndexBuffer[] indexBuffer = new RageResource.IndexBuffer[geometryCount];
			RageResource.VertexDeclaration[] vertexDeclaration = new RageResource.VertexDeclaration[geometryCount];
			// vertex buffer
			for (uint a = 0; a < geometryCount; a++)
			{
				br.Position = geometry[a].m_pVertexBuffer;
				vertexBuffer[a] = ReadRageResource.VertexBuffer(br);
			}
			// index buffer
			for (uint a = 0; a < geometryCount; a++)
			{
				br.Position = geometry[a].m_pIndexBuffer;
				indexBuffer[a].vtable = br.ReadUInt32();
				indexBuffer[a].m_dwIndexCount = br.ReadUInt32();
				indexBuffer[a].m_pIndexData = br.ReadOffset();
			}
			// reading vertex declaration sections
			for (int a = 0; a < geometryCount; a++)
			{
				br.Position = vertexBuffer[a].m_pDeclaration;
				vertexDeclaration[a] = ReadRageResource.VertexDeclaration(br);
			}
			// bounds
			Vector4[,] vBouds = new Vector4[modelCount, 100];
			for (int a = 0; a < modelCount; a++)
			{
				br.Position = model[a].m_pBounds;
				uint boundsCount = model[a].m_pGeometry.m_wCount;
				if (model[a].m_pGeometry.m_wCount > 1) boundsCount++;
				for (int b = 0; b < boundsCount; b++) vBouds[a, b] = br.ReadVector4();
			}
			IV_odd.Build(volumeData.cDrawable.m_wCount, shaderFXCount2, shaderFX, br, drawable, modelCount2, model, vBouds, indexBuffer, vertexBuffer, vertexDeclaration);
			return true;
			//Console.ReadKey();
		}
	}
}
