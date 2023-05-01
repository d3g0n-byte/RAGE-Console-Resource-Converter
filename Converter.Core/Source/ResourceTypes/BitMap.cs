using Converter.Core.Utils;
using System;

namespace Converter.Core.ResourceTypes
{
	// only for xbox
	public class BitMap
	{
		// _f0
		public uint cFlags;
		public uint cType;
		
		// _f4
		public uint nRefCount;
		
		// _f8
		public uint Fence;
		
		// _fc
		public uint ReadFence;
		
		// _f10
		public uint Identifier;
		
		// _f14
		public uint BaseFlush;
		
		// _f18
		public uint MipFlush;

		// _f1c
		public uint bTiled;
		public uint Pitch;
		public uint _f1c_1;
		public uint _f1c_2;
		public uint ClampZ;
		public uint ClampY;
		public uint ClampX;
		public uint SignW;
		public uint SignZ;
		public uint SignY;
		public uint SignX;
		public uint Type;

		// _f20
		public uint BaseAddress;
		public uint ClampPolicy;
		public uint Stacked;
		public uint RequestSize;
		public uint Endian;
		public uint DataFormat;
		
		// _f24
		public uint Size; // нет. Более точная картина этого значения в cmode
		
		// _f28
		public uint BorderSize;
		public uint _f28_1;
		public uint AnisoFilter;
		public uint MipFilter;
		public uint MinFilter;
		public uint MagFilter;
		public int ExpAdjust;
		public uint SwizzleW;
		public uint SwizzleZ;
		public uint SwizzleY;
		public uint SwizzleX;
		public uint NumFormat;
		
		// _f2c
		public int GradExpAdjustV;
		public int GradExpAdjustH;
		public int LODBias;
		public uint MinAnisoWalk;
		public uint MagAnisoWalk;
		public uint MaxMipLevel;
		public uint MinMipLevel;
		public uint VolMinFilter;
		public uint VolMagFilter;

		// _f30
		public uint MipAddress;
		public uint PackedMips;
		public uint Dimension;
		public int AnisoBias;
		public uint TriClamp;
		public uint ForceBCWToMax;
		public uint BorderColor;

		public static BitMap Read(EndianBinaryReader br)
		{
			uint _f0 = br.ReadUInt32();
			uint _f4 = br.ReadUInt32();
			uint _f8 = br.ReadUInt32();
			uint _fc = br.ReadUInt32();
			uint _f10 = br.ReadUInt32();
			uint _f14 = br.ReadUInt32();
			uint _f18 = br.ReadUInt32();
			uint _f1c = br.ReadUInt32();
			uint _f20 = br.ReadUInt32();
			uint _f24 = br.ReadUInt32();
			uint _f28 = br.ReadUInt32();
			uint _f2c = br.ReadUInt32();
			uint _f30 = br.ReadUInt32();

			BitMap bitMap = new BitMap
			{
				cFlags = _f0 >> 4,
				cType = (_f0 << 28) >> 28,

				nRefCount = _f4,
				Fence = _f8,
				ReadFence = _fc,
				Identifier = _f10,
				BaseFlush = _f14,
				MipFlush = _f18,

				bTiled = _f1c >> 31,
				Pitch = (_f1c << 3) >> 3 >> 20,
				_f1c_1 = (_f1c << 12) >> 12 >> 19,
				_f1c_2 = (_f1c << 13) >> 13 >> 17,
				ClampZ = (_f1c << 15) >> 15 >> 14,
				ClampY = (_f1c << 18) >> 18 >> 11,
				ClampX = (_f1c << 21) >> 21 >> 8,
				SignW = (_f1c << 24) >> 24 >> 6,
				SignZ = (_f1c << 26) >> 26 >> 4,
				SignY = (_f1c << 28) >> 28 >> 2,
				SignX = (_f1c << 30) >> 30,

				BaseAddress = _f20 >> 12,
				ClampPolicy = (_f20 << 20) >> 20 >> 11,
				Stacked = (_f20 << 21) >> 21 >> 10,
				RequestSize = (_f20 << 22) >> 22 >> 8,
				Endian = (_f20 << 24) >> 24 >> 6,
				DataFormat = (_f20 << 26) >> 26,

				Size = _f24,

				BorderSize = _f28 >> 31,
				_f28_1 = (_f28 << 1) >> 1 >> 28,
				AnisoFilter = (_f28 << 4) >> 4 >> 25,
				MipFilter = (_f28 << 7) >> 7 >> 23,
				MinFilter = (_f28 << 9) >> 9 >> 21,
				MagFilter = (_f28 << 11) >> 11 >> 19,
				ExpAdjust = Convert.ToInt32((_f28 << 13) >> 13 >> 13),
				SwizzleW = (_f28 << 19) >> 19 >> 10,
				SwizzleZ = (_f28 << 22) >> 2 >> 7,
				SwizzleY = (_f28 << 25) >> 25 >> 4,
				SwizzleX = (_f28 << 28) >> 28 >> 1,
				NumFormat = (_f28 << 31) >> 31,

				GradExpAdjustV = Convert.ToInt32(_f2c >> 27),
				GradExpAdjustH = Convert.ToInt32((_f2c << 5) >> 4 >> 22),
				LODBias = Convert.ToInt32((_f2c << 10) >> 10 >> 12),
				MinAnisoWalk = (_f2c << 20) >> 20 >> 11,
				MagAnisoWalk = (_f2c << 21) >> 21 >> 10,
				MaxMipLevel = (_f2c << 22) >> 22 >> 6,
				MinMipLevel = (_f2c << 26) >> 26 >> 2,
				VolMinFilter = (_f2c << 30) >> 30 >> 1,
				VolMagFilter = (_f2c << 31) >> 31,

				MipAddress = _f30 >> 12,
				PackedMips = (_f30 << 20) >> 20 >> 11,
				Dimension = (_f30 << 21) >> 21 >> 9,
				AnisoBias = Convert.ToInt32((_f30 << 23) >> 23 >> 5),
				TriClamp = (_f30 << 27) >> 27 >> 3,
				ForceBCWToMax = (_f30 << 29) >> 29 >> 2,
				BorderColor = (_f30 << 30) >> 30,
			};

			if (bitMap.MipAddress >> 28 == 6)
			{
				bitMap.MipAddress += (uint)FlagInfo.BaseResourceSizeV;
			}

			bitMap.MipAddress &= 0x0fffffff;

			if (bitMap.BaseAddress >> 28 == 6)
			{
				bitMap.BaseAddress += (uint)FlagInfo.BaseResourceSizeV;
			}

			bitMap.MipAddress &= 0x0fffffff;
			
			return bitMap;
		}
	}
}
