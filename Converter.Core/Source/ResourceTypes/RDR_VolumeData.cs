using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class RDR_VolumeData
	{
		public uint _vmt;
		public uint m_pBlockMapAdress;
		public uint _f8;
		public uint m_pBlockMapAdress2;
		public uint m_pParentDictionary;
		public int m_nUsageCount; // 1
		public Collection m_cNameHash;
		public Collection m_cDrawable;
		public uint m_pTexture; // xtd file
		public int _f2C;
		public int _f30;
		public int _f34;
		public int _f38;

		public static RDR_VolumeData Read(EndianBinaryReader br)
		{
			return new RDR_VolumeData
			{
				_vmt = br.ReadUInt32(),
				m_pBlockMapAdress = br.ReadOffset(),
				_f8 = br.ReadUInt32(),
				m_pBlockMapAdress2 = br.ReadOffset(),
				m_pParentDictionary = br.ReadOffset(),
				m_nUsageCount = br.ReadInt32(),
				m_cNameHash = Collection.Read(br),
				m_cDrawable = Collection.Read(br),
				m_pTexture = br.ReadOffset(),
				_f2C = br.ReadInt32(),
				_f30 = br.ReadInt32(),
				_f34 = br.ReadInt32(),
				_f38 = br.ReadInt32()
			};
		}

	}
}
