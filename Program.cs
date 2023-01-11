using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Converter.Utils;
using System.Numerics;
using System.Collections;
using Converter;
using Converter.openformat;

namespace ConsoleApp1
{
	internal class Program
	{
		public static int cpuSize;
		public static string fileName;
		public static string fileMask;
		static unsafe void Main(string[] args)
		{
			//Array.Resize<string>(ref args, 1);
			Program.fileName = args[0];
			//{
				//IV_Animation.ExportWAD();
				//return;
			//}
			//Program.fileName = "C:\\Users\\d3g0n\\Desktop\\3d\\tile_146_0.xvd";
			if (args.Length < 1) return;
			fileMask = Other.GetFileMask(fileName);
			FileStream file;
			file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
			EndianBinaryReader br = new EndianBinaryReader(file);
			bool isCompressed = true;
			br.Endianness = Endian.BigEndian;
			if (isCompressed)
			{
				uint magic = br.ReadUInt32();
				if (magic == 0x52534305 || magic == 0x52534385 || magic == 0x52534306 || magic == 0x52534386 || magic == 0x52534337) br.Endianness = Endian.LittleEndian;

				uint version = br.ReadUInt32();
				uint flags = br.ReadUInt32();
				uint secondFlags = 0;
				
				if (magic == 0x85435352 || magic == 0x86435352 || magic == 0x37435352) secondFlags = br.ReadUInt32();
				cpuSize = 0;
				int gpuSize = 0;
				if (magic == 0x05435352 || magic == 0x06435352)// mc:la iv rdr mp3
				{
					cpuSize = XCompress.GetCPUSizeRSC5(flags);
					gpuSize = XCompress.GetGPUSizeRSC5(flags);
				}
				else if (magic == 0x85435352 || magic == 0x86435352)// rdr
				{
					cpuSize = XCompress.GetCPUSizeRSC85(flags, secondFlags);
					gpuSize = XCompress.GetGPUSizeRSC85(flags, secondFlags);
				}
				else if (magic == 0x37435352)// v
				{
					cpuSize = XCompress.GetSizeRSC37(flags, 0x2000);
					gpuSize = XCompress.GetSizeRSC37(secondFlags, 0x2000);
				}
				else return;

				uint cmagic = br.ReadUInt32();
				if ((cmagic & 0xFFFF0000) != 0x0FF50000) { Console.WriteLine("It's not lzx."); return; }
				int csize = br.ReadInt32();
				int usize = csize * 4;
				byte[] origBuffer = br.ReadBytes(csize);
				file.Close();
				// min. size
				if (usize < 203968) usize = 203968;
				byte[] decompBuffer = XCompress.Decompress(origBuffer, csize, usize, false);
				Array.Resize<byte>(ref decompBuffer, cpuSize + gpuSize);
				//string decompressedFileName = fileName + ".tmp";
				//File.WriteAllBytes(decompressedFileName, decompBuffer);
				MemoryStream memStream = new MemoryStream(decompBuffer);
				if (magic == 0x85435352 && version == 133)
				{
					if (!RDR_VolumeData.ReadVolumeData(memStream, true))
					{
						Console.WriteLine("Error");
						Console.ReadKey();
					}
				}
				/*else if (magic == 0x05435352 && version == 6)
				{
					if (!RDR_Animation.Animation(memStream, true))
					{
						Console.WriteLine("Error to export anims");
						Console.ReadKey();
					}

				}*/



				return;

			}
		}
	}
}
