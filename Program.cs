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
using Converter.openFormats;
using static Converter.ResourceUtils;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ConsoleApp1
{
	internal class Program
	{
		static unsafe void Main(string[] args)
		{
			//Array.Resize<string>(ref args, 1);
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\player_elegantsuit.xft";
			//args[0] = "C:\\Users\\d3g0n\\source\\repos\\Converter\\bin\\x64\\Debug\\net7.0\\test.xvd"; // тестовая модель. создана з нуля
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\tile_130_10.xvd";
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\steamer01x_hilod.xfd";
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\pln_derrick02x_oilrig02anim_hilod.xtd";
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\swterrain.xsi";

			if (args.Length < 1) { Console.WriteLine("Usage: Converter.exe input_file"); return; }
			Converter.FileInfo.fileName = args[0];
			using (EndianBinaryReader br = new EndianBinaryReader(File.OpenRead(Converter.FileInfo.fileName)))
			{
				RSCHeader hdr = new RSCHeader();
				byte[] magic = br.ReadBytes(4);
				if (magic[0] != 82)
				{
					Array.Reverse(magic, 0, magic.Length);
					br.Endianness = Endian.BigEndian;
				}
				hdr.Magic = Encoding.ASCII.GetBytes(Encoding.Default.GetString(magic.Take(3).ToArray()));
				hdr.Version = magic[3];

				if (Encoding.Default.GetString(hdr.Magic) != "RSC") return;
				ResourceUtils.ResourceInfo.version = br.ReadInt32();
				ResourceUtils.FlagInfo.Flag1 = br.ReadInt32();
				if (hdr.Version == 0x85 || hdr.Version == 0x86 || hdr.Version == 0x37)
					ResourceUtils.FlagInfo.Flag2 = br.ReadInt32();
				uint cmagic = br.ReadUInt32();
				if ((cmagic & 0xFFFF0000) != 0x0FF50000)
				{
					// it's not a XCompress so move the position back
					br.BaseStream.Position -= 4;

					int dLen = FlagInfo.BaseResourceSizeP + FlagInfo.BaseResourceSizeV;
					byte[] origBuffer = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length - br.BaseStream.Position));
					byte[] decompBuffer = DataUtils.DecompressDeflate(origBuffer, dLen, noHeader: false);
					ReadMemoryData(hdr, decompBuffer);
					byte[] sys = new byte[FlagInfo.BaseResourceSizeV];
					byte[] gfx = new byte[FlagInfo.BaseResourceSizeP];
					Buffer.BlockCopy(decompBuffer, 0, sys, 0, sys.Length);
					Buffer.BlockCopy(decompBuffer, sys.Length, gfx, 0, gfx.Length);
					File.WriteAllBytes($"{Converter.FileInfo.filePath}\\{Converter.FileInfo.baseFileName}.sys", sys);
					File.WriteAllBytes($"{Converter.FileInfo.filePath}\\{Converter.FileInfo.baseFileName}.gfx", gfx);
				}
				else
				{
					int csize = br.ReadInt32();
					int usize = csize * 4;
					if (usize < 203968) usize = 203968;
					byte[] origBuffer = br.ReadBytes(csize);
					byte[] decompBuffer = XCompress.Decompress(origBuffer, csize, usize, false);
					// 
					//byte[] sys = new byte[FlagInfo.BaseResourceSizeV];
					//byte[] gfx = new byte[FlagInfo.BaseResourceSizeP];
					//Buffer.BlockCopy(decompBuffer, 0, sys, 0, sys.Length);
					//Buffer.BlockCopy(decompBuffer, sys.Length, gfx, 0, gfx.Length);
					//File.WriteAllBytes($"{Converter.FileInfo.filePath}\\{Converter.FileInfo.baseFileName}.sys", sys);
					//File.WriteAllBytes($"{Converter.FileInfo.filePath}\\{Converter.FileInfo.baseFileName}.gfx", gfx);
					//
					ReadMemoryData(hdr, decompBuffer);
				}
			}
		}
		private static void ReadMemoryData(RSCHeader hdr, byte[] data)
		{
			using (MemoryStream ms = new MemoryStream(data))
			{
				if (hdr.Version == 0x85 && ResourceUtils.ResourceInfo.version == 133)
				{
					if (!RDR_VolumeData.ReadVolumeData(ms, true))
					{
						throw new Exception("Error while reading RDR model data.");
					}
				}
				/*
				else if (rsc.m_Header.Version == 5 && rsc.m_ResourceVersion == 6)
				{
					if (!RDR_Animation.Animation(ms, true))
					{
						throw new Exception("Error while reading RDR animation data");
					}
				}
				*/
				else if (hdr.Version == 0x85 && ResourceUtils.ResourceInfo.version == 138)
				{
					if (!RDR_Fragment.ReadFragment(ms, true))
					{
						throw new Exception("Error while reading RDR fragment model.");
					}
				}
				else if (hdr.Version == 0x85 && ResourceUtils.ResourceInfo.version == 1)
				{
					if (!RDR_FragmentDictionary.ReadFragmentDictionary(ms, true))
					{
						throw new Exception("Error while reading RDR Fragment Dictionary.");
					}
				}
				else if (hdr.Version == 0x85 && ResourceUtils.ResourceInfo.version == 10)
				{
					EndianBinaryReader br = new EndianBinaryReader(ms);
					if (true) br.Endianness = Endian.BigEndian;// 0 - lit, 1 - big
					if (!TextureDictionary.ReadTextureDictionary(br, (uint)ResourceUtils.FlagInfo.RSC85_ObjectStart))
					{
						throw new Exception("Error while reading RDR Texture Dictionary.");
					}
					br.Close();
				}
			}
		}

	}
	public class RSCHeader
	{
		public byte[] Magic = new byte[3];
		public byte Version;
		public RSCHeader()
		{
		}
	}
}
