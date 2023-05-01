using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class RDR_ShaderFX
	{
		public uint m_pValue;
		public uint m_nNameHash;
		public byte m_nParamCount;
		public sbyte _f9;
		public short _fA;
		public int _fC;
		public int _f20;
		public int _f24;
		public int _f28;
		public int _f2C;
		public RDR_ShaderValue[] value; // has nothing to do with structure

		public static RDR_ShaderFX Read(EndianBinaryReader br)
		{
			RDR_ShaderFX shaderFX = new RDR_ShaderFX
			{
				m_pValue = br.ReadOffset(),
				m_nNameHash = br.ReadUInt32(),
				m_nParamCount = br.ReadByte(),
				_f9 = br.ReadSByte(),
				_fA = br.ReadInt16(),
				_fC = br.ReadInt32(),
				_f20 = br.ReadInt32(),
				_f24 = br.ReadInt32(),
				_f28 = br.ReadInt32(),
				_f2C = br.ReadInt32()
			};

			shaderFX.value = new RDR_ShaderValue[shaderFX.m_nParamCount];

			for (int i = 0; i < shaderFX.m_nParamCount; i++)
			{
				br.Position = shaderFX.m_pValue + (i * 8);
				shaderFX.value[i] = RDR_ShaderValue.Read(br);
			}

			return shaderFX;
		}
	}
}
