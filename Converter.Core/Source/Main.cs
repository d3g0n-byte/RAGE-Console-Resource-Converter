using System;
using System.IO;
using System.Text;
using Converter.Core.Utils;
using Converter.Core.Utils.Compression;
using Converter.Core.Games.RDR;
using Converter.Core.Games.MCLA;
using Converter.Core.Games.IV;

namespace Converter.Core
{
	public static class Main
	{
		public static bool useVerboseMode = false;
		public static bool useVeryVerboseMode = false;
		public static string inputPath;
		public static string outputPath;

		/// <summary>
		/// Process the single input file
		/// </summary>
		/// <param name="filename">Input file path</param>
		/// <param name="outputFileName">Output file path (optional)</param>
		public static void ProcessSingleFile()
		{
			// check the file existance first
			if (File.Exists(inputPath))
			{
				Console.WriteLine($"[INFO] Reading \"{Path.GetFileName(inputPath)}\"...");

				// read the file
				using (EndianBinaryReader br = new EndianBinaryReader(File.OpenRead(inputPath)))
				{
					// read magic bytes and determine the version and endianness
					RSCHeader hdr = new RSCHeader();
					byte[] magicAsByte = br.ReadBytes(4);
					string magic = Encoding.Default.GetString(magicAsByte);

					if (magic.Contains("RSC"))
					{
						br.Endianness = Endian.LittleEndian;
						hdr.Version = magicAsByte[3]; // or (byte)magic[3], it's the same
					}
					else if (magic.Contains("CSR"))
					{
						br.Endianness = Endian.BigEndian;
						hdr.Version = magicAsByte[0];
					}
					else
					{
						if (useVerboseMode || useVeryVerboseMode)
						{
							Console.WriteLine($"[ERROR] Incorrect signature 0x{BitConverter.ToUInt32(magicAsByte, 0):X8}.");
						}
						else
						{
							Console.WriteLine($"[ERROR] Incorrect signature.");
						}

						return;
					}

					// read resource version
					ResourceInfo.Version = br.ReadInt32() & 0xFF;

					// read resource flags (or first part of flags for some versions)
					FlagInfo.Flag1 = br.ReadInt32();

					if (hdr.Version == 0x85 || hdr.Version == 0x86 || hdr.Version == 0x37)
					{
						// read second part of the flags (only present in these three versions)
						FlagInfo.Flag2 = br.ReadInt32();
					}

					// print some information about our file (only in verbose mode)
					if (useVerboseMode || useVeryVerboseMode)
					{
						Console.WriteLine($"[INFO] RSC Version: {hdr.Version}");
						Console.WriteLine($"[INFO] Resource version: {ResourceInfo.Version}");
						Console.WriteLine($"[INFO] Endianness: {br.Endianness}");
						Console.WriteLine($"[INFO] System memory segment size: {FlagInfo.BaseResourceSizeV}");
						Console.WriteLine($"[INFO] Graphics memory segment size: {FlagInfo.BaseResourceSizeP}");
					}

					// read compressed data's magic bytes
					uint cmagic = br.ReadUInt32();

					// check compression method and process the data
					if ((cmagic & 0xFFFF0000) == 0x78DA0000)
					{
						if (useVerboseMode || useVeryVerboseMode)
						{
							Console.WriteLine("[INFO] Compression: Zlib");
						}

						// because the tool does not support the PS3 resources yet,
						// we will do all the work only if SaveUnpackedResource option is enabled,
						// otherwise we will just print a message and exit.
						// it will helps us to save the memory and increases the execution time.
						if (Settings.bSaveUnpackedResource)
						{
							Console.WriteLine("[INFO] Saving CPU and GPU memory data from resource...");

							// jump back
							br.BaseStream.Position -= 4;

							// calculate the decompressed size
							int dLen = FlagInfo.BaseResourceSizeP + FlagInfo.BaseResourceSizeV;

							// read the data
							byte[] origBuffer = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length - br.BaseStream.Position));

							// ...and decompress it
							byte[] decompBuffer = Zlib.Decompress(origBuffer);

							// initialize the arrays for storing the unpacked buffers
							byte[] sys = new byte[FlagInfo.BaseResourceSizeV];
							byte[] gfx = new byte[FlagInfo.BaseResourceSizeP];

							// get cpu buffer data from decompressed bytes
							Buffer.BlockCopy(decompBuffer, 0, sys, 0, sys.Length);

							// get gpu buffer data from decompressed bytes
							Buffer.BlockCopy(decompBuffer, sys.Length, gfx, 0, gfx.Length);

							// write them on disk
							string outPathCPU = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(inputPath)}.sys");
							string outPathGPU = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(inputPath)}.gfx");
							File.WriteAllBytes(outPathCPU, sys);
							File.WriteAllBytes(outPathGPU, gfx);

							return;
						}

						Console.WriteLine("[ERROR] PS3 resources not supported.");
						Console.WriteLine("        Only unpacking (with enabled \"SaveUnpackedResource\" option in settings.xml) is available at this moment.");
					}
					else if ((cmagic & 0xFFFF0000) == 0x0FF50000)
					{
						if (useVerboseMode || useVeryVerboseMode)
						{
							Console.WriteLine("[INFO] Compression: MS LZX (XCompress)");
						}

						// read compressed size
						int csize = br.ReadInt32();

						// read the data
						byte[] origBuffer = br.ReadBytes(csize);

						// initialize the array for the decompressed data
						byte[] decompBuffer = new byte[FlagInfo.BaseResourceSizeV + FlagInfo.BaseResourceSizeP];

						// ...and decompress it
						LZX.Decompress(origBuffer, decompBuffer);

						if (Settings.bSaveUnpackedResource)
						{
							Console.WriteLine("[INFO] Saving CPU and GPU memory data from resource...");

							// initialize the arrays for storing the unpacked buffers
							byte[] sys = new byte[FlagInfo.BaseResourceSizeV];
							byte[] gfx = new byte[FlagInfo.BaseResourceSizeP];

							// get cpu buffer data from decompressed bytes
							Buffer.BlockCopy(decompBuffer, 0, sys, 0, sys.Length);

							// get gpu buffer data from decompressed bytes
							Buffer.BlockCopy(decompBuffer, sys.Length, gfx, 0, gfx.Length);

							// write them on disk
							string outPathCPU = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(inputPath)}.sys");
							string outPathGPU = Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(inputPath)}.gfx");
							File.WriteAllBytes(outPathCPU, sys);
							File.WriteAllBytes(outPathGPU, gfx);

							return;
						}

						// read our buffers
						if (ReadMemoryData(hdr, decompBuffer) == 0)
						{
							Console.WriteLine("[ERROR] This resource is not supported.");
						}
					}
					else
					{
						if (useVerboseMode || useVeryVerboseMode)
						{
							Console.WriteLine($"[ERROR] Unsupported resource compression method 0x{cmagic & 0xFFFF0000:X8}.");
						}
						else
						{
							Console.WriteLine($"[ERROR] Unsupported resource compression.");
						}

						return;
					}
				}
			}
			else
			{
				Console.WriteLine("[ERROR] Input file does not exist or access denied.");
				return;
			}
		}

		/// <summary>
		/// Reads the data from RAGE resource
		/// </summary>
		/// <param name="hdr">RSC Header</param>
		/// <param name="data">Decompressed data with CPU and GPU buffer</param>
		/// <returns>0, if error is present, and 1 otherwise</returns>
		private static int ReadMemoryData(RSCHeader hdr, byte[] data)
		{
			using (MemoryStream ms = new MemoryStream(data))
			{
				// hdr, res, game, ext, path, desc
				//
				// 0x85, 2 = RDR, .xsc, /content/release/, "xenon script"
				// 0x5,  1 = RDR, .xst, /content/stringtable/, "xenon string table"
				// 0x5, 33 = RDR, .xsf, /flash/, "xenon shockwave flash"?
				// 


				if (hdr.Version == 0x85 && ResourceInfo.Version == 133)
				{
					if (!RDR_VolumeDataFile.ReadVolumeData(ms, true))
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
				else if (hdr.Version == 0x85 && ResourceInfo.Version == 138)
				{
					if (!RDR_Fragment.ReadFragment(ms, true))
					{
						throw new Exception("Error while reading RDR fragment model.");
					}
				}
				else if (hdr.Version == 0x85 && ResourceInfo.Version == 1)
				{
					if (!RDR_FragmentDictionary.ReadFragmentDictionary(ms, true))
					{
						throw new Exception("Error while reading RDR Fragment Dictionary.");
					}
				}
				else if (hdr.Version == 0x85 && ResourceInfo.Version == 134)
				{
					if (!RDR_SectorInfo.ReadXSIFile(ms))
					{
						throw new Exception("Error while reading RDR Sector Info file.");
					}
				}
				else if (hdr.Version == 0x85 && ResourceInfo.Version == 10)
				{
					if (useVerboseMode || useVeryVerboseMode)
					{
						Console.WriteLine("[INFO] Red Dead Redemption Texture Dictionary");
						Console.WriteLine($"[INFO] RSC85 Res Start: 0x{FlagInfo.RSC85_ObjectStart:X8}");
					}

					using (EndianBinaryReader br = new EndianBinaryReader(ms, Endian.BigEndian))
					{
						if (!TextureDictionary.ReadTextureDictionary(br, (uint)FlagInfo.RSC85_ObjectStart))
						{
							throw new Exception("Error while reading RDR Texture Dictionary.");
						}
					}
				}
				
				else if (hdr.Version == 0x5 && ResourceInfo.Version == 109)
				{
					if (!IV_Textures.ExportTexturesFromIVRes(ms, true, ResourceInfo.Version))
					{
						throw new Exception("Error while reading IV Textures.");
					}
				}
				
				// MC:LA Texture Pack (.xtl, .xtp)
				else if (hdr.Version == 0x5 && ResourceInfo.Version == 83)
				{
					if (!MCLA_83.Read(ms, true))
					{
						throw new Exception("Error while reading MC:LA Texture Pack.");
					}
				}
				
				// MC:LA Drawable (.xrsc)
				else if (hdr.Version == 0x5 && ResourceInfo.Version == 63)
				{
					if (!MCLA_63.Read(ms, true))
					{
						throw new Exception("Error while reading MC:LA Model.");
					}
				}
				
				// MC:LA .xtd
				else if (hdr.Version == 0x5 && ResourceInfo.Version == 9)
				{
					using (EndianBinaryReader br = new EndianBinaryReader(ms))
					{
						br.Endianness = Endian.BigEndian;

						if (!TextureDictionary.ReadTextureDictionary(br, 0))
						{
							throw new Exception("Error while reading MCLA Texture Dictionary.");
						}
					}
				}
				
				else if (hdr.Version == 0x5 && ResourceInfo.Version == 1)
				{
					if (!MCLA_1.Read(ms, true))
					{
						throw new Exception("Error while reading MC:LA Res1.");
					}
				}
				
				// RDR animationres (.xas)
				// MC:LA ?
				else if (hdr.Version == 0x5 && ResourceInfo.Version == 6)
				{
					if (!MCLA_XRSC.Read(ms, true))
					{
						throw new Exception("Error while reading MC:LA XRSC.");
					}
				}
				
				// MC:LA Drawable
				else if (hdr.Version == 0x5 && ResourceInfo.Version == 102)
				{
					if (!MCLA_Drawable.ReadDrawable(ms, true))
					{
						throw new Exception("Error while reading MC:LA Drawable.");
					}
				}
				
				else if (hdr.Version == 0x5 && ResourceInfo.Version == 131)
				{
					if (!MCLA_131.Read(ms, true))
					{
						throw new Exception("Error while reading MC:LA Res131.");
					}
				}
				else
				{
					return 0;
				}
			}

			return 1;
		}
	}
}
