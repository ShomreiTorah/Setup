using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.Linq;
using Microsoft.Win32;

namespace Configurator {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	partial class MainWindow : Window {
		///<summary>Gets the root directory containing the git repo tree.</summary>
		///<remarks>This directory contains the Config folder.</remarks>
		readonly string sourceRoot;

		Schema.ConfigRoot root;
		public bool HasEnvironmentLocations { get; private set; }
		public MainWindow() {
			//This must run before InitializeComponent so that HasLocations can be data-bound.
			sourceRoot = Path.GetDirectoryName(typeof(MainWindow).Assembly.Location);
			while (true) {
				if (string.IsNullOrEmpty(sourceRoot)) {
					MessageBox.Show("This utility must be run from within the git source tree.\nThe environmental buttons will not work.",
									"Shomrei Torah Configurator", MessageBoxButton.OK, MessageBoxImage.Warning);
					HasEnvironmentLocations = false;
					break;
				}
				if (Directory.Exists(Path.Combine(sourceRoot, @"Setup\.git"))) {
					HasEnvironmentLocations = true;
					Directory.CreateDirectory(Path.Combine(sourceRoot, @"Config\Debug"));
					Directory.CreateDirectory(Path.Combine(sourceRoot, @"Config\Production"));
					break;
				}
				sourceRoot = Path.GetDirectoryName(sourceRoot);
			}

			InitializeComponent();
			var args = Environment.GetCommandLineArgs();
			if (args.Length >= 2)
				OpenFile(String.Join(" ", args, 1, args.Length - 1));
			else
				CreateNew();
		}

		void Rebind() {
			if (tabs.DataContext == root)
				tabs.DataContext = null;
			tabs.DataContext = root;
		}

		#region File Management
		XElement original;
		void SetOriginal() {
			original = new XElement("TempContainer");
			root.WriteXml(original);
			original.Trim();
		}
		///<summary>Confirms whether the user would like to discard any unsaved changes.</summary>
		///<returns>True if the user wishes to proceed; false to cancel.</returns>
		bool ConfirmClose() {
			var comparand = new XElement(original.Name);
			root.WriteXml(comparand);
			comparand.Trim();
			if (XElement.DeepEquals(original, comparand))
				return true;

			switch (MessageBox.Show("Would you like to save your changes to " + (CurrentPath ?? "this unsaved file") + "?", "Shomrei Torah Configurator", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)) {
				case MessageBoxResult.Yes:
					DoSave();
					return true;
				case MessageBoxResult.No:
					return true;
				case MessageBoxResult.Cancel:
					return false;
			}
			throw new InvalidProgramException("Broken switch");
		}

		string currentPath;
		public string CurrentPath {
			get { return currentPath; }
			set {
				currentPath = value;
				Title = "Shomrei Torah Configurator - " + (currentPath ?? "Unsaved");
			}
		}

		void OpenFile(string path) {
			CurrentPath = path;
			root = new Schema.ConfigRoot();
			root.ReadXml(XElement.Load(path));
			SetOriginal();
			Rebind();
		}

		void SaveFile(string path) {
			CurrentPath = path;

			XDocument xml;
			if (File.Exists(path))
				xml = XDocument.Load(path);
			else
				xml = new XDocument(
					new XDeclaration("1.0", "utf-8", "yes"),
					new XElement("ShomreiTorahConfig")
				);

			root.WriteXml(xml.Root);
			xml.Root.Trim();
			xml.SaveIndent(path);
			SetOriginal();
		}

		void DoSave() {
			if (currentPath != null)
				SaveFile(currentPath);
			else {
				var dialog = new SaveFileDialog { Filter = "XML Files|*.xml|All Files|*", Title = "Save ShomreiTorah Config File" };
				if (dialog.ShowDialog(this) != true)
					return;
				SaveFile(dialog.FileName);
			}
		}
		void CreateNew() {
			CurrentPath = null;
			root = new Schema.ConfigRoot();
			Rebind();
		}

		private void Open_Click(object sender, RoutedEventArgs e) {
			var dialog = new OpenFileDialog { Filter = "XML Files|*.xml|All Files|*", Title = "Open ShomreiTorah Config File" };
			if (dialog.ShowDialog(this) != true)
				return;
			OpenFile(dialog.FileName);
		}
		private void New_Click(object sender, RoutedEventArgs e) { CreateNew(); }
		private void Save_Click(object sender, RoutedEventArgs e) { DoSave(); }

		private void OpenDebug_Click(object sender, RoutedEventArgs e) {
			OpenFile(Path.Combine(sourceRoot, @"Config\Debug\ShomreiTorahConfig.xml"));
		}
		private void OpenProduction_Click(object sender, RoutedEventArgs e) {
			OpenFile(Path.Combine(sourceRoot, @"Config\Production\ShomreiTorahConfig.xml"));
		}

		private void SaveDebug_Click(object sender, RoutedEventArgs e) {
			SaveFile(Path.Combine(sourceRoot, @"Config\Debug\ShomreiTorahConfig.xml"));
			root.CreateDebugDoc().SaveIndent(Path.Combine(sourceRoot, @"Config\Debug\Web.ConnectionStrings.config"));
		}
		private void SaveProduction_Click(object sender, RoutedEventArgs e) {
			SaveFile(Path.Combine(sourceRoot, @"Config\Production\ShomreiTorahConfig.xml"));
			root.CreateProductionDoc().SaveIndent(Path.Combine(sourceRoot, @"Config\Production\Web.Release.config"));
		}
		#endregion
	}
}
