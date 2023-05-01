using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;
using System.IO;

namespace Converter.Core.Utils.Compression
{
	public static class Zlib
	{
		public static byte[] Compress(byte[] data)
		{
			if (data == null)
			{
				return null;
			}

			using (MemoryStream m = new MemoryStream())
			{
				using (ZlibStream d = new ZlibStream(m, CompressionMode.Compress, CompressionLevel.Default))
				{
					d.Write(data, 0, data.Length);
				}
				return m.ToArray();
			}
		}
		
		public static byte[] Decompress(byte[] data)
		{
			if (data == null)
			{
				return null;
			}

			using (MemoryStream m = new MemoryStream(data))
			{
				using (ZlibStream d = new ZlibStream(m, CompressionMode.Decompress, CompressionLevel.Default))
				{
					using (MemoryStream ms = new MemoryStream())
					{
						d.CopyTo(ms);
						return ms.ToArray();
					}
				}
			}
		}
	}
}
