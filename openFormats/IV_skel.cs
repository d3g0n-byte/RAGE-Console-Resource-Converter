using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.openFormats
{
	internal class IV_skel
	{
		public static void Build(EndianBinaryReader br, uint m_pSkeleton, string skelFileName)
		{
			FileStream outFileSkel;
			StreamWriter swOutFileSkel;
			StringBuilder sbOutFileSkel = new StringBuilder();
			uint[] ChildCountBuffer = new uint[255];

			br.Position = m_pSkeleton;
			RageResource.RDRSkeletonData skelData;

			skelData = ReadRageResource.RDRSkeletonData(br);
			uint[] MappingBuffer = new uint[255];
			br.Position = skelData.m_pChildrenMapping;
			for (int b = 0; b < skelData.m_wBoneCount; b++) MappingBuffer[b] = br.ReadUInt32();
			for (int b = 0; b < skelData.m_wBoneCount; b++)
				for (int c = 0; c < skelData.m_wBoneCount; c++)
					if (MappingBuffer[c] == b) ChildCountBuffer[b] += 1;
			ChildCountBuffer[0] -= 1;

			sbOutFileSkel.AppendLine($"Version {107} {11}");
			sbOutFileSkel.AppendLine($"NumBones {skelData.m_wBoneCount}");
			sbOutFileSkel.Append($"Flags ");
			for (int c = 0; c < skelData.flagsAsString.Length; c++) sbOutFileSkel.Append($" {skelData.flagsAsString[c]}");
			sbOutFileSkel.AppendLine("");

			RageResource.RDRBone[] bone = new RageResource.RDRBone[skelData.m_wBoneCount];

			uint currentOffsetToBone = skelData.m_pBone;
			int tabsCount = 0;
			for (int b = 0; b < skelData.m_wBoneCount; b++)
			{
				br.Position = currentOffsetToBone;
				bone[b] = ReadRageResource.RDRBone(br);

				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"bone {DataUtils.ReadStringAtOffset(bone[b].m_pName, br)}");
				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"{{");
				tabsCount++;//
				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.Append($"Flags");
				for (int c = 0; c < bone[b].flagsAsString.Length; c++) sbOutFileSkel.Append($" {bone[b].flagsAsString[c]}");
				sbOutFileSkel.AppendLine("");
				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"Index {bone[b].m_wBoneIndex}");
				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"Id {bone[b].m_wBoneId}");
				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"Mirror {bone[b].m_wMirror}");

				if (b == 0&& Settings.bSwapYAndZ)// root
				{
					float tmp = 0;
					bone[b].m_vRotationQuaternion.Y += 180f;
					bone[b].m_vRotationQuaternion.Z += 180f;
					tmp = bone[b].m_vOffset.Z;
					bone[b].m_vOffset.Z = bone[b].m_vOffset.Y;
					bone[b].m_vOffset.Y = tmp;
					tmp = bone[b].m_vWorldOffset.Z;
					bone[b].m_vWorldOffset.Z = bone[b].m_vWorldOffset.Y;
					bone[b].m_vWorldOffset.Y = tmp;
				}

				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"LocalOffset {bone[b].m_vOffset.X} {bone[b].m_vOffset.Y} {bone[b].m_vOffset.Z}");
				
				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"RotationEuler {bone[b].m_vRotationEuler.X} {bone[b].m_vRotationEuler.Y} {bone[b].m_vRotationEuler.Z}");

				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"RotationQuaternion {bone[b].m_vRotationQuaternion.X} {bone[b].m_vRotationQuaternion.Y} {bone[b].m_vRotationQuaternion.Z} {bone[b].m_vRotationQuaternion.W}");

				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"Scale {bone[b].m_vScale.X} {bone[b].m_vScale.Y} {bone[b].m_vScale.Z}");

				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"WorldOffset {bone[b].m_vWorldOffset.X} {bone[b].m_vWorldOffset.Y} {bone[b].m_vWorldOffset.Z}");

				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"Orient {bone[b].m_vOrient.X} {bone[b].m_vOrient.Y} {bone[b].m_vOrient.Z}");

				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"Sorient {bone[b].m_vSorient.X} {bone[b].m_vSorient.Y} {bone[b].m_vSorient.Z}");

				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"TransMin {bone[b].m_vTransMin.X} {bone[b].m_vTransMin.Y} {bone[b].m_vTransMin.Z}");

				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"TransMax {bone[b].m_vTransMax.X} {bone[b].m_vTransMax.Y} {bone[b].m_vTransMax.Z}");

				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"RotMin {bone[b].m_vRotMin.X} {bone[b].m_vRotMin.Y} {bone[b].m_vRotMin.Z}");

				for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
				sbOutFileSkel.AppendLine($"RotMax {bone[b].m_vRotMax.X} {bone[b].m_vRotMax.Y} {bone[b].m_vRotMax.Z}");

				if (ChildCountBuffer[b] > 0)
				{
					for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
					sbOutFileSkel.AppendLine($"Children  {ChildCountBuffer[b]}");
				}

				if (ChildCountBuffer[b] > 0)
				{
					for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
					sbOutFileSkel.AppendLine($"{{");
					if (bone[b].m_pFirstChild != 0)
					{
						tabsCount++;
						currentOffsetToBone = bone[b].m_pFirstChild;
					}
					else
					{
						currentOffsetToBone = bone[b].m_pNextSibling;
					}
				}
				else
				{
					if (bone[b].m_pNextSibling != 0)
					{
						tabsCount--;
						if (tabsCount < 0) break;
						for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
						sbOutFileSkel.AppendLine($"}}");

						currentOffsetToBone = bone[b].m_pNextSibling;
					}
					else
					{
						for (int c = 0; c < 1;)
						{
							tabsCount--;
							if (tabsCount < 0) break;
							for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
							sbOutFileSkel.AppendLine($"}}");
							tabsCount--;
							if (tabsCount < 0) break;
							for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
							sbOutFileSkel.AppendLine($"}}");

							br.Position = bone[b].m_pParent + 8;
							//skel.open(Globals::FilePath, ios::binary);
							//skel.seekg(ParentOffset);
							//ReadUInt32(skel);
							//ReadUInt32(skel);
							uint tempOffset = br.ReadOffset();
							//ReadUInt32(skel);
							br.Position = br.Position + 4;
							uint tempoffset2 = br.ReadOffset();
							//skel.close();
							if (tempOffset != 0)
							{
								currentOffsetToBone = tempOffset;
								c++;
								tabsCount--;
								if (tabsCount < 0) break;
								for (int d = 0; d < tabsCount; d++) sbOutFileSkel.Append($"\t");
								sbOutFileSkel.AppendLine($"}}");
							}
							else if (tempoffset2 != 0)
							{
								if (tabsCount < 0) break;
								bone[b].m_pParent = tempoffset2;
							}
							else c--;
						}
					}
				}
			}
			outFileSkel = System.IO.File.Create(skelFileName);
			swOutFileSkel = new StreamWriter(outFileSkel);
			swOutFileSkel.Write(sbOutFileSkel.ToString());
			swOutFileSkel.Close();
			sbOutFileSkel.Clear();
		}

	}
}
