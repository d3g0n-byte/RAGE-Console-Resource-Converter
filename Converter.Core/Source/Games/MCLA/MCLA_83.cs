using Converter.Core.Utils;
using Converter.Core.ResourceTypes;
using System;
using System.IO;
using System.Numerics;
using Converter.Core.Utils.openFormats;
using System.Linq;

namespace Converter.Core.Games.MCLA
{
	public struct RES_83
	{
		public uint _vmt;
		public uint end;
		public uint _f8; // zeroes?
		public uint m_pShaderGroup;
		public uint _f10;
		public uint _f14; // pointer to zone texture?
		public uint _f18; // pointer to max damage texture?
		public uint _f1c; // pointer to scratch texture?
		// that's not all...
	}

	public static class MCLA_83
	{
		public static bool Read(MemoryStream mem, bool endian)
		{
			using (EndianBinaryReader br = new EndianBinaryReader(mem))
			{
				if (endian)
				{
					br.Endianness = Endian.BigEndian;
				}

				RES_83 file = new RES_83
				{
					_vmt = br.ReadUInt32(),
					end = br.ReadOffset(),
					_f8 = br.ReadUInt32(),
					m_pShaderGroup = br.ReadOffset(),
					_f10 = br.ReadOffset(),
					_f14 = br.ReadOffset(),
					_f18 = br.ReadOffset(),
					_f1c = br.ReadOffset()
				};

				if (file._f14 != 0)
				{
					br.Position = file._f14;
					RDR_Texture zoneTexture = RDR_Texture.Read(br);

					br.Position = zoneTexture.m_pBitmap;
					BitMap zoneBitMap = BitMap.Read(br);

					TextureDictionary.ReadTexture(TextureDictionary.UniversalTexture.ConvertToUniversalTexture(zoneTexture), zoneBitMap, br, 0);
				}

				if (file._f18 != 0)
				{
					br.Position = file._f18;
					RDR_Texture maxDamageTexture = RDR_Texture.Read(br);

					br.Position = maxDamageTexture.m_pBitmap;
					BitMap maxDamageBitMap = BitMap.Read(br);

					TextureDictionary.ReadTexture(TextureDictionary.UniversalTexture.ConvertToUniversalTexture(maxDamageTexture), maxDamageBitMap, br, 0);
				}
				
				if (file._f1c != 0)
				{
					br.Position = file._f1c;
					RDR_Texture scratchTexture = RDR_Texture.Read(br);

					br.Position = scratchTexture.m_pBitmap;
					BitMap scratchBitMap = BitMap.Read(br);

					TextureDictionary.ReadTexture(TextureDictionary.UniversalTexture.ConvertToUniversalTexture(scratchTexture), scratchBitMap, br, 0);
				}

				br.Position = file.m_pShaderGroup;
				IV_ShaderGroup shadeGroup = IV_ShaderGroup.Read(br);

				if (!TextureDictionary.ReadTextureDictionary(br, shadeGroup.m_pTexture))
				{
					throw new Exception("Error while reading Texture Dictionary.");
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

				IV_odd.UniversalShaderFX.ShaderFXToShaderLine(br, fx);
				Drawable drawable = new Drawable();
				drawable.m_nObjectCount = Enumerable.Repeat(-1, drawable.m_nObjectCount.Length).ToArray();
				Model[] model = new Model[0];
				IndexBuffer[] indexBuffer = new IndexBuffer[0];
				VertexBuffer[] vertexBuffer = new VertexBuffer[0];
				VertexDeclaration[] vertexDeclaration = new VertexDeclaration[0];
				Collection[] cModel = new Collection[0];
				Vector4[,] vBounds = new Vector4[0,0];
				IV_skel.UniversalSkeletonData skelData = new IV_skel.UniversalSkeletonData();

				return IV_odr.Build(br, drawable, model, vBounds, indexBuffer, vertexBuffer, vertexDeclaration, cModel, IV_odd.UniversalShaderFX.ShaderFXToShaderLine(br, fx), skelData);
			}
		}
	}
}
