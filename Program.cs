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
using System.Diagnostics;
using System.Runtime.InteropServices;
using HelixToolkit.Wpf.SharpDX.Helper;
using Converter.utils;
using Converter.Test;

namespace ConsoleApp1
{
	internal class Program
	{
		public static void OpenBrowser(string url)
		{
			try
			{
				Process.Start(url);
			}
			catch
			{
				// hack because of this: https://github.com/dotnet/corefx/issues/10361
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					Process.Start("xdg-open", url);
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					Process.Start("open", url);
				}
				else
				{
					throw;
				}
			}
		}
		static void Main(string[] args)
		{
			//Test_Zone.LOADDDS();
			//Array.Resize<string>(ref args, 1);
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\player_elegantsuit.xft";
			//args[0] = "C:\\Users\\d3g0n\\source\\repos\\Converter\\bin\\x64\\Debug\\net7.0\\test.xvd"; // тестовая модель. создана з нуля
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\tile_130_10.xvd";
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\steamer01x_hilod.xfd";
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\pln_derrick02x_oilrig02anim_hilod.xtd";
			//	args[0] = "E:\\mcla\\0x2f5680af.xERR";
			//args[0] = "C:\\Users\\d3g0n\\Desktop\\3d\\rdr2.test";
			//	args[0] = "E:\\mcla\\Files\\0xa2358e9d.1";
			//	args[0] = "E:\\mcla\\0xe66973c.3";
			//args[0] = "E:\\rdr\\smic_st_saguarocactus01.xtd";
			//args[0] = "C:\\Users\\im\\Desktop\\xtd1\\armadillo.xvd";

			//args[0] = "E:\\mcla\\0xf0488f14";
			// место зарезервировано под тесты
			//byte[] data = new byte[0];
			//LZXTestUtils.DecompressConverterData(ref data, $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\\RDR_Shadermanager.xml");
			//File.WriteAllBytes("tmp.txt", data);
			//return;
			//int a = 0x70;
			//Console.WriteLine((a&0x40).ToString("X8"));
			//RDR_XTD_CREATOR.ReplaceTexturesInXVD("E:\\rdr\\arm_saloon01x.xvd", "E:\\rdr\\arm_saloon01x_NEW.xvd");
			//return;
			// Console.WriteLine(TextureUtils.CalculateTextureSize(256, 256, 7, "DXT1"));
			//return;
			//RDR_XTD_CREATOR.SwizzleTextureDXT1("E:\\rdr\\Untitled.dds", "E:\\rdr\\Untitled2.dds");
			//return;
			//RDR_XTD_CREATOR.SwizzleTextureDXT5("E:\\rdr\\gri_cliff_122_n.dds", "E:\\rdr\\gri_cliff_12_nSWIZZ.dds", 1024, 1024);
			//RDR_XTD_CREATOR.SwizzleTextureDXT1("E:\\rdr\\gri_cliff_123.dds", "E:\\rdr\\gri_cliff_123_SWIZZ.dds", 2048, 2048);

			//return;
			/*Array.Resize<string>(ref args, 4);
			args[0] = "E:\\rdr\\die_01_dst_x_hilod.xtd";
			args[1] = "E:\\rdr\\die_01_dst_x_hilod.sys";
			args[2] = "E:\\rdr\\die_01_dst_x_hilod.gfx";
			args[3] = "E:\\rdr\\tmp2.xtd";
			//args[0] = "E:\\rdr\\gri_cliff_12_hilod.xtd";
			//args[1] = "E:\\rdr\\gri_cliff_12_hilod.sys";
			//args[2] = "E:\\rdr\\gri_cliff_12_hilod.gfx";
			//args[3] = "E:\\rdr\\tmp.xtd";


			string origFilepath = args[0];
			string sysPath = args[1];
			string gfxPath = args[2];

			byte[] sysArray = File.ReadAllBytes(sysPath);
			byte[] gfxArray = File.ReadAllBytes(gfxPath);
			byte[] sysgfxArray = sysArray.Concat(gfxArray).ToArray();
			//byte[] sysgfxArray = new byte[sysArray.Length + gfxArray.Length];
			//Buffer.BlockCopy(sysArray, 0, sysgfxArray, 0, sysArray.Length);
			//Buffer.BlockCopy(gfxArray, 0, sysgfxArray, sysArray.Length, gfxArray.Length);


			BinaryReader br2 = new BinaryReader(File.OpenRead(origFilepath));
			uint magicNew = br2.ReadUInt32();
			uint versionNew = br2.ReadUInt32();
			uint flags1New = br2.ReadUInt32();
			uint flags2New = br2.ReadUInt32();
			uint lzxMagicNew = 4044551439;
			byte[] compressed = DataUtils.CompressLZX2(sysgfxArray);
			int lzxSizeNew = compressed.Length;
			lzxSizeNew = EBRExtensions.Reverse(lzxSizeNew);

			BinaryWriter dataOut = new BinaryWriter(new FileStream(args[3], FileMode.Create));
			dataOut.Write(magicNew);
			dataOut.Write(versionNew);
			dataOut.Write(flags1New);
			dataOut.Write(flags2New);
			dataOut.Write(lzxMagicNew);
			dataOut.Write(lzxSizeNew);
			dataOut.Write(DataUtils.CompressLZX2(sysgfxArray));
			dataOut.Close();
			return;*/

			//RDR_ShaderManagerCreator.ScanFXCShaders(exportVarNames: true);
			//return;
			//byte[] testobj = File.ReadAllBytes("E:\\tenis\\out\\tenis\\assets\\resource\\crowd\\crowd.rpf");

			//for (int a = 0; a < compressedTestOBJ.Length; a++) compressedTestOBJ[a] = 0xcd;
			//return;
			//byte[] DecompressedData = new byte[4096];
			//DataUtils.DecompressLZX(TestOBJ2, DecompressedData);

			//RDR_ShaderManagerCreator.ScanFXCShaders(useLZX: true);
			//byte[] tmp = File.ReadAllBytes("RDR_FileNames.txt");
			//LZXTestUtils.CompressAndWriteConverterData(tmp, "RDR_FileNames.data");
			//return;
			// конец места зарезервировано под тесты\
			
			//Array.Resize<string>(ref args, 3);
			//args[0] = "-a";
			//args[1] = "-extractXtdInfo";
			//args[2] = "C:\\Users\\im\\Downloads\\lodstreamingmap.xtd";
			
			//args[3] = "E:\\rdr\\New folder\\";

			//Array.Resize<string>(ref args, 3);
			//args[0] = "-a";
			//args[1] = "-extractFlashTexturesInfo";
			//args[2] = "E:\\rdr\\wares.xsf";

			//Array.Resize<string>(ref args, 5);
			//args[0] = "/cmode";
			//args[1] = "-replaceXtdInfo";
			//args[2] = "E:\\rdr\\arm_saloon01x.xvd";
			//args[3] = "E:\\rdr\\arm_saloon01x_NEW.xvd";
			//args[4] = "E:\\rdr\\arm_saloon01x.xml";

			//Array.Resize<string>(ref args, 5);
			//args[0] = "/cmode";
			//args[1] = "-compressRSC85";
			//args[2] = "E:\\rdr\\gri_cliff_12_hilod.xml";
			//args[3] = "E:\\rdr\\gri_cliff_12_hilod_NEW2.xtd";
			//args[4] = "calculateFlags:true";

			Settings.ReadSettings();
			if (args.Length < 1) { Console.WriteLine("Usage: Converter.exe input_file"); return; }

			// подгружаем шейдеры
			RDR_ShaderManager.LoadShaders();
			Log.ToLog(Log.MessageType.INFO, $"Loaded {RDR_ShaderManager.ShaderParams.Length} Red Dead Redemption shaders");
			RDR_FileNames.LoadRDRFileNames();
			Log.ToLog(Log.MessageType.INFO, $"Loaded {RDR_FileNames.fileNames.Count} Red Dead Redemption names");

			if (Settings.bSaveUnpackedResource) Log.ToLog(Log.MessageType.INFO, "Unpacking mode enabled");

			// меняем культуру
			System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
			customCulture.NumberFormat.NumberDecimalSeparator = Settings.sNumberDecimalSeparator;
			//customCulture.NumberFormat.CurrencySymbol = "+";
			System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

			//return;
			//if (!File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\XnaNative.dll")) 
			//	throw new Exception("XnaNative.dll not found. " +
			//	"You can take it in a folder along the path C:\\Program Files (x86)\\Common Files\\Microsoft Shared\\XNA\\Framework\\v3.0 " +
			//	"or download it from the Internet");
			if (!File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\XnaNative.dll"))
			{
				Console.WriteLine("XnaNative.dll not found. The download link should open automatically.");
				OpenBrowser("https://drive.google.com/file/d/1YRk8I7r0MM3fopS-w_LE0CF_Q5oPOa22/view?usp=share_link");
				Console.ReadKey();
				return;
			}
			if (!Environment.Is64BitOperatingSystem)
				throw new Exception("This exe is compiled for a 32 bit system, but my check will not let you run it on a 32 bit system XD. " +
					"Go download visual studio to remove the check or install yourself windows x64");
			if (args[0] == "-a")
			{
				Cmode.RunCmode(args);
				return;
			}
			Converter.FileInfo.fileName = args[0];
			using (EndianBinaryReader br = new EndianBinaryReader(File.OpenRead(Converter.FileInfo.fileName)))
			{
				RSCHeader hdr = new RSCHeader();
				byte[] magicAsByte = br.ReadBytes(4);
				string magic = Encoding.Default.GetString(magicAsByte);
				if (magic.Contains("RSC"))
				{
					br.Endianness = Endian.LittleEndian;
					hdr.Version = magicAsByte[3];
					Log.ToLog(Log.MessageType.INFO, "Endian: LitEndian");
				}
				else if (magic.Contains("CSR"))
				{
					br.Endianness = Endian.BigEndian;
					hdr.Version = magicAsByte[0];
					Log.ToLog(Log.MessageType.INFO, "Endian: BigEndian");
				}
				else { Log.ToLog(Log.MessageType.ERROR, $"RSC Header not found"); throw new Exception("Unknown file"); }
				ResourceUtils.ResourceInfo.version = br.ReadInt32();
				ResourceUtils.FlagInfo.Flag1 = br.ReadInt32();
				if (hdr.Version == 0x85 || hdr.Version == 0x86 || hdr.Version == 0x37) ResourceUtils.FlagInfo.Flag2 = br.ReadInt32();
				uint cmagic = br.ReadUInt32();
			
				Log.ToLog(Log.MessageType.INFO, $"RSC Version: {hdr.Version}");
				Log.ToLog(Log.MessageType.INFO, $"Resource version: {ResourceUtils.ResourceInfo.version}");
				Log.ToLog(Log.MessageType.INFO, $"System memory segment: {FlagInfo.BaseResourceSizeV}, graphics memory segment: {FlagInfo.BaseResourceSizeP}");
				if ((cmagic & 0xFFFF0000) == 0x78DA0000)
				{
					Log.ToLog(Log.MessageType.INFO, $"Compression: Zlib");
					br.BaseStream.Position -= 4;
					int dLen = FlagInfo.BaseResourceSizeP + FlagInfo.BaseResourceSizeV;
					byte[] origBuffer = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length - br.BaseStream.Position));
					byte[] decompBuffer = DataUtils.DecompressDeflate(origBuffer, dLen, noHeader: false);
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
					Log.ToLog(Log.MessageType.ERROR, "PS3 resources not supported. Only unpacking");
				}
				else if ((cmagic & 0xFFFF0000) == 0x0FF50000)
				{
					Log.ToLog(Log.MessageType.INFO, $"Compression: LZX");
					int csize = br.ReadInt32();
					int usize = csize * 4;
					byte[] origBuffer = br.ReadBytes(csize);
					byte[] decompBuffer = new byte[FlagInfo.BaseResourceSizeV + FlagInfo.BaseResourceSizeP];
					DataUtils.DecompressLZX(origBuffer, decompBuffer);
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
					//
					int result = ReadMemoryData(hdr, decompBuffer);
					switch (result)
					{
						case 0:
							Log.ToLog(Log.MessageType.ERROR, "This resource is not supported. Press any key to exit.");
							Console.ReadKey();
							break;
						case 1:
							Log.ToLog(Log.MessageType.INFO, "Finished. Press any key to exit.");
							Console.ReadKey();
							break;
					}
				}
				else
				{
					Log.ToLog(Log.MessageType.ERROR, "Zlib or LZX compression not detected.");
					throw new Exception("Unknown compression");
				}

			}
		}
		private static int ReadMemoryData(RSCHeader hdr, byte[] data)
		{
			using (MemoryStream ms = new MemoryStream(data))
			{
				if (hdr.Version == 0x85 && ResourceUtils.ResourceInfo.version == 133)
				{
					if (!RDR_VolumeData.ReadVolumeData(ms, true))
					{
						throw new Exception("Error while converting RDR volume data");
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
				else if (hdr.Version == 0x85 && ResourceUtils.ResourceInfo.version == 902)
				{
					if (!RDR_SectorInfo.ReadXSIFile(ms))
					{
						throw new Exception("Error while reading RDR Fragment Dictionary.");
					}
				}
				else if (hdr.Version == 0x85 && ResourceUtils.ResourceInfo.version == 10)
				{
					Log.ToLog(Log.MessageType.INFO, "Red Dead Redemption Texture Dictionary");
					Log.ToLog(Log.MessageType.INFO, $"RSC85 Res Start: 0x{ResourceUtils.FlagInfo.RSC85_ObjectStart.ToString("X8")}");

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
				else if (hdr.Version == 0x5 && ResourceUtils.ResourceInfo.version == 102)
				{
					if (!MCLA_Drawable.ReadDrawable(ms, true))
					{
						throw new Exception("Error while reading MC:LA Drawable.");
					}
				}
				else if (hdr.Version == 0x5 && ResourceUtils.ResourceInfo.version == 131)
				{
					if (!MCLA_131.Read(ms, true))
					{
						throw new Exception("Error while reading MC:LA Res131.");
					}
				}
				else return 0;
			}
			return 1;
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
