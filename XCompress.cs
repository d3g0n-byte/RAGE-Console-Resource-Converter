using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Converter.Utils
{
	public class XCompress
	{
        //lzx.dll
        [DllImport(@"lzx.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr LZXinit(int windowSize);

        [DllImport(@"lzx.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int LZXdecompress(IntPtr State, byte[] pSource, byte[] pDestination, int SrcSize, int DestSize);

        [DllImport(@"lzx.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int LZXteardown(IntPtr State);
        //

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

        //
        public static int GetCPUSizeRSC85(uint dwFlags1, uint dwFlags2)// from Rage Console Texture Editor
        {
            if ((dwFlags2 & 0x80000000) == 0)
                return (int)(dwFlags1 & 0x7FF) << (int)(((dwFlags1 >> 11) & 15) + 8);
            else
                return (int)(dwFlags2 & 0x3FFF) << 12;
        }
        public static int GetGPUSizeRSC85(uint dwFlags1, uint dwFlags2) // from Rage Console Texture Editor
		{
            if ((dwFlags2 & 0x80000000) == 0)
                return (int)((dwFlags1 & 15) & 0x7FF) << (int)(((dwFlags1 >> 26) & 15) + 8);
            else
                return (int)(dwFlags2 >> 2) & 0x3FFF000;
        }
        public static int GetCPUSizeRSC5(uint dwFlags) // from Rage Console Texture Editor
		{
            return (int)(dwFlags & 0x7FF) << (int)(((dwFlags >> 11) & 0xF) + 8);
        }
        public static int GetGPUSizeRSC5(uint dwFlags) // from Rage Console Texture Editor
		{
            return (int)((dwFlags >> 15) & 0x7FF) << (int)(((dwFlags >> 26) & 0xF) + 8);
        }
        public static int GetSizeRSC37(uint dwFlag, uint baseSize) // from Rage Console Texture Editor
		{ 
            int newBaseSize, size;
            newBaseSize = (int)baseSize << (int)(dwFlag & 0xf);
            size = (((((int)dwFlag >> 17) & 0x7f) + ((((int)dwFlag >> 11) & 0x3f) << 1) + ((((int)dwFlag >> 7) & 0xf) << 2) + ((((int)dwFlag >> 5) & 0x3) << 3) + ((((int)dwFlag >> 4) & 0X1) << 4)) * newBaseSize);
            for (int i = 0; i < 3; i++)size = size + newBaseSize >>(1 + i);
            return size;
        }

        //
        public static byte[] Decompress(byte[] data, int compressedSize, int uncompressedSize, bool mode)// by 
        {
            byte[] outData = new byte[uncompressedSize];
            IntPtr ctx = IntPtr.Zero;

            if (!mode)
            {
                if (XMemCreateDecompressionContext(XMemCodecType.LZX, 0, 0, ref ctx) != 0)
                {
                    throw new Exception("XMemCreateDecompressionContext failed");
                }

                if (XMemDecompress(ctx, outData, ref uncompressedSize, data, compressedSize) != 0)
                {
                    XMemDestroyDecompressionContext(ctx);
                    throw new Exception("XMemDecompress failed");
                }
                XMemDestroyDecompressionContext(ctx);
            }
            //}
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
