using SharpDX.Direct3D11;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Converter.utils
{
	internal class RDR_ShaderManagerCreator
	{
		// пцель этого класса - создать базу с шейдерами для конвертера, чтобы можно было их добавить в ofio в далеком будущеем.
		// для этого я буду использовать xml файл и создавать что-то типу того, что в openiv.
		// структура fxc файлов взята из моего редактора шейдеров, так что не думайте, что я взял ее из MagicRDR.

		/* НЕ ИСПОЛЬЗУЕТСЯ!
		struct SHADERMANAGER
		{
			uint magic; // 
			uint version; // в чтобы если я узнаю что-то новое о шейдерах, добавить что-то новое и сделать как другую версию чтобы конвертер понимал и старую и новую
			uint mainSygment;
			uint secondSygment; // для названий и чего-то еще
			// дальше идут два массива, которые будут сжатыми
			byte[] mainBuffer;
			byte[] secondBuffer;
		}*/
		static string ReadString(BinaryReader br)
		{
			byte size = br.ReadByte();
			char[] tmp = new char[size - 1];
			tmp = br.ReadChars(size - 1);
			size = br.ReadByte();
			string tempstring = new string(tmp);
			return tempstring;
		}

		struct SHADERPARAM
		{
			public string type;
			public string name;
			public bool skip;
		}
		struct SHADERINFO
		{
			public string name;
			public uint vertexFormat;
			public SHADERPARAM[] shaderparams;
		}
		static void ReadAnnotation(BinaryReader br)
		{
			ReadString(br);
			byte type = br.ReadByte();
			if (type == 2) ReadString(br);
			else br.ReadUInt32();
		}
		static string TypeAsString(byte type)
		{
			switch (type)
			{
				case 1: return "int";
				case 2: return "float";
				case 3: return "float2";
				case 4: return "float3";
				case 5: return "float4";
				case 6: return "sampler";
				case 7: return "bool";
				case 8: return "float4x3";
				case 9: return "float4x4";
			}
			return "unkType";
		}
		static string[] VarNames = new string[1000];
		static uint VarNamesCount = 0;
		static void AddNameToBuffer(string name)
		{
			for (int a = 0; a < VarNamesCount; a++)
			{
				if (name.ToLower() == VarNames[a].ToLower()) return;
			}
			VarNames[VarNamesCount] = name;
			VarNamesCount++;
		}
		public static void ScanFXCShaders(string filePath = "E:\\rdr\\shaders\\fxl_final\\rage_curvedmodel.fxc", bool exportVarNames = false, bool useLZX = false)
		{
			string[] files = Directory.GetFiles(Path.GetDirectoryName(filePath));
			MemoryStream stream;
			BinaryReader br;
			SHADERINFO[] shaders = new SHADERINFO[files.Length];
			for (int a = 0; a < files.Length; a++)
			{
				shaders[a].name = Path.GetFileName(files[a].Replace(".fxc", ""));
				byte[] buffer = File.ReadAllBytes(files[a]);
				stream = new MemoryStream(buffer);
				br = new BinaryReader(stream);
				br.ReadUInt32();
				shaders[a].vertexFormat = br.ReadUInt32();
				byte preparamCount = br.ReadByte();
				for (int b = 0; b < preparamCount; b++) ReadAnnotation(br);
				byte vertexFragCount = br.ReadByte();
				for (int b = 0; b < vertexFragCount; b++)
				{
					ReadString(br);// имя
					byte paramCount = br.ReadByte();
					for (int c = 0; c < paramCount; c++) ReadString(br);
					ushort shaderSize = br.ReadUInt16();
					br.ReadBytes(shaderSize);
				}
				byte pixelFragCount = br.ReadByte();
				pixelFragCount--;
				ReadString(br); // null
				br.ReadUInt16(); // нули
				br.ReadByte(); // нули
				for (int b = 0; b < pixelFragCount; b++)
				{
					ReadString(br);// имя
					byte paramCount = br.ReadByte();
					for (int c = 0; c < paramCount; c++) ReadString(br);
					ushort shaderSize = br.ReadUInt16();
					br.ReadBytes(shaderSize);
				}
				byte globalParamCount = br.ReadByte();
				for (int b = 0; b < globalParamCount; b++)
				{
					byte type = br.ReadByte();
					byte arrayCount = br.ReadByte();
					byte slot = br.ReadByte();
					byte group = br.ReadByte();
					string name = ReadString(br);
					string name2 = ReadString(br);
					byte annotationCount = br.ReadByte();
					for (int c = 0; c < annotationCount; c++) ReadAnnotation(br);
					byte valueCount = br.ReadByte();
					for (int c = 0; c < valueCount; c++) br.ReadInt32();
					AddNameToBuffer(name);
				}
				byte localParamCount = br.ReadByte();
				Array.Resize<SHADERPARAM>(ref shaders[a].shaderparams, localParamCount);
				for (int b = 0; b < localParamCount; b++)
				{
					shaders[a].shaderparams[b].type = TypeAsString(br.ReadByte());
					byte arrayCount = br.ReadByte();
					byte slot = br.ReadByte();
					byte group = br.ReadByte();
					shaders[a].shaderparams[b].name = ReadString(br);
					string name2 = ReadString(br);
					byte annotationCount = br.ReadByte();
					for (int c = 0; c < annotationCount; c++) ReadAnnotation(br);
					byte valueCount = br.ReadByte();
					for (int c = 0; c < valueCount; c++) br.ReadInt32();
					AddNameToBuffer(shaders[a].shaderparams[b].name);
				}
			}
			//
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			// other settings...
			//settings.Encoding = Encoding.ASCII;
			settings.IndentChars = ("\t");
			//settings.ConformanceLevel = ConformanceLevel.Fragment;
			settings.DoNotEscapeUriAttributes = true;
			settings.OmitXmlDeclaration = false;
			string shaderManagerPath = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\\RDR_Shadermanager.xml";
			XmlWriter xmlWriter = XmlWriter.Create(shaderManagerPath, settings);

			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("ShaderManager");
			for (int a = 0; a < shaders.Length; a++)
			{
				xmlWriter.WriteStartElement("Shader");
				xmlWriter.WriteElementString("Name", shaders[a].name);
				xmlWriter.WriteElementString("VertexFormat", shaders[a].vertexFormat.ToString());
				xmlWriter.WriteStartElement("Variables");
				for (int b = 0; b < shaders[a].shaderparams.Length; b++)
				{
					xmlWriter.WriteStartElement("Item");
					xmlWriter.WriteAttributeString("type", shaders[a].shaderparams[b].type);
					xmlWriter.WriteAttributeString("name", shaders[a].shaderparams[b].name);
					xmlWriter.WriteAttributeString("skip", "false");
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Close();
			if (useLZX)
			{
				byte[] shadersAsBytes = File.ReadAllBytes(shaderManagerPath);
				LZXTestUtils.CompressAndWriteConverterData(shadersAsBytes, shaderManagerPath);
			}
			if (exportVarNames)
			{
				StringBuilder stringBuilder= new StringBuilder();
				for (int a = 0; a < VarNamesCount; a++)
				{
					stringBuilder.AppendLine(VarNames[a]);
				}
				File.WriteAllText($"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\\RDR_ShaderVariableNames.txt", stringBuilder.ToString());
			}

		}
	}
}
