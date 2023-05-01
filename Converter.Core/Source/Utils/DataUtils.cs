using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace Converter.Core.Utils
{
	public static class DataUtils
	{
		// HashUtils.Jenkins.Calculate
		public static uint GetHash(string str)
		{
			char[] array = str.ToLower().ToCharArray();
			uint num;
			uint num2 = num = 0;

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

		// ...TrimEnd?
		public static int TrailingZeroes(int n)
		{
			int num = 1;
			int num2 = 0;
			
			while (num2 < 32)
			{
				if ((n & num) != 0)
				{
					return num2;
				}

				num2++;
				num <<= 1;
			}

			return 32;
		}

		// it's almost the same as ReadNullTerminatedString
		// but it saves the original position in the stream and return it back after reading
		public static string ReadStringAtOffset(uint p, EndianBinaryReader br)
		{
			uint oldPos = (uint)br.Position;
			br.Position = p;

			string result = br.ReadNullTerminatedString();
			
			br.Position = oldPos;
			
			return result;
		}

		public static string ReadNullTerminatedString(this BinaryReader stream)
		{
			StringBuilder str = new StringBuilder();
			char ch;

			while ((ch = stream.ReadChar()) != 0)
			{
				str.Append(ch);
			}

			return str.ToString();
		}

		public static void ReverseBytes(ref byte[] src, int count)
		{
			if (count == 0)
			{
				return;
			}
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

						Buffer.BlockCopy(outStream.ToArray(), 0, src, 0, src.Length);
						return;
					}
				}
			}
		}

		// PackedVector.DEC3N.Unpack
		public static Vector4 DEC3NToVector4(uint value)
		{
			int x = ((int)value >> 0) & 0x3FF;
			int y = ((int)value >> 10) & 0x3FF;
			int z = ((int)value >> 20) & 0x3FF;
			int w = ((int)value >> 30) & 0x3;

			if (x > 511)
			{
				x -= 1024;
			}

			if (y > 511)
			{
				y -= 1024;
			}

			if (z > 511)
			{
				z -= 1024;
			}

			if (w > 1)
			{
				w -= 4;
			}

			return new Vector4(x / 511.0f, y / 511.0f, z / 511.0f, w);
		}
	}
}
