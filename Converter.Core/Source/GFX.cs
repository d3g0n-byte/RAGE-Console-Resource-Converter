using Converter.Core.ResourceTypes;
using Converter.Core.Utils;
using System.Numerics;
using System.Text;

namespace Converter.Core
{
	public static class GFX
	{
		public static bool ExportIndexBuffer(EndianBinaryReader br, StringBuilder outFile, uint pIndicesData, uint dwCount)
		{
			uint indexesCountPerLine = 0;
			br.Position = pIndicesData;
			outFile.Append("\t\t\t\t");
			ushort idx;

			for (uint i = 0; i < dwCount; i++)
			{
				idx = br.ReadUInt16();
				outFile.Append(idx);
				indexesCountPerLine++;

				if (indexesCountPerLine > 14 && i != dwCount - 1)
				{
					outFile.Append("\r\n\t\t\t\t");
					indexesCountPerLine = 0;
				}
				else if (i == (dwCount - 1))
				{
					outFile.AppendLine("");
				}
				else
				{
					outFile.Append(" ");
				}
			}

			return true;
		}

		public static bool ExportVertexBuffer(EndianBinaryReader br, StringBuilder outFile, VertexDeclaration decl, uint pVertexData, uint wCount, bool skinned, uint boneCount)
		{
			float[] PositionBuffer = new float[3];
			float[] BlendWeightBuffer = new float[4];
			uint[] BlendIndicesBuffer = new uint[4];
			Vector4 NormalBuffer = new Vector4();
			uint[] ColorBuffer = new uint[4];
			uint[] SpecularBuffer = new uint[4];
			float[] TexCoord0Buffer = new float[2];
			float[] TexCoord1Buffer = new float[2];
			float[] TexCoord2Buffer = new float[2];
			float[] TexCoord3Buffer = new float[2];
			float[] TexCoord4Buffer = new float[2];
			float[] TexCoord5Buffer = new float[2];
			float[] TexCoord6Buffer = new float[2];
			float[] TexCoord7Buffer = new float[2];
			Vector4 TangentBuffer = new Vector4();
			Vector4 BinormalBuffer = new Vector4();

			br.Position = pVertexData;

			for (ushort k = 0; k < wCount; k++)
			{
				uint currentByte = 0;

				if (decl.m_UsedElements.m_bPosition)
				{
					for (int a = 0; a < 3; a++)
					{
						PositionBuffer[a] = br.ReadSingle();
					}
					currentByte += 12;
				}

				if (decl.m_UsedElements.m_bBlendWeight)
				{
					for (int a = 0; a < 4; a++)
					{
						BlendWeightBuffer[3 - a] = ((float)br.ReadByte()) / 255;
					}
					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bBlendIndices)
				{
					for (int a = 0; a < 4; a++)
					{
						BlendIndicesBuffer[3 - a] = br.ReadByte();
					}
					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bNormal)
				{
					NormalBuffer = DataUtils.DEC3NToVector4(br.ReadUInt32());
					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bColor)
				{
					for (int a = 0; a < 4; a++)
					{
						ColorBuffer[3 - a] = br.ReadByte();
					}
					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bSpecular)
				{
					for (int a = 0; a < 4; a++)
					{
						SpecularBuffer[3 - a] = br.ReadByte();
					}
					currentByte += 4;
				}

				/*
				1 - float16 is used in most RDR models
				5 - float
				11 - uint16 is used where UV coordinates are overscaled
				*/

				if (decl.m_UsedElements.m_bTexCoord0)
				{
					switch (decl.m_ElementTypes.m_nTexCoord0Type)
					{
						case 1:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord0Buffer[a] = (float)br.ReadHalf();
								}

								break;
							}

						case 14:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord0Buffer[a] = br.ReadUInt16() * (float)3.05185094e-005;
								}

								break;
							}

						//case 5:
						//	{
						//		for (int a = 0; a < 2; a++)
						//		{
						//			TexCoord0Buffer[a] = br.ReadSingle();
						//		}

						//		break;
						//	}
					}

					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bTexCoord1)
				{
					switch (decl.m_ElementTypes.m_nTexCoord1Type)
					{
						case 1:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = (float)br.ReadHalf();
								}

								break;
							}

						case 14:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = br.ReadUInt16() * (float)3.05185094e-005;
								}

								break;
							}

							//case 5:
							//	{
							//		for (int a = 0; a < 2; a++)
							//		{
							//			TexCoord1Buffer[a] = br.ReadSingle();
							//		}

							//		break;
							//	}
					}

					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bTexCoord2)
				{
					switch (decl.m_ElementTypes.m_nTexCoord2Type)
					{
						case 1:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = (float)br.ReadHalf();
								}

								break;
							}

						case 14:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = br.ReadUInt16() * (float)3.05185094e-005;
								}

								break;
							}

							//case 5:
							//	{
							//		for (int a = 0; a < 2; a++)
							//		{
							//			TexCoord1Buffer[a] = br.ReadSingle();
							//		}

							//		break;
							//	}
					}

					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bTexCoord3)
				{
					switch (decl.m_ElementTypes.m_nTexCoord3Type)
					{
						case 1:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = (float)br.ReadHalf();
								}

								break;
							}

						case 14:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = br.ReadUInt16() * (float)3.05185094e-005;
								}

								break;
							}

							//case 5:
							//	{
							//		for (int a = 0; a < 2; a++)
							//		{
							//			TexCoord1Buffer[a] = br.ReadSingle();
							//		}

							//		break;
							//	}
					}

					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bTexCoord4)
				{
					switch (decl.m_ElementTypes.m_nTexCoord4Type)
					{
						case 1:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = (float)br.ReadHalf();
								}

								break;
							}

						case 14:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = br.ReadUInt16() * (float)3.05185094e-005;
								}

								break;
							}

							//case 5:
							//	{
							//		for (int a = 0; a < 2; a++)
							//		{
							//			TexCoord1Buffer[a] = br.ReadSingle();
							//		}

							//		break;
							//	}
					}

					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bTexCoord5)
				{
					switch (decl.m_ElementTypes.m_nTexCoord5Type)
					{
						case 1:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = (float)br.ReadHalf();
								}

								break;
							}

						case 14:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = br.ReadUInt16() * (float)3.05185094e-005;
								}

								break;
							}

							//case 5:
							//	{
							//		for (int a = 0; a < 2; a++)
							//		{
							//			TexCoord1Buffer[a] = br.ReadSingle();
							//		}

							//		break;
							//	}
					}

					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bTexCoord6)
				{
					switch (decl.m_ElementTypes.m_nTexCoord6Type)
					{
						case 1:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = (float)br.ReadHalf();
								}

								break;
							}

						case 14:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = br.ReadUInt16() * (float)3.05185094e-005;
								}

								break;
							}

							//case 5:
							//	{
							//		for (int a = 0; a < 2; a++)
							//		{
							//			TexCoord1Buffer[a] = br.ReadSingle();
							//		}

							//		break;
							//	}
					}

					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bTexCoord7)
				{
					switch (decl.m_ElementTypes.m_nTexCoord7Type)
					{
						case 1:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = (float)br.ReadHalf();
								}

								break;
							}

						case 14:
							{
								for (int a = 0; a < 2; a++)
								{
									TexCoord1Buffer[a] = br.ReadUInt16() * (float)3.05185094e-005;
								}

								break;
							}

							//case 5:
							//	{
							//		for (int a = 0; a < 2; a++)
							//		{
							//			TexCoord1Buffer[a] = br.ReadSingle();
							//		}

							//		break;
							//	}
					}

					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bTangent)
				{
					TangentBuffer = DataUtils.DEC3NToVector4(br.ReadUInt32());
					currentByte += 4;
				}

				if (decl.m_UsedElements.m_bBinormal)
				{
					BinormalBuffer = DataUtils.DEC3NToVector4(br.ReadUInt32());
					currentByte += 4;
				}

				br.Position += decl.m_nTotaSize - currentByte;

				// write
				outFile.Append("\t\t\t\t");
				if (!skinned)
				{
					if (decl.m_UsedElements.m_bPosition)
					{
						if (!Settings.bSwapYAndZ || boneCount > 0)
						{
							outFile.Append($"{PositionBuffer[0]} {PositionBuffer[1]} {PositionBuffer[2]} / ");
						}
						else
						{
							outFile.Append($"{PositionBuffer[0] * -1} {PositionBuffer[2]} {PositionBuffer[1]} / ");
						}
					}
					else
					{
						outFile.Append("0 0 0 / ");
					}

					if (decl.m_UsedElements.m_bNormal) // outFile.Append($"{NormalBuffer.X} {NormalBuffer.Y} {NormalBuffer.Z} / ");
					{
						if (!Settings.bSwapYAndZ || boneCount > 0)
						{
							outFile.Append($"{NormalBuffer.X} {NormalBuffer.Y} {NormalBuffer.Z} / ");
						}
						else
						{
							outFile.Append($"{NormalBuffer.X} {NormalBuffer.Z} {NormalBuffer.Y} / ");
						}
					}
					else
					{
						outFile.Append("0 0 0 / ");
					}

					if (decl.m_UsedElements.m_bColor)
					{
						outFile.Append($"{ColorBuffer[0]} {ColorBuffer[1]} {ColorBuffer[2]} {ColorBuffer[3]} / ");
					}
					else
					{
						outFile.Append("255 255 255 255 / ");
					}

					if (decl.m_UsedElements.m_bTangent) //outFile.Append($"{TangentBuffer.X} {TangentBuffer.Y} {TangentBuffer.Z} {TangentBuffer.W} / ");
					{
						if (!Settings.bSwapYAndZ)
						{
							outFile.Append($"{TangentBuffer.X} {TangentBuffer.Y} {TangentBuffer.Z} {TangentBuffer.W} / ");
						}
						else
						{
							outFile.Append($"{TangentBuffer.X} {TangentBuffer.Z} {TangentBuffer.Y} {TangentBuffer.W} / ");
						}
					}
					else
					{
						outFile.Append("0 0 0 0 / ");
					}
					
					if (decl.m_UsedElements.m_bTexCoord0)
					{
						outFile.Append($"{TexCoord0Buffer[0]} {TexCoord0Buffer[1]} / ");
					}
					else
					{
						outFile.Append("0 0 / ");
					}

					if (decl.m_UsedElements.m_bTexCoord1)
					{
						outFile.Append($"{TexCoord1Buffer[0]} {TexCoord1Buffer[1]} / ");
					}
					else
					{
						outFile.Append("0 0 / ");
					}
					
					if (decl.m_UsedElements.m_bTexCoord2)
					{
						outFile.Append($"{TexCoord2Buffer[0]} {TexCoord2Buffer[1]} / ");
					}
					else
					{
						outFile.Append("0 0 / ");
					}

					if (decl.m_UsedElements.m_bTexCoord3)
					{
						outFile.Append($"{TexCoord3Buffer[0]} {TexCoord3Buffer[1]} / ");
					}
					else
					{
						outFile.Append("0 0 / ");
					}

					if (!decl.m_UsedElements.m_bSpecular)
					{
						if (decl.m_UsedElements.m_bTexCoord4)
						{
							outFile.Append($"{TexCoord4Buffer[0]} {TexCoord4Buffer[1]} / ");
						}
						else
						{
							outFile.Append("0 0 / ");
						}

						if (decl.m_UsedElements.m_bTexCoord5)
						{
							outFile.AppendLine($"{TexCoord5Buffer[0]} {TexCoord5Buffer[1]}");
						}
						else
						{
							outFile.AppendLine("0 0");
						}
					}
					else
					{
						outFile.AppendLine($"0 {SpecularBuffer[0] / 255} {SpecularBuffer[1] / 255} {SpecularBuffer[2] / 255}");
					}
				}
				else
				{
					if (decl.m_UsedElements.m_bPosition)
					{
						if (!Settings.bSwapYAndZ)
						{
							outFile.Append($"{PositionBuffer[0]} {PositionBuffer[1]} {PositionBuffer[2]} / ");
						}
						else
						{
							outFile.Append($"{PositionBuffer[0] * -1} {PositionBuffer[2]} {PositionBuffer[1]} / ");
						}
					}
					else
					{
						outFile.Append("0 0 0 / ");
					}

					if (decl.m_UsedElements.m_bNormal)
					{
						if (!Settings.bSwapYAndZ)
						{
							outFile.Append($"{NormalBuffer.X} {NormalBuffer.Y} {NormalBuffer.Z} / ");
						}
						else
						{
							outFile.Append($"{NormalBuffer.X} {NormalBuffer.Z} {NormalBuffer.Y} / ");
						}
					}
					else
					{
						outFile.Append("0 0 0 / ");
					}

					if (decl.m_UsedElements.m_bBlendWeight)
					{
						outFile.Append($"{BlendWeightBuffer[0]} {BlendWeightBuffer[1]} {BlendWeightBuffer[2]} {BlendWeightBuffer[3]} / ");
					}
					else
					{
						outFile.Append("0 0 0 0 / ");
					}

					if (decl.m_UsedElements.m_bBlendIndices)
					{
						outFile.Append($"{BlendIndicesBuffer[0]} {BlendIndicesBuffer[1]} {BlendIndicesBuffer[2]} {BlendIndicesBuffer[3]} / ");
					}
					else
					{
						outFile.Append("0 0 0 0 / ");
					}

					if (decl.m_UsedElements.m_bColor)
					{
						outFile.Append($"{ColorBuffer[0]} {ColorBuffer[1]} {ColorBuffer[2]} {ColorBuffer[3]} / ");
					}
					else
					{
						outFile.Append("255 255 255 255 / ");
					}

					if (decl.m_UsedElements.m_bTangent)
					{
						if (!Settings.bSwapYAndZ || boneCount < 1)
						{
							outFile.Append($"{TangentBuffer.X} {TangentBuffer.Y} {TangentBuffer.Z} {TangentBuffer.W} / ");
						}
						else
						{
							outFile.Append($"{TangentBuffer.X} {TangentBuffer.Z} {TangentBuffer.Y} {TangentBuffer.W} / ");
						}
					}
					else
					{
						outFile.Append("0 0 0 0 / ");
					}

					if (decl.m_UsedElements.m_bTexCoord0)
					{
						outFile.Append($"{TexCoord0Buffer[0]} {TexCoord0Buffer[1]} / ");
					}
					else
					{
						outFile.Append("0 0 / ");
					}

					if (decl.m_UsedElements.m_bTexCoord1)
					{
						outFile.AppendLine($"{TexCoord1Buffer[0]} {TexCoord1Buffer[1]}");
					}
					else
					{
						outFile.AppendLine("0 0");
					}
				}
			}

			return true;
		}
	}
}
