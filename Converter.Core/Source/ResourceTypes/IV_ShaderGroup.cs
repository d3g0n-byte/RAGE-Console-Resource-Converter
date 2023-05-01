using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class IV_ShaderGroup
	{
		public uint _vmt;
		public uint m_pTexture;
		public Collection m_pShaders;
		public Collection _f10;
		public Collection _f18;
		public Collection _f20;
		public Collection _f28;
		public Collection _f30;
		public Collection _f38;
		public Collection m_pVertexFormat;
		public Collection m_pIndexMapping;

		public static IV_ShaderGroup Read(EndianBinaryReader br)
		{
			return new IV_ShaderGroup
			{
				_vmt = br.ReadUInt32(),
				m_pTexture = br.ReadOffset(),
				m_pShaders = Collection.Read(br),
				_f10 = Collection.Read(br),
				_f18 = Collection.Read(br),
				_f20 = Collection.Read(br),
				_f28 = Collection.Read(br),
				_f30 = Collection.Read(br),
				_f38 = Collection.Read(br),
				m_pVertexFormat = Collection.Read(br),
				m_pIndexMapping = Collection.Read(br)
			};
		}
	}
}
