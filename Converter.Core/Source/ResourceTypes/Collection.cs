using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class Collection
	{
		public uint m_pList;
		public ushort m_nCount;
		public ushort m_nSize;

		public static Collection Read(EndianBinaryReader br)
		{
			Collection value = new Collection
			{
				m_pList = br.ReadUInt32(br.Endianness)
			};

			if (value.m_pList == 0)
			{
				value.m_pList = 0;
			}
			else if ((value.m_pList >> 28 != 5) && (value.m_pList >> 28 != 6))
			{
				value.m_pList = 0;
			}
			else if (value.m_pList >> 28 == 5)
			{
				value.m_pList &= 0x0FFFFFFF;
			}
			else
			{
				value.m_pList = (value.m_pList & 0x0FFFFFFF) + (uint)FlagInfo.BaseResourceSizeV;
			}

			value.m_nCount = br.ReadUInt16(br.Endianness);
			value.m_nSize = br.ReadUInt16(br.Endianness);

			return value;
		}
	}
}
