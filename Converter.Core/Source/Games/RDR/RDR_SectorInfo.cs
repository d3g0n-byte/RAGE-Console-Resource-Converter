using Converter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml;
using System.IO;
using Converter.Core.ResourceTypes;

namespace Converter.Core.Games.RDR
{
	public static class RDR_SectorInfo
	{
		public static Dictionary<uint, string> exportedFileNames;

		public struct XSI
		{
			public uint _vmt;
			public uint m_pBlockEnd;
			public uint m_pName;
			public float _fc;
			public float _f10;
			public float _f14;
			public float _f18;
			public uint m_nGroupNameHash;
			public uint _f20;
			public uint _f24; // pointer
			public uint _f28;
			public uint _f2c;
			public uint _f30;
			public uint _f34;
			public uint _f38;
			public uint _f3c;
			public uint _f40;
			public uint _f44;
			public uint _f48;
			public uint _f4c;
			public uint _f50;
			public uint _f54;
			public uint m_pCurve; // pointer?
			public uint _f5c;
			public uint _f60; // pointer
			public uint _f64;
			public uint _f68;
			public uint _f6c;
			public uint _f70;
			public uint _f74;
			public uint _f78;
			public uint _f7c;
			public int _f80; // number
			public int _f84; // -1
			public uint _f88;
			public uint _f8c;
			public Vector4 _f90;
			public Vector4 _fa0;
			public Vector4 m_vBoundsMin; // need verify
			public Vector4 m_vBoundsMax; // need verify
			public uint _fd0; // pointer
			public Collection _fd4;
			public uint _fdc; // seems to be this is a pointer to RageResource.Collection
			public ushort _fe0;
			public ushort _fe2;
			public ushort _fe4; // 0xcdcd
			public byte _fe6; // 0xcd
			public byte _fe7;
			public uint _fe8;
			public uint _fec;
			public uint m_pChilds; // seems to be this is a pointer to the collection with the same sections
			public Collection _ff4;
			public Collection _ffc; // the collection. In the first was a pointer to the name of xsi section
			public Collection _f104;
			public uint _f10c;
			public uint _f110;
			public uint _f114;
			public uint _f118;
			public uint _f11c;
			public int _f120; // -1
			public uint _f124;
			public uint _f128;
			public uint _f12c;
			public int _f130; // -1
			public uint _f134;
			public uint _f138;
			public uint _f13c;
			public uint _f140;
			public uint _f144;
			public uint _f148;
			public uint _f14c;
			public int _f150; // -1
			public uint _f154;
			public uint _f158;
			public uint _f15c;
			public int _f160; // -1
			public uint _f164;
			public uint _f168;
			public uint _f16c;
			public uint _f170;
			public float _f174; // 1
			public uint m_nNameHash;
			public uint _f17c;
			public uint _f180;
			public uint _f184;
			public int _f188; // number
			public uint _f18c;
			public uint _f190;
			public uint _f194;
			public uint _f198;
			public Collection _f19c;
			public uint _f1a4;
			public uint _f1a8;
			public uint _f1ac;
			public uint _f1b0; // pointer
			public uint _f1b4;
			public uint _f1b8;
			public int _f1bc; // number?
			public uint _f1c0; // 0xcdcdcdcd
			public byte _f1c4;
			public byte _f1c5; // 0xcd
			public ushort _f1c6;
			public byte _f1c8;
			public byte _f1c9; // 0xcd
			public ushort _f1ca; // 0xcdcd
			public uint _f1cc;
			public uint _f1d0; // number
			public uint _f1d4;
			public byte _f1d8;
			public byte _f1d9; // 0xcd
			public byte _f1da; // 0xcd
			public byte _f1db;
			public uint _f1dc;
		}

		public static void ReadXSI(EndianBinaryReader br, ref XSI xsi)
		{
			xsi._vmt = br.ReadUInt32();
			xsi.m_pBlockEnd = br.ReadOffset();
			xsi.m_pName = br.ReadOffset();
			xsi._fc = br.ReadSingle();
			xsi._f10 = br.ReadSingle();
			xsi._f14 = br.ReadSingle();
			xsi._f18 = br.ReadSingle();
			xsi.m_nGroupNameHash = br.ReadUInt32();
			xsi._f20 = br.ReadUInt32();
			xsi._f24 = br.ReadOffset();
			xsi._f28 = br.ReadUInt32();
			xsi._f2c = br.ReadUInt32();
			xsi._f30 = br.ReadUInt32();
			xsi._f34 = br.ReadUInt32();
			xsi._f38 = br.ReadUInt32();
			xsi._f3c = br.ReadUInt32();
			xsi._f40 = br.ReadUInt32();
			xsi._f44 = br.ReadUInt32();
			xsi._f48 = br.ReadUInt32();
			xsi._f4c = br.ReadUInt32();
			xsi._f50 = br.ReadUInt32();
			xsi._f54 = br.ReadUInt32();
			xsi.m_pCurve = br.ReadOffset();
			xsi._f5c = br.ReadUInt32();
			xsi._f60 = br.ReadOffset();
			xsi._f64 = br.ReadUInt32();
			xsi._f68 = br.ReadUInt32();
			xsi._f6c = br.ReadUInt32();
			xsi._f70 = br.ReadUInt32();
			xsi._f74 = br.ReadUInt32();
			xsi._f78 = br.ReadUInt32();
			xsi._f7c = br.ReadUInt32();
			xsi._f80 = br.ReadInt32();
			xsi._f84 = br.ReadInt32();
			xsi._f88 = br.ReadUInt32();
			xsi._f8c = br.ReadUInt32();
			xsi._f90 = br.ReadVector4();
			xsi._fa0 = br.ReadVector4();
			xsi.m_vBoundsMin = br.ReadVector4();
			xsi.m_vBoundsMax = br.ReadVector4();
			xsi._fd0 = br.ReadOffset();
			xsi._fd4 = Collection.Read(br);
			xsi._fdc = br.ReadOffset();
			xsi._fe0 = br.ReadUInt16();
			xsi._fe2 = br.ReadUInt16();
			xsi._fe4 = br.ReadUInt16();
			xsi._fe6 = br.ReadByte();
			xsi._fe7 = br.ReadByte();
			xsi._fe8 = br.ReadUInt32();
			xsi._fec = br.ReadUInt32();
			xsi.m_pChilds = br.ReadOffset();
			xsi._ff4 = Collection.Read(br);
			xsi._ffc = Collection.Read(br);
			xsi._f104 = Collection.Read(br);
			xsi._f10c = br.ReadUInt32();
			xsi._f110 = br.ReadUInt32();
			xsi._f114 = br.ReadUInt32();
			xsi._f118 = br.ReadUInt32();
			xsi._f11c = br.ReadUInt32();
			xsi._f120 = br.ReadInt32();
			xsi._f124 = br.ReadUInt32();
			xsi._f128 = br.ReadUInt32();
			xsi._f12c = br.ReadUInt32();
			xsi._f130 = br.ReadInt32();
			xsi._f134 = br.ReadUInt32();
			xsi._f138 = br.ReadUInt32();
			xsi._f13c = br.ReadUInt32();
			xsi._f140 = br.ReadUInt32();
			xsi._f144 = br.ReadUInt32();
			xsi._f148 = br.ReadUInt32();
			xsi._f14c = br.ReadUInt32();
			xsi._f150 = br.ReadInt32();
			xsi._f154 = br.ReadUInt32();
			xsi._f158 = br.ReadUInt32();
			xsi._f15c = br.ReadUInt32();
			xsi._f160 = br.ReadInt32();
			xsi._f164 = br.ReadUInt32();
			xsi._f168 = br.ReadUInt32();
			xsi._f16c = br.ReadUInt32();
			xsi._f170 = br.ReadUInt32();
			xsi._f174 = br.ReadSingle();
			xsi.m_nNameHash = br.ReadUInt32();
			xsi._f17c = br.ReadUInt32();
			xsi._f180 = br.ReadUInt32();
			xsi._f184 = br.ReadUInt32();
			xsi._f188 = br.ReadInt32();
			xsi._f18c = br.ReadUInt32();
			xsi._f190 = br.ReadUInt32();
			xsi._f194 = br.ReadUInt32();
			xsi._f198 = br.ReadUInt32();
			xsi._f19c = Collection.Read(br);
			xsi._f1a4 = br.ReadUInt32();
			xsi._f1a8 = br.ReadUInt32();
			xsi._f1ac = br.ReadUInt32();
			xsi._f1b0 = br.ReadOffset();
			xsi._f1b4 = br.ReadUInt32();
			xsi._f1b8 = br.ReadUInt32();
			xsi._f1bc = br.ReadInt32();
			xsi._f1c0 = br.ReadUInt32();
			xsi._f1c4 = br.ReadByte();
			xsi._f1c5 = br.ReadByte();
			xsi._f1c6 = br.ReadUInt16();
			xsi._f1c8 = br.ReadByte();
			xsi._f1c9 = br.ReadByte();
			xsi._f1ca = br.ReadUInt16();
			xsi._f1cc = br.ReadUInt32();
			xsi._f1d0 = br.ReadOffset();
			xsi._f1d4 = br.ReadUInt32();
			xsi._f1d8 = br.ReadByte();
			xsi._f1d9 = br.ReadByte();
			xsi._f1da = br.ReadByte();
			xsi._f1db = br.ReadByte();
			xsi._f1dc = br.ReadUInt32();
		}
		
		public struct ChildsCollection
		{
			public Collection _f0;
			public Collection _f8;
			public Collection _f10;
			public uint m_pName;
		}

		public static void ReadChildsCollection(EndianBinaryReader br, ref ChildsCollection childsCollection)
		{
			childsCollection._f0 = Collection.Read(br);
			childsCollection._f8 = Collection.Read(br);
			childsCollection._f10 = Collection.Read(br);
			childsCollection.m_pName = br.ReadOffset();
			string name = DataUtils.ReadStringAtOffset(childsCollection.m_pName, br);

			if (!exportedFileNames.ContainsKey(DataUtils.GetHash(name)))
			{
				exportedFileNames.Add(DataUtils.GetHash(name), name);
			}
		}

		public struct Prop
		{
			public uint p_Name;
			public uint _f4;
			public uint _f8;
			public uint _fc;
			public Vector4 m_vPos;
			public uint _f20;
			public uint _f24;
			public uint _f28; // pointer
			public uint _f2c;
		}

		public static void ReadProp(EndianBinaryReader br, ref Prop prop)
		{
			prop.p_Name = br.ReadOffset();
			prop._f4 = br.ReadUInt32();
			prop._f8 = br.ReadUInt32();
			prop._fc = br.ReadUInt32();
			prop.m_vPos = br.ReadVector4();
			prop._f20 = br.ReadUInt32();
			prop._f24 = br.ReadUInt32(); // flags?
			prop._f28 = br.ReadOffset();
			prop._f2c = br.ReadUInt32();
			string name = DataUtils.ReadStringAtOffset(prop.p_Name, br);
			if (!exportedFileNames.ContainsKey(DataUtils.GetHash(name)))
			{
				exportedFileNames.Add(DataUtils.GetHash(name), name);
			}
		}

		public struct Curve
		{
			public Vector4 m_vBoundsMin;
			public Vector4 m_vBoundsMax;
			public Collection _f20;
			public uint _f28;
			public uint _f2c;
		}

		public static void ReadCurve(EndianBinaryReader br, ref Curve curve)
		{
			curve.m_vBoundsMin = br.ReadVector4();
			curve.m_vBoundsMax = br.ReadVector4();
			curve._f20 = Collection.Read(br);
			curve._f28 = br.ReadUInt32();
			curve._f2c = br.ReadUInt32();
		}

		public struct Curve2
		{
			public uint _vmt;
			public uint _f4; // pointer
			public uint _f8;
			public uint _fc; // pointer to vector
			public int _f10; // number
			public float _f14;
			public float _f18;
			public ushort _f1c;
			public ushort _f1e;
			public Vector4 _f20; // min
			public Vector4 _f30; // max
			public uint m_pName;
			public ushort _f44;
			public ushort _f46;
			public ushort _f48;
			public ushort _f4a;
			public uint _f4c;
		}

		public struct cc0d7200
		{
			public uint _vmt;
			public float _f4;
			public uint _f8;
			public uint _fc;
			public Vector4 _f10;
			public uint _f20;
			public uint _f24;
			public uint _f28;
			public uint _f2c;
			public uint _f30;
			public ushort _f34;
			public ushort _f36;
			public uint _f38;
			public uint _f3c;
			public Vector4 matrix_m0;
			public Vector4 matrix_m1;
			public Vector4 matrix_m2;
			public Vector4 m_vPos; // need verify
			public Vector4 m_vBoundsMin; // need verify
			public Vector4 m_vBoundsMax; // need verify
			public uint m_nNameHash;
			public int _fa4;
			public int _fa8;
			public int _fac;
			public ushort _fb0;
			public byte _fb2;
			public byte _fb3;
			public uint _fb4; // 0xcdcdcdcd
			public uint m_pName;
			public uint _fbc;
			public uint _fc0;
			public uint _fc4;
			public uint _fc8;
			public uint _fcc;
			public uint _fd0;
			public uint _fd4;
			public uint _fd8;
			public uint _fdc;
		}
		
		public static void Readcc0d7200(EndianBinaryReader br, ref cc0d7200 section)
		{
			section._vmt = br.ReadUInt32();
			section._f4 = br.ReadSingle();
			section._f8 = br.ReadUInt32();
			section._fc = br.ReadUInt32();
			section._f10 = br.ReadVector4();
			section._f20 = br.ReadUInt32();
			section._f24 = br.ReadUInt32();
			section._f28 = br.ReadUInt32();
			section._f2c = br.ReadUInt32();
			section._f30 = br.ReadUInt32();
			section._f34 = br.ReadUInt16();
			section._f36 = br.ReadUInt16();
			section._f38 = br.ReadUInt32();
			section._f3c = br.ReadUInt32();
			section.matrix_m0 = br.ReadVector4();
			section.matrix_m1 = br.ReadVector4();
			section.matrix_m2 = br.ReadVector4();
			section.m_vPos = br.ReadVector4();
			section.m_vBoundsMin = br.ReadVector4();
			section.m_vBoundsMax = br.ReadVector4();
			section.m_nNameHash = br.ReadUInt32();
			section._fa4 = br.ReadInt32();
			section._fa8 = br.ReadInt32();
			section._fac = br.ReadInt32();
			section._fb0 = br.ReadUInt16();
			section._fb2 = br.ReadByte();
			section._fb3 = br.ReadByte();
			section._fb4 = br.ReadUInt32();
			section.m_pName = br.ReadOffset();
			section._fbc = br.ReadUInt32();
			section._fc0 = br.ReadUInt32();
			section._fc4 = br.ReadUInt32();
			section._fc8 = br.ReadUInt32();
			section._fcc = br.ReadUInt32();
			section._fd0 = br.ReadUInt32();
			section._fd4 = br.ReadUInt32();
			section._fd8 = br.ReadUInt32();
			section._fdc = br.ReadUInt32();

			string name = DataUtils.ReadStringAtOffset(section.m_pName, br);
			if (!exportedFileNames.ContainsKey(DataUtils.GetHash(name)))
			{
				exportedFileNames.Add(DataUtils.GetHash(name), name);
			}
		}

		public static void WriteVector(this XmlWriter xml, string name, float x = -3.40282346638528859e+38f, float y = -3.40282346638528859e+38f,
			float z = -3.40282346638528859e+38f, float w = -3.40282346638528859e+38f)
		{
			xml.WriteStartElement($"{name}");

			if (x != -3.40282346638528859e+38f)
			{
				xml.WriteAttributeString("x", x.ToString());
			}

			if (y != -3.40282346638528859e+38f)
			{
				xml.WriteAttributeString("y", y.ToString());
			}

			if (z != -3.40282346638528859e+38f)
			{
				xml.WriteAttributeString("z", z.ToString());
			}

			if (w != -3.40282346638528859e+38f)
			{
				xml.WriteAttributeString("w", w.ToString());
			}

			xml.WriteEndElement();
		}

		public static bool ReadXSIFile(MemoryStream ms)
		{
			exportedFileNames = new Dictionary<uint, string>();

			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
				DoNotEscapeUriAttributes = true,
				OmitXmlDeclaration = false
			};

			string xmlPath = $"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}.xml";
			XmlWriter xml = XmlWriter.Create(xmlPath, settings);

			xml.WriteStartDocument();
			xml.WriteStartElement("XenonSectorInfo");
			xml.WriteStartElement("Main");

			EndianBinaryReader br = new EndianBinaryReader(ms)
			{
				Endianness = Endian.BigEndian,
				Position = FlagInfo.RSC85_StartPagePosition
			};

			XSI xsi = new XSI();
			ReadXSI(br, ref xsi);

			if (!(xsi._f90.X.ToString().ToLower().Contains("e") ||
				xsi._f90.Y.ToString().ToLower().Contains("e") ||
				xsi._f90.Z.ToString().ToLower().Contains("e") ||
				xsi._f90.W.ToString().ToLower().Contains("e")))
			{
				xml.WriteStartElement("_f90");
				xml.WriteAttributeString("x", xsi._f90.X.ToString());
				xml.WriteAttributeString("y", xsi._f90.Y.ToString());
				xml.WriteAttributeString("z", xsi._f90.Z.ToString());
				xml.WriteAttributeString("w", xsi._f90.W.ToString());
				xml.WriteEndElement();
			}

			if (!(xsi._fa0.X.ToString().ToLower().Contains("e") ||
				xsi._fa0.Y.ToString().ToLower().Contains("e") ||
				xsi._fa0.Z.ToString().ToLower().Contains("e") ||
				xsi._fa0.W.ToString().ToLower().Contains("e")))
			{
				xml.WriteStartElement("_fa0");
				xml.WriteAttributeString("x", xsi._fa0.X.ToString());
				xml.WriteAttributeString("y", xsi._fa0.Y.ToString());
				xml.WriteAttributeString("z", xsi._fa0.Z.ToString());
				xml.WriteAttributeString("w", xsi._fa0.W.ToString());
				xml.WriteEndElement();
			}

			xml.WriteStartElement("_fb0");
			xml.WriteAttributeString("x", xsi.m_vBoundsMin.X.ToString());
			xml.WriteAttributeString("y", xsi.m_vBoundsMin.Y.ToString());
			xml.WriteAttributeString("z", xsi.m_vBoundsMin.Z.ToString());
			xml.WriteAttributeString("w", xsi.m_vBoundsMin.W.ToString());
			xml.WriteEndElement();

			xml.WriteStartElement("_fc0");
			xml.WriteAttributeString("x", xsi.m_vBoundsMax.X.ToString());
			xml.WriteAttributeString("y", xsi.m_vBoundsMax.Y.ToString());
			xml.WriteAttributeString("z", xsi.m_vBoundsMax.Z.ToString());
			xml.WriteAttributeString("w", xsi.m_vBoundsMax.W.ToString());
			xml.WriteEndElement();

			xml.WriteStartElement("Childs");
			ChildsCollection childsCollection = new ChildsCollection();
			if (xsi.m_pChilds != 0)
			{
				br.Position = xsi.m_pChilds;
				ReadChildsCollection(br, ref childsCollection);
				_ = DataUtils.ReadStringAtOffset(childsCollection.m_pName, br); // unused
				XSI[] childs = new XSI[childsCollection._f0.m_nCount];
				uint[] pChilds = new uint[childsCollection._f0.m_nCount];
				br.Position = childsCollection._f0.m_pList;

				for (int a = 0; a < childsCollection._f0.m_nCount; a++)
				{
					pChilds[a] = br.ReadOffset();
				}
				
				for (int a = 0; a < childsCollection._f0.m_nCount; a++)
				{
					br.Position = pChilds[a];
					ReadXSI(br, ref childs[a]);
				}

				uint propCount = 0;
				for (int a = 0; a < childsCollection._f0.m_nCount; a++)
				{
					propCount += childs[a]._fd4.m_nCount;
				}

				Prop[] prop = new Prop[propCount];
				uint currentProp = 0;

				Curve[] curva = new Curve[childsCollection._f0.m_nCount];

				for (int a = 0; a < childsCollection._f0.m_nCount; a++)
				{
					uint[] p_fd4 = new uint[childs[a]._fd4.m_nCount];
					br.Position = childs[a]._fd4.m_pList;

					for (int b = 0; b < childs[a]._fd4.m_nCount; b++)
					{
						ReadProp(br, ref prop[currentProp]);
						currentProp++;
					}

					if (childs[a].m_pCurve != 0)
					{
						br.Position = childs[a].m_pCurve;
						ReadCurve(br, ref curva[a]);
					}
				}

				uint unksect1Count = 0;
				for (int a = 0; a < childsCollection._f0.m_nCount; a++)
				{
					unksect1Count += childs[a]._ffc.m_nCount;
				}

				int currentUnkSect1 = 0;
				cc0d7200[] unksect1 = new cc0d7200[unksect1Count];

				for (int a = 0; a < childsCollection._f0.m_nCount; a++)
				{
					if (childs[a]._ffc.m_pList == 0)
					{
						continue;
					}

					br.Position = childs[a]._ffc.m_pList;

					for (int b = 0; b < childs[a]._ffc.m_nCount; b++)
					{
						Readcc0d7200(br, ref unksect1[currentUnkSect1++]);
					}
				}

				currentUnkSect1 = 0;
				currentProp = 0;

				for (int a = 0; a < childsCollection._f0.m_nCount; a++)
				{
					xml.WriteStartElement("Item");

					if (!RDR_FileNames.fileNames.TryGetValue(childs[a].m_nGroupNameHash, out string childName))
					{
						childName = $"0x{childs[a].m_nGroupNameHash:X8}";
					}

					xml.WriteStartElement("GroupName");
					xml.WriteString(childName);
					xml.WriteEndElement();

					if (!RDR_FileNames.fileNames.TryGetValue(childs[a].m_nNameHash, out childName))
					{
						childName = $"0x{childs[a].m_nNameHash:X8}";
					}

					xml.WriteStartElement("Name");
					xml.WriteString(childName);
					xml.WriteEndElement();

					if (!(childs[a]._f90.X.ToString().ToLower().Contains("e") ||
						childs[a]._f90.Y.ToString().ToLower().Contains("e") ||
						childs[a]._f90.Z.ToString().ToLower().Contains("e") ||
						childs[a]._f90.W.ToString().ToLower().Contains("e")))
					{
						WriteVector(xml, "_f90", x: childs[a]._f90.X, y: childs[a]._f90.Y, z: childs[a]._f90.Z, w: childs[a]._f90.W);
					}

					if (!(childs[a]._fa0.X.ToString().ToLower().Contains("e") ||
						childs[a]._fa0.Y.ToString().ToLower().Contains("e") ||
						childs[a]._fa0.Z.ToString().ToLower().Contains("e") ||
						childs[a]._fa0.W.ToString().ToLower().Contains("e")))
					{
						WriteVector(xml, "_fa0", x: childs[a]._fa0.X, y: childs[a]._fa0.Y, z: childs[a]._fa0.Z, w: childs[a]._fa0.W);
					}

					WriteVector(xml, "_fb0", x: childs[a].m_vBoundsMin.X, y: childs[a].m_vBoundsMin.Y, z: childs[a].m_vBoundsMin.Z, w: childs[a].m_vBoundsMin.W);
					WriteVector(xml, "_fc0", x: childs[a].m_vBoundsMax.X, y: childs[a].m_vBoundsMax.Y, z: childs[a].m_vBoundsMax.Z, w: childs[a].m_vBoundsMax.W);

					xml.WriteStartElement("Curves");
					if (childs[a].m_pCurve != 0)
					{
						xml.WriteStartElement("Item");
						WriteVector(xml, "BoundsMin", x: curva[a].m_vBoundsMin.X, y: curva[a].m_vBoundsMax.Y, z: curva[a].m_vBoundsMax.Z);
						WriteVector(xml, "BoundsMax", x: curva[a].m_vBoundsMax.X, y: curva[a].m_vBoundsMax.Y, z: curva[a].m_vBoundsMax.Z);
						xml.WriteEndElement();
					}
					xml.WriteEndElement();

					xml.WriteStartElement("UnkSections1");
					if (childs[a]._ffc.m_pList != 0)
					{
						for (int b = 0; b < childs[a]._ffc.m_nCount; b++)
						{
							xml.WriteStartElement("Item");

							xml.WriteStartElement("Name");
							xml.WriteString(DataUtils.ReadStringAtOffset(unksect1[currentUnkSect1].m_pName, br));
							xml.WriteEndElement();

							WriteVector(xml, "BoundsMin", x: unksect1[currentUnkSect1].m_vBoundsMin.X, y: unksect1[currentUnkSect1].m_vBoundsMin.Y, z: unksect1[currentUnkSect1].m_vBoundsMin.Z);
							WriteVector(xml, "BoundsMax", x: unksect1[currentUnkSect1].m_vBoundsMax.X, y: unksect1[currentUnkSect1].m_vBoundsMax.Y, z: unksect1[currentUnkSect1].m_vBoundsMax.Z);
							WriteVector(xml, "Position", x: unksect1[currentUnkSect1].m_vPos.X, y: unksect1[currentUnkSect1].m_vPos.Y, z: unksect1[currentUnkSect1].m_vPos.Z);

							WriteVector(xml, "_f10", x: unksect1[currentUnkSect1]._f10.X, y: unksect1[currentUnkSect1]._f10.Y, z: unksect1[currentUnkSect1]._f10.Z);
							WriteVector(xml, "_f40_m0", x: unksect1[currentUnkSect1].matrix_m0.X, y: unksect1[currentUnkSect1].matrix_m0.Y, z: unksect1[currentUnkSect1].matrix_m0.Z);
							WriteVector(xml, "_f40_m1", x: unksect1[currentUnkSect1].matrix_m1.X, y: unksect1[currentUnkSect1].matrix_m1.Y, z: unksect1[currentUnkSect1].matrix_m1.Z);
							WriteVector(xml, "_f40_m2", x: unksect1[currentUnkSect1].matrix_m2.X, y: unksect1[currentUnkSect1].matrix_m2.Y, z: unksect1[currentUnkSect1].matrix_m2.Z);
							xml.WriteComment("Auto generated");
							float rotX = Convert.ToSingle(Math.Atan2(unksect1[currentUnkSect1].matrix_m2.Y, unksect1[currentUnkSect1].matrix_m2.Z));
							float rotY = Convert.ToSingle(Math.Atan2(-unksect1[currentUnkSect1].matrix_m2.X, Math.Sqrt(Math.Pow(unksect1[currentUnkSect1].matrix_m2.Y, 2d) + Math.Pow(unksect1[currentUnkSect1].matrix_m2.Z, 2d))));
							float rotZ = Convert.ToSingle(Math.Atan2(unksect1[currentUnkSect1].matrix_m1.X, unksect1[currentUnkSect1].matrix_m0.X));

							// convert the angles
							rotX = Convert.ToSingle(rotX * (180.0 / Math.PI));
							rotY = Convert.ToSingle(rotY * (180.0 / Math.PI));
							rotZ = Convert.ToSingle(rotZ * (180.0 / Math.PI));

							xml.WriteVector("Rotation", x: rotX, y: rotY, z: rotZ);

							currentUnkSect1++;
							xml.WriteEndElement();
						}
					}

					xml.WriteEndElement();
					xml.WriteStartElement("Props");

					for (int b = 0; b < childs[a]._fd4.m_nCount; b++)
					{
						xml.WriteStartElement("Item");
						xml.WriteStartElement("Name");
						xml.WriteString(DataUtils.ReadStringAtOffset(prop[currentProp].p_Name, br));
						xml.WriteEndElement();
						WriteVector(xml, "Position", x: prop[currentProp].m_vPos.X, y: prop[currentProp].m_vPos.Y, z: prop[currentProp].m_vPos.Z);
						xml.WriteEndElement();
						currentProp++;

					}
					xml.WriteEndElement();
					xml.WriteEndElement();
				}
			}

			xml.WriteEndElement();
			xml.WriteEndDocument();
			xml.Close();

			StringBuilder sb = new StringBuilder();
			for (int a = 0; a < exportedFileNames.Count; a++)
			{
				sb.AppendLine(exportedFileNames.ElementAt(a).Value);
			}

			File.WriteAllText($"{Path.GetDirectoryName(Main.inputPath)}\\{Path.GetFileNameWithoutExtension(Main.inputPath)}_names.txt", sb.ToString());
			return true;
		}
	}
}
