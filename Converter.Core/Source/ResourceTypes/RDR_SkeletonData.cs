using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class RDR_SkeletonData
	{
		public uint m_pBone;
		public uint m_pChildrenMapping;
		public uint _f8;
		public uint _fc;
		public uint _f10; // pointer
		public uint _f14; // pointer
		public ushort m_nBoneCount;
		public ushort _f18; // size
		public ushort _f1a;
		public ushort _f1c;
		public uint m_nFlags; // flags
		public Collection m_cBoneIdMapping;
		public ushort m_nUsageCount;
		public uint _f30;
		public uint _f34;
		public uint _f38;
		public uint _f3c;
		public uint _f40;
		public string[] flagsAsString;
		// joint

		public static RDR_SkeletonData Read(EndianBinaryReader br)
		{
			RDR_SkeletonData skeletonData = new RDR_SkeletonData
			{
				m_pBone = br.ReadOffset(),
				m_pChildrenMapping = br.ReadOffset(),
				_f8 = br.ReadUInt32(),
				_fc = br.ReadUInt32(),
				_f10 = br.ReadUInt32(),
				_f14 = br.ReadUInt32(),
				m_nBoneCount = br.ReadUInt16(),
				_f18 = br.ReadUInt16(),
				_f1a = br.ReadUInt16(),
				_f1c = br.ReadUInt16(),
				m_nFlags = br.ReadUInt32(),
				m_cBoneIdMapping = Collection.Read(br),
				_f30 = br.ReadUInt32(),
				_f34 = br.ReadUInt32(),
				_f38 = br.ReadUInt32(),
				_f3c = br.ReadUInt32(),
				_f40 = br.ReadUInt32()
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
			string flag;

			for (int i = 0; i < 32; i++)
			{
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
