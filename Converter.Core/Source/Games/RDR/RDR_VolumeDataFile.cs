using Converter.Core.Utils.openFormats;
using Converter.Core.Utils;
using Converter.Core.ResourceTypes;
using System.IO;
using System.Numerics;
using System;

namespace Converter.Core.Games.RDR
{
	public static class RDR_VolumeDataFile
	{
		public static bool ReadVolumeData(MemoryStream decompMem, bool endian)
		{
			Console.WriteLine("[INFO] Red Dead Redemption Volume Data");
			if (Main.useVerboseMode || Main.useVeryVerboseMode)
			{
				Console.WriteLine($"[INFO] RSC85 Res Start: 0x{FlagInfo.RSC85_ObjectStart:X8}");
			}
			

			EndianBinaryReader br = new EndianBinaryReader(decompMem);
			if (endian)
			{
				br.Endianness = Endian.BigEndian; // 0 - lit, 1 - big
			}

			// get a position of a start page in the file and then read the start section
			br.Position = FlagInfo.RSC85_ObjectStart;
			RDR_VolumeData volumeData = RDR_VolumeData.Read(br);

			// export textures if they are present
			if (volumeData.m_pTexture != 0)
			{
				Console.WriteLine("[INFO] Textures detected");
				TextureDictionary.ReadTextureDictionary(br, volumeData.m_pTexture);
			}

			// the collection has objects and hashes
			uint[] hash = new uint[volumeData.m_cNameHash.m_nCount];
			br.Position = volumeData.m_cNameHash.m_pList;
			for (int a = 0; a < volumeData.m_cNameHash.m_nCount; a++)
			{
				hash[a] = br.ReadUInt32();
			}

			// xvd file may have more than one drawable section, so read the pointer to the each section
			Console.WriteLine($"[INFO] Drawable sections count: {volumeData.m_cDrawable.m_nCount}");
			br.Position = volumeData.m_cDrawable.m_pList;
			uint[] pDrawable = new uint[volumeData.m_cDrawable.m_nCount];
			for (int a = 0; a < volumeData.m_cDrawable.m_nCount; a++)
			{
				pDrawable[a] = br.ReadOffset();
			}

			// read each drawable section
			Drawable[] drawable = new Drawable[volumeData.m_cDrawable.m_nCount];
			for (ushort a = 0; a < volumeData.m_cDrawable.m_nCount; a++)
			{
				br.Position = pDrawable[a];
				drawable[a] = Drawable.Read(br);
			}

			// the count of ShaderGroup sections are always equal to Drawable sections count
			RDR_ShaderGroup[] shaderGroup = new RDR_ShaderGroup[volumeData.m_cDrawable.m_nCount];
			uint[] pShaderGroup = new uint[volumeData.m_cDrawable.m_nCount];
			for (int a = 0; a < volumeData.m_cDrawable.m_nCount; a++)
			{
				pShaderGroup[a] = drawable[a].m_pShaderGroup;
			}
			for (int a = 0; a < volumeData.m_cDrawable.m_nCount; a++)
			{
				//if (pShaderGroup[a] != 0)
				{
					br.Position = pShaderGroup[a];
					shaderGroup[a] = RDR_ShaderGroup.Read(br);
				}
			}
			
			// because we have a some Drawable sections, we need to know the count of shaders in all ShaderGroup sections
			uint shaderFXCount = 0; // total count
			for (int a = 0; a < volumeData.m_cDrawable.m_nCount; a++)
			{
				shaderFXCount += /*shaderFXCount2[a] =*/ shaderGroup[a].m_cShaderFX.m_nCount;
			}

			// read pointers to all ShaderFX sections
			uint currentShaderFX = 0;
			uint[] pShaderFX = new uint[shaderFXCount];
			for (int a = 0; a < volumeData.m_cDrawable.m_nCount; a++)
			{
				Console.WriteLine($"[INFO] Shaders count in Drawable{a}: {shaderGroup[a].m_cShaderFX.m_nCount}");
				br.Position = shaderGroup[a].m_cShaderFX.m_pList;
				for (int b = 0; b < shaderGroup[a].m_cShaderFX.m_nCount; b++)
				{
					pShaderFX[currentShaderFX++] = br.ReadOffset();
				}
			}

			// read each ShaderFX section
			RDR_ShaderFX[] shaderFX = new RDR_ShaderFX[shaderFXCount];
			for (int a = 0; a < shaderFXCount; a++)
			{
				br.Position = pShaderFX[a];
				shaderFX[a] = RDR_ShaderFX.Read(br);
			}

			// get the count of Model sections
			uint modelCount = 0;
			Collection[,] modelCollection = new Collection[volumeData.m_cDrawable.m_nCount, 4]; // count of Drawable sections and 4 level of details
			
			string level ="";
			for (ushort a = 0; a < volumeData.m_cDrawable.m_nCount; a++)
			{
				for (int b = 0; b < 4; b++)
				{
					if (drawable[a].m_pModelCollection[b] != 0)
					{
						br.Position = drawable[a].m_pModelCollection[b];
						modelCount += (modelCollection[a, b] = Collection.Read(br)).m_nCount;

						switch (b)
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

						Console.WriteLine($"[INFO] {level} models count in Drawable{a}: {modelCollection[a, b].m_nCount}");
					}
				}
			}

			Model[] model = new Model[modelCount];

			// read Model sections for each level of detail
			int currentModel = 0;
			uint[] pModel;
			for (int a = 0; a < drawable.Length; a++)
			{
				for (int b = 0; b < 4; b++) // lod
				{
					if (modelCollection[a, b].m_pList != 0)
					{
						br.Position = modelCollection[a, b].m_pList;
						pModel = new uint[modelCollection[a, b].m_nCount];
						for (int c = 0; c < modelCollection[a, b].m_nCount; c++)
						{
							pModel[c] = br.ReadOffset();
						}
						for (int c = 0; c < modelCollection[a, b].m_nCount; c++)
						{
							br.Position = pModel[c];
							model[currentModel++] = Model.Read(br);
						}
					}
				}
			}

			Console.WriteLine($"[INFO] Models count: {currentModel}");

			// get total count of Geometry sections
			uint geometryCount = 0;
			
			for (int a = 0; a < modelCount; a++)
			{
				geometryCount += model[a].m_cGeometry.m_nCount;
			}
			
			Console.WriteLine($"[INFO] Geometries count: {geometryCount}");
			Geometry[] geometry = new Geometry[geometryCount];
			
			// and pointers
			uint[] pGeometry = new uint[geometryCount];
			uint currentGeometry = 0;
			
			for (int a = 0; a < modelCount; a++)
			{
				br.Position = model[a].m_cGeometry.m_nCount;
				for (int b = 0; b < model[a].m_cGeometry.m_nCount; b++)
				{
					pGeometry[currentGeometry++] = br.ReadOffset();
				}
			}

			// read Geometry sections with other
			VertexBuffer[] vertexBuffer = new VertexBuffer[geometryCount];
			IndexBuffer[] indexBuffer = new IndexBuffer[geometryCount];
			VertexDeclaration[] vertexDeclaration = new VertexDeclaration[geometryCount];
			
			for (uint a = 0; a < geometryCount; a++)
			{
				br.Position = pGeometry[a];
				geometry[a] = Geometry.Read(br);

				br.Position = geometry[a].m_pVertexBuffer;
				vertexBuffer[a] = VertexBuffer.Read(br);

				br.Position = geometry[a].m_pIndexBuffer;
				indexBuffer[a] = IndexBuffer.Read(br);

				br.Position = vertexBuffer[a].m_pDeclaration;
				vertexDeclaration[a] = VertexDeclaration.Read(br);
			}

			// bounds
			uint boundsCount;
			Vector4[,] vBounds = new Vector4[modelCount, 100];
			
			for (int a = 0; a < modelCount; a++)
			{
				br.Position = model[a].m_pBounds;

				boundsCount = model[a].m_cGeometry.m_nCount > 1 ? (uint)model[a].m_cGeometry.m_nCount + 1 : model[a].m_cGeometry.m_nCount;
				for (int b = 0; b < boundsCount; b++)
				{
					vBounds[a, b] = br.ReadVector4();
				}
			}

			IV_odd.UniversalShaderGroup[] universalShaderGroup = new IV_odd.UniversalShaderGroup[shaderGroup.Length];
			for (int a = 0; a < shaderGroup.Length; a++)
			{
				universalShaderGroup[a] = IV_odd.UniversalShaderGroup.ConvertToUniversalShaderGroup(shaderGroup[a]);
			}

			uint skelDataCount = 0;
			for (int a = 0; a < drawable.Length; a++)
			{
				if (drawable[a].m_pSkeleton != 0)
				{
					skelDataCount++;
				}
			}

			IV_skel.UniversalSkeletonData[] universalSkeletonData= new IV_skel.UniversalSkeletonData[skelDataCount];
			uint currentSkelData = 0;
			for (int a = 0; a < drawable.Length; a++)
			{
				if (drawable[a].m_pSkeleton != 0)
				{
					br.Position = drawable[a].m_pSkeleton;
					universalSkeletonData[currentSkelData++] = IV_skel.UniversalSkeletonData.ConvertToUniversalSkeletonData(RDR_SkeletonData.Read(br));
				}
			}

			string[] shaderLine = IV_odd.UniversalShaderFX.ShaderFXToShaderLine(br, shaderFX);
			return IV_odd.Build(volumeData.m_cDrawable.m_nCount, universalShaderGroup, br, drawable, modelCollection,
				model, vBounds, indexBuffer, vertexBuffer, vertexDeclaration, hash, universalSkeletonData, shaderLine);
		}
	}
}
