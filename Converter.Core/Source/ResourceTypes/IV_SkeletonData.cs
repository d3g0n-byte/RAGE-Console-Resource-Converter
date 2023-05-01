using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class IV_SkeletonData
	{
		public uint m_pBone;
		public uint m_pChildrenMapping;
		public uint _f8; // pointer
		public uint _fc; // pointer
		public uint _f10; // pointer
		public ushort m_nBoneCount;
		public short _f16;
		public short _f18;
		public short _f1A;
		public uint m_nFlags; // flags
		public Collection m_cBoneIdMapping;
		public short m_nUsageCount;
		public short _f2a;
		public uint _f2c;
		public uint _f30;
		public string[] flagsAsString;
		// joint

		public static IV_SkeletonData Read(EndianBinaryReader br)
		{
			IV_SkeletonData skeletonData = new IV_SkeletonData
			{
				m_pBone = br.ReadOffset(),
				m_pChildrenMapping = br.ReadOffset(),
				_f8 = br.ReadUInt32(),
				_fc = br.ReadUInt32(),
				_f10 = br.ReadUInt32(),
				m_nBoneCount = br.ReadUInt16(),
				_f16 = br.ReadInt16(),
				_f18 = br.ReadInt16(),
				_f1A = br.ReadInt16(),
				m_nFlags = br.ReadUInt32(), // flags
				m_cBoneIdMapping = Collection.Read(br),
				m_nUsageCount = br.ReadInt16(),
				_f2a = br.ReadInt16(),
				_f2c = br.ReadUInt32(),
				_f30 = br.ReadUInt32()
			};

			uint flagsCount = 0;

			for (int i = 0; i < 32; i++)
			{
				if (BitUtils.Get(skeletonData.m_nFlags, i))
				{
					flagsCount++;
				}
			}

			skeletonData.flagsAsString = new string[flagsCount];
			uint currentFlags = 0;

			for (int i = 0; i < 32; i++)
			{
				string flag;
				switch (i)
				{
					case 1:
						flag = "HaveBoneMappings";
						break;

					case 2:
						flag = "HaveBoneWorldOrient";
						break;

					case 3:
						flag = "AuthoredOrientation";
						break;

					default:
						flag = $"UnknownFlag{i}";
						break;
				}

				if (BitUtils.Get(skeletonData.m_nFlags, i))
				{
					skeletonData.flagsAsString[currentFlags++] = flag;
				}
			}

			return skeletonData;
		}
	}
}
