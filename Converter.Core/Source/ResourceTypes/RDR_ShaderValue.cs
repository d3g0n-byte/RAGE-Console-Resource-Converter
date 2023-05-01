using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class RDR_ShaderValue
	{
		public byte m_nParamType;
		public byte m_nType; // from v2saconv
		public short _f2;
		public uint m_pValue;

		public static RDR_ShaderValue Read(EndianBinaryReader br)
		{
			return new RDR_ShaderValue
			{
				m_nParamType = br.ReadByte(),
				m_nType = br.ReadByte(),
				_f2 = br.ReadInt16(),
				m_pValue = br.ReadOffset()
			};
		}
	}
}
