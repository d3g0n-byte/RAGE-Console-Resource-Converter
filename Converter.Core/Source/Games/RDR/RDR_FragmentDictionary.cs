using Converter.Core.Utils;
using System;
using System.Numerics;
using Converter.Core.ResourceTypes;
using System.IO;
using Converter.Core.Utils.openFormats;

namespace Converter.Core.Games.RDR
{
	public static class RDR_FragmentDictionary
	{
		public static bool ReadFragmentDictionary(MemoryStream decompMem, bool endian)
		{
			Console.WriteLine("[INFO] Red Dead Redemption Fragment Dictionary");
			if (Main.useVerboseMode || Main.useVeryVerboseMode)
			{
				Console.WriteLine($"[INFO] RSC85 Res Start: 0x{FlagInfo.RSC85_ObjectStart:X8}");
			}

			using (EndianBinaryReader br = new EndianBinaryReader(decompMem))
			{
				if (endian)
				{
					br.Endianness = Endian.BigEndian;
				}

				br.Position = FlagInfo.RSC85_ObjectStart;
				FragmentDictionary dict = FragmentDictionary.Read(br);

				if (dict.m_pTextureDictionary != 0)
				{
					if (Main.useVerboseMode || Main.useVeryVerboseMode)
					{
						Console.WriteLine("[INFO] Textures detected.");
					}
					
					TextureDictionary.ReadTextureDictionary(br, dict.m_pTextureDictionary);
				}

				br.Position = dict.m_pDrawable;
				Drawable drawable = Drawable.Read(br);

				// shaders
				br.Position = drawable.m_pShaderGroup;
				RDR_ShaderGroup shaderGroup = RDR_ShaderGroup.Read(br);
				uint[] pShaderFX = new uint[shaderGroup.m_cShaderFX.m_nCount];
				RDR_ShaderFX[] shaderFX = new RDR_ShaderFX[shaderGroup.m_cShaderFX.m_nCount];
				br.Position = shaderGroup.m_cShaderFX.m_pList;

				if (Main.useVerboseMode || Main.useVeryVerboseMode)
				{
					Console.WriteLine($"[INFO] Shaders count: {shaderGroup.m_cShaderFX.m_nCount}");
				}

				for (int a = 0; a < shaderGroup.m_cShaderFX.m_nCount; a++)
				{
					pShaderFX[a] = br.ReadOffset();
				}
				for (int a = 0; a < shaderGroup.m_cShaderFX.m_nCount; a++)
				{
					br.Position = pShaderFX[a];
					shaderFX[a] = RDR_ShaderFX.Read(br);
				}

				// model
				uint currentModel = 0;
				uint[] pModel = new uint[200];

				string level = "";
				Collection[] сModel = new Collection[4];

				for (int a = 0; a < 4; a++) // lod
				{
					if (drawable.m_pModelCollection[a] != 0)
					{
						br.Position = drawable.m_pModelCollection[a];
						сModel[a] = Collection.Read(br);
						br.Position = сModel[a].m_pList;

						for (int b = 0; b < сModel[a].m_nCount; b++)
						{
							pModel[currentModel++] = br.ReadOffset();
						}

						switch (a)
						{
							case 0:
								level = "High";
								break;

							case 1:
								level = "Med";
								break;

							case 2:
								level = "Low";
								break;

							case 3:
								level = "Vlow";
								break;

						}

						if (Main.useVerboseMode || Main.useVeryVerboseMode)
						{
							Console.WriteLine($"[INFO] {level} models count: {сModel[a].m_nCount}");
						}
					}
				}

				uint modelCount = currentModel;
				Array.Resize(ref pModel, (int)modelCount);
				Model[] model = new Model[pModel.Length];

				if (Main.useVerboseMode || Main.useVeryVerboseMode)
				{
					Console.WriteLine($"[INFO] Models count: {currentModel}");
				}

				for (int a = 0; a < pModel.Length; a++)
				{
					br.Position = pModel[a];
					model[a] = Model.Read(br);
				}

				uint geometryCount = 0;

				for (int a = 0; a < modelCount; a++)
				{
					geometryCount += model[a].m_cGeometry.m_nCount;
				}

				uint[] pGeometry = new uint[geometryCount];
				uint currentGeometry = 0;

				if (Main.useVerboseMode || Main.useVeryVerboseMode)
				{
					Console.WriteLine($"[INFO] Geometries count: {geometryCount}");
				}

				for (int a = 0; a < modelCount; a++)
				{
					br.Position = model[a].m_cGeometry.m_nCount;
					for (int b = 0; b < model[a].m_cGeometry.m_nCount; b++)
					{
						pGeometry[currentGeometry++] = br.ReadOffset();
					}
				}

				Geometry[] geometry = new Geometry[geometryCount];
				VertexBuffer[] vertexBuffer = new VertexBuffer[geometryCount];
				IndexBuffer[] indexBuffer = new IndexBuffer[geometryCount];
				VertexDeclaration[] vertexDeclaration = new VertexDeclaration[geometryCount];

				for (int a = 0; a < geometryCount; a++)
				{
					br.Position = pGeometry[a];
					geometry[a] = Geometry.Read(br);

					br.Position = geometry[a].m_pVertexBuffer;
					vertexBuffer[a] = VertexBuffer.Read(br);

					br.Position = vertexBuffer[a].m_pDeclaration;
					vertexDeclaration[a] = VertexDeclaration.Read(br);

					br.Position = geometry[a].m_pIndexBuffer;
					indexBuffer[a] = IndexBuffer.Read(br);
				}

				// model bounds
				uint boundsCount;
				Vector4[,] vBounds = new Vector4[modelCount, 100];

				for (int a = 0; a < modelCount; a++)
				{
					br.Position = model[a].m_pBounds;
					// if Geometry sections are more than 1, then add 1 to bounds count
					boundsCount = model[a].m_cGeometry.m_nCount > 1 ? (uint)model[a].m_cGeometry.m_nCount + 1 : model[a].m_cGeometry.m_nCount;
					for (int b = 0; b < boundsCount; b++)
					{
						vBounds[a, b] = br.ReadVector4();
					}
				}

				IV_skel.UniversalSkeletonData skelData = new IV_skel.UniversalSkeletonData();
				if (drawable.m_pSkeleton != 0)
				{
					br.Position = drawable.m_pSkeleton;
					skelData = IV_skel.UniversalSkeletonData.ConvertToUniversalSkeletonData(IV_SkeletonData.Read(br));
				}
				string[] shaderLine = IV_odd.UniversalShaderFX.ShaderFXToShaderLine(br, shaderFX);
				return IV_odr.Build(br, drawable, model, vBounds, indexBuffer, vertexBuffer, vertexDeclaration, сModel, shaderLine, skelData);
			}
		}
	}
}
