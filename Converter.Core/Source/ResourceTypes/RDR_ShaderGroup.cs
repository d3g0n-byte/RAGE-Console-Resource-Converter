using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class RDR_ShaderGroup
	{
		public uint _vmt; // 0x409a6d00
		public uint m_pTextureDictionary; // xtd file
		public Collection m_cShaderFX;

		public static RDR_ShaderGroup Read(EndianBinaryReader br)
		{
			return new RDR_ShaderGroup
			{
				_vmt = br.ReadUInt32(),
				m_pTextureDictionary = br.ReadOffset(),
				m_cShaderFX = Collection.Read(br)
			};
		}
	}
}
