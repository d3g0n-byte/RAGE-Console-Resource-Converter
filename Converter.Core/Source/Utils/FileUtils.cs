using System;
using System.ComponentModel;
using System.IO;

namespace Converter.Core.Utils
{
	public static class FileUtils
	{
		/// <summary>
		/// If directory or file exists.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public static bool Exists(string location)
		{
			return File.Exists(location) || Directory.Exists(location);
		}

		[Localizable(false)]
		public static string EnsureUnique(string filename, string postfix, bool forcePostfix, int startFrom = 1)
		{
			return EnsureUnique(false, filename, postfix, forcePostfix, startFrom);
		}

		public static string EnsureUnique(bool holdPlace, string filename, string postfix, bool forcePostfix, int startFrom = 1)
		{
			if (!forcePostfix && !Exists(filename))
			{
				if (holdPlace)
				{
					using (File.Create(filename))
					{
					}
				}

				return filename;
			}

			string ext = Path.GetExtension(filename);
			string start = filename.Substring(0, filename.Length - ext.Length);

			for (int i = startFrom; i < 99999; i++)
			{
				string result = start + string.Format(postfix, i) + ext;
				if (Exists(result))
				{
					continue;
				}

				if (holdPlace)
				{
					using (File.Create(result))
					{
					}
				}

				return result;
			}

			throw new Exception("Can’t find unique filename");
		}

		[Localizable(false)]
		public static string EnsureUnique(string filename, string postfix = "-{0}")
		{
			return EnsureUnique(false, filename, postfix);
		}

		[Localizable(false)]
		public static string EnsureUnique(bool holdPlace, string filename, string postfix = "-{0}")
		{
			return EnsureUnique(holdPlace, filename, postfix, false);
		}
	}
}
