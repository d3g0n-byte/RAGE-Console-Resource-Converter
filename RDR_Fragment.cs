using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Converter.RageResource;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Converter
{
	internal class RDR_Fragment
	{
		public unsafe void f1()
		{
			int a;
			int* b = &a;
			
		}
		public static bool ReadFragment(MemoryStream decompMem, bool endian)// конверт фрагментной модели в обычную
		{
			// мы будем конвертировать в odr формат, поэтому мы пропускаем секции, которые имеют отношение к фрагментной модели
			EndianBinaryReader br = new EndianBinaryReader(decompMem);
			if (endian) br.Endianness = Endian.BigEndian;// 0 - lit, 1 - big
			br.Position = ResourceUtils.FlagInfo.RSC85_ObjectStart;
			RageResource.Drawable drawable = new RageResource.Drawable();
			br.Position += 0xb0;
			string fileName = DataUtils.ReadStringAtOffset(br.ReadOffset(), br);
			uint pDrawable = br.ReadOffset();
			br.Position +=312;
			uint pTexture = br.ReadOffset();
			if (pTexture!=0)TextureDictionary.ReadTextureDictionary(br, pTexture);
			br.Position = pDrawable;
			drawable = ReadRageResource.Drawable(br);
			// шейдеры
			RageResource.RDRShaderGroup shaderGroup = new RageResource.RDRShaderGroup();
			br.Position = drawable.m_pShaderGroup;
			shaderGroup = ReadRageResource.RDRShaderGroup(br);
			uint[] pShaderFX = new uint[shaderGroup.m_cShaderFX.m_wCount];
			RageResource.RDRShaderFX[] shaderFX = new RageResource.RDRShaderFX[shaderGroup.m_cShaderFX.m_wCount];
			br.Position = shaderGroup.m_cShaderFX.m_pList;
			for (int a = 0; a < shaderGroup.m_cShaderFX.m_wCount; a++) pShaderFX[a] = br.ReadOffset();
			for (int a = 0; a < shaderGroup.m_cShaderFX.m_wCount; a++)
			{
				br.Position = pShaderFX[a];
				shaderFX[a] = ReadRageResource.RDRShaderFX(br);
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
				for (int b = 0; b < model[a].m_pGeometry.m_wCount; b++)pGeometry[currentGeometry++] = br.ReadOffset();
			}
			uint geometryCount = currentGeometry;
			Array.Resize<uint>(ref pGeometry, (int)geometryCount);
			RageResource.Geometry[] geometry = new RageResource.Geometry[geometryCount];
			RageResource.VertexBuffer[] vertexBuffer= new RageResource.VertexBuffer[geometryCount];
			RageResource.IndexBuffer[] indexBuffer = new RageResource.IndexBuffer[geometryCount];
			RageResource.VertexDeclaration[] vertexDeclaration = new RageResource.VertexDeclaration[geometryCount];
			for (int a = 0; a < geometryCount; a++)
			{
				br.Position = pGeometry[a];
				geometry[a] = ReadRageResource.Geometry(br);
				br.Position = geometry[a].m_pVertexBuffer;
				vertexBuffer[a] = ReadRageResource.VertexBuffer(br);
				br.Position = vertexBuffer[a].m_pDeclaration;
				vertexDeclaration[a] = ReadRageResource.VertexDeclaration(br);
				br.Position = geometry[a].m_pIndexBuffer;
				indexBuffer[a] = ReadRageResource.IndexBuffer(br);
			}
			Vector4[,] vBouds = new Vector4[modelCount, 100];
			for (int a = 0; a < modelCount; a++)
			{
				br.Position = model[a].m_pBounds;
				uint boundsCount = model[a].m_pGeometry.m_wCount;
				if (model[a].m_pGeometry.m_wCount > 1) boundsCount++;
				for (int b = 0; b < boundsCount; b++) vBouds[a, b] = br.ReadVector4();
			}
			openFormats.IV_odr.Build(shaderFX, br, drawable, model, vBouds, indexBuffer, vertexBuffer, vertexDeclaration);
			return true;
		}
	}
}
