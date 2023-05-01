using Converter.Core.Utils.openFormats;
using Converter.Core.Utils;
using Converter.Core.ResourceTypes;
using System.IO;
using System.Numerics;
using System.Linq;

namespace Converter.Core.Games.MCLA
{
	public struct RES_63
	{
		public uint _vmt;
		public uint end;
		public uint m_pModelCollection;
		public uint _fc; // zeroes?
		public uint _f10; // zeroes?
		public uint m_pName;
		public uint _f18; // zeroes?
		public uint _f1c; // zeroes?
		public ushort _f20; //
		public ushort _f22; //
		public uint m_pSkelData;
		// that's not all...
	}

	public static class MCLA_63
	{
		public static bool Read(MemoryStream mem, bool endian)
		{
			uint tmp;

			using (EndianBinaryReader br = new EndianBinaryReader(mem))
			{
				if (endian)
				{
					br.Endianness = Endian.BigEndian;
				}

				RES_63 file = new RES_63
				{
					_vmt = br.ReadUInt32(),
					end = br.ReadOffset(),
					m_pModelCollection = br.ReadOffset(),
					_fc = br.ReadUInt32(),
					_f10 = br.ReadUInt32(),
					m_pName = br.ReadOffset(),
					_f18 = br.ReadUInt32(),
					_f1c = br.ReadUInt32(),
					_f20 = br.ReadUInt16(),
					_f22 = br.ReadUInt16(),
					m_pSkelData = br.ReadOffset()
				};

				br.Position = file.m_pModelCollection;

				Collection modelCollection = Collection.Read(br);
				Model[] model = new Model[modelCollection.m_nCount];
				uint[] pModel = new uint[modelCollection.m_nCount];

				br.Position = modelCollection.m_pList;

				for (int a = 0; a < modelCollection.m_nCount; a++)
				{
					pModel[a] = br.ReadOffset();
				}

				for (int a = 0; a < modelCollection.m_nCount; a++)
				{
					br.Position = pModel[a];
					model[a] = Model.Read(br);
				}

				uint geometryCount = 0;
				for (int a = 0; a < modelCollection.m_nCount; a++)
				{
					geometryCount += model[a].m_cGeometry.m_nCount;
				}

				uint[] pGeometry = new uint[geometryCount];
				Geometry[] geometry = new Geometry[geometryCount];
				VertexBuffer[] vertexBuffer = new VertexBuffer[geometryCount];
				IndexBuffer[] indexBuffer = new IndexBuffer[geometryCount];
				VertexDeclaration[] vertexDeclaration = new VertexDeclaration[geometryCount];

				uint currentGeometry = 0;
				for (int a = 0; a < modelCollection.m_nCount; a++)
				{
					br.Position = model[a].m_cGeometry.m_pList;

					for (int b = 0; b < model[a].m_cGeometry.m_nCount; b++)
					{
						pGeometry[currentGeometry++] = br.ReadOffset();
					}
				}

				for (int a = 0; a < geometryCount; a++)
				{
					br.Position = pGeometry[a];
					geometry[a] = Geometry.Read(br);

					br.Position = geometry[a].m_pVertexBuffer;
					vertexBuffer[a] = VertexBuffer.Read(br);

					tmp = vertexBuffer[a].m_pVertexData;
					vertexBuffer[a].m_pVertexData = vertexBuffer[a].m_pDeclaration;
					vertexBuffer[a].m_pDeclaration = tmp;

					br.Position = geometry[a].m_pIndexBuffer;
					indexBuffer[a] = IndexBuffer.Read(br);

					br.Position = vertexBuffer[a].m_pDeclaration;
					vertexDeclaration[a] = VertexDeclaration.Read(br);
				}

				Vector4[,] vBounds = new Vector4[modelCollection.m_nCount, 100];
				for (int a = 0; a < modelCollection.m_nCount; a++)
				{
					br.Position = model[a].m_pBounds;
					uint boundsCount = model[a].m_cGeometry.m_nCount;

					if (model[a].m_cGeometry.m_nCount > 1)
					{
						boundsCount++;
					}

					for (int b = 0; b < boundsCount; b++)
					{
						vBounds[a, b] = br.ReadVector4();
					}
				}

				// create odr file
				Drawable drawable = new Drawable();
				drawable.m_nObjectCount = Enumerable.Repeat(-1, drawable.m_nObjectCount.Length).ToArray();
				drawable.m_nObjectCount[0] = 1;
				drawable.m_pSkeleton = file.m_pSkelData;
				Collection[] cModel = new Collection[4] { new Collection(), new Collection(), new Collection(), new Collection() };
				cModel[0].m_nCount = (ushort)model.Length;
				cModel[0].m_nSize = (ushort)model.Length;
				string[] shaderLine = new string[Settings.nTempShadersCount];
				shaderLine = Enumerable.Repeat("gta_default.sps temptxd", (int)Settings.nTempShadersCount).ToArray();

				IV_skel.UniversalSkeletonData skelData = new IV_skel.UniversalSkeletonData();
				if (file.m_pSkelData != 0)
				{
					br.Position = file.m_pSkelData;
					skelData = IV_skel.UniversalSkeletonData.ConvertToUniversalSkeletonData(IV_SkeletonData.Read(br));
				}

				return IV_odr.Build(br, drawable, model, vBounds, indexBuffer, vertexBuffer, vertexDeclaration, cModel, shaderLine, skelData);
			}
		}
	}
}
