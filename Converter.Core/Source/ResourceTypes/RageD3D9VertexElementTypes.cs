namespace Converter.Core.ResourceTypes
{
	public class RageD3D9VertexElementTypes
	{
		public byte m_nPositionType;
		public byte m_nBlendWeightType;
		public byte m_nBlendIndicesType;
		public byte m_nNormalType;
		public byte m_nColorType;
		public byte m_nSpecularType;
		public byte m_nTexCoord0Type;
		public byte m_nTexCoord1Type;
		public byte m_nTexCoord2Type;
		public byte m_nTexCoord3Type;
		public byte m_nTexCoord4Type;
		public byte m_nTexCoord5Type;
		public byte m_nTexCoord6Type;
		public byte m_nTexCoord7Type;
		public byte m_nTangentType;
		public byte m_nBinormalType;

		public static RageD3D9VertexElementTypes Read(ulong Types)
		{
			RageD3D9VertexElementTypes vertexElementTypes = new RageD3D9VertexElementTypes();

			byte tmpValue = (byte)((Types & 0xFF00000000000000) >> 56);
			vertexElementTypes.m_nBinormalType = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nTangentType = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)((Types & 0x00FF000000000000) >> 48);
			vertexElementTypes.m_nTexCoord7Type = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nTexCoord6Type = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)((Types & 0x0000FF0000000000) >> 40);
			vertexElementTypes.m_nTexCoord5Type = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nTexCoord4Type = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)((Types & 0x000000FF00000000) >> 32);
			vertexElementTypes.m_nTexCoord3Type = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nTexCoord2Type = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)((Types & 0x00000000FF000000) >> 24);
			vertexElementTypes.m_nTexCoord1Type = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nTexCoord0Type = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)((Types & 0x0000000000FF0000) >> 16);
			vertexElementTypes.m_nSpecularType = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nColorType = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)((Types & 0x000000000000FF00) >> 8);
			vertexElementTypes.m_nNormalType = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nBlendIndicesType = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)(Types & 0x00000000000000FF);
			vertexElementTypes.m_nBlendWeightType = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nPositionType = (byte)((tmpValue & 0xF0) >> 4);

			return vertexElementTypes;
		}
	}
}
