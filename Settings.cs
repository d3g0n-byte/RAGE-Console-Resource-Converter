using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Converter
{
	internal class Settings
	{
		/// для тестирования
		static string settingsPath = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\\settings.xml";
		public static bool bExportShaders = true;
		public static bool bSwapYAndZ = false;
		public static uint nTempShadersCount = 50;
		public static bool bSaveUnpackedResource = false;
		public static string sNumberDecimalSeparator = ".";

		static void WriteSettings()
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			// other settings...
			//settings.Encoding = Encoding.ASCII;
			settings.IndentChars = ("\t");
			//settings.ConformanceLevel = ConformanceLevel.Fragment;
			settings.DoNotEscapeUriAttributes = true;
			settings.OmitXmlDeclaration = false;

			XmlWriter xmlWriter = XmlWriter.Create(@"settings.xml", settings);
			xmlWriter.WriteStartElement("Settings");
			xmlWriter.WriteStartElement("Item");
			xmlWriter.WriteAttributeString("ExportShaders", "true");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("Item");
			xmlWriter.WriteAttributeString("SwapAxis", "false");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("Item");
			xmlWriter.WriteAttributeString("TempShadersCount", "50");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("Item");
			xmlWriter.WriteAttributeString("SaveUnpackedResource", "false");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("Item");
			xmlWriter.WriteAttributeString("NumberDecimalSeparator", ".");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.Flush();
			xmlWriter.Close();
		}
		public static void ReadSettings()
		{

			if (!File.Exists(settingsPath)) WriteSettings();
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreComments = true;
			settings.IgnoreProcessingInstructions = true;
			settings.IgnoreWhitespace = true;
			uint magic;
			XmlReader reader = XmlReader.Create(settingsPath, settings);
			reader.Read();
			reader.Read();
			if (reader.NodeType != XmlNodeType.Element || reader.Name != "Settings") throw new Exception("Settings not loaded");
			reader.Read();
			//while (reader.NodeType != XmlNodeType.EndElement && reader.Name != "Settings")
			bExportShaders = Convert.ToBoolean(reader.GetAttribute("ExportShaders"));
			reader.Read();
			bSwapYAndZ = Convert.ToBoolean(reader.GetAttribute("SwapAxis"));
			reader.Read();
			nTempShadersCount = Convert.ToUInt32(reader.GetAttribute("TempShadersCount"));
			reader.Read();
			bSaveUnpackedResource = Convert.ToBoolean(reader.GetAttribute("SaveUnpackedResource"));
			reader.Read();
			sNumberDecimalSeparator = reader.GetAttribute("NumberDecimalSeparator");
			reader.Read();
			if (reader.NodeType != XmlNodeType.EndElement || reader.Name != "Settings") throw new Exception("Settings not loaded");
		}
	}
}
