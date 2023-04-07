using ConsoleApp1;
using Converter.Test;
using Converter.utils;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Converter.RageResource;
using static Converter.ResourceUtils;
using static Converter.Test.RDR_XTD_CREATOR;
using static Converter.utils.RDR_ShaderManager;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Converter
{
	internal class Cmode
	{
		public static void RunCmode(string[] args)
		{
			if (args.Length < 2) { Console.WriteLine("err"); return; }
			if (args[1] == "-unpackAllRDRTextures")
			{
				if (args.Length != 4)
				{
					Console.WriteLine("Usage: Converter.exe -a -unpackAllTextures inFoler outFolder");
					return;
				}
				string inFolder = args[2];
				string outFolder = args[3];
				if (!Directory.Exists(inFolder)) throw new Exception("wrong inFolder");
				if (!Directory.Exists(outFolder)) throw new Exception("wrong outFolder");
				string[] files = Directory.GetFiles(Path.GetDirectoryName(inFolder));
				EndianBinaryReader br;
				Converter.FileInfo.fileName = outFolder + "\\tmp.txd";
				for (int a = 0; a < files.Length; a++)
				{
					br = new EndianBinaryReader(File.OpenRead(files[a]));
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
						Log.ToLog(Log.MessageType.ERROR, $"Unknown file {files[a]}. Skipped");
						continue;
					}
					ResourceUtils.ResourceInfo.version = br.ReadInt32();
					ResourceUtils.FlagInfo.Flag1 = br.ReadInt32();
					if (hdr.Version == 0x85 || hdr.Version == 0x86 || hdr.Version == 0x37) ResourceUtils.FlagInfo.Flag2 = br.ReadInt32();
					uint cmagic = br.ReadUInt32();
					if ((cmagic & 0xFFFF0000) == 0x0FF50000)
					{
						int csize = br.ReadInt32();
						int usize = csize * 4;
						byte[] origBuffer = br.ReadBytes(csize);
						byte[] decompBuffer = new byte[FlagInfo.BaseResourceSizeV + FlagInfo.BaseResourceSizeP];
						DataUtils.DecompressLZX(origBuffer, decompBuffer);
						MemoryStream ms = new MemoryStream(decompBuffer);
						Log.ToLog(Log.MessageType.INFO, $"Path: {files[a]}");
						if (hdr.Version == 0x85 && ResourceUtils.ResourceInfo.version == 10)
						{
							br = new EndianBinaryReader(ms);
							br.Endianness = Endian.BigEndian;
							if (!TextureDictionary.ReadTextureDictionary(br, (uint)ResourceUtils.FlagInfo.RSC85_ObjectStart, overwriteTexture: false))
								Log.ToLog(Log.MessageType.ERROR, $"Error while reading RDR Texture Dictionary. Skipped");
							br.Close();
						}
					}
					else
					{
						Log.ToLog(Log.MessageType.ERROR, "LZX compression not detected. Skipped");
						continue;
					}
				}
				Log.ToLog(Log.MessageType.INFO, "Finish");
			}
			else if (args[1] == "-searchStringByUInt")
			{
				if (args.Length != 3)
				{
					Console.WriteLine("Usage: Converter.exe -a -searchStringByUInt uintValue");
					return;
				}
				uint value;
				if (!uint.TryParse(args[2], out value))
				{
					Log.ToLog(Log.MessageType.ERROR, $"{args[2]} is not uint value");
					throw new Exception("failed to parse value");
				}
				string name;
				if (RDR_FileNames.fileNames.TryGetValue(value, out name)) Console.WriteLine($"{name}");
				else Console.WriteLine("string not found");
			}
			else if (args[1] == "-extractFlashTexturesInfo")
			{
				if (args.Length != 3)
				{
					Console.WriteLine("Usage: Converter.exe -a -extractFlashTexturesInfo filePath");
					return;
				}
				string inFile = args[2];
				if (!File.Exists(inFile)) throw new Exception("wrong file path");

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
				ms = new MemoryStream(decompBuffer);

				br = new EndianBinaryReader(ms);
				br.Endianness = Endian.BigEndian;
				// br.Position = FlagInfo.ResourceStart; в rsc5 такого нет
				br.Position += 0x2c; // идем к поинтеру
				br.Position = br.ReadOffset();
				br.Position += 0x18;
				uint pFlashTextures = br.ReadOffset();
				br.Position += 0x16;
				ushort nFlashTexturesCount = br.ReadUInt16();

				ushort currentTexture = 0;

				uint[] pFlashTexture = new uint[nFlashTexturesCount];
				br.Position = pFlashTextures;
				//Console.WriteLine("dd");
				for (int a = 0; a < nFlashTexturesCount; a++)
				{
					pFlashTexture[currentTexture] = br.ReadOffset();
					if (pFlashTexture[currentTexture] != 0) currentTexture++;
				}
				nFlashTexturesCount = currentTexture;

				RageResource.FlashTexture[] flashTexture = new RageResource.FlashTexture[nFlashTexturesCount];
				RageResource.Texture[] texture = new RageResource.Texture[nFlashTexturesCount];
				currentTexture = 0;
				for (int a = 0; a < nFlashTexturesCount; a++)
				{
					br.Position = pFlashTexture[a];
					flashTexture[currentTexture] = ReadRageResource.FlashTexture(br);
					//Console.WriteLine(flashTexture[currentTexture]._vmt);
					if (flashTexture[currentTexture]._vmt == 3166264320/*||
						flashTexture[currentTexture]._vmt == 3569048576*/) currentTexture++;
				}
				nFlashTexturesCount = currentTexture;

				for (int a = 0; a < nFlashTexturesCount; a++)
				{
					br.Position = flashTexture[a].m_pTexture;
					texture[a] = ReadRageResource.Texture(br);
				}
				RageResource.BitMap[] bitMap = new RageResource.BitMap[nFlashTexturesCount];
				for (int a = 0; a < nFlashTexturesCount; a++)
				{
					br.Position = texture[a].m_pBitmap;
					bitMap[a] = ReadRageResource.BitMap(br);
				}
				Converter.FileInfo.fileName = inFile;
				for (int a = 0; a < nFlashTexturesCount; a++) TextureDictionary.ReadTexture(texture[a], bitMap[a], br, 0);

				// теперь создаем xml файл
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				// other settings...
				//settings.Encoding = Encoding.ASCII;
				settings.IndentChars = ("\t");
				//settings.ConformanceLevel = ConformanceLevel.Fragment;
				settings.DoNotEscapeUriAttributes = true;
				settings.OmitXmlDeclaration = false;
				string shaderManagerPath = $"{FileInfo.filePath}\\{FileInfo.baseFileName}.xml";
				XmlWriter xml = XmlWriter.Create(shaderManagerPath, settings);
				xml.WriteStartDocument();
				xml.WriteStartElement("XenonTextureDictionary");
				for (int a = 0; a < nFlashTexturesCount; a++)
				{
					Log.ToLog(Log.MessageType.INFO, $"Name: {DataUtils.ReadStringAtOffset(texture[a].m_pName, br)}");

					string textureName = DataUtils.ReadStringAtOffset(texture[a].m_pName, br);
					textureName = Path.GetFileName(textureName.Replace(':', '\\'));
					textureName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{textureName}";
					xml.WriteStartElement("Texture");
					xml.WriteElementString("Name", DataUtils.ReadStringAtOffset(texture[a].m_pName, br));
					xml.WriteElementString("Path", textureName);
					xml.WriteStartElement("Size");
					xml.WriteAttributeString("height", texture[a].m_dwHeight.ToString());
					xml.WriteAttributeString("width", texture[a].m_dwWidth.ToString());
					xml.WriteEndElement();
					xml.WriteElementString("MipsCount", texture[a].m_Lod.ToString());
					xml.WriteElementString("TypeAsString", TextureUtils.GetFormat(bitMap[a].m_dwTextureType));
					xml.WriteEndElement();
				}
				xml.WriteEndElement();
				xml.WriteEndDocument();
				xml.Close();

				Log.ToLog(Log.MessageType.INFO, "Finished");
				Console.WriteLine("Press any key to exit");
				Console.ReadKey();

			}
			else if (args[1] == "-extractXtdInfo")
			{
				if (args.Length != 3)
				{
					Console.WriteLine("Usage: Converter.exe -a -extractXtdInfo xtdPath");
					return;
				}
				string inFile = args[2];
				if (!File.Exists(inFile)) throw new Exception("wrong file path");

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
				ms = new MemoryStream(decompBuffer);

				br = new EndianBinaryReader(ms);
				br.Endianness = Endian.BigEndian;
				if (ResourceInfo.version == 10)
				{
					br.Position = FlagInfo.ResourceStart;
				}
				else if (ResourceInfo.version == 0x85)
				{
					br.Position = FlagInfo.ResourceStart;
					RageResource.RDRVolumeData xvd = ReadRageResource.RDRVolumeData(br);
					if (xvd.pTexture == 0) throw new Exception("textures not found");
					br.Position = xvd.pTexture;
				}
				else if (ResourceInfo.version == 1)
				{
					br.Position = FlagInfo.ResourceStart;
					RageResource.FragmentDictionary dict = ReadRageResource.FragmentDictionary(br);
					if (dict.m_pTextureDictionary == 0) throw new Exception("textures not found");
					br.Position = dict.m_pTextureDictionary;

				}
				else throw new Exception("unk res");

				RageResource.XTDHeader header = new RageResource.XTDHeader();
				header = ReadRageResource.XTDHeader(br);

				RageResource.Texture[] texture = new RageResource.Texture[header.m_cTexture.m_wCount];
				uint[] pTexture = new uint[header.m_cTexture.m_wCount];
				br.Position = header.m_cTexture.m_pList;
				for (int a = 0; a < header.m_cTexture.m_wCount; a++) pTexture[a] = br.ReadOffset();
				for (int a = 0; a < header.m_cTexture.m_wCount; a++)
				{
					br.Position = pTexture[a];
					texture[a] = ReadRageResource.Texture(br);
				}
				RageResource.BitMap[] bitMap = new RageResource.BitMap[header.m_cTexture.m_wCount];
				for (int a = 0; a < header.m_cTexture.m_wCount; a++)
				{
					br.Position = texture[a].m_pBitmap;
					bitMap[a] = ReadRageResource.BitMap(br);
				}
				Converter.FileInfo.fileName = inFile;
				for (int a = 0; a < header.m_cTexture.m_wCount; a++) TextureDictionary.ReadTexture(texture[a], bitMap[a], br, 0);


				// теперь создаем xml файл
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				// other settings...
				//settings.Encoding = Encoding.ASCII;
				settings.IndentChars = ("\t");
				//settings.ConformanceLevel = ConformanceLevel.Fragment;
				settings.DoNotEscapeUriAttributes = true;
				settings.OmitXmlDeclaration = false;
				string shaderManagerPath = $"{FileInfo.filePath}\\{FileInfo.baseFileName}.xml";
				XmlWriter xml = XmlWriter.Create(shaderManagerPath, settings);
				xml.WriteStartDocument();
				xml.WriteStartElement("XenonTextureDictionary");
				for (int a = 0; a < header.m_cTexture.m_wCount; a++)
				{
					Log.ToLog(Log.MessageType.INFO, $"Name: {DataUtils.ReadStringAtOffset(texture[a].m_pName, br)}");

					string textureName = DataUtils.ReadStringAtOffset(texture[a].m_pName, br);
					textureName = Path.GetFileName(textureName.Replace(':', '\\'));
					textureName = $"{FileInfo.filePath}\\{FileInfo.baseFileName}\\{textureName}";
					xml.WriteStartElement("Texture");
					xml.WriteElementString("Name", DataUtils.ReadStringAtOffset(texture[a].m_pName, br));
					xml.WriteElementString("Path", textureName);
					xml.WriteStartElement("Size");
					xml.WriteAttributeString("height", texture[a].m_dwHeight.ToString());
					xml.WriteAttributeString("width", texture[a].m_dwWidth.ToString());
					xml.WriteEndElement();
					xml.WriteElementString("MipsCount", texture[a].m_Lod.ToString());
					xml.WriteElementString("TypeAsString", TextureUtils.GetFormat(bitMap[a].m_dwTextureType));
					xml.WriteEndElement();
				}
				xml.WriteEndElement();
				xml.WriteEndDocument();
				xml.Close();

				Log.ToLog(Log.MessageType.INFO, "Finished");
				Console.WriteLine("Press any key to exit");
				Console.ReadKey();

			}
			else if (args[1] == "-replaceXtdInfo")
			{
				if (args.Length != 5)
				{
					Console.WriteLine("Usage: Converter.exe -a -replaceXtdInfo inFile outFile xmlPath");
					return;
				}
				string inFile = args[2];
				string outFile = args[3];
				string xmlPath = args[4];
				Converter.FileInfo.fileName = inFile;
				XmlReaderSettings settings = new XmlReaderSettings();
				settings.IgnoreComments = true;
				settings.IgnoreProcessingInstructions = true;
				settings.IgnoreWhitespace = true;
				byte[] origTexture = File.ReadAllBytes(xmlPath);
				MemoryStream stream = new MemoryStream(origTexture);
				XmlReader xml = XmlReader.Create(stream, settings);
				xml.Read();
				RDR_XTD_CREATOR.XmlInfo[] xmlInfo = new RDR_XTD_CREATOR.XmlInfo[255];
				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "XenonTextureDictionary") throw new Exception("ERR");
				for (int a = 0; a < 255; a++)
				{
					xml.Read();
					if (xml.NodeType != XmlNodeType.Element || xml.Name != "Texture")
					{
						if (xml.NodeType == XmlNodeType.EndElement || xml.Name == "XenonTextureDictionary")
						{
							Array.Resize<XmlInfo>(ref xmlInfo, a);
							break;
						}
						else throw new Exception("ERR2");
					}
					xml.Read();
					if (xml.NodeType != XmlNodeType.Element || xml.Name != "Name") throw new Exception("ERR9");
					xml.Read();
					xmlInfo[a].name = xml.Value;
					xml.Read();
					if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "Name") throw new Exception("ERR9_1");

					xml.Read();
					if (xml.NodeType != XmlNodeType.Element || xml.Name != "Path") throw new Exception("ERR4");
					xml.Read();
					xmlInfo[a].path = xml.Value;
					xml.Read();
					if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "Path") throw new Exception("ERR4_1");

					xml.Read();
					if (xml.NodeType != XmlNodeType.Element || xml.Name != "Size") throw new Exception("ERR5");
					xmlInfo[a].height = Convert.ToInt32(xml.GetAttribute("height"));
					xmlInfo[a].width = Convert.ToInt32(xml.GetAttribute("width"));

					if (xmlInfo[a].height < 32 || xmlInfo[a].width < 32) throw new Exception("minimal size - 32px");

					xml.Read();
					if (xml.NodeType != XmlNodeType.Element || xml.Name != "MipsCount") throw new Exception("ERR6");
					xml.Read();
					xmlInfo[a].mipCount = Convert.ToInt32(xml.Value);
					xml.Read();
					if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "MipsCount") throw new Exception("ERR6_1");

					xml.Read();
					if (xml.NodeType != XmlNodeType.Element || xml.Name != "TypeAsString") throw new Exception("ERR8");
					xml.Read();
					xmlInfo[a].typeAsString = xml.Value;
					xml.Read();
					if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "TypeAsString") throw new Exception("ERR8_1");

					xml.Read();
					if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "Texture") throw new Exception($"ERR7");
				}
				if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "XenonTextureDictionary") throw new Exception("ERR3");

				EndianBinaryReader br = new EndianBinaryReader(File.OpenRead(inFile));
				br.Endianness = Endian.BigEndian;
				uint magic = br.ReadUInt32();
				uint version = br.ReadUInt32();
				br.Close();
				if (magic != 2235781970) throw new Exception("unk magic");
				if (version == 10) ReplaceTextures(xmlInfo, inFile, outFile);
				else if (version == 0x85) ReplaceTexturesInXVD(inFile, outFile, xmlInfo);
				else if (version == 1) ReplaceTexturesInXFD(inFile, outFile, xmlInfo);
				else throw new Exception("unsupported version");
			}
			else if (args[1] == "-decompressRSC85")
			{
				if (args.Length != 3)
				{
					Console.WriteLine("Usage: Converter.exe -a -decompressRSC85 inFile");
					return;
				}

				string inFile = args[2];

				Converter.FileInfo.fileName = inFile;

				if (!File.Exists(inFile)) throw new Exception("wrong file path");
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

				byte[] sys = new byte[FlagInfo.BaseResourceSizeV];
				byte[] gfx = new byte[FlagInfo.BaseResourceSizeP];
				Buffer.BlockCopy(decompBuffer, 0, sys, 0, sys.Length);
				Buffer.BlockCopy(decompBuffer, sys.Length, gfx, 0, gfx.Length);
				string sysPath = $"{Converter.FileInfo.filePath}\\{Converter.FileInfo.baseFileName}.sys";
				string gfxPath = $"{Converter.FileInfo.filePath}\\{Converter.FileInfo.baseFileName}.gfx";
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				// other settings...
				//settings.Encoding = Encoding.ASCII;
				settings.IndentChars = ("\t");
				//settings.ConformanceLevel = ConformanceLevel.Fragment;
				settings.DoNotEscapeUriAttributes = true;
				settings.OmitXmlDeclaration = false;
				XmlWriter xml = System.Xml.XmlWriter.Create($"{FileInfo.filePath}\\{FileInfo.baseFileName}.xml", settings);
				xml.WriteStartDocument();
				xml.WriteStartElement("RSC85");

				xml.WriteStartElement("Segments");
				xml.WriteAttributeString("system", sysPath);
				xml.WriteAttributeString("graphic", gfxPath);
				xml.WriteEndElement();
				xml.WriteStartElement("Flags");
				xml.WriteAttributeString("system", $"0x{((uint)FlagInfo.Flag1).ToString("X8")}");
				xml.WriteAttributeString("graphic", $"0x{((uint)FlagInfo.Flag2).ToString("X8")}");
				xml.WriteEndElement();
				xml.WriteStartElement("Version");
				xml.WriteString(ResourceInfo.version.ToString());
				xml.WriteEndElement();
				xml.WriteEndElement();
				xml.WriteEndDocument();
				xml.Close();
				File.WriteAllBytes(sysPath, sys);
				File.WriteAllBytes(gfxPath, gfx);

			}
			else if (args[1] == "-compressRSC85")
			{

				if (args.Length != 5)
				{
					Console.WriteLine("Usage: Converter.exe -a -compressRSC85 xml outFile calculateFlags:true/false");
					return;
				}

				string inFile = args[2];
				string outFile = args[3];
				if (!args[4].StartsWith("calculateFlags:")) throw new Exception("calculateFlags not found");
				bool calculateNewFlags;
				if (!bool.TryParse(args[4].Replace("calculateFlags:", ""), out calculateNewFlags)) throw new Exception("wrong bool value");


				Converter.FileInfo.fileName = outFile;

				XmlReaderSettings settings = new XmlReaderSettings();
				settings.IgnoreComments = true;
				settings.IgnoreProcessingInstructions = true;
				settings.IgnoreWhitespace = true;

				byte[] xmlArray = File.ReadAllBytes(inFile);
				MemoryStream stream = new MemoryStream(xmlArray);
				XmlReader xml = XmlReader.Create(stream, settings);
				xml.Read();
				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "RSC85") throw new Exception("isNotRSC85");
				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Segments") throw new Exception("segmentsNotFound");
				string sysPath = xml.GetAttribute("system");
				string gfxPath = xml.GetAttribute("graphic");
				//int version = 133;

				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Flags") throw new Exception("flagsNotFound");
				FlagInfo.Flag1 = (int)Convert.ToUInt32(xml.GetAttribute("system"), 16);
				FlagInfo.Flag2 = (int)Convert.ToUInt32(xml.GetAttribute("graphic"), 16);

				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Version") throw new Exception("unkNode");
				xml.Read();
				ResourceInfo.version = Convert.ToInt32(xml.Value);
				xml.Read();
				if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "Version") throw new Exception("unkNode");

				xml.Read();
				if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "RSC85") throw new Exception("isNotRSC85");

				//byte[] finalFile = new byte[67108864];
				DataUtils.GetFileSize(sysPath);
				DataUtils.GetFileSize(gfxPath);
				BinaryWriter dataOut = new BinaryWriter(new FileStream(outFile, FileMode.Create));
				uint magic = 1381188485;
				//byte[] fileTest = File.ReadAllBytes(gfxPath);
				byte[] compressedAllData = DataUtils.CompressLZX2(File.ReadAllBytes(sysPath).Concat(File.ReadAllBytes(gfxPath)).ToArray());

				if (calculateNewFlags) ResourceUtils.FlagInfo.RSC85_SetMemSizes(DataUtils.GetFileSize(sysPath), DataUtils.GetFileSize(gfxPath));

				dataOut.Write(magic);
				dataOut.Write(ResourceInfo.version.Reverse());
				dataOut.Write(FlagInfo.Flag1.Reverse());
				dataOut.Write(FlagInfo.Flag2.Reverse());
				dataOut.Write(4044551439);
				dataOut.Write(compressedAllData.Length.Reverse());
				dataOut.Write(compressedAllData);
				dataOut.Close();
			}
			else throw new Exception("unk command");
		}
	}
}
