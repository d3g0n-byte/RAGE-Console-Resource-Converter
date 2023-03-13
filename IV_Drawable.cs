using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static Converter.IV_Drawable;
using static Converter.RageResource;


namespace Converter
{
	public class IV_Drawable
	{
		//public static uint currentPosInSysBuffer
		//{
		//	get
		//	{
		//		return currentPosInSysBuffer;
		//	}
		//	set
		//	{
		//		currentPosInSysBuffer = value;
		//	}
		//}
		//public uint Hash { get; set; }
		public static uint currentPosInSysBuffer = 0;
		public static uint currentPosInGfxBuffer = 0;
		public struct Matrix
		{
			public Vector4 m0, m1,m2,m3;
			//public float x0, y0, z0, w0, x1, y1, z1, w1, x2, y2, z2, w2, x3, y3, z3, w3;
		}
		public struct Shader
		{
			public string fxcName;
			public string spsName;
			public uint vertexFormat;
			public byte[] paramTypeBuffer;
			public byte drawBucket;
			public ushort index;
			public uint effectSize;
			public uint paramCount;
			public string[] stringValueBuffer;
			public Vector4[] vector4ValueBuffer;
			public Matrix[] matrixBuffer;
			public uint[] paramNameHashBuffer;

		}
		public static Matrix GetMatrix(string str)
		{
			Matrix v = new Matrix();
			string[] str2 = str.Split(';');
			v.m0.X = float.Parse(str2[0]);
			v.m0.Y = float.Parse(str2[1]);
			v.m0.Z = float.Parse(str2[2]);
			v.m0.W = float.Parse(str2[3]);
			v.m1.X = float.Parse(str2[4]);
			v.m1.Y = float.Parse(str2[5]);
			v.m1.Z = float.Parse(str2[6]);
			v.m1.W = float.Parse(str2[7]);
			v.m2.X = float.Parse(str2[8]);
			v.m2.Y = float.Parse(str2[9]);
			v.m2.Z = float.Parse(str2[10]);
			v.m2.W = float.Parse(str2[11]);
			v.m3.X = float.Parse(str2[12]);
			v.m3.Y = float.Parse(str2[13]);
			v.m3.Z = float.Parse(str2[14]);
			v.m3.W = float.Parse(str2[15]);

			return v;
		}
		public static Vector4 GetVector4(string str)
		{
			Vector4 v = new Vector4();
			string[] value = str.Split(';');
			v.X = float.Parse(value[0]);
			v.Y = float.Parse(value[1]);
			v.Z = float.Parse(value[2]);
			v.W = float.Parse(value[3]);
			return v;
		}
		public static void Align(uint value)
		{
			while (currentPosInSysBuffer % value != 0)
			{
				currentPosInSysBuffer += 1;
			}
		}
		public static void AlignGFX(uint value)
		{
			while (currentPosInGfxBuffer % value != 0)
			{
				currentPosInGfxBuffer += 1;
			}
		}
		public static void WriteValueToByteArray(ref byte[] array, string str, ref uint pos)
		{
			byte[] array2 = Encoding.ASCII.GetBytes(str);
			Array.Resize<byte>(ref array2, array2.Length+1);
			WriteToByteArray(ref array, array2, ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, uint value, ref uint pos)
		{
			WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, int value, ref uint pos)
		{
			WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, short value, ref uint pos)
		{
			WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, ushort value, ref uint pos)
		{
			WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, byte value, ref uint pos)
		{
			WriteToByteArray(ref array, new byte[] { value }, ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, sbyte value, ref uint pos)
		{
			WriteToByteArray(ref array, new byte[] { (byte)value }, ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, float value, ref uint pos)
		{
			WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, Vector4 value, ref uint pos)
		{
			WriteValueToByteArray(ref array, value.X, ref pos);
			WriteValueToByteArray(ref array, value.Y, ref pos);
			WriteValueToByteArray(ref array, value.Z, ref pos);
			WriteValueToByteArray(ref array, value.W, ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, Matrix value, ref uint pos)
		{
			WriteValueToByteArray(ref array, value.m0, ref pos);
			WriteValueToByteArray(ref array, value.m1, ref pos);
			WriteValueToByteArray(ref array, value.m2, ref pos);
			WriteValueToByteArray(ref array, value.m3, ref pos);
		}


		public static void WriteToByteArray(ref byte[] array, byte[] array2, ref uint pos)
		{
			for (int a = 0; a < array2.Length; a++)
				array[pos + a] = array2[a];
			pos += (uint)array2.Length;

		}


		/*		public static byte[] WriteToByteArray(byte[] array, string str){
					byte[] bytes = Encoding.ASCII.GetBytes(str);
					for (int a = 0; a < str.Length; a++)
					{
						array[currentPosInSysBuffer + a] = bytes[a];
					}
					array[currentPosInSysBuffer + str.Length] = 0;
					//			if (str.Length>0x1f) currentPosInSysBuffer += 0x30;
					//			else if (str.Length > 0x0f) currentPosInSysBuffer += 0x20;
					//			else currentPosInSysBuffer += 0x10;
					currentPosInSysBuffer += (uint)str.Length+1;
					//uint ff = (currentPosInSysBuffer % 4!=0);
					Align(4);
		//			currentPosInSysBuffer += (currentPosInSysBuffer % 4)*2; // выравниваем
					return array;
				}
				public static byte[] WriteToByteArray(byte[] array, byte[] array2)
				{
					for (int a = 0; a < array2.Length; a++)
					{
						array[currentPosInSysBuffer + a] = array2[a];
					}
					currentPosInSysBuffer += (uint)array2.Length;
					return array;
				}
				public static byte[] WriteToByteArray(byte[] array, uint value)
				{
					byte[] array2 = BitConverter.GetBytes(value);
					for (int a = 0; a < array2.Length; a++)
					{
						array[currentPosInSysBuffer + a] = array2[a];
					}
					currentPosInSysBuffer += (uint)array2.Length;
					return array;
				}
				public static byte[] WriteToByteArray(byte[] array, int value)
				{
					byte[] array2 = BitConverter.GetBytes(value);
					for (int a = 0; a < array2.Length; a++)
					{
						array[currentPosInSysBuffer + a] = array2[a];
					}
					currentPosInSysBuffer += (uint)array2.Length;
					return array;
				}
				public static byte[] WriteToByteArray(byte[] array, short value)
				{
					byte[] array2 = BitConverter.GetBytes(value);
					for (int a = 0; a < array2.Length; a++)
					{
						array[currentPosInSysBuffer + a] = array2[a];
					}
					currentPosInSysBuffer += (uint)array2.Length;
					return array;
				}
				public static byte[] WriteToByteArray(byte[] array, ushort value)
				{
					byte[] array2 = BitConverter.GetBytes(value);
					for (int a = 0; a < array2.Length; a++)
					{
						array[currentPosInSysBuffer + a] = array2[a];
					}
					currentPosInSysBuffer += (uint)array2.Length;
					return array;
				}
				public static byte[] WriteToByteArray(byte[] array, float value)
				{
					byte[] array2 = BitConverter.GetBytes(value);
					for (int a = 0; a < array2.Length; a++)
					{
						array[currentPosInSysBuffer + a] = array2[a];
					}
					currentPosInSysBuffer += (uint)array2.Length;
					return array;
				}
				public static byte[] WriteToByteArray(byte[] array, Vector4 value)
				{
					array = WriteToByteArray(array, value.X);
					array = WriteToByteArray(array, value.Y);
					array = WriteToByteArray(array, value.Z);
					array = WriteToByteArray(array, value.W);
					return array;
				}
				public static byte[] WriteToByteArray(byte[] array, Matrix value)
				{
					array = WriteToByteArray(array, value.m0);
					array = WriteToByteArray(array, value.m1);
					array = WriteToByteArray(array, value.m2);
					array = WriteToByteArray(array, value.m3);
					return array;
				}
				public static byte[] WriteToByteArray(byte[] array, sbyte value)
				{
					array[currentPosInSysBuffer] = (byte)value;
					currentPosInSysBuffer += 1;
					return array;
				}
				public static byte[] WriteToByteArray(byte[] array, byte value)
				{
					array[currentPosInSysBuffer] = value;
					currentPosInSysBuffer += 1;
					return array;
				}*/
		//		public static RageResource.IV_TextureDefinition 

		public static Shader GetShaderInfo(string shaderName)
		{
			Shader shader = new Shader();
			if (shaderName == "gta_normal_spec.sps")
			{
				shader.fxcName = "gta_normal_spec";
				shader.spsName = "gta_normal_spec.sps";
				shader.vertexFormat = 89;
				Array.Resize<byte>(ref shader.paramTypeBuffer, 10);

				shader.paramTypeBuffer[0] = 0;
				shader.paramTypeBuffer[1] = 1;
				shader.paramTypeBuffer[2] = 4;
				shader.paramTypeBuffer[3] = 1;
				shader.paramTypeBuffer[4] = 1;
				shader.paramTypeBuffer[5] = 1;
				shader.paramTypeBuffer[6] = 1;
				shader.paramTypeBuffer[7] = 1;
				shader.paramTypeBuffer[8] = 0;
				shader.paramTypeBuffer[9] = 0;

				shader.drawBucket = 0;
				shader.index = 3;
				shader.effectSize = 272;
				shader.paramCount = 10;
				Array.Resize<string>(ref shader.stringValueBuffer, 3);
				Array.Resize<Vector4>(ref shader.vector4ValueBuffer, 6);
				Array.Resize<Matrix>(ref shader.matrixBuffer, 1);
				Array.Resize<uint>(ref shader.paramNameHashBuffer, 10);
				shader.paramNameHashBuffer[0] = DataUtils.GetHash("texturesampler");
				shader.paramNameHashBuffer[1] = DataUtils.GetHash("shadowmap_res");
				shader.paramNameHashBuffer[2] = DataUtils.GetHash("facetmask");
				shader.paramNameHashBuffer[3] = DataUtils.GetHash("specularfactor");
				shader.paramNameHashBuffer[4] = DataUtils.GetHash("specularcolorfactor");
				shader.paramNameHashBuffer[5] = DataUtils.GetHash("specmapintmask");
				shader.paramNameHashBuffer[6] = DataUtils.GetHash("bumpiness");
				shader.paramNameHashBuffer[7] = DataUtils.GetHash("luminanceconstants");
				shader.paramNameHashBuffer[8] = DataUtils.GetHash("bumpsampler");
				shader.paramNameHashBuffer[9] = DataUtils.GetHash("specsampler");
			}

			return shader;
		}
		public static uint[] WriteShaderParams(byte[] array, Shader shader)
		{
			uint[] offsetBuffer= new uint[shader.paramCount];
			int currentString = 0;
			int currentVector4 = 0;
			int currentMatrix = 0;
			for (int a = 0; a < shader.paramCount; a++)
			{
				if (shader.paramTypeBuffer[a]==0)
				{
					RageResource.IV_TextureDefinition value = new IV_TextureDefinition();
					value.vtable = 7038812u;
					value._f4 = 0;
					value._f8 = 2;
					value._fA = 1;
					value._fC = 0;
					value._f10 = 0;
					value.m_pName = currentPosInSysBuffer+0x50000000;
					WriteValueToByteArray(ref array, shader.stringValueBuffer[currentString++], ref currentPosInSysBuffer);
					value._f18 = 0;
					Align(16);
					offsetBuffer[a] = currentPosInSysBuffer + 0x50000000;
					{// пишем эту секцию
						WriteValueToByteArray(ref array, value.vtable, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref array, value._f4, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref array, value._f8, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref array, value._fA, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref array, value._fC, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref array, value._f10, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref array, value.m_pName, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref array, value._f18, ref currentPosInSysBuffer);
						Align(16);
					}
				}
				else if (shader.paramTypeBuffer[a] == 1)
				{
					offsetBuffer[a] = currentPosInSysBuffer + 0x50000000;
					WriteValueToByteArray(ref array, shader.vector4ValueBuffer[currentVector4++], ref currentPosInSysBuffer);
				}
				else if (shader.paramTypeBuffer[a] == 4)
				{
					offsetBuffer[a] = currentPosInSysBuffer + 0x50000000;
					WriteValueToByteArray(ref array, shader.matrixBuffer[currentMatrix++], ref currentPosInSysBuffer);
				}
				else offsetBuffer[a] = 0;
			}
			return offsetBuffer;
		}
		public static string[] ReadLine(StreamReader sr)
		{
			string line = sr.ReadLine();
			//line.Trim('\t');
			line = line.Replace("\t", "");
			//			line = line.Replace('\t', ' ');
			string[] lineWords = line.Split(' ');

			return lineWords;
		}
		public struct Vertex
		{
			public float posX;
			public float posY;
			public float posZ;
			public float nrmX;
			public float nrmY;
			public float nrmZ;
			public byte colorX;
			public byte colorY;
			public byte colorZ;
			public byte colorW;
			public byte specularX;
			public byte specularY;
			public byte specularZ;
			public byte specularW;
			public float uv0X;
			public float uv0Y;
			public float uv1X;
			public float uv1Y;
			public float uv2X;
			public float uv2Y;
			public float uv3X;
			public float uv3Y;
			public float uv4X;
			public float uv4Y;
			public float uv5X;
			public float uv5Y;
			public float uv6X;
			public float uv6Y;
			public float uv7X;
			public float uv7Y;
			public float tangX;
			public float tangY;
			public float tangZ;
			public float tangW;
		}
		struct FakeVertexDeclaration
		{
			public uint vertexFormat;
			public byte totalSize;
			public sbyte _f6;
			public byte StoreNormalsDataFirst;
			public byte ElementsCount;
			public uint elementTypes1;
			public uint elementTypes2;
		}
		public static void Build()
		{	// тестовая функция. Она нужна для того, чтобы изучить сборку wdr файла 110 версии(iv)
			// сначала создаем два буфера по 32мб ибо мы не знаем какой будет размер
			byte[] sys = new byte[33554432];
			for (int a = 0; a < sys.Length; a++)sys[a] = 205; // временно
			byte[] gfx = new byte[33554432];
			for (int a = 0; a < gfx.Length; a++) gfx[a] = 205; // временно
			// начало wdr состоит из секции Drawable, размер которой 0x90, но выделяем под нее 0xa0
			int drawableSectionSize = 0x90;
			// создаем переменную типа drawable и по немного заполняем ее
			RageResource.Drawable drawable= new RageResource.Drawable();
			drawable._vmt = 6902356;
			drawable.m_pEndOfTheHeader = 0x90 + 0x50000000;
			// shadinggroup
			// писать информацию будем начиная с 0x250

			currentPosInSysBuffer = 672;
			currentPosInGfxBuffer = 0;
			// drawable.m_pShaderGroup = 0; // секции нет
			string mainFilePath = "C:\\Users\\d3g0n\\source\\repos\\Converter\\bin\\x64\\Debug\\net7.0\\test.odr";
			var mappedFile1 = MemoryMappedFile.CreateFromFile(mainFilePath);
			Stream mmStream = mappedFile1.CreateViewStream();
			StreamReader sr = new StreamReader(mmStream, ASCIIEncoding.ASCII);
			//
			string[] tempstring;
			string shaderName;
			tempstring = ReadLine(sr);
			ushort shaderCount;
			Shader[] shader = new Shader[100];
			{
				tempstring = ReadLine(sr);// shadinggroup
				tempstring = ReadLine(sr);// {
				tempstring = ReadLine(sr);// shader
				shaderCount = Convert.ToUInt16(tempstring[1]);
				tempstring = ReadLine(sr);// {
										  // читаем сам шейдер
				//Shader[] shader = new Shader[100];
				for (int a = 0; a < shaderCount; a++)
				{
					int currentString = 0;
					int currentVector4 = 0;
					int currentMatrix = 0;
					tempstring = ReadLine(sr);
					shaderName = tempstring[0];
					shader[a] = GetShaderInfo(shaderName);
					for (int b = 0; b < shader[a].paramCount; b++)
					{
						if (shader[a].paramTypeBuffer[b]==0)
						{
							shader[a].stringValueBuffer[currentString++] = tempstring[b + 1];
						}
						else if (shader[a].paramTypeBuffer[b] == 1)
						{
							shader[a].vector4ValueBuffer[currentVector4++] = GetVector4(tempstring[b + 1]);
						}
						else if (shader[a].paramTypeBuffer[b] == 4)
						{
							shader[a].matrixBuffer[currentMatrix++] = GetMatrix(tempstring[b + 1]);
						}
					}
				}
				tempstring = ReadLine(sr);// }
				tempstring = ReadLine(sr);// }
										  // это вся информация
				RageResource.IV_ShaderFX[] shaderFX = new RageResource.IV_ShaderFX[shaderCount];
				for (int a = 0; a < shaderCount; a++)
				{
					shaderFX[a].vtable = 7021116;
					shaderFX[a].m_dwBlockMapAdress = 0;
					shaderFX[a]._f8= 2;
					shaderFX[a].m_nDrawBucket = shader[a].drawBucket;
					shaderFX[a]._fA = 1;
					shaderFX[a]._fB = -51;
					shaderFX[a]._fC = 0;
					shaderFX[a].m_wIndex = shader[a].index;
					shaderFX[a]._f10 = 0;
					uint[] pParam = WriteShaderParams(sys, shader[a]);
					Align(4); 
					shaderFX[a].m_pShaderParams = currentPosInSysBuffer + 0x50000000;
					for (int b = 0; b < shader[a].paramCount; b++)
					{
						WriteValueToByteArray(ref sys, pParam[b], ref currentPosInSysBuffer);
					}
	//				Align(4); // чтобы все было в границах 4 байтов
					shaderFX[a]._f18 = 0;
					shaderFX[a].m_dwParamsCount = shader[a].paramCount;
					shaderFX[a].m_dwEffectSize = shader[a].effectSize;
					Align(4);
					shaderFX[a].m_pParameterTypes = currentPosInSysBuffer+ 0x50000000;
					for (int b = 0; b < shader[a].paramCount; b++)
					{
						WriteValueToByteArray(ref sys, shader[a].paramTypeBuffer[b], ref currentPosInSysBuffer);
					}
					shaderFX[a].m_dwHash = 2066433082;
					shaderFX[a]._f2C = 0;
					shaderFX[a]._f30 = 0;
					Align(4);
					shaderFX[a].m_pParamsHash = currentPosInSysBuffer + 0x50000000;
					for (int b = 0; b < shader[a].paramCount; b++)
					{
						WriteValueToByteArray(ref sys, shader[a].paramNameHashBuffer[b], ref currentPosInSysBuffer);
					}
					shaderFX[a]._f38 = 0;
					shaderFX[a]._f3C = 0;
					shaderFX[a]._f40 = 0;
					Align(16);
					shaderFX[a].m_pName = currentPosInSysBuffer + 0x50000000;
					WriteValueToByteArray(ref sys, shader[a].fxcName, ref currentPosInSysBuffer);
					Align(16);
					shaderFX[a].m_pSPS = currentPosInSysBuffer + 0x50000000;
					WriteValueToByteArray(ref sys, shader[a].spsName, ref currentPosInSysBuffer);
					shaderFX[a]._f4C = 0;
					shaderFX[a]._f50 = 0;
					shaderFX[a]._f54 = 0;
					shaderFX[a]._f58 = 0;
				}
				uint[] pShaderFX = new uint[shaderCount];
				for (int a = 0; a < shaderCount; a++)
				{
					Align(16); // выравниваем
					pShaderFX[a] = currentPosInSysBuffer + 0x50000000;
					WriteValueToByteArray(ref sys, shaderFX[a].vtable, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a].m_dwBlockMapAdress, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._f8, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a].m_nDrawBucket, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._fA, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._fB, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._fC, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a].m_wIndex, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._f10, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a].m_pShaderParams, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._f18, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a].m_dwParamsCount, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a].m_dwEffectSize, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a].m_pParameterTypes, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a].m_dwHash, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._f2C, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._f30, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a].m_pParamsHash, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._f38, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._f3C, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._f40, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a].m_pName, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a].m_pSPS, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._f4C, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._f50, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._f54, ref currentPosInSysBuffer);
					WriteValueToByteArray(ref sys, shaderFX[a]._f58, ref currentPosInSysBuffer);
					Align(16);
				}
				RageResource.IV_ShaderGroup shaderGroup = new RageResource.IV_ShaderGroup();
				// пишем shaderGroup
				shaderGroup.vtable = 7018052;
				shaderGroup.m_pTexture = 0;
				shaderGroup.m_pShaders.m_pList = currentPosInSysBuffer + 0x50000000;
				for (int a = 0; a < shaderCount; a++)
				{
					WriteValueToByteArray(ref sys, pShaderFX[a], ref currentPosInSysBuffer);
				}
				shaderGroup.m_pShaders.m_wCount = shaderCount;
				shaderGroup.m_pShaders.m_wSize = shaderCount;
				shaderGroup._f10.m0 = new Vector4(0,0,0,0);
				shaderGroup._f10.m1 = new Vector4(0, 0, 0, 0);
				shaderGroup._f10.m2 = new Vector4(0, 0, 0, 0);
				//currentPosInSysBuffer += currentPosInSysBuffer % 16; // выравниваем
				Align(16);
				shaderGroup.m_pVertexFormat.m_pList = currentPosInSysBuffer + 0x50000000;
				for (int a = 0; a < shaderCount; a++)
				{
					WriteValueToByteArray(ref sys, (uint)shader[a].vertexFormat, ref currentPosInSysBuffer);
				}
				shaderGroup.m_pVertexFormat.m_wCount = shaderCount;
				shaderGroup.m_pVertexFormat.m_wSize = shaderCount;
				Align(16);
				shaderGroup.m_pIndexMapping.m_pList = currentPosInSysBuffer + 0x50000000;
				for (int a = 0; a < shaderCount; a++)
				{
					WriteValueToByteArray(ref sys, (uint)shader[a].index, ref currentPosInSysBuffer);
				}
				shaderGroup.m_pIndexMapping.m_wCount = shaderCount;
				shaderGroup.m_pIndexMapping.m_wSize = shaderCount;

				Align(16);
				drawable.m_pShaderGroup = currentPosInSysBuffer + 0x50000000;
				WriteValueToByteArray(ref sys, shaderGroup.vtable, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup.m_pTexture, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup.m_pShaders.m_pList, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup.m_pShaders.m_wCount, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup.m_pShaders.m_wSize, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup._f10.m0, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup._f10.m1, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup._f10.m2, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup.m_pVertexFormat.m_pList, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup.m_pVertexFormat.m_wCount, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup.m_pVertexFormat.m_wSize, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup.m_pIndexMapping.m_pList, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup.m_pIndexMapping.m_wCount, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, shaderGroup.m_pIndexMapping.m_wSize, ref currentPosInSysBuffer);

			}
			//File.WriteAllBytes("tmp.sys", sys);

			/*Align(256);
			Array.Resize<byte>(ref sys, (int)currentPosInSysBuffer);
			// пишем временную секцию drawable
			drawable.m_pSkeleton = 0; // секции нет
			drawable.m_vCenter = new Vector4(0, 0, 0, 0);
			drawable.m_vAabbMin = new Vector4(-10, -10, -10, 0);
			drawable.m_vAabbMax = new Vector4(10, 10, 10, 0);
			Array.Resize<uint>(ref drawable.m_pModelCollection, 4);
			Array.Resize<int>(ref drawable.m_dwObjectCount, 4);
			for (int i = 0; i < 4; i++) drawable.m_pModelCollection[i] = 0;
			drawable.m_vDrawDistance.X = 9999;
			drawable.m_vDrawDistance.Y = 9999;
			drawable.m_vDrawDistance.Z = 9999;
			drawable.m_vDrawDistance.W = 9999;
			for (int i = 0; i < 4; i++) drawable.m_dwObjectCount[i] = -1;
			drawable.m_vRadius = new Vector4(10, 0, 0, 0);
			drawable.m_p2DFX.m_pList = 0;
			drawable.m_p2DFX.m_wCount = 0;
			drawable.m_p2DFX.m_wSize = 0;


			uint tmpPosInDrawable = 0;
			WriteValueToByteArray(ref sys, drawable._vmt, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pEndOfTheHeader, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pShaderGroup, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pSkeleton, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vCenter, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vAabbMin, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vAabbMax, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pModelCollection[0], ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pModelCollection[1], ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pModelCollection[2], ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pModelCollection[3], ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vDrawDistance.X, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vDrawDistance.Y, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vDrawDistance.Z, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vDrawDistance.W, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_dwObjectCount[0], ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_dwObjectCount[1], ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_dwObjectCount[2], ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_dwObjectCount[3], ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vRadius.X, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vRadius.Y, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vRadius.Z, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vRadius.W, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_p2DFX.m_pList, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_p2DFX.m_wCount, ref tmpPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_p2DFX.m_wSize, ref tmpPosInDrawable);

			tmpPosInDrawable = 0x90;
			WriteValueToByteArray(ref sys, (uint)0, ref tmpPosInDrawable);



			//
			BinaryWriter dataOut = new BinaryWriter(new FileStream("tmp2.wdr", FileMode.Create));
			int num = -1073741824;
			num |= ResourceUtils.FlagInfo.RSC05_GenerateMemSizes((int)currentPosInSysBuffer, 0);
			//byte[] data = File.ReadAllBytes("test_unpacked.sys");
			byte[] array = null;
			array = DataUtils.Compress(sys, 9, noHeader: false);

			uint val = 88298322u;
			int resVersion = 110;
			dataOut.Write(val);
			dataOut.Write(resVersion);
			dataOut.Write(num);
			dataOut.Write(array);
			dataOut.Close();


			return;*/


			drawable.m_pSkeleton = 0; // секции нет
			drawable.m_vCenter = new Vector4(0, 0, 0, 0);
			drawable.m_vAabbMin = new Vector4(-10, -10, -10, 0);
			drawable.m_vAabbMax = new Vector4(10, 10, 10, 0);
			Array.Resize<uint>(ref drawable.m_pModelCollection, 4);
			Array.Resize<int>(ref drawable.m_dwObjectCount, 4);
			tempstring = ReadLine(sr); // lodgroup
			tempstring = ReadLine(sr); // {
			string[] tempstring2;

			MemoryMappedFile mappedFile2 /*= MemoryMappedFile.CreateFromFile("test.odr")*/;
			Stream mmStream2/* = mappedFile1.CreateViewStream()*/;
			StreamReader meshFile/* = new StreamReader(mmStream2, ASCIIEncoding.ASCII)*/;

			for (int a = 0; a < 4; a++) // 4 уровня детализации
			{
				tempstring = ReadLine(sr);
				string lodLevel = tempstring[0];
				if (tempstring[1]=="none")
				{
					drawable.m_pModelCollection[a] = 0;
					drawable.m_dwObjectCount[a] = -1;
					continue;
				}
				uint modelCount = Convert.ToUInt32(tempstring[1]);
				Model[] model = new Model[modelCount];
				for (int b = 0; b < modelCount*2; b+=2)
				{
					string modelpath = tempstring[b + 2];
					uint boneIndex = Convert.ToUInt32(tempstring[b + 3]);
					//////////////////////
					mappedFile2 = MemoryMappedFile.CreateFromFile($"{Path.GetDirectoryName(mainFilePath)}\\{modelpath}");
					mmStream2 = mappedFile2.CreateViewStream();
					meshFile = new StreamReader(mmStream2, ASCIIEncoding.ASCII);
					// читаем mesh файл
					tempstring2 = ReadLine(meshFile);// версия
					tempstring2 = ReadLine(meshFile);//{
					tempstring2 = ReadLine(meshFile);// skinned
					bool bSkinned = Convert.ToBoolean(Convert.ToByte(tempstring2[1]));
					uint boundsCount = 0;
					Vector4[] vBounds = new Vector4[1];
					if (!bSkinned)// границей неи в skinned модели
					{
						tempstring2 = ReadLine(meshFile);// bounds
						boundsCount = Convert.ToUInt32(tempstring2[1]);
						tempstring2 = ReadLine(meshFile);// {
						Array.Resize<Vector4>(ref vBounds, (int)boundsCount);
						//Vector4[] vBounds = new Vector4[boundsCount];
						for (int c = 0; c < boundsCount; c++)
						{
							tempstring2 = ReadLine(meshFile);
							vBounds[c].X = Convert.ToSingle(tempstring2[0]);
							vBounds[c].Y = Convert.ToSingle(tempstring2[1]);
							vBounds[c].Z = Convert.ToSingle(tempstring2[2]);
							vBounds[c].W = Convert.ToSingle(tempstring2[3]);
						}
						tempstring2 = ReadLine(meshFile);// }
					}
					uint maxGeometryCount = 255;
					ushort[] mtlMapping= new ushort[maxGeometryCount];// макс. количество метериалов в одной моделе
					Geometry[] geometryBuffer = new Geometry[maxGeometryCount];
					FakeVertexDeclaration[] vertexDeclaration = new FakeVertexDeclaration[maxGeometryCount];
					IndexBuffer[] indexBuffer = new IndexBuffer[maxGeometryCount];
					VertexBuffer[] vertexBuffer = new VertexBuffer[maxGeometryCount];
					uint geometryCount = 0;
					for (int c = 0; c < mtlMapping.Length; c++)// читаем секцию geometry
					{
						tempstring2 = ReadLine(meshFile);// mtl
						if (tempstring2[0] != "Mtl") break;
						mtlMapping[c] = Convert.ToUInt16(tempstring2[1]);
						tempstring2 = ReadLine(meshFile);// {
						tempstring2 = ReadLine(meshFile);// prim
						uint prim = Convert.ToUInt16(tempstring2[1]);
						tempstring2 = ReadLine(meshFile);// {
						tempstring2 = ReadLine(meshFile);// vects
						uint IdxCount = Convert.ToUInt32(tempstring2[1]);
						tempstring2 = ReadLine(meshFile);// {
														 //
						uint fullLineCount = IdxCount / 15u;
						uint currentIdx = 0;
						ushort[] idxBuffer = new ushort[IdxCount];
						for (int d = 0; d < fullLineCount; d++)
						{
							tempstring2 = ReadLine(meshFile);
							for (int e = 0; e < 15; e++)
							{
								idxBuffer[currentIdx++] = Convert.ToUInt16(tempstring2[e]);
							}
						}
						uint idxCountInHalfLine = IdxCount - fullLineCount * 15;
						tempstring2 = ReadLine(meshFile);
						for (int d = 0; d < idxCountInHalfLine; d++)
						{
							idxBuffer[currentIdx++] = Convert.ToUInt16(tempstring2[d]);
						}
						tempstring2 = ReadLine(meshFile);// }
						// verts
						tempstring2 = ReadLine(meshFile);// verts
						ushort vertsCount = Convert.ToUInt16(tempstring2[1]);
						tempstring2 = ReadLine(meshFile);// {
						Vertex[] vertsBuffer = new Vertex[vertsCount];
						for (int d = 0; d < vertsCount; d++)
						{
							tempstring2 = ReadLine(meshFile);// VERTEX
							vertsBuffer[d].posX = Convert.ToSingle(tempstring2[0]);
							vertsBuffer[d].posY = Convert.ToSingle(tempstring2[1]);
							vertsBuffer[d].posZ = Convert.ToSingle(tempstring2[2]);
							//
							vertsBuffer[d].nrmX = Convert.ToSingle(tempstring2[4]);
							vertsBuffer[d].nrmY = Convert.ToSingle(tempstring2[5]);
							vertsBuffer[d].nrmZ = Convert.ToSingle(tempstring2[6]);
							//
							vertsBuffer[d].colorX = Convert.ToByte(tempstring2[8]);
							vertsBuffer[d].colorY = Convert.ToByte(tempstring2[9]);
							vertsBuffer[d].colorZ = Convert.ToByte(tempstring2[10]);
							vertsBuffer[d].colorW = Convert.ToByte(tempstring2[11]);
							//
							vertsBuffer[d].tangX = Convert.ToSingle(tempstring2[13]);
							vertsBuffer[d].tangY = Convert.ToSingle(tempstring2[14]);
							vertsBuffer[d].tangZ = Convert.ToSingle(tempstring2[15]);
							vertsBuffer[d].tangW = Convert.ToSingle(tempstring2[16]);
							//
							vertsBuffer[d].uv0X = Convert.ToSingle(tempstring2[18]);
							vertsBuffer[d].uv0Y = Convert.ToSingle(tempstring2[19]);
							//
							vertsBuffer[d].uv1X = Convert.ToSingle(tempstring2[21]);
							vertsBuffer[d].uv1Y = Convert.ToSingle(tempstring2[22]);
							//
							vertsBuffer[d].uv2X = Convert.ToSingle(tempstring2[24]);
							vertsBuffer[d].uv2Y = Convert.ToSingle(tempstring2[25]);
							//
							vertsBuffer[d].uv3X = Convert.ToSingle(tempstring2[27]);
							vertsBuffer[d].uv3Y = Convert.ToSingle(tempstring2[28]);
							//
							vertsBuffer[d].uv4X = Convert.ToSingle(tempstring2[30]);
							vertsBuffer[d].uv4Y = Convert.ToSingle(tempstring2[31]);
							//
							vertsBuffer[d].uv5X = Convert.ToSingle(tempstring2[33]);
							vertsBuffer[d].uv5Y = Convert.ToSingle(tempstring2[34]);
						}
						tempstring2 = ReadLine(meshFile);// }
						tempstring2 = ReadLine(meshFile);// }
						tempstring2 = ReadLine(meshFile);// }
														 // мы прочитали секцию geometry
														 // vertexBuffer
														 //geometryBuffer[c].vtable = 7031028u;
														 //geometryBuffer[c].m_piVertexDeclaration= 0;
														 //geometryBuffer[c]._f8 = 0;
						vertexBuffer[c].vtable = 7060184u;
						vertexBuffer[c].m_wVertexCount = vertsCount;
						vertexBuffer[c].m_bLocked = 0;
						vertexBuffer[c]._f7 = 0;
						AlignGFX(256);
						vertexBuffer[c].m_pLockedData = currentPosInGfxBuffer + 0x60000000;
						if (shader[mtlMapping[c]].vertexFormat == 89) 
						{
							for (int d = 0; d < vertsCount; d++)
							{
								WriteValueToByteArray(ref gfx, vertsBuffer[d].posX, ref currentPosInGfxBuffer);
								WriteValueToByteArray(ref gfx, vertsBuffer[d].posY, ref currentPosInGfxBuffer);
								WriteValueToByteArray(ref gfx, vertsBuffer[d].posZ, ref currentPosInGfxBuffer);
								WriteValueToByteArray(ref gfx, vertsBuffer[d].nrmX, ref currentPosInGfxBuffer);
								WriteValueToByteArray(ref gfx, vertsBuffer[d].nrmY, ref currentPosInGfxBuffer);
								WriteValueToByteArray(ref gfx, vertsBuffer[d].nrmZ, ref currentPosInGfxBuffer);
								WriteValueToByteArray(ref gfx, vertsBuffer[d].colorX, ref currentPosInGfxBuffer);
								WriteValueToByteArray(ref gfx, vertsBuffer[d].colorY, ref currentPosInGfxBuffer);
								WriteValueToByteArray(ref gfx, vertsBuffer[d].colorZ, ref currentPosInGfxBuffer);
								WriteValueToByteArray(ref gfx, vertsBuffer[d].colorW, ref currentPosInGfxBuffer);
								WriteValueToByteArray(ref gfx, vertsBuffer[d].uv0X, ref currentPosInGfxBuffer);
								WriteValueToByteArray(ref gfx, vertsBuffer[d].uv0Y, ref currentPosInGfxBuffer);
							}
							vertexBuffer[c].m_dwVertexSize = 36;
							vertexDeclaration[c].vertexFormat = shader[mtlMapping[c]].vertexFormat;
							vertexDeclaration[c].totalSize = 36;
							vertexDeclaration[c]._f6 = 0;
							vertexDeclaration[c].StoreNormalsDataFirst = 0;
							vertexDeclaration[c].ElementsCount = 4;
							vertexDeclaration[c].elementTypes1 = 1436117398;
							vertexDeclaration[c].elementTypes2 = 1733645653;
						}
						// ss
						Align(672);
						vertexBuffer[c].m_pDeclaration = currentPosInSysBuffer + 0x50000000;
						{   // пишем секцию vertexDeclaration
							WriteValueToByteArray(ref sys, vertexDeclaration[c].vertexFormat, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexDeclaration[c].totalSize, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexDeclaration[c]._f6, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexDeclaration[c].StoreNormalsDataFirst, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexDeclaration[c].ElementsCount, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexDeclaration[c].elementTypes1, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexDeclaration[c].elementTypes2, ref currentPosInSysBuffer);
						}
						vertexBuffer[c]._f14 = 0;
						vertexBuffer[c].m_pVertexData = vertexBuffer[c].m_pLockedData;
						// indexBuffer
						indexBuffer[c].vtable = 7059568u;
						indexBuffer[c].m_dwIndexCount = IdxCount;
						AlignGFX(16);
						indexBuffer[c].m_pIndexData = currentPosInGfxBuffer + 0x60000000;
						for (uint d = 0; d < IdxCount; d++)
						{
							WriteValueToByteArray(ref gfx, idxBuffer[d], ref currentPosInGfxBuffer);
						}
						// geometry
						geometryBuffer[c].vtable = 7031028u;
						geometryBuffer[c].m_piVertexDeclaration= 0;
						geometryBuffer[c]._f8 = 0;
						Align(16);
						geometryBuffer[c].m_pVertexBuffer = currentPosInSysBuffer + 0x50000000;
						{   // пишем секцию vertexBuffer
							WriteValueToByteArray(ref sys, vertexBuffer[c].vtable, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexBuffer[c].m_wVertexCount, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexBuffer[c].m_bLocked, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexBuffer[c]._f7, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexBuffer[c].m_pLockedData, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexBuffer[c].m_dwVertexSize, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexBuffer[c].m_pDeclaration, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexBuffer[c]._f14, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, vertexBuffer[c].m_pVertexData, ref currentPosInSysBuffer);
							//
							WriteValueToByteArray(ref sys, (uint)0, ref currentPosInSysBuffer);
							for (int d = 0; d < 8; d++) WriteValueToByteArray(ref sys, (uint)3452816845, ref currentPosInSysBuffer);
						}
						geometryBuffer[c]._f10 = 0;
						geometryBuffer[c]._f14 = 0;
						geometryBuffer[c]._f18 = 0;
						Align(16);
						geometryBuffer[c].m_pIndexBuffer = currentPosInSysBuffer + 0x50000000;
						{
							WriteValueToByteArray(ref sys, indexBuffer[c].vtable, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, indexBuffer[c].m_dwIndexCount, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, indexBuffer[c].m_pIndexData, ref currentPosInSysBuffer);
							//
							WriteValueToByteArray(ref sys, (uint)0, ref currentPosInSysBuffer);
							for (int d = 0; d < 8; d++) WriteValueToByteArray(ref sys, (uint)3452816845, ref currentPosInSysBuffer);
						}
						geometryBuffer[c]._f20 = 0;
						geometryBuffer[c]._f24 = 0;
						geometryBuffer[c]._f28 = 0;
						geometryBuffer[c].m_dwIndexCount = indexBuffer[c].m_dwIndexCount;
						geometryBuffer[c].m_dwFaceCount = geometryBuffer[c].m_dwIndexCount/3;
						geometryBuffer[c].m_wVertexCount = vertexBuffer[c].m_wVertexCount;
						geometryBuffer[c].m_wIndicesPerFace = 3;
						geometryBuffer[c].m_pBoneMapping = 0;
						geometryBuffer[c].m_wVertexStride = (ushort)vertexBuffer[c].m_dwVertexSize;
						geometryBuffer[c].m_wBoneCount = 0;
						geometryBuffer[c].m_pVertexDeclaration = 0;
						geometryBuffer[c]._f44 = 0;
						geometryBuffer[c]._f48 = 0;

						geometryCount++;
					}
					mmStream2.Close();
					meshFile.Close();
					Array.Resize<VertexBuffer>(ref vertexBuffer, (int)geometryCount);
					Array.Resize<FakeVertexDeclaration>(ref vertexDeclaration, (int)geometryCount);
					Array.Resize<IndexBuffer>(ref indexBuffer, (int)geometryCount);
					Array.Resize<Geometry>(ref geometryBuffer, (int)geometryCount);
					uint[] pGeometry = new uint[geometryCount];
					Align(16);
					for (int c = 0; c < geometryCount; c++)
					{
						pGeometry[c] = currentPosInSysBuffer + 0x50000000;
						{   // пишем geometry
							WriteValueToByteArray(ref sys, geometryBuffer[c].vtable, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c].m_piVertexDeclaration, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c]._f8, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c].m_pVertexBuffer, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c]._f10, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c]._f14, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c]._f18, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c].m_pIndexBuffer, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c]._f20, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c]._f24, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c]._f28, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c].m_dwIndexCount, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c].m_dwFaceCount, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c].m_wVertexCount, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c].m_wIndicesPerFace, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c].m_pBoneMapping, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c].m_wVertexStride, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c].m_wBoneCount, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c].m_pVertexDeclaration, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c]._f44, ref currentPosInSysBuffer);
							WriteValueToByteArray(ref sys, geometryBuffer[c]._f48, ref currentPosInSysBuffer);
						}
					}
					// собираем секцию model
					//Model[] model = new Model[100];
					model[b / 2]._vmt = 7012916;
					Align(16);
					model[b / 2].m_pGeometry.m_pList = currentPosInSysBuffer + 0x50000000;
					for (int c = 0; c < geometryCount; c++)
					{
						WriteValueToByteArray(ref sys, pGeometry[c], ref currentPosInSysBuffer);
					}
					model[b / 2].m_pGeometry.m_wCount = (ushort)geometryCount;
					model[b / 2].m_pGeometry.m_wSize = (ushort)geometryCount;
					Align(16);
					model[b / 2].m_pBounds = currentPosInSysBuffer + 0x50000000;
					for (int c = 0; c < boundsCount; c++)
					{
						WriteValueToByteArray(ref sys, vBounds[c], ref currentPosInSysBuffer);
					}
					Align(16);
					model[b / 2].m_pShaderMapping = currentPosInSysBuffer + 0x50000000;
					for (int c = 0; c < boundsCount; c++)
					{
						WriteValueToByteArray(ref sys, mtlMapping[c], ref currentPosInSysBuffer);
					}
					model[b / 2].m_bSkinned = bSkinned;
					model[b / 2]._f16 = 0;
					model[b / 2].m_nBoneIndex = (byte)boneIndex;
					model[b / 2]._f18 = 0;
					model[b / 2].m_bHasOffset = 0;
					model[b / 2].m_nShaderMappingsCount = (byte)geometryCount;
				}
				uint[] pModel = new uint[modelCount];
				Align(16);
				for (int b = 0; b < modelCount; b++)
				{
					pModel[b] = currentPosInSysBuffer + 0x50000000;
					{
						WriteValueToByteArray(ref sys, model[b]._vmt, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref sys, model[b].m_pGeometry.m_pList, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref sys, model[b].m_pGeometry.m_wCount, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref sys, model[b].m_pGeometry.m_wSize, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref sys, model[b].m_pBounds, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref sys, model[b].m_pShaderMapping, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref sys, model[b].m_nBoneCount, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref sys, Convert.ToByte(model[b].m_bSkinned), ref currentPosInSysBuffer);
						WriteValueToByteArray(ref sys, model[b]._f16, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref sys, model[b].m_nBoneIndex, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref sys, model[b]._f18, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref sys, model[b].m_bHasOffset, ref currentPosInSysBuffer);
						WriteValueToByteArray(ref sys, (ushort)model[b].m_nShaderMappingsCount, ref currentPosInSysBuffer);
					}

				}
				// пишем modelCollection
				Align(16);
				RageResource.Collection modelCollection;
				//drawable.m_pModelCollection[a] = currentPosInSysBuffer + 0x50000000;
				modelCollection.m_pList = currentPosInSysBuffer + 0x50000000;
				for (int b = 0; b < modelCount; b++)
				{
					WriteValueToByteArray(ref sys, pModel[b], ref currentPosInSysBuffer);
				}
				modelCollection.m_wCount = (ushort)modelCount;
				modelCollection.m_wSize = (ushort)modelCount;
				Align(16);
				drawable.m_pModelCollection[a] = currentPosInSysBuffer + 0x50000000;
				WriteValueToByteArray(ref sys, modelCollection.m_pList, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, modelCollection.m_wCount, ref currentPosInSysBuffer);
				WriteValueToByteArray(ref sys, modelCollection.m_wSize, ref currentPosInSysBuffer);


				switch (a)
				{
					case 0:
						drawable.m_vDrawDistance.X = Convert.ToSingle(tempstring[tempstring.Length - 1]);
						break;
					case 1:
						drawable.m_vDrawDistance.Y = Convert.ToSingle(tempstring[tempstring.Length - 1]);
						break;
					case 2:
						drawable.m_vDrawDistance.Z = Convert.ToSingle(tempstring[tempstring.Length - 1]);
						break;
					case 3:
						drawable.m_vDrawDistance.W = Convert.ToSingle(tempstring[tempstring.Length - 1]);
						break;
				}
				

				drawable.m_dwObjectCount[a] = (int)modelCount;
			}
			drawable.m_vRadius = new Vector4(10, 0, 0, 0);
			drawable.m_p2DFX.m_pList = 0;
			drawable.m_p2DFX.m_wCount = 0;
			drawable.m_p2DFX.m_wSize = 0;
			// пишем секцию drawable
			uint currentPosInDrawable =0;
			WriteValueToByteArray(ref sys, drawable._vmt, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pEndOfTheHeader, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pShaderGroup, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pSkeleton, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vCenter, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vAabbMin, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vAabbMax, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pModelCollection[0], ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pModelCollection[1], ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pModelCollection[2], ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_pModelCollection[3], ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vDrawDistance.X, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vDrawDistance.Y, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vDrawDistance.Z, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vDrawDistance.W, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_dwObjectCount[0], ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_dwObjectCount[1], ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_dwObjectCount[2], ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_dwObjectCount[3], ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vRadius.X, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vRadius.Y, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vRadius.Z, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_vRadius.W, ref currentPosInDrawable);
			WriteValueToByteArray(ref sys, drawable.m_p2DFX.m_pList , ref currentPosInDrawable);// я перемiх юлiка
			WriteValueToByteArray(ref sys, drawable.m_p2DFX.m_wCount, ref currentPosInDrawable);// я перемiх тебе
			WriteValueToByteArray(ref sys, drawable.m_p2DFX.m_wSize, ref currentPosInDrawable);

			currentPosInDrawable = 0x90;
			WriteValueToByteArray(ref sys, (uint)0, ref currentPosInDrawable);


			Align(4096);
			AlignGFX(4096);
			Array.Resize<byte>(ref sys, (int)currentPosInSysBuffer);
			Array.Resize<byte>(ref gfx, (int)currentPosInGfxBuffer);
			File.WriteAllBytes("system", sys);
			File.WriteAllBytes("graphic", gfx);

			BinaryWriter dataOut = new BinaryWriter(new FileStream("tmp2.wdr", FileMode.Create));
			int num = -1073741824;
			num |= ResourceUtils.FlagInfo.RSC05_GenerateMemSizes((int)currentPosInSysBuffer, (int)currentPosInGfxBuffer);
			//byte[] data = File.ReadAllBytes("test_unpacked.sys");
			byte[] array = null;
			byte[] allData = sys;
			Array.Resize<byte>(ref allData, (int)(currentPosInSysBuffer + currentPosInGfxBuffer));
			Buffer.BlockCopy(gfx, 0, allData, (int)currentPosInSysBuffer, (int)currentPosInGfxBuffer);
			
			array = DataUtils.Compress(allData, 9, noHeader: false);

			uint val = 88298322u;
			int resVersion = 110;
			dataOut.Write(val);
			dataOut.Write(resVersion);
			dataOut.Write(num);
			dataOut.Write(array);
			dataOut.Close();


			//			File.WriteAllBytes("tmp.sys", sys);
			//			File.WriteAllBytes("tmp.gfx", gfx);



		}


	}
}
