using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Converter.Utils;
using ICSharpCode.SharpZipLib.Zip.Compression;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

namespace Converter
{
	public static class DataUtils
	{
		public static uint GetHash(string str)
		{
			char[] array = str.ToLower().ToCharArray();
			uint num;
			uint num2 = (num = 0u);
			for (; num < array.Length; num++)
			{
				num2 += array[num];
				num2 += num2 << 10;
				num2 ^= num2 >> 6;
			}
			num2 += num2 << 3;
			num2 ^= num2 >> 11;
			return num2 + (num2 << 15);
		}
		public static byte[] Compress(byte[] input, int level, bool noHeader = true)
		{
			byte[] array = new byte[input.Length];
			Deflater deflater = new Deflater(level, noHeader);
			byte[] array2;
			try
			{
				deflater.SetInput(input, 0, input.Length);
				deflater.Finish();
				array2 = new byte[deflater.Deflate(array)];
			}
			catch (Exception ex)
			{
				throw ex;
			}
			Array.Copy(array, 0, array2, 0, array2.Length);
			return array2;
		}
		public static int SetBit(int val, int bit, bool trueORfalse)
		{
			bool flag = (val & (1 << bit)) != 0;
			if (trueORfalse)
			{
				if (!flag)
					return val |= 1 << bit;
			}
			else if (flag)
			{
				return val ^ (1 << bit);
			}
			return val;
		}
		public static int TrailingZeroes(int n)
		{
			int num = 1;
			int num2 = 0;
			while (num2 < 32)
			{
				if ((n & num) != 0)
					return num2;
				num2++;
				num <<= 1;
			}
			return 32;
		}
		public static byte[] DecompressDeflate(byte[] data, int decompSize, bool noHeader = true)
		{
			byte[] array = new byte[decompSize];
			Inflater inflater = new Inflater(noHeader);
			inflater.SetInput(data);
			inflater.Inflate(array);
			return array;
		}
		public static bool GetBit(ushort b, int i)
		{
			return (b & (1 << i)) != 0;
		}
		public static bool GetBit(uint b, int i)
		{
			return (b & (1 << i)) != 0;
		}
		internal static bool GetBit(byte b, int i)
		{
			return (b & (1 << i)) != 0;
		}

		public static string GetFileMask(string fileName)
		{
			return System.IO.Path.GetExtension(fileName);
		}
		public static string ReadStringAtOffset(uint p, EndianBinaryReader br)
		{
			uint oldPos = (uint)br.Position;
			br.Position = p;
			string tempstring = "";
			char tmp;
			for (int a = 0; a < 1;)
			{
				tmp = br.ReadChar();
				if (tmp != 0) tempstring += tmp;
				else break;
			}
			br.Position = oldPos;
			return tempstring;
		}
		public static void ReverseBytes(ref byte[] src, int count)
		{
			if (count==0) return;
			if (count % 2 != 0)
			{
				throw new NotSupportedException("This operation is possible only for even numbers.");
			}

			using (MemoryStream outStream = new MemoryStream())
			{
				using (BinaryReader br = new BinaryReader(new MemoryStream(src)))
				{
					using (BinaryWriter bw = new BinaryWriter(outStream))
					{
						for (int i = 0; i < (int)br.BaseStream.Length / count; i++)
						{
							byte[] bytes = br.ReadBytes(count);
							Array.Reverse(bytes);
							bw.Write(bytes);
						}

						//return outStream.ToArray();
						Buffer.BlockCopy(outStream.ToArray(), 0, src, 0, src.Length);
						return;
					}
				}
			}
		}
	}
	public static class FileInfo
	{
		public static string fileName;
		public static string fileMask
		{
			get
			{
				if (fileName.Contains('.')) return DataUtils.GetFileMask(fileName);
				else return "";
			}
		}
		public static string baseFileName
		{
			get
			{
				return Path.GetFileName(FileInfo.fileName.Substring(0, FileInfo.fileName.Length - FileInfo.fileMask.Length));
			}
		}
		
		public static string filePath
		{
			get
			{
				return Path.GetDirectoryName(FileInfo.fileName.Substring(0, FileInfo.fileName.Length - FileInfo.fileMask.Length));
			}
		}


	}

}
