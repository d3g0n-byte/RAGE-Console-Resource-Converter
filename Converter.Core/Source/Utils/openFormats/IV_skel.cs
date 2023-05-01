using System;
using System.Text;
using System.IO;
using Converter.Core.ResourceTypes;

namespace Converter.Core.Utils.openFormats
{
	public static class IV_skel
	{
		// roll (x), pitch (Y), yaw (z)
		static void ToQuaternion(float roll, float pitch, float yaw, ref float valX, ref float valY, ref float valZ, ref float valW)
		{
			// Abbreviations for the various angular functions
			
			float cr = Convert.ToSingle(Math.Cos(roll * 0.5));
			float sr = Convert.ToSingle(Math.Sin(roll * 0.5));
			float cp = Convert.ToSingle(Math.Cos(pitch * 0.5));
			float sp = Convert.ToSingle(Math.Sin(pitch * 0.5));
			float cy = Convert.ToSingle(Math.Cos(yaw * 0.5));
			float sy = Convert.ToSingle(Math.Sin(yaw * 0.5));

			valX = sr * cp * cy - cr * sp * sy;
			valY = cr * sp * cy + sr * cp * sy;
			valZ = cr * cp * sy - sr * sp * cy;
			valW = cr * cp * cy + sr * sp * sy;
		}

		public class UniversalSkeletonData
		{
			public uint m_pBone;
			public uint m_pChildrenMapping;
			public ushort m_nBoneCount;
			public uint m_nFlags;
			public string[] flagsAsString;

			public static UniversalSkeletonData ConvertToUniversalSkeletonData(RDR_SkeletonData skelData)
			{
				return new UniversalSkeletonData
				{
					m_pBone = skelData.m_pBone,
					m_pChildrenMapping = skelData.m_pChildrenMapping,
					m_nBoneCount = skelData.m_nBoneCount,
					m_nFlags = skelData.m_nFlags,
					flagsAsString = skelData.flagsAsString
				};
			}

			public static UniversalSkeletonData ConvertToUniversalSkeletonData(IV_SkeletonData skelData)
			{
				return new UniversalSkeletonData
				{
					m_pBone = skelData.m_pBone,
					m_pChildrenMapping = skelData.m_pChildrenMapping,
					m_nBoneCount = skelData.m_nBoneCount,
					m_nFlags = skelData.m_nFlags,
					flagsAsString = skelData.flagsAsString
				};
			}
		}

		// 0 - rdr
		// 1 - iv & mcla
		public static void Build(EndianBinaryReader br, uint m_pSkeleton, string skelFileName, UniversalSkeletonData skelData)
		{
			StringBuilder sbOutFileSkel = new StringBuilder();
			uint[] ChildCountBuffer = new uint[255];
			uint[] MappingBuffer = new uint[255];
			br.Position = skelData.m_pChildrenMapping;

			for (int b = 0; b < skelData.m_nBoneCount; b++)
			{
				MappingBuffer[b] = br.ReadUInt32();
			}

			for (int b = 0; b < skelData.m_nBoneCount; b++)
			{
				for (int c = 0; c < skelData.m_nBoneCount; c++)
				{
					if (MappingBuffer[c] == b)
					{
						ChildCountBuffer[b] += 1;
					}
				}
			}

			ChildCountBuffer[0] -= 1;

			short lastHighBoneIndex = -1;
			if (Main.useVerboseMode || Main.useVeryVerboseMode)
			{
				Console.WriteLine($"[INFO] Bones count: {skelData.m_nBoneCount}");
			}

			sbOutFileSkel.AppendLine("Version 107 11");
			ushort numBones = 0;
			RDR_Bone bone = RDR_Bone.Read(br, skelData.m_pBone, -1, -1, skelData.m_pBone, ref numBones, ChildCountBuffer, ref lastHighBoneIndex);

			// try to read bones in resources with incorrect count
			if (numBones != skelData.m_nBoneCount)
			{
				if (Main.useVerboseMode || Main.useVeryVerboseMode)
				{
					Console.WriteLine("[WARNING] Wrong bones count detected, trying to find missing bones...");
				}

				numBones = 0;
				bone = RDR_Bone.Read(br, skelData.m_pBone, -1, -1, skelData.m_pBone, ref numBones, ChildCountBuffer, ref lastHighBoneIndex, findMissingBone: true);

				if (numBones == skelData.m_nBoneCount)
				{
					if (Main.useVerboseMode || Main.useVeryVerboseMode)
					{
						Console.WriteLine("[INFO] Bones was successfully found.");
					}
				}
				else
				{
					if (Main.useVerboseMode || Main.useVeryVerboseMode)
					{
						Console.WriteLine("[ERROR] Missing bones was not found.");
					}
					skelData.m_nBoneCount = numBones;

					if (Main.useVerboseMode || Main.useVeryVerboseMode)
					{
						Console.WriteLine("[INFO] New bones count: {skelData.m_nBoneCount}");
					}
					br.Position = skelData.m_pChildrenMapping;
				}
			}

			sbOutFileSkel.AppendLine($"NumBones {skelData.m_nBoneCount}");
			sbOutFileSkel.Append($"Flags");

			for (int c = 0; c < skelData.flagsAsString.Length; c++)
			{
				sbOutFileSkel.Append($" {skelData.flagsAsString[c]}");
			}

			sbOutFileSkel.AppendLine("");

			int tabsCount = 0;
			RDR_Bone currentBone = bone;

			if (Settings.bSwapYAndZ)
			{
				float tmp = currentBone.m_vOffset.Y;
				currentBone.m_vOffset.Y = currentBone.m_vOffset.Z;
				currentBone.m_vOffset.Z = tmp;

				tmp = currentBone.m_vRotationEuler.Z += 1.5708f * 2;
				currentBone.m_vRotationEuler.X += 1.5708f;

				ToQuaternion(
					currentBone.m_vRotationEuler.X, currentBone.m_vRotationEuler.Y, currentBone.m_vRotationEuler.Z,
					ref currentBone.m_vRotationQuaternion.X, ref currentBone.m_vRotationQuaternion.Y, ref currentBone.m_vRotationQuaternion.Z,
					ref currentBone.m_vRotationQuaternion.W
				);

				tmp = currentBone.m_vScale.Y;
				currentBone.m_vScale.Y = currentBone.m_vScale.Z;
				currentBone.m_vScale.Z = tmp;

				tmp = currentBone.m_vWorldOffset.Z;
				currentBone.m_vWorldOffset.Z = currentBone.m_vWorldOffset.Y;
				currentBone.m_vWorldOffset.Y = tmp;

				tmp = currentBone.m_vOrient.Y;
				currentBone.m_vOrient.Y = currentBone.m_vOrient.Z;
				currentBone.m_vOrient.Z = tmp;

				tmp = currentBone.m_vSorient.Y;
				currentBone.m_vSorient.Y = currentBone.m_vSorient.Z;
				currentBone.m_vSorient.Z = tmp;

				tmp = currentBone.m_vTransMin.Y;
				currentBone.m_vTransMin.Y = currentBone.m_vTransMin.Z;
				currentBone.m_vTransMin.Z = tmp;

				tmp = currentBone.m_vTransMax.Y;
				currentBone.m_vTransMax.Y = currentBone.m_vTransMax.Z;
				currentBone.m_vTransMax.Z = tmp;

				tmp = currentBone.m_vRotMin.Y;
				currentBone.m_vRotMin.Y = currentBone.m_vRotMin.Z;
				currentBone.m_vRotMin.Z = tmp;

				tmp = currentBone.m_vRotMax.Y;
				currentBone.m_vRotMax.Y = currentBone.m_vRotMax.Z;
				currentBone.m_vRotMax.Z = tmp;
			}

			for (int a = 0; a < skelData.m_nBoneCount; a++)
			{
				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"bone {DataUtils.ReadStringAtOffset(currentBone.m_pName, br)}");
				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"{{");
				tabsCount++;
				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.Append($"Flags");

				for (int c = 0; c < currentBone.flagsAsString.Length; c++)
				{
					sbOutFileSkel.Append($" {currentBone.flagsAsString[c]}");
				}

				sbOutFileSkel.AppendLine("");
				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"Index {currentBone.m_wBoneIndex}");
				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"Id {currentBone.m_wBoneId}");
				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"Mirror {currentBone.m_wMirror}");

				/*--==--*/

				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"LocalOffset {currentBone.m_vOffset.X} {currentBone.m_vOffset.Y} {currentBone.m_vOffset.Z}");

				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"RotationEuler {currentBone.m_vRotationEuler.X} {currentBone.m_vRotationEuler.Y} {currentBone.m_vRotationEuler.Z}");

				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"RotationQuaternion {currentBone.m_vRotationQuaternion.X} {currentBone.m_vRotationQuaternion.Y} {currentBone.m_vRotationQuaternion.Z} {currentBone.m_vRotationQuaternion.W}");

				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"Scale {currentBone.m_vScale.X} {currentBone.m_vScale.Y} {currentBone.m_vScale.Z}");

				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"WorldOffset {currentBone.m_vWorldOffset.X} {currentBone.m_vWorldOffset.Y} {currentBone.m_vWorldOffset.Z}");

				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"Orient {currentBone.m_vOrient.X} {currentBone.m_vOrient.Y} {currentBone.m_vOrient.Z}");

				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"Sorient {currentBone.m_vSorient.X} {currentBone.m_vSorient.Y} {currentBone.m_vSorient.Z}");

				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"TransMin {currentBone.m_vTransMin.X} {currentBone.m_vTransMin.Y} {currentBone.m_vTransMin.Z}");

				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"TransMax {currentBone.m_vTransMax.X} {currentBone.m_vTransMax.Y} {currentBone.m_vTransMax.Z}");

				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"RotMin {currentBone.m_vRotMin.X} {currentBone.m_vRotMin.Y} {currentBone.m_vRotMin.Z}");

				sbOutFileSkel.Append('\t', tabsCount);
				sbOutFileSkel.AppendLine($"RotMax {currentBone.m_vRotMax.X} {currentBone.m_vRotMax.Y} {currentBone.m_vRotMax.Z}");

				if (ChildCountBuffer[a] > 0)
				{
					sbOutFileSkel.Append('\t', tabsCount);
					sbOutFileSkel.AppendLine($"Children {ChildCountBuffer[a]}");
				}

				/*--===--*/
				if (currentBone.m_pNextOnHierarchy.m_wBoneId != 0 && currentBone.m_pNextOnHierarchy.m_wBoneIndex == a + 1)
				{
					sbOutFileSkel.Append('\t', tabsCount);
					sbOutFileSkel.AppendLine($"{{");
					tabsCount++;
					currentBone = currentBone.m_pNextOnHierarchy;
				}
				else if (currentBone.m_pParallelOnHierarchy.m_wBoneId != 0 && currentBone.m_pNextOnHierarchy.m_wBoneIndex == a + 1)
				{
					tabsCount--;
					if (tabsCount < 0)
					{
						break;
					}

					sbOutFileSkel.Append('\t', tabsCount);
					sbOutFileSkel.AppendLine($"}}");
					currentBone = currentBone.m_pParallelOnHierarchy;
				}
				else
				{
					for (int c = 0; c < 1; c++)
					{
						if (currentBone.m_pNextOnHierarchy.m_wBoneId != 0 && currentBone.m_pNextOnHierarchy.m_wBoneIndex == a + 1)
						{
							sbOutFileSkel.Append('\t', tabsCount);
							sbOutFileSkel.AppendLine($"{{");
							tabsCount++;
							currentBone = currentBone.m_pNextOnHierarchy;
						}
						else if (currentBone.m_pParallelOnHierarchy.m_wBoneId != 0 && currentBone.m_pParallelOnHierarchy.m_wBoneIndex == a + 1)
						{
							tabsCount--;

							if (tabsCount < 0)
							{
								break;
							}

							sbOutFileSkel.Append('\t', tabsCount);
							sbOutFileSkel.AppendLine($"}}");
							currentBone = currentBone.m_pParallelOnHierarchy;
						}
						else if (currentBone.m_pPastOnHierarchy.m_wBoneId != 0)
						{
							if (tabsCount < 0)
							{
								break;
							}

							currentBone = currentBone.m_pPastOnHierarchy;
							c--;

							tabsCount--;

							if (tabsCount < 0)
							{
								break;
							}

							sbOutFileSkel.Append('\t', tabsCount);
							sbOutFileSkel.AppendLine($"}}");

							tabsCount--;

							if (tabsCount < 0)
							{
								break;
							}

							sbOutFileSkel.Append('\t', tabsCount);
							sbOutFileSkel.AppendLine($"}}");

						}
						else
						{
							a += 0xffff;
							while (tabsCount > -1)
							{
								tabsCount--;

								if (tabsCount < 0)
								{
									break;
								}

								sbOutFileSkel.Append('\t', tabsCount);
								sbOutFileSkel.AppendLine($"}}");
							}
						}
					}
				}
			}

			using (FileStream outFileSkel = File.Create(skelFileName))
			{
				using (StreamWriter swOutFileSkel = new StreamWriter(outFileSkel))
				{
					swOutFileSkel.Write(sbOutFileSkel.ToString());
				}
			}

			sbOutFileSkel.Clear();
		}
	}
}
