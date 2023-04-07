using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Converter.utils
{
	internal class RDR_ShaderManager
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
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreComments = true;
			settings.IgnoreProcessingInstructions = true;
			settings.IgnoreWhitespace = true;
			byte[] shadmanger = File.ReadAllBytes($"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\\RDR_Shadermanager.xml");
			MemoryStream stream = new MemoryStream(shadmanger);
			BinaryReader br = new BinaryReader(stream);
			if(br.ReadInt32()== 372003) // я использую сжатые, но и в обычнов виде он также принимает
			{
				int version = br.ReadInt32();
				int usize = br.ReadInt32();
				byte[] tmp = br.ReadBytes(shadmanger.Length-12);
				byte[] shaderManagerNew = new byte[usize];
				DataUtils.DecompressLZX(tmp, shaderManagerNew);
				//File.WriteAllBytes("tmp.bin", shaderManagerNew);
				Array.Resize<byte>(ref shadmanger, shaderManagerNew.Length);
				Buffer.BlockCopy(shaderManagerNew, 0, shadmanger, 0, shaderManagerNew.Length);
			}
			br.Close();
			stream = new MemoryStream(shadmanger);
			XmlReader xml = XmlReader.Create(stream, settings);
			xml.Read();
			xml.Read();
			if (xml.NodeType != XmlNodeType.Element || xml.Name != "ShaderManager") throw new Exception("Bad ShaderManager");

			for (int a = 0; a < 255; a++)
			{
				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Shader")
				{
					if (xml.NodeType == XmlNodeType.EndElement || xml.Name == "ShaderManager")
					{
						Array.Resize<SHADERINFO>(ref ShaderParams, a);
						break;
					}
					else throw new Exception("Bad ShaderManager");
				}

				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Name") throw new Exception("Bad ShaderManager");
				xml.Read();
				ShaderParams[a].name = xml.Value;
				xml.Read();
				if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "Name") throw new Exception("Bad ShaderManager");

				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "VertexFormat") throw new Exception("Bad ShaderManager");
				xml.Read();
				if (!uint.TryParse(xml.Value, out ShaderParams[a].vertexFormat)) throw new Exception("Bad uint value");
				xml.Read();
				if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "VertexFormat") throw new Exception("Bad ShaderManager");

				xml.Read();
				if (xml.NodeType != XmlNodeType.Element || xml.Name != "Variables") throw new Exception("Bad ShaderManager");
				if (xml.IsEmptyElement) Array.Resize<SHADERPARAM>(ref ShaderParams[a].shaderparams, 0);
				else
				{
					Array.Resize<SHADERPARAM>(ref ShaderParams[a].shaderparams, 255);
					for (int b = 0; b < 255; b++)
					{

						xml.Read();
						if (xml.NodeType != XmlNodeType.Element || xml.Name != "Item")
						{
							if (xml.NodeType == XmlNodeType.EndElement || xml.Name == "Variables")
							{
								Array.Resize<SHADERPARAM>(ref ShaderParams[a].shaderparams, b);
								break;
							}
							else throw new Exception("Bad ShaderManager");
						}
						ShaderParams[a].shaderparams[b].type = xml.GetAttribute("type");
						ShaderParams[a].shaderparams[b].name = xml.GetAttribute("name");
						if (!bool.TryParse(xml.GetAttribute("skip"), out ShaderParams[a].shaderparams[b].skip)) throw new Exception("Bad bool value");
					}
				}
				xml.Read();
				if (xml.NodeType != XmlNodeType.EndElement || xml.Name != "Shader") throw new Exception("Bad ShaderManager");

			}
			//
			ShaderNames.LoadRDRShaderNames();
		}
		public static int GetShaderIndex(string name)
		{
			for (int a = 0; a < ShaderParams.Length; a++)
			{
				if (name == ShaderParams[a].name) return a;
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
					if (ShaderParams[index].shaderparams[a].name == "DiffuseSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "DiffuseTexSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "TextureSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "TextureSampler2") return x;
					else if (ShaderParams[index].shaderparams[a].name == "TextureSampler3") return x;
					else if (ShaderParams[index].shaderparams[a].name == "TerrainDiffuseSampler1") return x;
					else if (ShaderParams[index].shaderparams[a].name == "TerrainDiffuseSampler2") return x;
					else if (ShaderParams[index].shaderparams[a].name == "TerrainDiffuseSampler3") return x;
					else if (ShaderParams[index].shaderparams[a].name == "TerrainDiffuseSampler4") return x;
					else if (ShaderParams[index].shaderparams[a].name == "TerrainDiffuseSampler5") return x;
					else if (ShaderParams[index].shaderparams[a].name == "TerrainDiffuseSampler6") return x;
					else if (ShaderParams[index].shaderparams[a].name == "TrackTextureSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "VertexTextureSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "DetailMapSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "PerlinNoiseSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "HighDetailNoiseSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "DiffuseBillboardSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "CrackSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "DecalSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "BrokenDiffuseSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "DetailSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "DiffuseSamplerPhase1") return x;
					else if (ShaderParams[index].shaderparams[a].name == "DiffuseSamplerPhase2") return x;
					else if (ShaderParams[index].shaderparams[a].name == "GrassTintSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "ParticleSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "DeathSampler") return x;
					else if (ShaderParams[index].shaderparams[a].name == "RiverFoamSampler") return x;
					else x++;
				}
			}
			return -1;
		}

		public class ShaderNames
		{
			static string RDRShadersPath = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\\shadersRDR.txt";
			public static Dictionary<uint, string> shaderNames;
			public static void LoadRDRShaderNames()
			{
				shaderNames = new Dictionary<uint, string>();
				uint hash;
				for (int a = 0; a < RDR_ShaderManager.ShaderParams.Length; a++)
				{
					hash = DataUtils.GetHash(RDR_ShaderManager.ShaderParams[a].name);
					if (!shaderNames.ContainsKey(hash)) shaderNames.Add(hash, RDR_ShaderManager.ShaderParams[a].name);
				}

			}
		}
	}

}
