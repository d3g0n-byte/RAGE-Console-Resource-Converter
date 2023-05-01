using System.Numerics;
using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class Drawable
	{
		public uint _vmt;
		public uint m_pEndOfTheHeader;
		public uint m_pShaderGroup;
		public uint m_pSkeleton;
		public Vector4 m_vCenter;
		public Vector4 m_vAabbMin;
		public Vector4 m_vAabbMax;
		public uint[] m_pModelCollection = new uint[4];
		public Vector4 m_vDrawDistance;
		public int[] m_nObjectCount = new int[4];
		public Vector4 m_vRadius;
		public Collection m_p2DFX;

		public static Drawable Read(EndianBinaryReader br)
		{
			return new Drawable
			{
				_vmt = br.ReadUInt32(),
				m_pEndOfTheHeader = br.ReadOffset(),
				m_pShaderGroup = br.ReadOffset(),
				m_pSkeleton = br.ReadOffset(),
				m_vCenter = br.ReadVector4(),
				m_vAabbMin = br.ReadVector4(),
				m_vAabbMax = br.ReadVector4(),
				m_pModelCollection = new uint[4]
				{
						br.ReadOffset(),
						br.ReadOffset(),
						br.ReadOffset(),
						br.ReadOffset()
				},
				m_vDrawDistance = br.ReadVector4(),
				m_nObjectCount = new int[4]
				{
						br.ReadInt32(),
						br.ReadInt32(),
						br.ReadInt32(),
						br.ReadInt32()
				},
				m_vRadius = br.ReadVector4(),
				m_p2DFX = Collection.Read(br)
			};
		}
	}
}
