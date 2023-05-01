using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	// only for xbox
	public class RDR_Texture
	{
		public uint _vmt;
		public int _f4;
		public sbyte _f8;
		public sbyte _f9;
		public short _fa;
		public int _fc;
		public uint _f10; // pointer
		public uint m_nSize;
		public uint m_pName;
		public uint m_pBitmap;
		public ushort m_nWidth;
		public ushort m_nHeight;
		public uint m_nLodCount;
		public float _f28;
		public float _f2c;
		public float _f30;
		public float _f34;
		public float _f38;
		public float _f3c;

		public static RDR_Texture Read(EndianBinaryReader br)
		{
			return new RDR_Texture
			{
				_vmt = br.ReadUInt32(),
				_f4 = br.ReadInt32(),
				_f8 = br.ReadSByte(),
				_f9 = br.ReadSByte(),
				_fa = br.ReadInt16(),
				_fc = br.ReadInt32(),
				_f10 = br.ReadOffset(),
				m_nSize = br.ReadUInt32(),
				m_pName = br.ReadOffset(),
				m_pBitmap = br.ReadOffset(),
				m_nWidth = br.ReadUInt16(),
				m_nHeight = br.ReadUInt16(),
				m_nLodCount = br.ReadUInt32(),
				_f28 = br.ReadSingle(),
				_f2c = br.ReadSingle(),
				_f30 = br.ReadSingle(),
				_f34 = br.ReadSingle(),
				_f38 = br.ReadSingle(),
				_f3c = br.ReadSingle()
			};
		}
	}
}
