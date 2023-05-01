using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class FragmentDictionary
	{
		public uint _vmt;
		public uint m_pBlockMapAdress;
		public uint m_pDrawable;
		public uint m_pTextureDictionary;

		public static FragmentDictionary Read(EndianBinaryReader br)
		{
			return new FragmentDictionary
			{
				_vmt = br.ReadUInt32(),
				m_pBlockMapAdress = br.ReadOffset(),
				m_pDrawable = br.ReadOffset(),
				m_pTextureDictionary = br.ReadOffset()
			};
		}
	}
}
