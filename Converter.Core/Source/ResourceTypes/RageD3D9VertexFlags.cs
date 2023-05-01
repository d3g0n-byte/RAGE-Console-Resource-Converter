using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class RageD3D9VertexFlags
	{
		public bool m_bPosition;
		public bool m_bBlendWeight;
		public bool m_bBlendIndices;
		public bool m_bNormal;
		public bool m_bColor;
		public bool m_bSpecular;
		public bool m_bTexCoord0;
		public bool m_bTexCoord1;
		public bool m_bTexCoord2;
		public bool m_bTexCoord3;
		public bool m_bTexCoord4;
		public bool m_bTexCoord5;
		public bool m_bTexCoord6;
		public bool m_bTexCoord7;
		public bool m_bTangent;
		public bool m_bBinormal;

		public static RageD3D9VertexFlags Read(uint dwFlags)
		{
			ushort flags = (ushort)dwFlags;
			return new RageD3D9VertexFlags
			{
				m_bPosition = BitUtils.Get(flags, 0),
				m_bBlendWeight = BitUtils.Get(flags, 1),
				m_bBlendIndices = BitUtils.Get(flags, 2),
				m_bNormal = BitUtils.Get(flags, 3),
				m_bColor = BitUtils.Get(flags, 4),
				m_bSpecular = BitUtils.Get(flags, 5),
				m_bTexCoord0 = BitUtils.Get(flags, 6),
				m_bTexCoord1 = BitUtils.Get(flags, 7),
				m_bTexCoord2 = BitUtils.Get(flags, 8),
				m_bTexCoord3 = BitUtils.Get(flags, 9),
				m_bTexCoord4 = BitUtils.Get(flags, 10),
				m_bTexCoord5 = BitUtils.Get(flags, 11),
				m_bTexCoord6 = BitUtils.Get(flags, 12),
				m_bTexCoord7 = BitUtils.Get(flags, 13),
				m_bTangent = BitUtils.Get(flags, 14),
				m_bBinormal = BitUtils.Get(flags, 15)
			};
		}
	}
}
