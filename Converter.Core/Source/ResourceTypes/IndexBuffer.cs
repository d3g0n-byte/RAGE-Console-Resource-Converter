using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class IndexBuffer
	{
		public uint _vmt;
		public uint m_nIndexCount;
		public uint m_pIndexData;

		public static IndexBuffer Read(EndianBinaryReader br)
		{
			return new IndexBuffer
			{
				_vmt = br.ReadUInt32(),
				m_nIndexCount = br.ReadUInt32(),
				m_pIndexData = br.ReadOffset()
			};
		}
	}
}
