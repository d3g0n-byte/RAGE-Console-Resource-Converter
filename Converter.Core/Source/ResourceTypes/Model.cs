using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class Model
	{
		public uint _vmt;
		public Collection m_cGeometry;
		public uint m_pBounds;
		public uint m_pShaderMapping;
		public byte m_nBoneCount;
		public bool m_bSkinned;
		public sbyte _f16;
		public byte m_nBoneIndex;
		public sbyte _f18;
		public byte m_bHasOffset;
		public ushort m_nShaderMappingsCount;

		public static Model Read(EndianBinaryReader br)
		{
			return new Model
			{
				_vmt = br.ReadUInt32(),
				m_cGeometry = Collection.Read(br),
				m_pBounds = br.ReadOffset(),
				m_pShaderMapping = br.ReadOffset(),
				m_nBoneCount = br.ReadByte(),
				m_bSkinned = br.ReadBoolean(),
				_f16 = br.ReadSByte(),
				m_nBoneIndex = br.ReadByte(),
				_f18 = br.ReadSByte(),
				m_bHasOffset = br.ReadByte(),
				m_nShaderMappingsCount = br.ReadUInt16()
			};
		}
	}
}
