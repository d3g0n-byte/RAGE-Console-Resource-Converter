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
using System.Data;


namespace ConsoleApp1
{
	internal class Program
	{
		static unsafe void Main(string[] args)
		{
		//	Array.Resize<string>(ref args, 1);
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\player_elegantsuit.xft";
			//args[0] = "C:\\Users\\d3g0n\\source\\repos\\Converter\\bin\\x64\\Debug\\net7.0\\test.xvd"; // тестовая модель. создана з нуля
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\tile_130_10.xvd";
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\steamer01x_hilod.xfd";
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\pln_derrick02x_oilrig02anim_hilod.xtd";
			//	args[0] = "E:\\mcla\\0x2f5680af.xERR";
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\rdr2.test";
			//	args[0] = "E:\\mcla\\Files\\0xa2358e9d.1";
		//	args[0] = "E:\\mcla\\drv_fa_001_set.xrsc";

			//args[0] = "E:\\mcla\\0xf0488f14";
			// место зарезервировано под тесты

			Settings.ReadSettings();
		
			// меняем культуру
			System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
			customCulture.NumberFormat.NumberDecimalSeparator = Settings.sNumberDecimalSeparator;
			System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;


			if (!File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\XnaNative.dll")) 
				throw new Exception("XnaNative.dll not found. " +
				"You can take it in a folder along the path C:\\Program Files (x86)\\Common Files\\Microsoft Shared\\XNA\\Framework\\v3.0 " +
				"or download it from the Internet");
			if (args.Length < 1) { Console.WriteLine("Usage: Converter.exe input_file"); return; }
			if (!Environment.Is64BitOperatingSystem)
				throw new Exception("This exe is compiled for a 32 bit system, but my check will not let you run it on a 32 bit system XD. " +
					"Go download visual studio to remove the check or install yourself windows x64");
			Converter.FileInfo.fileName = args[0];
			using (EndianBinaryReader br = new EndianBinaryReader(File.OpenRead(Converter.FileInfo.fileName)))
			{
				RSCHeader hdr = new RSCHeader();
				string magic = Encoding.Default.GetString(br.ReadBytes(4));
				if (magic.Contains("RSC")) { br.Endianness = Endian.LittleEndian; hdr.Version = (byte)magic[3]; }
				else if (magic.Contains("CSR")) { br.Endianness = Endian.BigEndian; hdr.Version = (byte)magic[0]; }
				else throw new Exception("Unknown file");
				ResourceUtils.ResourceInfo.version = br.ReadInt32();
				ResourceUtils.FlagInfo.Flag1 = br.ReadInt32();
				if (hdr.Version == 0x85 || hdr.Version == 0x86 || hdr.Version == 0x37) ResourceUtils.FlagInfo.Flag2 = br.ReadInt32();
				uint cmagic = br.ReadUInt32();
				if ((cmagic & 0xFFFF0000) != 0x0FF50000)
				{
					br.BaseStream.Position -= 4;
					int dLen = FlagInfo.BaseResourceSizeP + FlagInfo.BaseResourceSizeV;
					byte[] origBuffer = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length - br.BaseStream.Position));
					byte[] decompBuffer = DataUtils.DecompressDeflate(origBuffer, dLen, noHeader: false);
					//ReadMemoryData(hdr, decompBuffer);
					byte[] sys = new byte[FlagInfo.BaseResourceSizeV];
					byte[] gfx = new byte[FlagInfo.BaseResourceSizeP];
					if (Settings.bSaveUnpackedResource)
					{
						Buffer.BlockCopy(decompBuffer, 0, sys, 0, sys.Length);
						Buffer.BlockCopy(decompBuffer, sys.Length, gfx, 0, gfx.Length);
						File.WriteAllBytes($"{Converter.FileInfo.filePath}\\{Converter.FileInfo.baseFileName}.sys", sys);
						File.WriteAllBytes($"{Converter.FileInfo.filePath}\\{Converter.FileInfo.baseFileName}.gfx", gfx);
						return;
					}
				}
				else
				{
					int csize = br.ReadInt32();
					int usize = csize * 4;
					byte[] origBuffer = br.ReadBytes(csize);
					//byte[] decompBuffer = XCompress.Decompress(origBuffer, csize, usize, false);
					byte[] decompBuffer = new byte[FlagInfo.BaseResourceSizeV+ FlagInfo.BaseResourceSizeP];
					DataUtils.DecompressLZX(origBuffer, decompBuffer);
					byte[] sys = new byte[FlagInfo.BaseResourceSizeV];
					byte[] gfx = new byte[FlagInfo.BaseResourceSizeP];
					if(Settings.bSaveUnpackedResource)
					{
						Buffer.BlockCopy(decompBuffer, 0, sys, 0, sys.Length);
						Buffer.BlockCopy(decompBuffer, sys.Length, gfx, 0, gfx.Length);
						File.WriteAllBytes($"{Converter.FileInfo.filePath}\\{Converter.FileInfo.baseFileName}.sys", sys);
						File.WriteAllBytes($"{Converter.FileInfo.filePath}\\{Converter.FileInfo.baseFileName}.gfx", gfx);
						return;
					}
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
				else if (hdr.Version == 0x5 && ResourceUtils.ResourceInfo.version == 109)
				{
					if (!IV_Textures.ExportTexturesFromIVRes(ms, true, ResourceUtils.ResourceInfo.version))
					{
						throw new Exception("Error while reading IV Textures.");
					}
				}
				else if (hdr.Version == 0x5 && ResourceUtils.ResourceInfo.version == 83)
				{
					if (!MCLA_83.Read(ms, true))
					{
						throw new Exception("Error while reading MC:LA Res83.");
					}
				}
				else if (hdr.Version == 0x5 && ResourceUtils.ResourceInfo.version == 63)
				{
					if (!MCLA_63.Read(ms, true))
					{
						throw new Exception("Error while reading MC:LA Model.");
					}
				}
				else if (hdr.Version == 0x5 && ResourceUtils.ResourceInfo.version == 9)
				{
					EndianBinaryReader br = new EndianBinaryReader(ms);
					if (true) br.Endianness = Endian.BigEndian;
					if (!TextureDictionary.ReadTextureDictionary(br, 0))
					{
						throw new Exception("Error while reading MCLA Texture Dictionary.");
					}
					br.Close();
				}
				else if (hdr.Version == 0x5 && ResourceUtils.ResourceInfo.version == 1)
				{
					if (!MCLA_1.Read(ms, true))
					{
						throw new Exception("Error while reading MC:LA Res1.");
					}
				}
				else if (hdr.Version == 0x5 && ResourceUtils.ResourceInfo.version == 6)
				{
					if (!MCLA_XRSC.Read(ms, true))
					{
						throw new Exception("Error while reading MC:LA XRSC.");
					}
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
