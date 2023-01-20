using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
	public static class ResourceUtils
	{
		public class FlagInfo
		{
			public const uint RSC05Magic = 88298322u;

			public const uint RSC06Magic = 105075538u;

			public const uint RSC85Magic = 2235781970u;

			public const uint RSC86Magic = 2252559186u;

			public const uint MaxPageSize = 524288;

			public static int Flag1 { get; set; }

			public static int Flag2 { get; set; }


			public static int RSC05_VPage4
			{
				get
				{
					return Flag1 & 1;
				}
				set
				{
					Flag1 = (Flag1 & -2) | (value & 1);
				}
			}

			public static int RSC05_VPage3
			{
				get
				{
					return (Flag1 >> 1) & 1;
				}
				set
				{
					Flag1 = (Flag1 & -3) | ((value & 1) << 1);
				}
			}

			public static int RSC05_VPage2
			{
				get
				{
					return (Flag1 >> 2) & 1;
				}
				set
				{
					Flag1 = (Flag1 & -5) | ((value & 1) << 2);
				}
			}

			public static int RSC05_VPage1
			{
				get
				{
					return (Flag1 >> 3) & 1;
				}
				set
				{
					Flag1 = (Flag1 & -9) | ((value & 1) << 3);
				}
			}

			public static int RSC05_VPage0
			{
				get
				{
					return (Flag1 >> 4) & 0x7F;
				}
				set
				{
					Flag1 = (Flag1 & -2033) | ((value & 0x7F) << 4);
				}
			}

			public static int RSC05_VSize
			{
				get
				{
					return (Flag1 >> 11) & 0xF;
				}
				set
				{
					Flag1 = (Flag1 & -30721) | ((value & 0xF) << 11);
				}
			}

			public static int RSC05_PPage4
			{
				get
				{
					return (Flag1 >> 15) & 1;
				}
				set
				{
					Flag1 = (Flag1 & -32769) | ((value & 1) << 15);
				}
			}

			public static int RSC05_PPage3
			{
				get
				{
					return (Flag1 >> 16) & 1;
				}
				set
				{
					Flag1 = (Flag1 & -65537) | ((value & 1) << 16);
				}
			}

			public static int RSC05_PPage2
			{
				get
				{
					return (Flag1 >> 17) & 1;
				}
				set
				{
					Flag1 = (Flag1 & -131073) | ((value & 1) << 17);
				}
			}

			public static int RSC05_PPage1
			{
				get
				{
					return (Flag1 >> 18) & 1;
				}
				set
				{
					Flag1 = (Flag1 & -262145) | ((value & 1) << 18);
				}
			}

			public static int RSC05_PPage0
			{
				get
				{
					return (Flag1 >> 19) & 0x7F;
				}
				set
				{
					Flag1 = (Flag1 & -66584577) | ((value & 0x7F) << 19);
				}
			}

			public static int RSC05_PSize
			{
				get
				{
					return (Flag1 >> 26) & 0xF;
				}
				set
				{
					Flag1 = (Flag1 & -1006632961) | ((value & 0xF) << 26);
				}
			}

			public static bool RSC05_Compressed
			{
				get
				{
					return ((Flag1 >> 30) & 1) == 1;
				}
				set
				{
					int num = (value ? 1 : 0);
					Flag1 = (Flag1 & -1073741825) | (num << 30);
				}
			}

			public static bool RSC05_Resource
			{
				get
				{
					return ((Flag1 >> 31) & 1) == 1;
				}
				set
				{
					int num = (value ? 1 : 0);
					Flag1 = (Flag1 & 0x7FFFFFFF) | (num << 31);
				}
			}
			public static int RSC05_VPageCount => RSC05_VPage4 + RSC05_VPage3 + RSC05_VPage2 + RSC05_VPage1 + RSC05_VPage0;
			public static int RSC05_PPageCount => RSC05_PPage4 + RSC05_PPage3 + RSC05_PPage2 + RSC05_PPage1 + RSC05_PPage0;
			public static int RSC05_GetTotalVSize => (Flag1 & 0x7FF) << RSC05_VSize + 8;
			public static int RSC05_GetTotalPSize => ((Flag1 >> 15) & 0x7FF) << RSC05_PSize + 8;
			public static int RSC05_GetSizeVPage0 => 4096 << RSC05_VSize;
			public static int RSC05_GetSizePPage0 => 4096 << RSC05_PSize;

			// rsc85
			public static int RSC85_StartPagePosition
			{
				get
				{
					return RSC85_ObjectStart;
					//int[] rSC85_PageSizesVitrual = ResourceUtils.FlagInfo.RSC85_PageSizesVitrual;
				}
			}

			public static bool RSC85_bResource
			{
				get
				{
					return (Flag1 & 0x80000000u) == 2147483648u;
				}
				set
				{
					Flag1 = DataUtils.SetBit(Flag1, 31, value);
				}
			}
			public static int RSC85_VPage0
			{
				get
				{
					return (Flag1 >> 14) & 3;
				}
				set
				{
					Flag1 = (Flag1 & -3145729) | ((value & 3) << 14);
				}
			}
			public static int RSC85_VPage1
			{
				get
				{
					return (Flag1 >> 8) & 0x3F;
				}
				set
				{
					Flag1 = (Flag1 & -16129) | ((value & 0x3F) << 8);
				}
			}
			public static int RSC85_VPage2
			{
				get
				{
					return Flag1 & 0xFF;
				}
				set
				{
					Flag1 = (Flag1 & -256) | (value & 0xFF);
				}
			}
			public static int RSC85_PPage0
			{
				get
				{
					return (Flag1 >> 28) & 7;
				}
				set
				{
					Flag1 = (Flag1 & -1879048193) | ((value & 7) << 28);
				}
			}
			public static int RSC85_PPage1
			{
				get
				{
					return (Flag1 >> 24) & 0xF;
				}
				set
				{
					Flag1 = (Flag1 & -251658241) | ((value & 0xF) << 24);
				}
			}
			public static int RSC85_PPage2
			{
				get
				{
					return (Flag1 >> 16) & 0xFF;
				}
				set
				{
					Flag1 = (Flag1 & -16711681) | ((value & 0xFF) << 16);
				}
			}
			public static bool RSC85_bUseExtendedSize
			{
				get
				{
					return (Flag2 & 0x80000000u) == 2147483648u;
				}
				set
				{
					Flag2 = (Flag2 & 0x7FFFFFFF) | ((value ? 1 : 0) << 31);
				}
			}
			public static int RSC85_ObjectStartPage
			{
				get
				{
					return (Flag2 >> 28) & 7;
				}
				set
				{
					Flag2 = (Flag2 & -1879048193) | ((value & 7) << 28);
				}
			}
			public static int RSC85_ObjectStartPageSize
			{
				get
				{
					return 4096 << RSC85_ObjectStartPage;
				}
				set
				{
					RSC85_ObjectStartPage = DataUtils.TrailingZeroes(value) - 12;
				}
			}
			public static int RSC85_TotalVSize
			{
				get
				{
					return (Flag2 & 0x3FFF) << 12;
				}
				set
				{
					Flag2 = (Flag2 & -16384) | ((value >> 12) & 0x3FFF);
				}
			}
			public static int RSC85_TotalPSize
			{
				get
				{
					return ((Flag2 >> 14) & 0x3FFF) << 12;
				}
				set
				{
					Flag2 = (Flag2 & -268419073) | (((value >> 12) & 0x3FFF) << 14);
				}
			}
			public static int[] RSC85_PageSizesVitrual
			{
				get
				{
					List<int> list = new List<int>();
					int num = -1;
					int num2 = 524288;
					int num3 = 0;
					int num4 = RSC85_TotalVSize;
					int rSC85_ObjectStartPage = RSC85_ObjectStartPage;
					int[] array = new int[4] { RSC85_VPage0, RSC85_VPage1, RSC85_VPage2, 2147483647 };
					for (int i = 0; i < 4; i++)
					{
						for (int j = 0; j < array[i]; j++)
						{
							if (num4 == 0)
								break;
							while (num2 > num4)
							{
								num2 >>= 1;
							}
							if (num2 == rSC85_ObjectStartPage && num == -1)
								num = num3;
							num3 += num2;
							list.Add(num2);
							num4 -= num2;
						}
						num2 >>= 1;
					}
					return list.ToArray();
				}
			}
			public static int[] RSC85_PageSizesPhysical
			{
				get
				{
					List<int> list = new List<int>();
					int num = -1;
					int num2 = 524288;
					int num3 = 0;
					int num4 = RSC85_TotalPSize;
					int rSC85_ObjectStartPage = RSC85_ObjectStartPage;
					int[] array = new int[4] { RSC85_PPage0, RSC85_PPage1, RSC85_PPage2, 2147483647 };
					for (int i = 0; i < 4; i++)
					{
						for (int j = 0; j < array[i]; j++)
						{
							if (num4 == 0)
								break;
							while (num2 > num4)
							{
								num2 >>= 1;
							}
							if (num2 == rSC85_ObjectStartPage && num == -1)
								num = num3;
							num3 += num2;
							list.Add(num2);
							num4 -= num2;
						}
						num2 >>= 1;
					}
					return list.ToArray();
				}
			}
			public static int RSC85_ObjectStart
			{
				get
				{
					int[] rSC85_PageSizesVitrual = RSC85_PageSizesVitrual;
					int num = 0;
					for (int i = 0; i < rSC85_PageSizesVitrual.Length; i++)
					{
						if (rSC85_PageSizesVitrual[i] == RSC85_ObjectStartPageSize)
							return num;
						num += rSC85_PageSizesVitrual[i];
					}
					return 0;
				}
			}
			public static bool IsResource //rsc5
			{
				get
				{
					return RSC05_Resource;
				}
				set
				{
					RSC05_Resource = value;
				}
			}
			public static bool IsExtendedFlags // rsc85
			{
				get
				{
					return RSC85_bUseExtendedSize;
				}
				set
				{
					RSC85_bUseExtendedSize = value;
				}
			}
			public static bool IsCompressed // rsc5
			{
				get
				{
					if (IsExtendedFlags)
						return false;
					return RSC05_Compressed;
				}
				set
				{
					if (!IsExtendedFlags)
						RSC05_Compressed = value;
				}
			}
			public static int BaseResourceSizeP
			{
				get
				{
					if (IsExtendedFlags)
						return RSC85_TotalPSize;
					return RSC05_GetTotalPSize;
				}
			}
			public static int BaseResourceSizeV
			{
				get
				{
					if (IsExtendedFlags)
						return RSC85_TotalVSize;
					return RSC05_GetTotalVSize;
				}
			}
			public static int ResourceStart
			{
				get
				{
					if (!IsResource || !IsExtendedFlags)
						return 0;
					return RSC85_ObjectStart;
				}
			}
			public static bool IsRSC85 => IsExtendedFlags;

			public static bool IsRSC05 => !IsExtendedFlags;

			public FlagInfo()
			{
			}

			public FlagInfo(int flag)
			{
				Flag1 = flag;
			}

			public static void RSC05_SetMemSizes(int vSize, int pSize)
			{
				Flag1 = RSC05_GenerateMemSizes(vSize, pSize);
			}

			public static int RSC05_GenerateMemSizes(int vSize, int pSize)
			{
				int num = vSize >> 8;
				int num2 = 0;
				while (num > 63)
				{
					if (((uint)num & (true ? 1u : 0u)) != 0)
						num += 2;
					num >>= 1;
					num2++;
				}
				int num3 = pSize >> 8;
				int num4 = 0;
				while (num3 > 63)
				{
					if (((uint)num3 & (true ? 1u : 0u)) != 0)
						num3 += 2;
					num3 >>= 1;
					num4++;
				}
				return num | (num2 << 11) | (num3 << 15) | (num4 << 26);
			}


			public FlagInfo(int flag1, int flag2)
			{
				Flag1 = flag1;
				Flag2 = flag2;
			}

			public static (int, int, int, int, int, int) RSC85_GenerateMemorySizes(int virt, int phys, int pageBaseSize = 4096)
			{
				int[] array = new int[6];
				int num = virt;
				int num2 = pageBaseSize << 7;
				int num3 = num2;
				for (int i = 0; i < 3; i++)
				{
					int num4 = num / (num3 >> i);
					num -= num4 * (num3 >> i);
					array[i] = num4;
				}
				num3 = num2;
				int num5 = phys;
				for (int j = 0; j < 3; j++)
				{
					int num6 = num5 / (num3 >> j);
					num5 -= num6 * (num3 >> j);
					array[j + 3] = num6;
				}
				return (array[0], array[1], array[2], array[3], array[4], array[5]);
			}


			public int[] RSC85_GetAvaliableObjectStartPage(int pageBaseSize = 4096)
			{
				List<int> list = new List<int>();
				int[] rSC85_PageSizesVitrual = RSC85_PageSizesVitrual;
				int num = 0;
				for (int i = 0; i < rSC85_PageSizesVitrual.Length; i++)
				{
					for (num = 0; num < 7; num++)
					{
						if (pageBaseSize << num == rSC85_PageSizesVitrual[i] && !list.Contains(num))
							list.Add(num);
					}
				}
				return list.ToArray();
			}

			public int GetTotalSize()
			{
				if (IsResource)
					return BaseResourceSizeP + BaseResourceSizeV;
				return Flag1 & Convert.ToInt32(3221225471);
			}



			public FlagInfo(uint flag1, uint flag2)
			{
				Flag1 = Convert.ToInt32(flag1);
				Flag2 = Convert.ToInt32(flag2);
			}

		}

		public class ResourceInfo
		{
			public static int version { get; set; }
			public static string GetResourceVersionStringFromIdent(uint ident)
			{
				switch (ident)
				{
					case 2235781970u:
						return "RSC85";
					case 2252559186u:
						return "RSC86";
					case 88298322u:
						return "RSC05";
					case 105075538u:
						return "RSC06";
					default:
						return "INVALID_RSC_IDENT";
				}
			}

		}
	}
}
