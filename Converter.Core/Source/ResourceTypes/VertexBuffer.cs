using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class VertexBuffer
	{
		public uint vtable;
		public ushort m_nVertexCount;
		public byte m_nLocked;
		public sbyte _f7;
		public uint m_pLockedData;
		public uint m_nVertexSize;
		public uint m_pVertexData;
		public int _f14;
		public uint m_pDeclaration;

		public static VertexBuffer Read(EndianBinaryReader br)
		{
			return new VertexBuffer
			{
				vtable = br.ReadUInt32(),
				m_nVertexCount = br.ReadUInt16(),
				m_nLocked = br.ReadByte(),
				_f7 = br.ReadSByte(),
				m_pLockedData = br.ReadOffset(), // not used by the game
				m_nVertexSize = br.ReadUInt32(),
				m_pVertexData = br.ReadOffset(),
				_f14 = br.ReadInt32(),
				m_pDeclaration = br.ReadOffset()
			};
		}
	}
}
