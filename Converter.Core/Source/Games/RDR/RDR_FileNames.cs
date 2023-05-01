using System;
using System.Collections.Generic;
using System.IO;
using Converter.Core.Utils;

namespace Converter.Core.Games.RDR
{
	public static class RDR_FileNames
	{
		public static Dictionary<uint, string> fileNames;

		public static void LoadRDRFileNames()
		{
			byte[] names = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RDR_FileNames.txt"));
			using (MemoryStream stream = new MemoryStream(names))
			{
				using (StreamReader sr = new StreamReader(stream))
				{
					string name;

					fileNames = new Dictionary<uint, string>();
					uint hash;

					while ((name = sr.ReadLine()) != null)
					{
						hash = DataUtils.GetHash(name);
						if (!fileNames.ContainsKey(hash))
						{
							fileNames.Add(hash, name);
						}
					}
				}
			}

			Console.WriteLine($"[INFO] Loaded {fileNames.Count} file names.");
		}
	}
}
