using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.utils
{
	internal class RDR_FileNames
	{
		static string RDRFileNamesPath = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\\RDR_FileNames.data";
		public static Dictionary<uint, string> fileNames;
		public static void LoadRDRFileNames()
		{
			byte[] names = File.ReadAllBytes(RDRFileNamesPath);
			MemoryStream stream = new MemoryStream(names);
			BinaryReader br = new BinaryReader(stream);
			if (br.ReadInt32() == 372003) // я использую сжатые, но и в обычнов виде он также принимает
			{
				int version = br.ReadInt32();
				switch (version)
				{
					case 1:
						int usize = br.ReadInt32();
						byte[] tmp = br.ReadBytes(names.Length - 12);
						byte[] shaderManagerNew = new byte[usize];
						DataUtils.DecompressLZX(tmp, shaderManagerNew);
						//File.WriteAllBytes("tmp.bin", shaderManagerNew);
						Array.Resize<byte>(ref names, shaderManagerNew.Length);
						Buffer.BlockCopy(shaderManagerNew, 0, names, 0, shaderManagerNew.Length);
						break;
					default:
						Log.ToLog(Log.MessageType.ERROR, $"Unknown RDR_FileNames.data file version {version}.");
						throw new Exception("wrong data");
				}
			}
			br.Close();
			stream = new MemoryStream(names);

			var sw = new StreamReader(stream);
			string name;

			fileNames = new Dictionary<uint, string>();
			uint hash;
			while ((name = sw.ReadLine()) != null)
			{
				hash = DataUtils.GetHash(name);
				if (!fileNames.ContainsKey(hash)) fileNames.Add(hash, name);
			}
		}

	}
}
