﻿using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static Converter.RageResource;

namespace Converter
{
	internal class RDR_FragmentDictionary
	{
		public static bool ReadFragmentDictionary(MemoryStream decompMem, bool endian)
		{
			Log.ToLog(Log.MessageType.INFO, "Red Dead Redemption Fragment Dictionary");
			Log.ToLog(Log.MessageType.INFO, $"RSC85 Res Start: 0x{ResourceUtils.FlagInfo.RSC85_ObjectStart.ToString("X8")}");

			EndianBinaryReader br = new EndianBinaryReader(decompMem);
			if (endian) br.Endianness = Endian.BigEndian;// 0 - lit, 1 - big
			br.Position = ResourceUtils.FlagInfo.RSC85_ObjectStart;
			RageResource.FragmentDictionary dict = new RageResource.FragmentDictionary();
			RageResource.Drawable drawable = new RageResource.Drawable();
			dict = ReadRageResource.FragmentDictionary(br);
			if (dict.m_pTextureDictionary != 0)
			{
				Log.ToLog(Log.MessageType.INFO, "Textures detected.");
				TextureDictionary.ReadTextureDictionary(br, dict.m_pTextureDictionary);
			}
			br.Position = dict.m_pDrawable;
			drawable = ReadRageResource.Drawable(br);
			// шейдеры
			RageResource.RDRShaderGroup shaderGroup = new RageResource.RDRShaderGroup();
			br.Position = drawable.m_pShaderGroup;
			shaderGroup = ReadRageResource.RDRShaderGroup(br);
			uint[] pShaderFX = new uint[shaderGroup.m_cShaderFX.m_wCount];
			RageResource.RDRShaderFX[] shaderFX = new RageResource.RDRShaderFX[shaderGroup.m_cShaderFX.m_wCount];
			br.Position = shaderGroup.m_cShaderFX.m_pList;
			Log.ToLog(Log.MessageType.INFO, $"Shaders count: {shaderGroup.m_cShaderFX.m_wCount}"); 
			for (int a = 0; a < shaderGroup.m_cShaderFX.m_wCount; a++) pShaderFX[a] = br.ReadOffset();
			for (int a = 0; a < shaderGroup.m_cShaderFX.m_wCount; a++)
			{
				br.Position = pShaderFX[a];
				shaderFX[a] = ReadRageResource.RDRShaderFX(br);
			}
			// модель
			uint currentModel = 0;
			uint[] pModel = new uint[200];

			string level ="";
			RageResource.Collection[] сModel = new RageResource.Collection[4];
			for (int a = 0; a < 4; a++) // lod
			{
				if (drawable.m_pModelCollection[a] != 0)
				{
					br.Position = drawable.m_pModelCollection[a];
					сModel[a] = br.ReadCollections();
					br.Position = сModel[a].m_pList;
					for (int b = 0; b < сModel[a].m_wCount; b++) pModel[currentModel++] = br.ReadOffset();

					level = a switch
					{
						0 => "High",
						1 => "Med",
						2 => "Low",
						3 => "Vlow"
					};
					Log.ToLog(Log.MessageType.INFO, $"{level} models count: {сModel[a].m_wCount}");
				}
			}
			uint modelCount = currentModel;
			Array.Resize<uint>(ref pModel, (int)modelCount);
			RageResource.Model[] model = new RageResource.Model[pModel.Length];

			Log.ToLog(Log.MessageType.INFO, $"Models count: {currentModel}");
			for (int a = 0; a < pModel.Length; a++)
			{
				br.Position = pModel[a];
				model[a] = ReadRageResource.Model(br);
			}
			uint geometryCount = 0;
			for (int a = 0; a < modelCount; a++) geometryCount += model[a].m_pGeometry.m_wCount;
			uint[] pGeometry = new uint[geometryCount];
			uint currentGeometry = 0;
			Log.ToLog(Log.MessageType.INFO, $"Geometries count: {geometryCount}");
			for (int a = 0; a < modelCount; a++)
			{
				br.Position = model[a].m_pGeometry.m_pList;
				for (int b = 0; b < model[a].m_pGeometry.m_wCount; b++) pGeometry[currentGeometry++] = br.ReadOffset();
			}
			RageResource.Geometry[] geometry = new RageResource.Geometry[geometryCount];
			RageResource.VertexBuffer[] vertexBuffer = new RageResource.VertexBuffer[geometryCount];
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
			// границы модели. Количесво границ
			uint boundsCount;
			Vector4[,] vBounds = new Vector4[modelCount, 100];
			for (int a = 0; a < modelCount; a++)
			{
				br.Position = model[a].m_pBounds;
				// если секций Geometry больше чем 1, то к количества границ добавляем 1
				boundsCount = model[a].m_pGeometry.m_wCount > 1 ? (uint)model[a].m_pGeometry.m_wCount + 1 : (uint)model[a].m_pGeometry.m_wCount;
				for (int b = 0; b < boundsCount; b++) vBounds[a, b] = br.ReadVector4();
			}
			openFormats.IV_odr.Build(shaderFX, br, drawable, model, vBounds, indexBuffer, vertexBuffer, vertexDeclaration, сModel);
			return true;
		}

	}
}
