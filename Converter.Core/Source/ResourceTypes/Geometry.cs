using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class Geometry
	{
		public uint vtable;
		public uint m_piVertexDeclaration;
		public int _f8;
		public uint m_pVertexBuffer;
		public int _f10;
		public int _f14;
		public int _f18;
		public uint m_pIndexBuffer;
		public int _f20;
		public int _f24;
		public int _f28;
		public uint m_nIndexCount;
		public uint m_nFaceCount;
		public ushort m_wVertexCount;
		public ushort m_wIndicesPerFace;
		public uint m_pBoneMapping;
		public ushort m_nVertexStride;
		public ushort m_nBoneCount;
		public uint m_pVertexDeclaration;
		public uint _f44;
		public int _f48;

		public static Geometry Read(EndianBinaryReader br)
		{
			return new Geometry
			{
				vtable = br.ReadUInt32(),
				m_piVertexDeclaration = br.ReadUInt32(),
				_f8 = br.ReadInt32(),
				m_pVertexBuffer = br.ReadOffset(),
				_f10 = br.ReadInt32(),
				_f14 = br.ReadInt32(),
				_f18 = br.ReadInt32(),
				m_pIndexBuffer = br.ReadOffset(),
				_f20 = br.ReadInt32(),
				_f24 = br.ReadInt32(),
				_f28 = br.ReadInt32(),
				m_nIndexCount = br.ReadUInt32(),
				m_nFaceCount = br.ReadUInt32(), // this value is used by openiv to protect the models
				m_wVertexCount = br.ReadUInt16(),
				m_wIndicesPerFace = br.ReadUInt16(),
				m_pBoneMapping = br.ReadOffset(),
				m_nVertexStride = br.ReadUInt16(),
				m_nBoneCount = br.ReadUInt16(),
				m_pVertexDeclaration = br.ReadOffset(),
				_f44 = br.ReadOffset(),
				_f48 = br.ReadInt32()
			};
		}
	}
}
