
namespace Converter.Core.Utils
{
	// TODO: try to replace it with system BitArray class
	public static class BitUtils
	{
		public static bool Get(ushort b, int i)
		{
			return (b & (1 << i)) != 0;
		}

		public static bool Get(uint b, int i)
		{
			return (b & (1 << i)) != 0;
		}

		internal static bool Get(byte b, int i)
		{
			return (b & (1 << i)) != 0;
		}

		public static int Set(int val, int bit, bool trueORfalse)
		{
			bool flag = (val & (1 << bit)) != 0;
			
			if (trueORfalse)
			{
				if (!flag)
				{
					return val |= 1 << bit;
				}
			}
			else if (flag)
			{
				return val ^ (1 << bit);
			}

			return val;
		}
	}
}
