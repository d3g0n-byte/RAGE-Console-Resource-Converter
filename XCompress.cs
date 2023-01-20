using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Converter.Utils
{
	public class XCompress
	{

		private const string modulePath = @"xcompress64.dll";

		[DllImport(modulePath, EntryPoint = "XMemCreateDecompressionContext")]
		private static extern int XMemCreateDecompressionContext(XMemCodecType CodecType, int pCodecParams, int Flags, ref IntPtr pContext);

		[DllImport(modulePath, EntryPoint = "XMemDestroyDecompressionContext")]
		private static extern int XMemDestroyDecompressionContext(IntPtr pContext);

		[DllImport(modulePath, EntryPoint = "XMemResetDecompressionContext")]
		public static extern int XMemResetDecompressionContext(IntPtr Context);

		[DllImport(modulePath, EntryPoint = "XMemDecompress")]
		private static extern int XMemDecompress(IntPtr Context, byte[] pDestination, ref int pDestSize, byte[] pSource, int SrcSize);

		[DllImport(modulePath, EntryPoint = "XMemDecompressStream")]
		public static extern int XMemDecompressStream(IntPtr Context, byte[] pDestination, ref int pDestSize, byte[] pSource, ref int pSrcSize);
		
		[DllImport(modulePath, EntryPoint = "XMemCreateCompressionContext")]
		public static extern int XMemCreateCompressionContext(XMemCodecType CodecType, int pCodecParams, int Flags, ref IntPtr pContext);

		[DllImport(modulePath, EntryPoint = "XMemDestroyCompressionContext")]
		public static extern void XMemDestroyCompressionContext(IntPtr Context);

		[DllImport(modulePath, EntryPoint = "XMemResetCompressionContext")]
		public static extern int XMemResetCompressionContext(IntPtr Context);

		[DllImport(modulePath, EntryPoint = "XMemCompress")]
		public static extern int XMemCompress(IntPtr Context, byte[] pDestination, ref int pDestSize, byte[] pSource, int pSrcSize);

		[DllImport(modulePath, EntryPoint = "XMemCompressStream")]
		public static extern int XMemCompressStream(IntPtr Context, byte[] pDestination, ref int pDestSize, byte[] pSource, ref int pSrcSize);

		public static byte[] Decompress(byte[] data, int compressedSize, int uncompressedSize, bool mode)// by 
		{
			byte[] outData = new byte[uncompressedSize];
			IntPtr ctx = IntPtr.Zero;

			if (!mode)
			{
				if (XMemCreateDecompressionContext(XMemCodecType.LZX, 0, 0, ref ctx) != 0)
					throw new Exception("XMemCreateDecompressionContext failed");
				if (XMemDecompress(ctx, outData, ref uncompressedSize, data, compressedSize) != 0)
				{
					XMemDestroyDecompressionContext(ctx);
					throw new Exception("XMemDecompress failed");
				}
				XMemDestroyDecompressionContext(ctx);
			}
			return outData;
		}

		public struct XMemCodecParametersLZX
		{
			public int Flags;
			public int WindowSize;
			public int CompressionPartitionSize;
		}
		public enum XMemCodecType
		{
			Default,
			LZX
		}
	}
}
