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
			Log.ToLog(Log.MessageType.INFO, "Red Dead Redemption Volume Data");
			Log.ToLog(Log.MessageType.INFO, $"RSC85 Res Start: 0x{ResourceUtils.FlagInfo.RSC85_ObjectStart.ToString("X8")}");

			EndianBinaryReader br = new EndianBinaryReader(decompMem);
			if (endian)br.Endianness = Endian.BigEndian;// 0 - lit, 1 - big
			
			// узнаем позицию начальной страницы в файле и читаем начальную секцию
			br.Position = ResourceUtils.FlagInfo.RSC85_ObjectStart;
			RageResource.RDRVolumeData volumeData;
			volumeData = ReadRageResource.RDRVolumeData(br);

			// экспортируем текстуры если имеются
			if (volumeData.pTexture != 0)
			{
				Log.ToLog(Log.MessageType.INFO, "Textures detected");
				TextureDictionary.ReadTextureDictionary(br, volumeData.pTexture);
			}
			// коллекция содержит объекты и хэшы
			uint[] hash = new uint[volumeData.cNameHash.m_wCount];
			br.Position = volumeData.cNameHash.m_pList;
			for (int a = 0; a < volumeData.cNameHash.m_wCount; a++) hash[a] = br.ReadUInt32();

			// xvd может содержать в себе несколько секций drawable, поэтому читаем поитер к каждомой секции
			Log.ToLog(Log.MessageType.INFO, $"Drawable sections count: {volumeData.cDrawable.m_wCount}");
			br.Position = volumeData.cDrawable.m_pList;
			uint[] pDrawable = new uint[volumeData.cDrawable.m_wCount];
			for (int a = 0; a  < volumeData.cDrawable.m_wCount; a ++) pDrawable[a] = br.ReadOffset();

			// читаем каждую секцию drawable
			RageResource.Drawable[] drawable = new RageResource.Drawable[volumeData.cDrawable.m_wCount];
			for (ushort a = 0; a < volumeData.cDrawable.m_wCount; a++)
			{
				br.Position = pDrawable[a];
				drawable[a] = ReadRageResource.Drawable(br);
			}

			// количество секций ShaderGrоup всегда равно количеству секций drawable
			RageResource.RDRShaderGroup[] shaderGroup = new RageResource.RDRShaderGroup[volumeData.cDrawable.m_wCount];
			uint[] pShaderGroup = new uint[volumeData.cDrawable.m_wCount];
			for (int a = 0; a < volumeData.cDrawable.m_wCount; a++) pShaderGroup[a] = drawable[a].m_pShaderGroup;
			for (int a = 0; a < volumeData.cDrawable.m_wCount; a++)
				//if (pShaderGroup[a] != 0)
				{
					br.Position = pShaderGroup[a];
					shaderGroup[a] = ReadRageResource.RDRShaderGroup(br);
				}
			
			// т.к. у нас несколько секций Drawable, то нам нужно узнать количество шейдеров во всех секциях ShaderGroup
			uint shaderFXCount = 0; // Общее количество
			//uint[] shaderFXCount2 = new uint[volumeData.cDrawable.m_wCount]; // Количество шейдеров в каждой секции ShaderGroup
			for (int a = 0; a < volumeData.cDrawable.m_wCount; a++) shaderFXCount += /*shaderFXCount2[a] =*/ shaderGroup[a].m_cShaderFX.m_wCount;


			// читаем поинтеры к всем секция ShaderFX
			uint currentShaderFX = 0;
			uint[] pShaderFX = new uint[shaderFXCount];
			for (int a = 0; a < volumeData.cDrawable.m_wCount; a++)
			{
				Log.ToLog(Log.MessageType.INFO, $"Shaders count in Drawable{a}: {shaderGroup[a].m_cShaderFX.m_wCount}");
				br.Position = shaderGroup[a].m_cShaderFX.m_pList;
				for (int b = 0; b < shaderGroup[a].m_cShaderFX.m_wCount; b++)pShaderFX[currentShaderFX++] = br.ReadOffset();
			}

			// читаем каждую секцию ShaderFX
			RageResource.RDRShaderFX[] shaderFX = new RageResource.RDRShaderFX[shaderFXCount];
			for (int a = 0; a < shaderFXCount; a++)
			{
				br.Position = pShaderFX[a];
				shaderFX[a] = ReadRageResource.RDRShaderFX(br);
			}

			// считаем количество секций Model
			uint modelCount = 0;
			//uint[,] modelCount2 = new uint[volumeData.cDrawable.m_wCount, 4];
			RageResource.Collection[,] modelCollection = new Collection[volumeData.cDrawable.m_wCount, 4]; // количество секций Drawable и 4 уровня детализации
																										   //RageResource.Collection tmpCollection;
			string level ="";
			for (ushort a = 0; a < volumeData.cDrawable.m_wCount; a++)
			{
				for (int b = 0; b < 4; b++)
				{
					if (drawable[a].m_pModelCollection[b] != 0)
					{
						br.Position = drawable[a].m_pModelCollection[b];
						//modelCollection[a][b] = br.ReadCollections();
						modelCount += (modelCollection[a, b] = br.ReadCollections()).m_wCount;

						level = b switch
						{
							0 => "High",
							1 => "Med",
							2 => "Low",
							3 => "Vlow"
						};
						Log.ToLog(Log.MessageType.INFO, $"{level} models count in Drawable{a}: {modelCollection[a, b].m_wCount}");
					}
				}
			}
			RageResource.Model[] model = new RageResource.Model[modelCount];

			// читаем секции Model для каждого уровня детализации
			int currentModel = 0;
			uint[] pModel;
			for (int a = 0; a < drawable.Length; a++)
			{
				for (int b = 0; b < 4; b++) // lod
				{
					if(modelCollection[a, b].m_pList != 0)
					{
						br.Position = modelCollection[a, b].m_pList;
						pModel = new uint[modelCollection[a, b].m_wCount];
						for (int c = 0; c < modelCollection[a, b].m_wCount; c++) pModel[c] = br.ReadOffset();
						for (int c = 0; c < modelCollection[a, b].m_wCount; c++)
						{
							br.Position = pModel[c];
							model[currentModel++] = ReadRageResource.Model(br);
						}
					}
				}
			}
			Log.ToLog(Log.MessageType.INFO, $"Models count: {currentModel}");

			// считаем общее количество секций Geometry
			uint geometryCount = 0;
			for (int a = 0; a < modelCount; a++)geometryCount += model[a].m_pGeometry.m_wCount;
			Log.ToLog(Log.MessageType.INFO, $"Geometries count: {geometryCount}");
			RageResource.Geometry[] geometry = new RageResource.Geometry[geometryCount];
			// и поинтеры
			uint[] pGeometry = new uint[geometryCount];
			uint currentGeometry = 0;
			for (int a = 0; a < modelCount; a++)
			{
				br.Position = model[a].m_pGeometry.m_pList;
				for (int b = 0; b < model[a].m_pGeometry.m_wCount; b++)pGeometry[currentGeometry++] = br.ReadOffset();
			}
			// читаем секции Geometry с остальнимы секциями
			RageResource.VertexBuffer[] vertexBuffer = new RageResource.VertexBuffer[geometryCount];
			RageResource.IndexBuffer[] indexBuffer = new RageResource.IndexBuffer[geometryCount];
			RageResource.VertexDeclaration[] vertexDeclaration = new RageResource.VertexDeclaration[geometryCount];
			for (uint a = 0; a < geometryCount; a++)
			{
				br.Position = pGeometry[a];
				geometry[a] = ReadRageResource.Geometry(br);
				br.Position = geometry[a].m_pVertexBuffer;
				vertexBuffer[a] = ReadRageResource.VertexBuffer(br);
				br.Position = geometry[a].m_pIndexBuffer;
				indexBuffer[a] = ReadRageResource.IndexBuffer(br);
				br.Position = vertexBuffer[a].m_pDeclaration;
				vertexDeclaration[a] = ReadRageResource.VertexDeclaration(br);
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
			if (!IV_odd.Build(volumeData.cDrawable.m_wCount, shaderGroup, shaderFX, br, drawable, modelCollection,
				model, vBounds, indexBuffer, vertexBuffer, vertexDeclaration, hash)) return false;
			return true;
			//Console.ReadKey();
		}
	}
}
