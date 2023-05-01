using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class Dictionary
	{
		public uint _vmt;
		public uint m_pBlockMap;
		public uint m_pParentDictionary;
		public uint m_dwUsageCount;
		public Collection m_cHash;
		public Collection m_cTexture;

		public static Dictionary Read(EndianBinaryReader br)
		{
			return new Dictionary
			{
				_vmt = br.ReadUInt32(),
				m_pBlockMap = br.ReadOffset(),
				m_pParentDictionary = br.ReadOffset(),
				m_dwUsageCount = br.ReadUInt32(),
				m_cHash = Collection.Read(br),
				m_cTexture = Collection.Read(br),
			};
		}
	}
}
