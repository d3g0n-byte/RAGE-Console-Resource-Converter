using ConsoleApp1;
using Converter.Utils;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static Converter.ResourceUtils;
using static Converter.Test.RDR_XTD_CREATOR;

namespace Converter.Test
{
	internal class RDR_XTD_CREATOR
	{
		// большинство функций я буду использовать из моего "создателя" wdr файлов
		// в этом месте даже не пахнет нормальной реализацией чего-то

		public static int tmpGFXSize = 0x1000000;
		
		public static uint currentPosInSysBuffer = 0;
		public static uint currentPosInGfxBuffer = 0;
		public static void AlignSYS(uint value) // не найлучшее решение, но нормальный вариант не работал(
		{
			while (currentPosInSysBuffer % value != 0) currentPosInSysBuffer += 1;
		}
		public static void AlignGFX(uint value)
		{
			while (currentPosInGfxBuffer % value != 0) currentPosInGfxBuffer += 1;
		}
		public static void WriteValueToByteArray(ref byte[] array, string str, ref uint pos)
		{
			byte[] array2 = Encoding.ASCII.GetBytes(str);
			Array.Resize<byte>(ref array2, array2.Length + 1);
			WriteToByteArray(ref array, array2, ref pos);
		}
		public static void WriteValueToByteArray(ref byte[] array, uint value, ref uint pos) => WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		public static void WriteValueToByteArray(ref byte[] array, int value, ref uint pos) => WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		public static void WriteValueToByteArray(ref byte[] array, short value, ref uint pos) => WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		public static void WriteValueToByteArray(ref byte[] array, ushort value, ref uint pos)=>WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		public static void WriteValueToByteArray(ref byte[] array, byte value, ref uint pos) => WriteToByteArray(ref array, new byte[] { value }, ref pos);
		public static void WriteValueToByteArray(ref byte[] array, sbyte value, ref uint pos) => WriteToByteArray(ref array, new byte[] { (byte)value }, ref pos);
		public static void WriteValueToByteArray(ref byte[] array, float value, ref uint pos) => WriteToByteArray(ref array, BitConverter.GetBytes(value), ref pos);
		public static void WriteToByteArray(ref byte[] array, byte[] array2, ref uint pos)
		{
			for (int a = 0; a < array2.Length; a++) array[pos + a] = array2[a];
			pos += (uint)array2.Length;
		}
		public struct XmlInfo
		{
			public string name;
			public string path;
			public int height;
			public int width;
			public int mipCount;
			public string typeAsString;
		}
		public static void ExtractTextures(XmlInfo[] xmlInfo, string inPath)
		{
			FileInfo.fileName = inPath;
			if (!File.Exists(inPath)) throw new Exception("wrong inFile path");
			EndianBinaryReader br;
			br = new EndianBinaryReader(File.OpenRead(inPath));

			RSCHeader hdr = new RSCHeader();
			byte[] magicAsByte = br.ReadBytes(4);
			string magic = Encoding.Default.GetString(magicAsByte);
			if (magic.Contains("CSR"))
			{
				br.Endianness = Endian.BigEndian;
				hdr.Version = magicAsByte[0];
			}
			else
			{
				Log.ToLog(Log.MessageType.ERROR, $"Unknown file {inPath}. Skipped");
				throw new Exception("unk file");
			}
			ResourceUtils.ResourceInfo.version = br.ReadInt32();
			ResourceUtils.FlagInfo.Flag1 = br.ReadInt32();
			if (hdr.Version == 0x85 || hdr.Version == 0x86 || hdr.Version == 0x37) ResourceUtils.FlagInfo.Flag2 = br.ReadInt32();
			uint cmagic = br.ReadUInt32();
			MemoryStream ms;
			if ((cmagic & 0xFFFF0000) != 0x0FF50000)
			{
				Log.ToLog(Log.MessageType.ERROR, "LZX compression not detected. Skipped");
				throw new Exception("unk compression");
			}
			int csize = br.ReadInt32();
			int usize = csize * 4;
			byte[] origBuffer = br.ReadBytes(csize);
			byte[] decompBuffer = new byte[FlagInfo.BaseResourceSizeV + FlagInfo.BaseResourceSizeP];
			DataUtils.DecompressLZX(origBuffer, decompBuffer);
			byte[] sysBuffer = new byte[FlagInfo.BaseResourceSizeV];
			//byte[] gfxBuffer = new byte[FlagInfo.BaseResourceSizeP];
			byte[] gfxBuffer = new byte[tmpGFXSize]; // в начале мы не знаем сколько нужно байтов
			Buffer.BlockCopy(decompBuffer, 0, sysBuffer, 0, FlagInfo.BaseResourceSizeV);
			Buffer.BlockCopy(decompBuffer, sysBuffer.Length, gfxBuffer, 0, FlagInfo.BaseResourceSizeP);
			// читаем системный сигмент
			ms = new MemoryStream(sysBuffer);
			br = new EndianBinaryReader(ms);
			br.Endianness = Endian.BigEndian;
			br.Position = FlagInfo.ResourceStart;

			RageResource.XTDHeader xtdHeader = new RageResource.XTDHeader();
			xtdHeader = ReadRageResource.XTDHeader(br);
			RageResource.Texture[] texture = new RageResource.Texture[xtdHeader.m_cTexture.m_wCount];
			RageResource.BitMap[] bitMap = new RageResource.BitMap[xtdHeader.m_cTexture.m_wCount];
			uint[] pTexture = new uint[xtdHeader.m_cTexture.m_wCount];
			br.Position = xtdHeader.m_cTexture.m_pList;
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++) pTexture[a] = br.ReadOffset();
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++)
			{
				br.Position = pTexture[a];
				texture[a] = ReadRageResource.Texture(br);
				br.Position = texture[a].m_pBitmap;
				bitMap[a] = ReadRageResource.BitMap(br);
			}
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++) TextureDictionary.ReadTexture(texture[a], bitMap[a], br, 0);

		}
		public static void ReplaceTextures(XmlInfo[] xmlInfo, string inPath, string outPath)
		{
			// мы прочитали информацию о текстурах из xml файле
			// теперь можем приступить у модицикации старого xtd файла
			// для начала нужно его распаковать и прочитать чтобы можно было редактировать адреса в системном фрагменте и пересоздать графический фрагмент
			if (!File.Exists(inPath)) throw new Exception("wrong inFile path");
			EndianBinaryReader br;
			br = new EndianBinaryReader(File.OpenRead(inPath));

			RSCHeader hdr = new RSCHeader();
			byte[] magicAsByte = br.ReadBytes(4);
			string magic = Encoding.Default.GetString(magicAsByte);
			if (magic.Contains("CSR"))
			{
				br.Endianness = Endian.BigEndian;
				hdr.Version = magicAsByte[0];
			}
			else
			{
				Log.ToLog(Log.MessageType.ERROR, $"Unknown file {inPath}. Skipped");
				throw new Exception("unk file");
			}
			ResourceUtils.ResourceInfo.version = br.ReadInt32();
			ResourceUtils.FlagInfo.Flag1 = br.ReadInt32();
			if (hdr.Version == 0x85 || hdr.Version == 0x86 || hdr.Version == 0x37) ResourceUtils.FlagInfo.Flag2 = br.ReadInt32();
			uint cmagic = br.ReadUInt32();
			MemoryStream ms;
			if ((cmagic & 0xFFFF0000) != 0x0FF50000)
			{
				Log.ToLog(Log.MessageType.ERROR, "LZX compression not detected. Skipped");
				throw new Exception("unk compression");
			}
			int csize = br.ReadInt32();
			int usize = csize * 4;
			byte[] origBuffer = br.ReadBytes(csize);
			byte[] decompBuffer = new byte[FlagInfo.BaseResourceSizeV + FlagInfo.BaseResourceSizeP];
			DataUtils.DecompressLZX(origBuffer, decompBuffer);
			byte[] sysBuffer = new byte[FlagInfo.BaseResourceSizeV];
			//byte[] gfxBuffer = new byte[FlagInfo.BaseResourceSizeP];
			byte[] gfxBuffer = new byte[tmpGFXSize]; // в начале мы не знаем сколько нужно байтов
			Buffer.BlockCopy(decompBuffer, 0, sysBuffer, 0, FlagInfo.BaseResourceSizeV);
			Buffer.BlockCopy(decompBuffer, sysBuffer.Length, gfxBuffer, 0, FlagInfo.BaseResourceSizeP);
			// читаем системный сигмент
			ms = new MemoryStream(sysBuffer);
			br = new EndianBinaryReader(ms);
			br.Endianness = Endian.BigEndian;
			br.Position = FlagInfo.ResourceStart;

			RageResource.XTDHeader xtdHeader = new RageResource.XTDHeader();
			xtdHeader = ReadRageResource.XTDHeader(br);
			RageResource.Texture[] texture = new RageResource.Texture[xtdHeader.m_cTexture.m_wCount];
			RageResource.BitMap[] bitMap = new RageResource.BitMap[xtdHeader.m_cTexture.m_wCount];
			uint[] pTexture = new uint[xtdHeader.m_cTexture.m_wCount];
			br.Position = xtdHeader.m_cTexture.m_pList;
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++) pTexture[a] = br.ReadOffset();
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++)
			{
				br.Position = pTexture[a];
				texture[a] = ReadRageResource.Texture(br);
				br.Position = texture[a].m_pBitmap;
				bitMap[a] = ReadRageResource.BitMap(br);
			}
			// мы прочитали нужную информацию. Теперь проверяем имена.
			if (xtdHeader.m_cTexture.m_wCount != xmlInfo.Length) throw new Exception("textures count != xml textures count");
			int[] index = new int[xtdHeader.m_cTexture.m_wCount];
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++)
			{
				string oldName = DataUtils.ReadStringAtOffset(texture[a].m_pName, br);
				for (int b = 0; b < xmlInfo.Length; b++)
				{
					if (!File.Exists(xmlInfo[b].path)) throw new Exception($"\"{xmlInfo[b].path}\" file not found");
					if (/*Path.GetFileName*/(xmlInfo[b].name) == oldName) { index[a] = b; break; }
					if (b == xmlInfo.Length - 1) throw new Exception("bad name");
				}
			}
			// проверили. Можно приступать к модицикации
			for (int a = 0; a < gfxBuffer.Length; a += 4)
				for (int b = 0; b < 4; b++) gfxBuffer[a + b] = (byte)(b * 30+120);

			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++) EditorFunc.WriteNewTextures(xmlInfo[index[a]], ref texture[a], ref bitMap[a], ref gfxBuffer);
			// теперь пишем новые данные в sys сигмент
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++) EditorFunc.WriteNewTexturesToBuffer(pTexture[a], ref texture[a], ref bitMap[a], ref sysBuffer);
			AlignGFX(16384);
			Array.Resize<byte>(ref gfxBuffer, (int)currentPosInGfxBuffer);
			byte[] allData = sysBuffer.Concat(gfxBuffer).ToArray();

			Log.ToLog(Log.MessageType.INFO, $"Compressing...");
			byte[] compressedAllData = DataUtils.CompressLZX2(allData);
			Log.ToLog(Log.MessageType.INFO, $"Successfully compressed");
			int newSize = (int)currentPosInGfxBuffer;
			Log.ToLog(Log.MessageType.INFO, $"New gfx segment size: {newSize}");
			Log.ToLog(Log.MessageType.INFO, $"Old flag - 0x{FlagInfo.Flag2.ToString("X8")}");
			FlagInfo.Flag2 = (int)((uint)FlagInfo.Flag2 & 0xF0000FFF); // стираем старый размер графического сегмента
			while (currentPosInGfxBuffer > 0)
			{
				FlagInfo.Flag2 += 0x00010000;
				currentPosInGfxBuffer -= 16384;
			}
			Log.ToLog(Log.MessageType.INFO, $"New flag - 0x{FlagInfo.Flag2.ToString("X8")}");
			BinaryWriter dataOut = new BinaryWriter(new FileStream(outPath, FileMode.Create));
			dataOut.Write(1381188485);
			dataOut.Write(167772160);
			dataOut.Write(FlagInfo.Flag1.Reverse());
			dataOut.Write(FlagInfo.Flag2.Reverse());
			dataOut.Write(4044551439);
			dataOut.Write(compressedAllData.Length.Reverse());
			dataOut.Write(compressedAllData);
			dataOut.Close();

			if (FlagInfo.RSC85_TotalPSize != newSize) Log.ToLog(Log.MessageType.ERROR, $"{FlagInfo.RSC85_TotalPSize} Checking the size showed that it is not identical to the desired one. But that doesn't mean it won't work in the game.");
			if(currentPosInGfxBuffer!=0) Log.ToLog(Log.MessageType.ERROR, $"Problems when resizing the graphic segment. But that doesn't mean it won't work in the game");
			//Console.ReadKey();

			Log.ToLog(Log.MessageType.INFO, "Finished");
			Console.WriteLine("Press any key to exit");
			Console.ReadKey();
		}
		public static void ReplaceTexturesInXVD(string inFile, string outFile, XmlInfo[] xmlInfo)
		{
			if (!File.Exists(inFile)) throw new Exception("wrong inFile path");
			EndianBinaryReader br;
			br = new EndianBinaryReader(File.OpenRead(inFile));

			RSCHeader hdr = new RSCHeader();
			byte[] magicAsByte = br.ReadBytes(4);
			string magic = Encoding.Default.GetString(magicAsByte);
			if (magic.Contains("CSR"))
			{
				br.Endianness = Endian.BigEndian;
				hdr.Version = magicAsByte[0];
			}
			else
			{
				Log.ToLog(Log.MessageType.ERROR, $"Unknown file {inFile}. Skipped");
				throw new Exception("unk file");
			}
			ResourceUtils.ResourceInfo.version = br.ReadInt32();
			ResourceUtils.FlagInfo.Flag1 = br.ReadInt32();
			if (hdr.Version == 0x85 || hdr.Version == 0x86 || hdr.Version == 0x37) ResourceUtils.FlagInfo.Flag2 = br.ReadInt32();
			uint cmagic = br.ReadUInt32();
			MemoryStream ms;
			if ((cmagic & 0xFFFF0000) != 0x0FF50000)
			{
				Log.ToLog(Log.MessageType.ERROR, "LZX compression not detected. Skipped");
				throw new Exception("unk compression");
			}
			int csize = br.ReadInt32();
			int usize = csize * 4;
			byte[] origBuffer = br.ReadBytes(csize);
			byte[] decompBuffer = new byte[FlagInfo.BaseResourceSizeV + FlagInfo.BaseResourceSizeP];
			DataUtils.DecompressLZX(origBuffer, decompBuffer);
			byte[] sysBuffer = new byte[FlagInfo.BaseResourceSizeV];
			byte[] gfxBuffer = new byte[FlagInfo.BaseResourceSizeP];
			//byte[] gfxBuffer = new byte[0x1000000]; // в начале мы не знаем сколько нужно байтов
			Buffer.BlockCopy(decompBuffer, 0, sysBuffer, 0, FlagInfo.BaseResourceSizeV);
			Buffer.BlockCopy(decompBuffer, sysBuffer.Length, gfxBuffer, 0, FlagInfo.BaseResourceSizeP);
			// читаем системный сигмент
			ms = new MemoryStream(sysBuffer);
			br = new EndianBinaryReader(ms);
			br.Endianness = Endian.BigEndian;
			br.Position = FlagInfo.ResourceStart;
			// узнаем позицию начальной страницы в файле и читаем начальную секцию
			br.Position = ResourceUtils.FlagInfo.RSC85_ObjectStart;
			RageResource.RDRVolumeData volumeData;
			volumeData = ReadRageResource.RDRVolumeData(br);
			//////////////////
			br.Position = volumeData.cDrawable.m_pList;
			uint[] pDrawable = new uint[volumeData.cDrawable.m_wCount];
			for (int a = 0; a < volumeData.cDrawable.m_wCount; a++) pDrawable[a] = br.ReadOffset();
			// читаем каждую секцию drawable
			RageResource.Drawable[] drawable = new RageResource.Drawable[volumeData.cDrawable.m_wCount];
			for (ushort a = 0; a < volumeData.cDrawable.m_wCount; a++)
			{
				br.Position = pDrawable[a];
				drawable[a] = ReadRageResource.Drawable(br);
			}
			// считаем количество секций Model
			uint modelCount = 0;
			RageResource.Collection[,] modelCollection = new RageResource.Collection[volumeData.cDrawable.m_wCount, 4]; // количество секций Drawable и 4 уровня детализации
			string level = "";
			for (ushort a = 0; a < volumeData.cDrawable.m_wCount; a++)
			{
				for (int b = 0; b < 4; b++)
				{
					if (drawable[a].m_pModelCollection[b] != 0)
					{
						br.Position = drawable[a].m_pModelCollection[b];
						modelCount += (modelCollection[a, b] = br.ReadCollections()).m_wCount;

						level = b switch
						{
							0 => "High",
							1 => "Med",
							2 => "Low",
							3 => "Vlow"
						};
						Log.ToLog(Log.MessageType.INFO, $"{level} models count in Drawable{a}: {modelCollection[a, b].m_wCount}");
					}
				}
			}
			RageResource.Model[] model = new RageResource.Model[modelCount];
			// читаем секции Model для каждого уровня детализации
			int currentModel = 0;
			uint[] pModel;
			for (int a = 0; a < drawable.Length; a++)
			{
				for (int b = 0; b < 4; b++) // lod
				{
					if (modelCollection[a, b].m_pList != 0)
					{
						br.Position = modelCollection[a, b].m_pList;
						pModel = new uint[modelCollection[a, b].m_wCount];
						for (int c = 0; c < modelCollection[a, b].m_wCount; c++) pModel[c] = br.ReadOffset();
						for (int c = 0; c < modelCollection[a, b].m_wCount; c++)
						{
							br.Position = pModel[c];
							model[currentModel++] = ReadRageResource.Model(br);
						}
					}
				}
			}
			Log.ToLog(Log.MessageType.INFO, $"Models count: {currentModel}");
			// считаем общее количество секций Geometry
			uint geometryCount = 0;
			for (int a = 0; a < modelCount; a++) geometryCount += model[a].m_pGeometry.m_wCount;
			Log.ToLog(Log.MessageType.INFO, $"Geometries count: {geometryCount}");
			RageResource.Geometry[] geometry = new RageResource.Geometry[geometryCount];
			// и поинтеры
			uint[] pGeometry = new uint[geometryCount];
			uint currentGeometry = 0;
			for (int a = 0; a < modelCount; a++)
			{
				br.Position = model[a].m_pGeometry.m_pList;
				for (int b = 0; b < model[a].m_pGeometry.m_wCount; b++) pGeometry[currentGeometry++] = br.ReadOffset();
			}
			// читаем секции Geometry с остальнимы секциями
			RageResource.VertexBuffer[] vertexBuffer = new RageResource.VertexBuffer[geometryCount];
			RageResource.IndexBuffer[] indexBuffer = new RageResource.IndexBuffer[geometryCount];
			RageResource.VertexDeclaration[] vertexDeclaration = new RageResource.VertexDeclaration[geometryCount];
			for (uint a = 0; a < geometryCount; a++)
			{
				br.Position = pGeometry[a];
				geometry[a] = ReadRageResource.Geometry(br);
				br.Position = geometry[a].m_pVertexBuffer;
				vertexBuffer[a] = ReadRageResource.VertexBuffer(br);
				br.Position = geometry[a].m_pIndexBuffer;
				indexBuffer[a] = ReadRageResource.IndexBuffer(br);
				br.Position = vertexBuffer[a].m_pDeclaration;
				vertexDeclaration[a] = ReadRageResource.VertexDeclaration(br);
			}
			// теперь узнаем позицию первой модели в графическом сегменте
			uint tmp = 0;
			byte[] newGFXBuffer = new byte[tmpGFXSize];

			// теперь редактируем поинтеры к графическому сегменту
			uint tmpPos = 0;
			uint tmpPointer;
			currentPosInGfxBuffer = 0;
			uint unkVal;
			uint vertexCount;
			int byteToRead;
			for (int a = 0; a < geometry.Length; a++)
			{
				vertexCount = vertexBuffer[a].m_wVertexCount;
				tmpPointer = vertexBuffer[a].m_pVertexData;
				if (tmpPointer >= FlagInfo.RSC85_TotalVSize) tmpPointer -= (uint)FlagInfo.RSC85_TotalVSize;
				else continue;
				byteToRead = (int)vertexCount * vertexDeclaration[a].m_nTotaSize;
				Buffer.BlockCopy(gfxBuffer, (int)tmpPointer, newGFXBuffer, (int)currentPosInGfxBuffer, byteToRead);

				tmp = currentPosInGfxBuffer+0x60000000;

				tmpPos = geometry[a].m_pVertexBuffer + 0x8;
				WriteValueToByteArray(ref sysBuffer, tmp.Reverse(), ref tmpPos);
				
				tmpPos = geometry[a].m_pVertexBuffer + 0x10;
				WriteValueToByteArray(ref sysBuffer, tmp.Reverse(), ref tmpPos);

				br.Position = geometry[a].m_pVertexBuffer + 0x1c;
				tmpPos = br.ReadOffset();
				tmpPos += 0x18;
				unkVal = 0;
				br.Position = tmpPos;
				unkVal = br.ReadUInt32();
				unkVal = unkVal & 0x0000000F;
				WriteValueToByteArray(ref sysBuffer, (tmp+ unkVal).Reverse(), ref tmpPos);

				tmpPos = pGeometry[a] + 0x40;
				WriteValueToByteArray(ref sysBuffer, tmp.Reverse(), ref tmpPos);

				currentPosInGfxBuffer += (uint)byteToRead;
				AlignGFX(0x10);

			}
			uint indexCount;
			for (int a = 0; a < geometry.Length; a++)
			{
				indexCount = indexBuffer[a].m_dwIndexCount;
				tmpPointer = indexBuffer[a].m_pIndexData;
				if (tmpPointer >= FlagInfo.RSC85_TotalVSize) tmpPointer -= (uint)FlagInfo.RSC85_TotalVSize;
				else continue;

				byteToRead = (int)indexCount*2;
				Buffer.BlockCopy(gfxBuffer, (int)tmpPointer, newGFXBuffer, (int)currentPosInGfxBuffer, byteToRead);

				tmp = currentPosInGfxBuffer + 0x60000000;

				// поинтер готов. пишем его в файл
				tmpPos = geometry[a].m_pIndexBuffer + 0x8;
				WriteValueToByteArray(ref sysBuffer, tmp.Reverse(), ref tmpPos);

				br.Position = geometry[a].m_pIndexBuffer + 0xc;
				tmpPos = br.ReadOffset();
				tmpPos += 0x18;
				WriteValueToByteArray(ref sysBuffer, tmp.Reverse(), ref tmpPos);

				currentPosInGfxBuffer += (uint)byteToRead;
				AlignGFX(0x10);
			}
			Array.Resize<byte>(ref gfxBuffer, newGFXBuffer.Length);
			Buffer.BlockCopy(newGFXBuffer, 0, gfxBuffer, 0, newGFXBuffer.Length);
			AlignGFX(0x10000);
			ms = new MemoryStream(sysBuffer);
			br = new EndianBinaryReader(ms);
			br.Endianness = Endian.BigEndian;
			br.Position = volumeData.pTexture;
			// все как и в xtd
			RageResource.XTDHeader xtdHeader = new RageResource.XTDHeader();
			xtdHeader = ReadRageResource.XTDHeader(br);
			RageResource.Texture[] texture = new RageResource.Texture[xtdHeader.m_cTexture.m_wCount];
			RageResource.BitMap[] bitMap = new RageResource.BitMap[xtdHeader.m_cTexture.m_wCount];
			uint[] pTexture = new uint[xtdHeader.m_cTexture.m_wCount];
			br.Position = xtdHeader.m_cTexture.m_pList;
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++) pTexture[a] = br.ReadOffset();
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++)
			{
				br.Position = pTexture[a];
				texture[a] = ReadRageResource.Texture(br);
				br.Position = texture[a].m_pBitmap;
				bitMap[a] = ReadRageResource.BitMap(br);
			}
			// мы прочитали нужную информацию. Теперь проверяем имена.
			if (xtdHeader.m_cTexture.m_wCount != xmlInfo.Length) throw new Exception("textures count != xml textures count");
			int[] index = new int[xtdHeader.m_cTexture.m_wCount];
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++)
			{
				string oldName = DataUtils.ReadStringAtOffset(texture[a].m_pName, br);
				for (int b = 0; b < xmlInfo.Length; b++)
				{
					if (!File.Exists(xmlInfo[b].path)) throw new Exception($"\"{xmlInfo[b].path}\" file not found");
					if (/*Path.GetFileName*/(xmlInfo[b].name) == oldName) { index[a] = b; break; }
					if (b == xmlInfo.Length - 1) throw new Exception("bad name");
				}
			}
			// проверили. Можно приступать к модицикации
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++) EditorFunc.WriteNewTextures(xmlInfo[index[a]], ref texture[a], ref bitMap[a], ref gfxBuffer);
			// теперь пишем новые данные в sys сигмент
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++) EditorFunc.WriteNewTexturesToBuffer(pTexture[a], ref texture[a], ref bitMap[a], ref sysBuffer);
			AlignGFX(16384);
			Array.Resize<byte>(ref gfxBuffer, (int)currentPosInGfxBuffer);
			byte[] allData = sysBuffer.Concat(gfxBuffer).ToArray();

			Log.ToLog(Log.MessageType.INFO, $"Compressing...");
			byte[] compressedAllData = DataUtils.CompressLZX2(allData);
			Log.ToLog(Log.MessageType.INFO, $"Successfully compressed");

			int newSize = (int)currentPosInGfxBuffer;
			Log.ToLog(Log.MessageType.INFO, $"New gfx segment size: {newSize}");

			Log.ToLog(Log.MessageType.INFO, $"Old flag - 0x{FlagInfo.Flag2.ToString("X8")}");
			FlagInfo.Flag2 = (int)((uint)FlagInfo.Flag2 & 0xF0000FFF); // стираем старый размер графического сегмента
			while (currentPosInGfxBuffer > 0)
			{
				FlagInfo.Flag2 += 0x00010000;
				currentPosInGfxBuffer -= 16384;
			}
			Log.ToLog(Log.MessageType.INFO, $"New flag - 0x{FlagInfo.Flag2.ToString("X8")}");

			uint version = 0x85;
			BinaryWriter dataOut = new BinaryWriter(new FileStream(outFile, FileMode.Create));
			dataOut.Write(1381188485);
			dataOut.Write(version.Reverse());
			dataOut.Write(FlagInfo.Flag1.Reverse());
			dataOut.Write(FlagInfo.Flag2.Reverse());
			dataOut.Write(4044551439);
			dataOut.Write(compressedAllData.Length.Reverse());
			dataOut.Write(compressedAllData);
			dataOut.Close();

			if (FlagInfo.RSC85_TotalPSize != newSize) Log.ToLog(Log.MessageType.ERROR, $"{FlagInfo.RSC85_TotalPSize}, {newSize} Checking the size showed that it is not identical to the desired one. But that doesn't mean it won't work in the game.");
			if (currentPosInGfxBuffer != 0) Log.ToLog(Log.MessageType.ERROR, $"Problems when resizing the graphic segment. But that doesn't mean it won't work in the game");

			Log.ToLog(Log.MessageType.INFO, "Finished");
			Console.WriteLine("Press any key to exit");
			Console.ReadKey();

		}
		public static void ReplaceTexturesInXFD(string inFile, string outFile, XmlInfo[] xmlInfo)
		{
			if (!File.Exists(inFile)) throw new Exception("wrong inFile path");
			EndianBinaryReader br;
			br = new EndianBinaryReader(File.OpenRead(inFile));

			RSCHeader hdr = new RSCHeader();
			byte[] magicAsByte = br.ReadBytes(4);
			string magic = Encoding.Default.GetString(magicAsByte);
			if (magic.Contains("CSR"))
			{
				br.Endianness = Endian.BigEndian;
				hdr.Version = magicAsByte[0];
			}
			else
			{
				Log.ToLog(Log.MessageType.ERROR, $"Unknown file {inFile}. Skipped");
				throw new Exception("unk file");
			}
			ResourceUtils.ResourceInfo.version = br.ReadInt32();
			ResourceUtils.FlagInfo.Flag1 = br.ReadInt32();
			if (hdr.Version == 0x85 || hdr.Version == 0x86 || hdr.Version == 0x37) ResourceUtils.FlagInfo.Flag2 = br.ReadInt32();
			uint cmagic = br.ReadUInt32();
			MemoryStream ms;
			if ((cmagic & 0xFFFF0000) != 0x0FF50000)
			{
				Log.ToLog(Log.MessageType.ERROR, "LZX compression not detected. Skipped");
				throw new Exception("unk compression");
			}
			int csize = br.ReadInt32();
			int usize = csize * 4;
			byte[] origBuffer = br.ReadBytes(csize);
			byte[] decompBuffer = new byte[FlagInfo.BaseResourceSizeV + FlagInfo.BaseResourceSizeP];
			DataUtils.DecompressLZX(origBuffer, decompBuffer);
			byte[] sysBuffer = new byte[FlagInfo.BaseResourceSizeV];
			byte[] gfxBuffer = new byte[FlagInfo.BaseResourceSizeP];
			Buffer.BlockCopy(decompBuffer, 0, sysBuffer, 0, FlagInfo.BaseResourceSizeV);
			Buffer.BlockCopy(decompBuffer, sysBuffer.Length, gfxBuffer, 0, FlagInfo.BaseResourceSizeP);
			// читаем системный сигмент
			ms = new MemoryStream(sysBuffer);
			br = new EndianBinaryReader(ms);
			br.Endianness = Endian.BigEndian;
			br.Position = ResourceUtils.FlagInfo.RSC85_ObjectStart;
			//////////
			///
			RageResource.FragmentDictionary dict = new RageResource.FragmentDictionary();
			RageResource.Drawable drawable = new RageResource.Drawable();
			dict = ReadRageResource.FragmentDictionary(br);
			if (dict.m_pTextureDictionary == 0) throw new Exception("textures not found");
			br.Position = dict.m_pDrawable;
			drawable = ReadRageResource.Drawable(br);
			// модель
			uint currentModel = 0;
			uint[] pModel = new uint[200];

			string level = "";
			RageResource.Collection[] сModel = new RageResource.Collection[4];
			for (int a = 0; a < 4; a++) // lod
			{
				if (drawable.m_pModelCollection[a] != 0)
				{
					br.Position = drawable.m_pModelCollection[a];
					сModel[a] = br.ReadCollections();
					br.Position = сModel[a].m_pList;
					for (int b = 0; b < сModel[a].m_wCount; b++) pModel[currentModel++] = br.ReadOffset();
					level = a switch
					{
						0 => "High",
						1 => "Med",
						2 => "Low",
						3 => "Vlow"
					};
					Log.ToLog(Log.MessageType.INFO, $"{level} models count: {сModel[a].m_wCount}");
				}
			}
			uint modelCount = currentModel;
			Array.Resize<uint>(ref pModel, (int)modelCount);
			RageResource.Model[] model = new RageResource.Model[pModel.Length];
			Log.ToLog(Log.MessageType.INFO, $"Models count: {currentModel}");
			for (int a = 0; a < pModel.Length; a++)
			{
				br.Position = pModel[a];
				model[a] = ReadRageResource.Model(br);
			}
			uint geometryCount = 0;
			for (int a = 0; a < modelCount; a++) geometryCount += model[a].m_pGeometry.m_wCount;
			uint[] pGeometry = new uint[geometryCount];
			uint currentGeometry = 0;
			Log.ToLog(Log.MessageType.INFO, $"Geometries count: {geometryCount}");
			for (int a = 0; a < modelCount; a++)
			{
				br.Position = model[a].m_pGeometry.m_pList;
				for (int b = 0; b < model[a].m_pGeometry.m_wCount; b++) pGeometry[currentGeometry++] = br.ReadOffset();
			}
			RageResource.Geometry[] geometry = new RageResource.Geometry[geometryCount];
			RageResource.VertexBuffer[] vertexBuffer = new RageResource.VertexBuffer[geometryCount];
			RageResource.IndexBuffer[] indexBuffer = new RageResource.IndexBuffer[geometryCount];
			RageResource.VertexDeclaration[] vertexDeclaration = new RageResource.VertexDeclaration[geometryCount];
			for (int a = 0; a < geometryCount; a++)
			{
				br.Position = pGeometry[a];
				geometry[a] = ReadRageResource.Geometry(br);
				br.Position = geometry[a].m_pVertexBuffer;
				vertexBuffer[a] = ReadRageResource.VertexBuffer(br);
				br.Position = vertexBuffer[a].m_pDeclaration;
				vertexDeclaration[a] = ReadRageResource.VertexDeclaration(br);
				br.Position = geometry[a].m_pIndexBuffer;
				indexBuffer[a] = ReadRageResource.IndexBuffer(br);
			}
			// конец 1
			uint tmp = 0;
			byte[] newGFXBuffer = new byte[tmpGFXSize];

			// теперь редактируем поинтеры к графическому сегменту
			uint tmpPos = 0;
			uint tmpPointer;
			currentPosInGfxBuffer = 0;
			uint vertexCount;
			int byteToRead;
			uint unkVal;
			for (int a = 0; a < geometry.Length; a++)
			{
				vertexCount = vertexBuffer[a].m_wVertexCount;
				tmpPointer = vertexBuffer[a].m_pVertexData;
				if (tmpPointer >= FlagInfo.RSC85_TotalVSize) tmpPointer -= (uint)FlagInfo.RSC85_TotalVSize;
				else continue;

				byteToRead = (int)vertexCount * vertexDeclaration[a].m_nTotaSize;
				Buffer.BlockCopy(gfxBuffer, (int)tmpPointer, newGFXBuffer, (int)currentPosInGfxBuffer, byteToRead);

				tmp = currentPosInGfxBuffer + 0x60000000;

				tmpPos = geometry[a].m_pVertexBuffer + 0x8;
				WriteValueToByteArray(ref sysBuffer, tmp.Reverse(), ref tmpPos);

				tmpPos = geometry[a].m_pVertexBuffer + 0x10;
				WriteValueToByteArray(ref sysBuffer, tmp.Reverse(), ref tmpPos);

				br.Position = geometry[a].m_pVertexBuffer + 0x1c;
				tmpPos = br.ReadOffset();
				tmpPos += 0x18;
				unkVal = 0;
				br.Position = tmpPos;
				unkVal = br.ReadUInt32();
				unkVal = unkVal & 0x0000000F;
				WriteValueToByteArray(ref sysBuffer, (tmp + unkVal).Reverse(), ref tmpPos);

				tmpPos = pGeometry[a] + 0x40;
				WriteValueToByteArray(ref sysBuffer, tmp.Reverse(), ref tmpPos);

				currentPosInGfxBuffer += (uint)byteToRead;
				AlignGFX(0x10);

			}
			uint indexCount;
			for (int a = 0; a < geometry.Length; a++)
			{
				indexCount = indexBuffer[a].m_dwIndexCount;
				tmpPointer = indexBuffer[a].m_pIndexData;
				if (tmpPointer >= FlagInfo.RSC85_TotalVSize) tmpPointer -= (uint)FlagInfo.RSC85_TotalVSize;
				else continue;

				byteToRead = (int)indexCount * 2;
				Buffer.BlockCopy(gfxBuffer, (int)tmpPointer, newGFXBuffer, (int)currentPosInGfxBuffer, byteToRead);

				tmp = currentPosInGfxBuffer + 0x60000000;

				// поинтер готов. пишем его в файл
				tmpPos = geometry[a].m_pIndexBuffer + 0x8;
				WriteValueToByteArray(ref sysBuffer, tmp.Reverse(), ref tmpPos);

				br.Position = geometry[a].m_pIndexBuffer + 0xc;
				tmpPos = br.ReadOffset();
				tmpPos += 0x18;
				WriteValueToByteArray(ref sysBuffer, tmp.Reverse(), ref tmpPos);

				currentPosInGfxBuffer += (uint)byteToRead;
				AlignGFX(0x10);
			}
			Array.Resize<byte>(ref gfxBuffer, newGFXBuffer.Length);
			Buffer.BlockCopy(newGFXBuffer, 0, gfxBuffer, 0, newGFXBuffer.Length);
			AlignGFX(0x10000);
			ms = new MemoryStream(sysBuffer);
			br = new EndianBinaryReader(ms);
			br.Endianness = Endian.BigEndian;
			br.Position = dict.m_pTextureDictionary;
			// все как и в xtd
			RageResource.XTDHeader xtdHeader = new RageResource.XTDHeader();
			xtdHeader = ReadRageResource.XTDHeader(br);
			RageResource.Texture[] texture = new RageResource.Texture[xtdHeader.m_cTexture.m_wCount];
			RageResource.BitMap[] bitMap = new RageResource.BitMap[xtdHeader.m_cTexture.m_wCount];
			uint[] pTexture = new uint[xtdHeader.m_cTexture.m_wCount];
			br.Position = xtdHeader.m_cTexture.m_pList;
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++) pTexture[a] = br.ReadOffset();
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++)
			{
				br.Position = pTexture[a];
				texture[a] = ReadRageResource.Texture(br);
				br.Position = texture[a].m_pBitmap;
				bitMap[a] = ReadRageResource.BitMap(br);
			}
			// мы прочитали нужную информацию. Теперь проверяем имена.
			if (xtdHeader.m_cTexture.m_wCount != xmlInfo.Length) throw new Exception("textures count != xml textures count");
			int[] index = new int[xtdHeader.m_cTexture.m_wCount];
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++)
			{
				string oldName = DataUtils.ReadStringAtOffset(texture[a].m_pName, br);
				for (int b = 0; b < xmlInfo.Length; b++)
				{
					if (!File.Exists(xmlInfo[b].path)) throw new Exception($"\"{xmlInfo[b].path}\" file not found");
					if (/*Path.GetFileName*/(xmlInfo[b].name) == oldName) { index[a] = b; break; }
					if (b == xmlInfo.Length - 1) throw new Exception("bad name");
				}
			}
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++) EditorFunc.WriteNewTextures(xmlInfo[index[a]], ref texture[a], ref bitMap[a], ref gfxBuffer);
			// теперь пишем новые данные в sys сигмент
			for (int a = 0; a < xtdHeader.m_cTexture.m_wCount; a++) EditorFunc.WriteNewTexturesToBuffer(pTexture[a], ref texture[a], ref bitMap[a], ref sysBuffer);
			AlignGFX(16384);
			Array.Resize<byte>(ref gfxBuffer, (int)currentPosInGfxBuffer);
			byte[] allData = sysBuffer.Concat(gfxBuffer).ToArray();

			Log.ToLog(Log.MessageType.INFO, $"Compressing...");
			byte[] compressedAllData = DataUtils.CompressLZX2(allData);
			Log.ToLog(Log.MessageType.INFO, $"Successfully compressed");

			int newSize = (int)currentPosInGfxBuffer;
			Log.ToLog(Log.MessageType.INFO, $"New gfx segment size: {newSize}");

			Log.ToLog(Log.MessageType.INFO, $"Old flag - 0x{FlagInfo.Flag2.ToString("X8")}");
			FlagInfo.Flag2 = (int)((uint)FlagInfo.Flag2 & 0xF0000FFF); // стираем старый размер графического сегмента
			while (currentPosInGfxBuffer > 0)
			{
				FlagInfo.Flag2 += 0x00010000;
				currentPosInGfxBuffer -= 16384;
			}
			Log.ToLog(Log.MessageType.INFO, $"New flag - 0x{FlagInfo.Flag2.ToString("X8")}");

			uint version = 1;
			BinaryWriter dataOut = new BinaryWriter(new FileStream(outFile, FileMode.Create));
			dataOut.Write(1381188485);
			dataOut.Write(version.Reverse());
			dataOut.Write(FlagInfo.Flag1.Reverse());
			dataOut.Write(FlagInfo.Flag2.Reverse());
			dataOut.Write(4044551439);
			dataOut.Write(compressedAllData.Length.Reverse());
			dataOut.Write(compressedAllData);
			dataOut.Close();

			if (FlagInfo.RSC85_TotalPSize != newSize) Log.ToLog(Log.MessageType.ERROR, $"{FlagInfo.RSC85_TotalPSize}, {newSize} Checking the size showed that it is not identical to the desired one. But that doesn't mean it won't work in the game.");
			if (currentPosInGfxBuffer != 0) Log.ToLog(Log.MessageType.ERROR, $"Problems when resizing the graphic segment. But that doesn't mean it won't work in the game");

			Log.ToLog(Log.MessageType.INFO, "Finished");
			Console.WriteLine("Press any key to exit");
			Console.ReadKey();
		}
		public static void XTD_Main()
		{
		}
	}
}
