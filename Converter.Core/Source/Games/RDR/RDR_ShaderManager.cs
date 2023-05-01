using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Converter.Core.Utils;

namespace Converter.Core.Games.RDR
{
	public static class RDR_ShaderManager
	{
		public struct SHADERPARAM
		{
			public string type;
			public string name;
			public bool skip;
		}

		public struct SHADERINFO
		{
			public string name;
			public uint vertexFormat;
			public SHADERPARAM[] shaderparams;
		}
		
		public static SHADERINFO[] ShaderParams = new SHADERINFO[255];
		
		public static void LoadShaders()
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				IgnoreComments = true,
				IgnoreProcessingInstructions = true,
				IgnoreWhitespace = true
			};

			byte[] shadmanger = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RDR_Shadermanager.xml"));
			MemoryStream stream = new MemoryStream(shadmanger);
			XmlReader xml = XmlReader.Create(stream, settings);
			xml.Read();
			xml.Read();

			if (xml.NodeType != XmlNodeType.Element || xml.Name != "ShaderManager")
			{
				throw new Exception("Bad ShaderManager");
			}

			for (int a = 0; a < 255; a++)
			{
				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Shader")
				{
					if (xml.NodeType == XmlNodeType.EndElement || xml.Name == "ShaderManager")
					{
						Array.Resize(ref ShaderParams, a);
						break;
					}
					else
					{
						throw new Exception("Bad ShaderManager");
					}
				}

				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Name")
				{
					throw new Exception("Bad ShaderManager");
				}

				xml.Read();
				ShaderParams[a].name = xml.Value;
				xml.Read();
				if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "Name")
				{
					throw new Exception("Bad ShaderManager");
				}

				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "VertexFormat")
				{
					throw new Exception("Bad ShaderManager");
				}

				xml.Read();
				if (!uint.TryParse(xml.Value, out ShaderParams[a].vertexFormat))
				{
					throw new Exception("Bad uint value");
				}

				xml.Read();
				if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "VertexFormat")
				{
					throw new Exception("Bad ShaderManager");
				}

				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Variables")
				{
					throw new Exception("Bad ShaderManager");
				}

				if (xml.IsEmptyElement)
				{
					Array.Resize(ref ShaderParams[a].shaderparams, 0);
				}
				else
				{
					Array.Resize(ref ShaderParams[a].shaderparams, 255);
					for (int b = 0; b < 255; b++)
					{
						xml.Read();
						if (xml.NodeType != XmlNodeType.Element || xml.Name != "Item")
						{
							if (xml.NodeType == XmlNodeType.EndElement || xml.Name == "Variables")
							{
								Array.Resize(ref ShaderParams[a].shaderparams, b);
								break;
							}
							else
							{
								throw new Exception("Bad ShaderManager");
							}
						}
						ShaderParams[a].shaderparams[b].type = xml.GetAttribute("type");
						ShaderParams[a].shaderparams[b].name = xml.GetAttribute("name");
						if (!bool.TryParse(xml.GetAttribute("skip"), out ShaderParams[a].shaderparams[b].skip))
						{
							throw new Exception("Bad bool value");
						}
					}
				}
				xml.Read();
				if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "Shader")
				{
					throw new Exception("Bad ShaderManager");
				}
			}

			ShaderNames.LoadRDRShaderNames();
			Console.WriteLine($"[INFO] Loaded {ShaderParams.Length} shaders.");
		}

		public static int GetShaderIndex(string name)
		{
			for (int a = 0; a < ShaderParams.Length; a++)
			{
				if (name == ShaderParams[a].name)
				{
					return a;
				}
			}

			return -1;
		}

		public static int GetDiffuseSamplerValue(int index)
		{
			int x = 0;
			for (int a = 0; a < ShaderParams[index].shaderparams.Length; a++)
			{
				if (ShaderParams[index].shaderparams[a].type == "sampler")
				{
					switch (ShaderParams[index].shaderparams[a].name)
					{
						case "DiffuseSampler":
							return x;

						case "DiffuseTexSampler":
							return x;

						case "TextureSampler":
							return x;

						case "TextureSampler2":
							return x;

						case "TextureSampler3":
							return x;

						case "TerrainDiffuseSampler1":
							return x;

						case "TerrainDiffuseSampler2":
							return x;

						case "TerrainDiffuseSampler3":
							return x;

						case "TerrainDiffuseSampler4":
							return x;

						case "TerrainDiffuseSampler5":
							return x;

						case "TerrainDiffuseSampler6":
							return x;

						case "TrackTextureSampler":
							return x;

						case "VertexTextureSampler":
							return x;

						case "DetailMapSampler":
							return x;

						case "PerlinNoiseSampler":
							return x;

						case "HighDetailNoiseSampler":
							return x;

						case "DiffuseBillboardSampler":
							return x;

						case "CrackSampler":
							return x;

						case "DecalSampler":
							return x;

						case "BrokenDiffuseSampler":
							return x;

						case "DetailSampler":
							return x;

						case "DiffuseSamplerPhase1":
							return x;

						case "DiffuseSamplerPhase2":
							return x;

						case "GrassTintSampler":
							return x;

						case "ParticleSampler":
							return x;

						case "DeathSampler":
							return x;

						case "RiverFoamSampler":
							return x;

						default:
							x++;
							break;
					}
				}
			}

			return -1;
		}

		public class ShaderNames
		{
			public static Dictionary<uint, string> shaderNames;

			public static void LoadRDRShaderNames()
			{
				shaderNames = new Dictionary<uint, string>();

				for (int a = 0; a < ShaderParams.Length; a++)
				{
					uint hash = DataUtils.GetHash(ShaderParams[a].name);

					if (!shaderNames.ContainsKey(hash))
					{
						shaderNames.Add(hash, ShaderParams[a].name);
					}
				}
			}
		}

		// ----- new code

		public static string shadersPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RDR_Shadermanager.xml");

		public static void Read()
		{
			if (!File.Exists(shadersPath))
			{
				Console.WriteLine("[ERROR] RDR_Shadermanager.xml was not found, exiting...");
				Environment.Exit(0);
			}

			XDocument doc = XDocument.Load(shadersPath);
			XElement rootNode = doc.Root;

			if (rootNode.Name == "ShaderManager")
			{
				// process sub-nodes
				foreach (XElement subNode in doc.Descendants().Descendants())
				{
					if (subNode.Name == "Shader")
					{
						// foreach all subnodes
						// switch by subnode name
						// if Name then read Value
						// if VertexFormat then read Value
						// if Variables then foreach all subnodes like in settings.xml
						//
						// ShaderParams[a].name = xml.Value;
						// uint.TryParse(xml.Value, out ShaderParams[a].vertexFormat);
						//
						// ShaderParams[a].shaderparams[b].type = xml.GetAttribute("type");
						// ShaderParams[a].shaderparams[b].name = xml.GetAttribute("name");
						// bool.TryParse(xml.GetAttribute("skip"), out ShaderParams[a].shaderparams[b].skip);
					}
					else
					{
						Console.WriteLine("[ERROR] Could not find valid shader data.");
						Environment.Exit(0);
					}
				}
			}
			else
			{
				Console.WriteLine("[ERROR] Incorrect shaders file.");
				Environment.Exit(0);
			}

			// is this call should be placed here?
			ShaderNames.LoadRDRShaderNames();
			Console.WriteLine($"[INFO] Loaded {ShaderParams.Length} shaders.");
		}
	}
}
