using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace Configurator {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	partial class App : Application {
		static App() {
			ExePath = typeof(App).Assembly.Location;
#if DEBUG
			//In the VS designer, the EXE is copied to a temp folder, without the XML comments file.
			//However, I want to load ShomreiTorahConfig and the XML doc file from the source tree in the VS designer.
			//I use a pre-build event to get the actual project directory for this scenario.
			if (!File.Exists(Path.ChangeExtension(ExePath, ".xml"))
			 && File.Exists(Path.ChangeExtension(OutputPath, ".xml")))
				ExePath = OutputPath;
#endif

			SourceRoot = FindRoot(ExePath);
		}

		///<summary>Indicates whether we can locate the standard environmental config files in the source tree.</summary>
		///<remarks>This property is data-bound in XAML to disable various buttons.</remarks>
		public static bool HasEnvironmentLocations { get { return SourceRoot != null; } }

		///<summary>Gets the path to the executing EXE file.</summary>
		///<remarks>This path is used to load the XML doc comments.</remarks>
		public static string ExePath { get; private set; }

		///<summary>Gets the root directory containing the git repo tree.</summary>
		///<remarks>This directory contains the Config folder.</remarks>
		public static string SourceRoot { get; private set; }

		static string FindRoot(string path) {
			while (true) {
				if (String.IsNullOrEmpty(path))
					return null;
				if (Directory.Exists(Path.Combine(path, @"Setup\.git")))
					return path;

				path = Path.GetDirectoryName(path);
			}
		}

	}
}
