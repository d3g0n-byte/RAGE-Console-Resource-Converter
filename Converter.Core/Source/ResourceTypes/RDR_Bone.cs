using System.Numerics;
using Converter.Core.Utils;

namespace Converter.Core.ResourceTypes
{
	public class RDR_Bone
	{
		public uint m_pName;
		public uint m_dwFlags;
		public RDR_Bone m_pParallelOnHierarchy; // next sibling
		public RDR_Bone m_pNextOnHierarchy; // first child
		public RDR_Bone m_pPastOnHierarchy; // parent
		public ushort m_wBoneIndex;
		public ushort m_wBoneId;
		public ushort m_wMirror;
		public sbyte _f1A;
		public sbyte _f1B;
		public sbyte _f1C;
		public sbyte[] __pad_1D;
		public Vector4 m_vOffset;
		public Vector4 m_vRotationEuler;
		public Vector4 m_vRotationQuaternion;
		public Vector4 m_vScale;
		public Vector4 m_vWorldOffset;
		public Vector4 m_vOrient;
		public Vector4 m_vSorient;
		public Vector4 m_vTransMin;
		public Vector4 m_vTransMax;
		public Vector4 m_vRotMin;
		public Vector4 m_vRotMax;
		public int _fD0;
		public int _fD4;
		public int _fD8;
		public int _fDC;
		public string[] flagsAsString;

		public static RDR_Bone Read(EndianBinaryReader br, uint p, int prevIndex, int currentBoneIndex, uint rootBonePos, ref ushort numBones, uint[] ChildCountBuffer, ref short lastHighBoneIndex, bool isPrev = false, uint pSelf = 0, bool findMissingBone = false)
		{
			if (!isPrev)
			{
				currentBoneIndex++;
			}

			uint nextSubling;
			uint first;
			uint parrent;

			uint oldpos = (uint)br.Position;
			br.Position = p;

			RDR_Bone bone = new RDR_Bone
			{
				m_pName = br.ReadOffset(),
				m_dwFlags = br.ReadUInt32()
			};

			nextSubling = br.ReadOffset();
			first = br.ReadOffset();
			parrent = br.ReadOffset();
			bone.m_wBoneIndex = br.ReadUInt16();
			bone.m_wBoneId = br.ReadUInt16();
			bone.m_wMirror = br.ReadUInt16();
			bone._f1A = br.ReadSByte();
			bone._f1B = br.ReadSByte();
			bone._f1C = br.ReadSByte();
			bone.__pad_1D = new sbyte[3];

			for (int i = 0; i < 3; i++)
			{
				bone.__pad_1D[i] = br.ReadSByte();
			}

			bone.m_vOffset = br.ReadVector4();
			bone.m_vRotationEuler = br.ReadVector4();
			bone.m_vRotationQuaternion = br.ReadVector4();
			bone.m_vScale = br.ReadVector4();
			bone.m_vWorldOffset = br.ReadVector4();
			bone.m_vOrient = br.ReadVector4();
			bone.m_vSorient = br.ReadVector4();
			bone.m_vTransMin = br.ReadVector4();
			bone.m_vTransMax = br.ReadVector4();
			bone.m_vRotMin = br.ReadVector4();
			bone.m_vRotMax = br.ReadVector4();
			bone._fD0 = br.ReadInt32();
			bone._fD4 = br.ReadInt32();
			bone._fD8 = br.ReadInt32();
			bone._fDC = br.ReadInt32();
			uint flagsCount = 0;

			for (int i = 0; i < 32; i++)
			{
				if (BitUtils.Get(bone.m_dwFlags, i))
				{
					flagsCount++;
				}
			}

			bone.flagsAsString = new string[flagsCount];
			uint currentFlags = 0;

			for (int i = 0; i < 32; i++)
			{
				string flag;
				switch (i)
				{
					case 0:
						flag = "LockRotXYZ";
						break;

					case 1:
						flag = "LockRotX";
						break;

					case 2:
						flag = "LockRotY";
						break;

					case 3:
						flag = "LockRotZ";
						break;

					case 4:
						flag = "LimitRotX";
						break;

					case 5:
						flag = "LimitRotY";
						break;

					case 6:
						flag = "LimitRotZ";
						break;

					case 7:
						flag = "LockTransX";
						break;

					case 8:
						flag = "LockTransY";
						break;

					case 9:
						flag = "LockTransZ";
						break;

					case 10:
						flag = "LimitTransX";
						break;

					case 11:
						flag = "LimitTransY";
						break;

					case 12:
						flag = "LimitTransZ";
						break;

					case 13:
						flag = "LockScaleX";
						break;

					case 14:
						flag = "LockScaleY";
						break;

					case 15:
						flag = "LockScaleZ";
						break;

					case 16:
						flag = "LimitScaleX";
						break;

					case 17:
						flag = "LimitScaleY";
						break;

					case 18:
						flag = "LimitScaleZ";
						break;

					case 19:
						flag = "Invisible";
						break;

					default:
						flag = $"UnknownFlag{i}";
						break;
				}

				if (BitUtils.Get(bone.m_dwFlags, i))
				{
					bone.flagsAsString[currentFlags++] = flag;
				}
			}

			if (!isPrev)
			{
				if (prevIndex + 1 != bone.m_wBoneIndex)
				{
					return new RDR_Bone();
				}
			}

			if (bone.m_wBoneIndex > lastHighBoneIndex && !findMissingBone)
			{
				lastHighBoneIndex = (short)bone.m_wBoneIndex;
			}

			if (findMissingBone)
			{
				if (bone.m_wBoneIndex == lastHighBoneIndex)
				{
					if (first == 0 && ChildCountBuffer[bone.m_wBoneIndex] > 0)
					{
						ushort index = bone.m_wBoneIndex;
						uint currentBonePos = (uint)br.Position - 0xe0;
						ushort nextIndex = (ushort)(index + 1);
						uint value = (uint)(nextIndex << 16) + 771;
						br.Position = 0;

						while (br.Position < FlagInfo.BaseResourceSizeV)
						{
							br.Position += 8;
							uint tmpValue = br.ReadUInt32();

							if (tmpValue == value)
							{
								br.Position -= 12;

								if (br.ReadOffset() == currentBonePos)
								{
									uint missingBonePos = (uint)br.Position - 0x14;
									first = missingBonePos;
								}
								else
								{
									uint missingBonePos = (uint)br.Position - 0x14;
									nextSubling = missingBonePos;
								}

								break;
							}
							else
							{
								br.Position += 4;
							}
						}
					}
				}
			}

			if (first != 0)
			{
				bone.m_pNextOnHierarchy = Read(br, first, currentBoneIndex, currentBoneIndex, rootBonePos, ref numBones, ChildCountBuffer, ref lastHighBoneIndex, findMissingBone: findMissingBone);
			}
			else
			{
				bone.m_pNextOnHierarchy = new RDR_Bone();
			}

			if (nextSubling != 0)
			{
				bone.m_pParallelOnHierarchy = Read(br, nextSubling, currentBoneIndex, currentBoneIndex, rootBonePos, ref numBones, ChildCountBuffer, ref lastHighBoneIndex, findMissingBone: findMissingBone);
			}
			else
			{
				bone.m_pParallelOnHierarchy = new RDR_Bone();
			}

			if (parrent != 0 && parrent != rootBonePos)
			{
				bone.m_pPastOnHierarchy = Read(br, parrent, currentBoneIndex, currentBoneIndex, rootBonePos, ref numBones, ChildCountBuffer, ref lastHighBoneIndex, isPrev: true, findMissingBone: findMissingBone);
			}
			else
			{
				bone.m_pPastOnHierarchy = new RDR_Bone();
			}

			br.Position = oldpos;
			if (!isPrev)
			{
				numBones++;
			}

			return bone;
		}
	}
}
