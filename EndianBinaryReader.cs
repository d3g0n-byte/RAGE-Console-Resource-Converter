using System;
using System.Text;
using System.IO;
using ConsoleApp1;
using System.Numerics;
using Converter;
using static Converter.ResourceUtils;

namespace Converter.Utils
{
	/// <summary>
	/// Specifies an endianness
	/// </summary>
	public enum Endian
	{
		/// <summary>
		/// Little endian (i.e. DDCCBBAA)
		/// </summary>
		LittleEndian = 0,

		/// <summary>
		/// Big endian (i.e. AABBCCDD)
		/// </summary>
		BigEndian = 1
	}

	/// <summary>
	/// Read data from stream with data of specified endianness
	/// </summary>
	public class EndianBinaryReader : BinaryReader
	{
		/* TODO: BIGENDIAN check taken from BitConverter source; does this work as intended? */
#if BIGENDIAN
		public const Endian NativeEndianness = Endian.BigEndian;
#else
		public const Endian NativeEndianness = Endian.LittleEndian;
#endif

		/// <summary>
		/// Currently specified endianness
		/// </summary>
		public Endian Endianness { get; set; }

		public long Position
		{
			get
			{
				return BaseStream.Position;
			}
			set
			{
				BaseStream.Position = value;
			}
		}

		/// <summary>
		/// Boolean representing if the currently specified endianness equal to the system's native endianness
		/// </summary>
		public bool IsNativeEndianness { get { return (NativeEndianness == Endianness); } }

		/* TODO: doublecheck every non-native read result; slim down reverse functions? */

		public EndianBinaryReader(Stream input) : this(input, Endian.LittleEndian) { }
		public EndianBinaryReader(Stream input, Encoding encoding) : this(input, encoding, Endian.LittleEndian) { }
		public EndianBinaryReader(Stream input, Endian endianness) : this(input, Encoding.UTF8, endianness) { }

		public EndianBinaryReader(Stream input, Encoding encoding, Endian endianness)
			: base(input, encoding)
		{
			this.Endianness = endianness;
		}

		public void Seek(long offset, SeekOrigin origin)
		{
			BaseStream.Seek(offset, origin);
		}

		public void SeekBegin(long offset)
		{
			BaseStream.Seek(offset, SeekOrigin.Begin);
		}

		public void SeekCurrent(long offset)
		{
			BaseStream.Seek(offset, SeekOrigin.Current);
		}

		public void SeekEnd(long offset)
		{
			BaseStream.Seek(offset, SeekOrigin.End);
		}

		public override float ReadSingle()
		{
			return ReadSingle(Endianness);
		}

		public float ReadSingle(Endian endianness)
		{
			if (endianness == NativeEndianness)
			{
				return base.ReadSingle();
			}
			else
			{
				return BitConverter.ToSingle(BitConverter.GetBytes(base.ReadUInt32().Reverse()), 0);
			}
		}
		//
		public override Half ReadHalf()
		{
			return ReadHalf(Endianness);
		}
		public Half ReadHalf(Endian endianness)
		{
			if (endianness == NativeEndianness)
			{
				return base.ReadHalf();
			}
			else
			{
				return BitConverter.ToHalf(BitConverter.GetBytes(base.ReadUInt16().Reverse()), 0);
			}
		}

	//
		public override double ReadDouble()
		{
			return ReadDouble(Endianness);
		}

		public double ReadDouble(Endian endianness)
		{
			if (endianness == NativeEndianness)
			{
				return base.ReadDouble();
			}
			else
			{
				return BitConverter.ToDouble(BitConverter.GetBytes(base.ReadUInt64().Reverse()), 0);
			}
		}

		public override short ReadInt16()
		{
			return ReadInt16(Endianness);
		}

		public short ReadInt16(Endian endianness)
		{
			if (endianness == NativeEndianness)
			{
				return base.ReadInt16();
			}
			else
			{
				return base.ReadInt16().Reverse();
			}
		}

		public override ushort ReadUInt16()
		{
			return ReadUInt16(Endianness);
		}

		public ushort ReadUInt16(Endian endianness)
		{
			if (endianness == NativeEndianness)
			{
				return base.ReadUInt16();
			}
			else
			{
				return base.ReadUInt16().Reverse();
			}
		}

		public override int ReadInt32()
		{
			return ReadInt32(Endianness);
		}

		public int ReadInt32(Endian endianness)
		{
			if (endianness == NativeEndianness)
			{
				return base.ReadInt32();
			}
			else
			{
				return base.ReadInt32().Reverse();
			}
		}

		public override uint ReadUInt32()
		{
			return ReadUInt32(Endianness);
		}

		public uint ReadUInt32(Endian endianness)
		{
			if (endianness == NativeEndianness)
			{
				return base.ReadUInt32();
			}
			else
			{
				return base.ReadUInt32().Reverse();
			}
		}

		public override long ReadInt64()
		{
			return ReadInt64(Endianness);
		}

		public long ReadInt64(Endian endianness)
		{
			if (endianness == NativeEndianness)
			{
				return base.ReadInt64();
			}
			else
			{
				return base.ReadInt64().Reverse();
			}
		}

		public override ulong ReadUInt64()
		{
			return ReadUInt64(Endianness);
		}

		public ulong ReadUInt64(Endian endianness)
		{
			if (endianness == NativeEndianness)
			{
				return base.ReadUInt64();
			}
			else
			{
				return base.ReadUInt64().Reverse();
			}
		}
		//
		public uint tmp;
		public uint ReadOffset()
		{
			if (Endianness == NativeEndianness)
			{
				tmp = base.ReadUInt32();
				if (tmp == 0) return 0;
				else if (((tmp >> 28 != 5) && (tmp >> 28 != 6))) return 0;
				else if (tmp >> 28 == 5) return tmp & 0x0FFFFFFF;
				else return (tmp & 0x0FFFFFFF) + (uint)(FlagInfo.BaseResourceSizeV);
			}
			else
			{
				tmp = base.ReadUInt32().Reverse();
				if (tmp == 0) return 0;
				else if (((tmp >> 28 != 5) && (tmp >> 28 != 6))) return 0;
				else if (tmp >> 28 == 5) return tmp & 0x0FFFFFFF;
				else return (tmp & 0x0FFFFFFF) + (uint)(FlagInfo.BaseResourceSizeV);
			}
		}
		//public RageResource.Collection ReadCollection()
		//{
		//	RageResource.Collection value;
		//	value.m_pList = ReadOffset();
		//	value.m_wCount = ReadUInt16();
		//	value.m_wSize = ReadUInt16();
		//	return value;
		//}
		public Vector4 ReadVector4()
		{
			Vector4 value;
			if (Endianness == NativeEndianness)
			{
				value.X = base.ReadSingle();
				value.Y = base.ReadSingle();
				value.Z = base.ReadSingle();
				value.W = base.ReadSingle();
			}
			else
			{
				value.X = BitConverter.ToSingle(BitConverter.GetBytes(base.ReadUInt32().Reverse()), 0);
				value.Y = BitConverter.ToSingle(BitConverter.GetBytes(base.ReadUInt32().Reverse()), 0);
				value.Z = BitConverter.ToSingle(BitConverter.GetBytes(base.ReadUInt32().Reverse()), 0);
				value.W = BitConverter.ToSingle(BitConverter.GetBytes(base.ReadUInt32().Reverse()), 0);
			}
			return value;
		}
		internal RageResource.Collection ReadCollections()
		{
			RageResource.Collection value;
			if (Endianness == NativeEndianness)
			{
				value.m_pList = base.ReadUInt32();
				if (value.m_pList == 0) value.m_pList = 0;
				else if (((value.m_pList >> 28 != 5) && (value.m_pList >> 28 != 6))) value.m_pList = 0;
				else if (value.m_pList >> 28 == 5) value.m_pList = value.m_pList & 0x0FFFFFFF;
				else value.m_pList = (value.m_pList & 0x0FFFFFFF) + (uint)(FlagInfo.BaseResourceSizeV);
				value.m_wCount = base.ReadUInt16();
				value.m_wSize = base.ReadUInt16();
			}
			else
			{
				value.m_pList = base.ReadUInt32().Reverse();
				if (value.m_pList == 0) value.m_pList = 0;
				else if (((value.m_pList >> 28 != 5) && (value.m_pList >> 28 != 6))) value.m_pList = 0;
				else if (value.m_pList >> 28 == 5) value.m_pList = value.m_pList & 0x0FFFFFFF;
				else value.m_pList = (value.m_pList & 0x0FFFFFFF) + (uint)(FlagInfo.BaseResourceSizeV);
				value.m_wCount = base.ReadUInt16().Reverse();
				value.m_wSize = base.ReadUInt16().Reverse();
			}
			return value;
		}

	}

	public static class EBRExtensions
	{
		public static short Reverse(this short value)
		{
			return (short)(
				((value & 0xFF00) >> 8) << 0 |
				((value & 0x00FF) >> 0) << 8);
		}

		public static ushort Reverse(this ushort value)
		{
			return (ushort)(
				((value & 0xFF00) >> 8) << 0 |
				((value & 0x00FF) >> 0) << 8);
		}

		public static int Reverse(this int value)
		{
			return (int)(
				(((uint)value & 0xFF000000) >> 24) << 0 |
				(((uint)value & 0x00FF0000) >> 16) << 8 |
				(((uint)value & 0x0000FF00) >> 8) << 16 |
				(((uint)value & 0x000000FF) >> 0) << 24);
		}

		public static uint Reverse(this uint value)
		{
			return (uint)(
				((value & 0xFF000000) >> 24) << 0 |
				((value & 0x00FF0000) >> 16) << 8 |
				((value & 0x0000FF00) >> 8) << 16 |
				((value & 0x000000FF) >> 0) << 24);
		}

		public static long Reverse(this long value)
		{
			return (long)(
				(((ulong)value & 0xFF00000000000000UL) >> 56) << 0 |
				(((ulong)value & 0x00FF000000000000UL) >> 48) << 8 |
				(((ulong)value & 0x0000FF0000000000UL) >> 40) << 16 |
				(((ulong)value & 0x000000FF00000000UL) >> 32) << 24 |
				(((ulong)value & 0x00000000FF000000UL) >> 24) << 32 |
				(((ulong)value & 0x0000000000FF0000UL) >> 16) << 40 |
				(((ulong)value & 0x000000000000FF00UL) >> 8) << 48 |
				(((ulong)value & 0x00000000000000FFUL) >> 0) << 56);
		}

		public static ulong Reverse(this ulong value)
		{
			return (ulong)(
				((value & 0xFF00000000000000UL) >> 56) << 0 |
				((value & 0x00FF000000000000UL) >> 48) << 8 |
				((value & 0x0000FF0000000000UL) >> 40) << 16 |
				((value & 0x000000FF00000000UL) >> 32) << 24 |
				((value & 0x00000000FF000000UL) >> 24) << 32 |
				((value & 0x0000000000FF0000UL) >> 16) << 40 |
				((value & 0x000000000000FF00UL) >> 8) << 48 |
				((value & 0x00000000000000FFUL) >> 0) << 56);
		}
		public static float Reverse(this float value)
		{
			return BitConverter.Int32BitsToSingle(Reverse(BitConverter.SingleToInt32Bits(value)));
		}
	}
}
