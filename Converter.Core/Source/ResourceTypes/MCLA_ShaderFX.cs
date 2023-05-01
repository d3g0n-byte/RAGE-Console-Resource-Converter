using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class MCLA_ShaderFX
	{
		public uint vtable;
		public uint m_pBlockMapAdress;
		public byte _f8;
		public byte m_nDrawBucket;
		public byte _fa;
		public byte _fb;
		public short _fc;
		public ushort m_nIndex;
		public uint m_pShaderParams;
		public uint _f14;
		public ushort m_nParamsCount;
		public ushort m_nEffectSize;
		public uint m_pParameterTypes;
		public uint m_nHash;
		public uint m_pParamsHash;
		public uint _f24; // pointer
		public uint _f28;
		public uint m_pName;
		public uint _f30;
		public uint m_pSPS;
		public uint m_sUnk;

		public static MCLA_ShaderFX Read(EndianBinaryReader br)
		{
			return new MCLA_ShaderFX
			{
				vtable = br.ReadUInt32(),
				m_pBlockMapAdress = br.ReadOffset(),
				_f8 = br.ReadByte(),
				m_nDrawBucket = br.ReadByte(),
				_fa = br.ReadByte(),
				_fb = br.ReadByte(),
				_fc = br.ReadInt16(),
				m_nIndex = br.ReadUInt16(),
				m_pShaderParams = br.ReadOffset(),
				_f14 = br.ReadUInt32(),
				m_nParamsCount = br.ReadUInt16(),
				m_nEffectSize = br.ReadUInt16(),
				m_pParameterTypes = br.ReadOffset(),
				m_nHash = br.ReadUInt32(),
				m_pParamsHash = br.ReadOffset(),
				_f24 = br.ReadOffset(),
				_f28 = br.ReadUInt32(),
				m_pName = br.ReadOffset(),
				_f30 = br.ReadUInt32(),
				m_pSPS = br.ReadOffset(),
				m_sUnk = br.ReadOffset()
			};
		}
	}
}
