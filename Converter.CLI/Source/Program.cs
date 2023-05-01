using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using CommandLine;
using Converter.Core;
using Converter.Core.Games.RDR;

namespace Converter.CLI
{
	internal class Program
	{
		public class Options
		{
			[Option('l', "basic-log", Required = false, HelpText = "Basic logging (verbose) mode. Contains a basic details about opened resource.")]
			public bool Verbose { get; set; }

			[Option('e', "extended-log", Required = false, HelpText = "Extended logging (very verbose) mode. Contains a basic + exporting details.")]
			public bool VeryVerbose { get; set; }

			[Option('n', "nowait", Required = false, HelpText = "Disable waiting for user input when all tasks are finished/failed.")]
			public bool WaitForUserInput { get; set; }

			[Option('i', "input", Required = true, HelpText = "The path to folder or file which needs to be converted.")]
			public string InputPath { get; set; }

			[Option('o', "output", Required = false, HelpText = "The path to folder(!) where you want to save the result. If it's not specified, then output directory will be the same as input.")]
			public string OutputPath { get; set; }
		}

		static void Main(string[] args)
		{
			Parser.Default.ParseArguments<Options>(args)
				.WithParsed(RunOptions)
				.WithNotParsed(HandleParseError);
		}

		static void RunOptions(Options opts)
		{
			// check all required libraries
			if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XnaNative.dll")))
			{
				Console.WriteLine("[ERROR] XnaNative.dll was not found.");
				Console.WriteLine("        This library is required to run this tool, but for copyright reasons we can't distribute it here.");
				Console.WriteLine("        Press Enter to open download link in your browser, or any key to exit.");
				Console.WriteLine("        Just place the downloaded file in the directory where Converter.exe is located.");

				if (Console.ReadKey().Key == ConsoleKey.Enter)
				{
					OpenBrowser("https://drive.google.com/file/d/1YRk8I7r0MM3fopS-w_LE0CF_Q5oPOa22/view?usp=share_link");
				}
				else
				{
					Environment.Exit(0);
				}
				return;
			}

			// next we should load the settings
			Settings.Read();

			// apply some global variables
			Core.Main.useVerboseMode = opts.Verbose;
			Core.Main.useVeryVerboseMode = opts.VeryVerbose;
			Core.Main.inputPath = opts.InputPath; // it will be needed later to get an input file name without full path
			Core.Main.outputPath = opts.OutputPath; // path to the output folder, not file! even if we work with single file!
			
			// next we should fix the number format separator (for some regional cultures) by forcing the culture to US
			System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

			// print a message which indicates about starting the work
			Console.WriteLine("[INFO] Started.");

			// load neccessary data
			// the output of information about the results
			// of loading has been moved to the appropriate methods
			RDR_ShaderManager.LoadShaders();
			RDR_FileNames.LoadRDRFileNames();

			// check the type of input path:
			// if the user wants to perform a batch processing (by picking a folder)
			// then grab all file paths in it and then process them
			// otherwise just process the input path "as is"
			// original idea was taken from:
			// https://stackoverflow.com/questions/1395205/better-way-to-check-if-a-path-is-a-file-or-a-directory
			//
			FileAttributes attr = File.GetAttributes(Core.Main.inputPath);
			if (attr.HasFlag(FileAttributes.Directory))
			{
				if (string.IsNullOrEmpty(Core.Main.outputPath))
				{
					Core.Main.outputPath = Core.Main.inputPath;
				}

				// search all files even without extension, only in current directory, without subdirectories
				string[] files = Directory.GetFiles(Core.Main.inputPath, "*", SearchOption.TopDirectoryOnly);

				// and process them
				for (int i = 0; i < files.Length; i++)
				{
					Core.Main.inputPath = files[i];
					Core.Main.ProcessSingleFile();
				}
			}
			else
			{
				if (string.IsNullOrEmpty(Core.Main.outputPath))
				{
					Core.Main.outputPath = Path.GetDirectoryName(Core.Main.inputPath);
				}

				// process the input as a file
				Core.Main.ProcessSingleFile();
			}

			// all tasks finished (or failed), so wait for user input (if needed) and exit
			if (opts.WaitForUserInput)
			{
				Console.WriteLine("[INFO] Finished. Press any key to exit.");
				Console.ReadLine();
			}
			else
			{
				Console.WriteLine("[INFO] Finished.");
			}
		}

		static void HandleParseError(IEnumerable<Error> errs)
		{
			Console.WriteLine("Press any key to exit.");
			Console.ReadLine();
			Environment.Exit(0);
		}

		// TODO: remove that
		/// <summary>
		/// Opens the default web browser with given URL
		/// </summary>
		/// <param name="url"></param>
		private static void OpenBrowser(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				Console.WriteLine("[ERROR] URL is not given.");
				return;
			}

			try
			{
				Process.Start(url);
			}
			catch
			{
				// hack because of this: https://github.com/dotnet/corefx/issues/10361
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
				}
			}
		}
	}
}
