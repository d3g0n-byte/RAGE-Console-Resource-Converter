using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class RDR_TextureDefinition
	{
		public uint _vmt;
		public int _f4;
		public int _f8;
		public short _fC;
		public short _fD;
		public int _f10;
		public int _f14;
		public uint m_pName;
		public int _f1C;

		public static RDR_TextureDefinition Read(EndianBinaryReader br)
		{
			return new RDR_TextureDefinition
			{
				_vmt = br.ReadUInt32(),
				_f4 = br.ReadInt32(),
				_f8 = br.ReadInt32(),
				_fC = br.ReadInt16(),
				_fD = br.ReadInt16(),
				_f10 = br.ReadInt32(),
				_f14 = br.ReadInt32(),
				m_pName = br.ReadOffset(),
				_f1C = br.ReadInt32()
			};
		}
	}
}
