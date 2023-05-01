using System;

namespace Converter.Core.Utils.Compression
{
	public static class LZX
	{
		public static int Decompress(byte[] compressedData, byte[] decompressedData)
		{
			Array.Resize(ref compressedData, compressedData.Length * 2);

			XCompression.DecompressionContext ctx = new XCompression.DecompressionContext();
			int pakLen = compressedData.Length;
			int outLen = decompressedData.Length;
			ErrorCode err = (ErrorCode)ctx.Decompress(compressedData, 0, ref pakLen, decompressedData, 0, ref outLen);
			return (int)err;
		}
	}

	public enum ErrorCode
	{
		None = 0
	}
}
