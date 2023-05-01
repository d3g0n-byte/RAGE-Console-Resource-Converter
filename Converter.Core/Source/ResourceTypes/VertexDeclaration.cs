using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class VertexDeclaration
	{
		public RageD3D9VertexFlags m_UsedElements;
		public byte m_nTotaSize;
		public sbyte _f6;
		public byte m_bStoreNormalsDataFirst;
		public byte m_nElementsCount;
		public RageD3D9VertexElementTypes m_ElementTypes;

		public static VertexDeclaration Read(EndianBinaryReader br)
		{
			return new VertexDeclaration
			{
				m_UsedElements = RageD3D9VertexFlags.Read(br.ReadUInt32()),
				m_nTotaSize = br.ReadByte(),
				_f6 = br.ReadSByte(),
				m_bStoreNormalsDataFirst = br.ReadByte(),
				m_nElementsCount = br.ReadByte(),
				m_ElementTypes = RageD3D9VertexElementTypes.Read(br.ReadUInt64())
			};
		}
	}
}
