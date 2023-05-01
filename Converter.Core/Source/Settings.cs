using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Converter.Core.Utils;

namespace Converter.Core
{
	public static class Settings
	{
		public static string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");
		public static bool bExportShaders = true;
		public static bool bSwapYAndZ = false;
		public static uint nTempShadersCount = 50;
		public static bool bSaveUnpackedResource = false;
		public static string sTexturesFolder = "null";
		public static string sAdditionalTexturesFolder = "null";
		//public static bool bExportMipMaps = true;
		public static bool bConvertShadersToIV = false;
		
		public static void Read()
		{
			if (!File.Exists(settingsPath))
			{
				Console.WriteLine("[WARNING] settings.xml was not found, creating a new one with default values...");
				Write();
			}

			XDocument doc = XDocument.Load(settingsPath);
			XElement rootNode = doc.Root;

			if (rootNode.Name == "Settings")
			{
				// process sub-nodes
				foreach (XElement subNode in doc.Descendants().Descendants())
				{
					if (subNode.Name == "Item")
					{
						foreach (KeyValuePair<string, string> kvp in subNode.Attributes().GetAttributes())
						{
							switch (kvp.Key)
							{
								case "ExportShaders":
									bExportShaders = Convert.ToBoolean(kvp.Value);
									break;

								case "SwapAxis":
									bSwapYAndZ = Convert.ToBoolean(kvp.Value);
									break;

								case "TempShadersCount":
									nTempShadersCount = Convert.ToUInt32(kvp.Value);
									break;

								case "SaveUnpackedResource":
									bSaveUnpackedResource = Convert.ToBoolean(kvp.Value);
									break;

								case "TexturesFolder":
									sTexturesFolder = kvp.Value;
									break;

								case "AdditionalTexturesFolder":
									sAdditionalTexturesFolder = kvp.Value;
									break;
							}
						}
					}
					else
					{
						Console.WriteLine("[ERROR] Could not find valid parameters for settings.");
						Environment.Exit(0);
					}
				}
			}
			else
			{
				Console.WriteLine("[ERROR] Incorrect settings file.");
				Environment.Exit(0);
			}
		}

		public static void Write()
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
				DoNotEscapeUriAttributes = true,
				OmitXmlDeclaration = false
			};

			using (XmlWriter xmlWriter = XmlWriter.Create(settingsPath, settings))
			{
				xmlWriter.WriteStartElement("Settings");

				xmlWriter.WriteStartElement("Item");
				xmlWriter.WriteAttributeString("ExportShaders", bExportShaders.ToString());
				xmlWriter.WriteEndElement();

				xmlWriter.WriteStartElement("Item");
				xmlWriter.WriteAttributeString("SwapAxis", bSwapYAndZ.ToString());
				xmlWriter.WriteEndElement();

				xmlWriter.WriteStartElement("Item");
				xmlWriter.WriteAttributeString("TempShadersCount", nTempShadersCount.ToString());
				xmlWriter.WriteEndElement();

				xmlWriter.WriteStartElement("Item");
				xmlWriter.WriteAttributeString("SaveUnpackedResource", bSaveUnpackedResource.ToString());
				xmlWriter.WriteEndElement();

				xmlWriter.WriteStartElement("Item");
				xmlWriter.WriteAttributeString("TexturesFolder", sTexturesFolder);
				xmlWriter.WriteEndElement();

				xmlWriter.WriteStartElement("Item");
				xmlWriter.WriteAttributeString("AdditionalTexturesFolder", sAdditionalTexturesFolder);
				xmlWriter.WriteEndElement();

				xmlWriter.WriteEndElement();
			}
		}
	}
}
