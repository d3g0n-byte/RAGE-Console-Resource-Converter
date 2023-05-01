﻿using Converter.Core.Utils.openFormats;
using Converter.Core.Utils;
using System;
using System.Numerics;
using System.Text;
using System.IO;
using Converter.Core.ResourceTypes;

namespace Converter.Core.Games.MCLA
{
	public static class MCLA_XRSC
	{
		public struct XRSC
		{
			public uint _vmt;
			public uint pEnd;
			public uint pName;
			public ushort nameLength;
			public ushort nameSize;
			public uint pDrawable;
			public uint _f14;
			public uint _f18;
			public uint _f1c; // pointer
			public int _f20; // number
		}

		public static bool Read(MemoryStream ms, bool endian)
		{
			using (EndianBinaryReader br = new EndianBinaryReader(ms))
			{
				if (endian)
				{
					br.Endianness = Endian.BigEndian;
				}

				XRSC xrsc = new XRSC
				{
					_vmt = br.ReadUInt32(),
					pEnd = br.ReadOffset(),
					pName = br.ReadOffset(),
					nameLength = br.ReadUInt16(),
					nameSize = br.ReadUInt16(),
					pDrawable = br.ReadOffset(),
					_f14 = br.ReadOffset(),
					_f18 = br.ReadOffset(),
					_f1c = br.ReadOffset(),
					_f20 = br.ReadInt32()
				};

				br.Position = xrsc.pDrawable;
				Drawable drawable = Drawable.Read(br);

				StringBuilder sbOutFileMain = new StringBuilder();
				string outFileName = $"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}.odr";

				br.Position = drawable.m_pShaderGroup;
				IV_ShaderGroup shadeGroup;
				shadeGroup = IV_ShaderGroup.Read(br);

				if (shadeGroup.m_pTexture != 0)
				{
					if (!TextureDictionary.ReadTextureDictionary(br, shadeGroup.m_pTexture))
					{
						throw new Exception("Error while reading Texture Dictionary.");
					}
				}

				MCLA_ShaderFX[] fx = new MCLA_ShaderFX[shadeGroup.m_pShaders.m_nCount];
				uint[] pShaderFX = new uint[shadeGroup.m_pShaders.m_nCount];
				br.Position = shadeGroup.m_pShaders.m_pList;

				for (int a = 0; a < shadeGroup.m_pShaders.m_nCount; a++)
				{
					pShaderFX[a] = br.ReadOffset();
				}

				for (int a = 0; a < shadeGroup.m_pShaders.m_nCount; a++)
				{
					br.Position = pShaderFX[a];
					fx[a] = MCLA_ShaderFX.Read(br);
				}

				// model
				uint currentModel = 0;
				uint[] pModel = new uint[200];

				Collection[] cModel = new Collection[4];
				for (int a = 0; a < 4; a++) // lod
				{
					if (drawable.m_pModelCollection[a] != 0)
					{
						br.Position = drawable.m_pModelCollection[a];
						cModel[a] = Collection.Read(br);
						br.Position = cModel[a].m_pList;

						for (int b = 0; b < cModel[a].m_nCount; b++)
						{
							pModel[currentModel++] = br.ReadOffset();
						}
					}
				}

				uint modelCount = currentModel;
				Array.Resize(ref pModel, (int)modelCount);
				Model[] model = new Model[pModel.Length];

				for (int a = 0; a < pModel.Length; a++)
				{
					br.Position = pModel[a];
					model[a] = Model.Read(br);

					// also disable m_bSkinned. A temporary fix.
					model[a].m_bSkinned = false;
				}

				uint[] pGeometry = new uint[200];
				uint currentGeometry = 0;

				for (int a = 0; a < pModel.Length; a++)
				{
					br.Position = model[a].m_cGeometry.m_pList;

					for (int b = 0; b < model[a].m_cGeometry.m_nCount; b++)
					{
						pGeometry[currentGeometry++] = br.ReadOffset();
					}
				}

				uint geometryCount = currentGeometry;
				Array.Resize(ref pGeometry, (int)geometryCount);
				Geometry[] geometry = new Geometry[geometryCount];
				VertexBuffer[] vertexBuffer = new VertexBuffer[geometryCount];
				IndexBuffer[] indexBuffer = new IndexBuffer[geometryCount];
				VertexDeclaration[] vertexDeclaration = new VertexDeclaration[geometryCount];
				uint tmp;

				for (int a = 0; a < geometryCount; a++)
				{
					br.Position = pGeometry[a];
					geometry[a] = Geometry.Read(br);
					br.Position = geometry[a].m_pVertexBuffer;
					vertexBuffer[a] = VertexBuffer.Read(br);

					tmp = vertexBuffer[a].m_pVertexData;
					vertexBuffer[a].m_pVertexData = vertexBuffer[a].m_pDeclaration;
					vertexBuffer[a].m_pDeclaration = tmp;

					br.Position = vertexBuffer[a].m_pDeclaration;
					vertexDeclaration[a] = VertexDeclaration.Read(br);
					br.Position = geometry[a].m_pIndexBuffer;
					indexBuffer[a] = IndexBuffer.Read(br);
				}

				Vector4[,] vBounds = new Vector4[modelCount, 100];

				// Don't read bounds. Also as temporary fix.
				/*
				for (int a = 0; a < modelCount; a++)
				{
					br.Position = model[a].m_pBounds;
					uint boundsCount = model[a].m_pGeometry.m_wCount;

					if (model[a].m_pGeometry.m_wCount > 1)
					{
						boundsCount++;
					}

					for (int b = 0; b < boundsCount; b++)
					{
						vBounds[a, b] = br.ReadVector4();
					}
				}
				*/

				// write down the ShaderGroup section

				IV_skel.UniversalSkeletonData skelData = new IV_skel.UniversalSkeletonData();
				if (drawable.m_pSkeleton != 0)
				{
					br.Position = drawable.m_pSkeleton;
					skelData = IV_skel.UniversalSkeletonData.ConvertToUniversalSkeletonData(IV_SkeletonData.Read(br));
				}

				return IV_odr.Build(br, drawable, model, vBounds, indexBuffer, vertexBuffer, vertexDeclaration, cModel, IV_odd.UniversalShaderFX.ShaderFXToShaderLine(br, fx), skelData);
			}
		}
	}
}
