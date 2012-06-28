using System;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Win32;

namespace Configurator {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	partial class MainWindow : Window {

		#region Designer Support
		//This is data-bound in the XAML so that I can get design-time data-binding help
		public static readonly Schema.ConfigRoot DesignerSample = OpenDesignerFile();

		static Schema.ConfigRoot OpenDesignerFile() {
			var retVal = new Schema.ConfigRoot();
#if DEBUG
			if (App.SourceRoot != null) {
				var path = Path.Combine(App.SourceRoot, @"Config\Debug\ShomreiTorahConfig.xml");
				if (File.Exists(path))
					retVal.ReadXml(XDocument.Load(path).Root);
			}
#endif
			return retVal;
		}
		#endregion

		Schema.ConfigRoot root;
		public MainWindow() {
			//This must run before InitializeComponent so that HasLocations can be data-bound.
			if (string.IsNullOrEmpty(App.SourceRoot)) {
				MessageBox.Show("This utility should be run from within the git source tree.\nSince it is now elsewhere, the environmental buttons will not work.",
								"Shomrei Torah Configurator", MessageBoxButton.OK, MessageBoxImage.Warning);
			} else {
				Directory.CreateDirectory(Path.Combine(App.SourceRoot, @"Config\Debug"));
				Directory.CreateDirectory(Path.Combine(App.SourceRoot, @"Config\Production"));
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
		XElement original = new XElement("TempContainer");
		void SetOriginal() {
			original = new XElement(original.Name);
			root.WriteXml(original);
			original.Trim();
		}
		///<summary>Confirms whether the user would like to discard any unsaved changes.</summary>
		///<returns>True if the user wishes to proceed; false to cancel.</returns>
		bool ConfirmClose() {
			if (root == null)
				return true;

			var comparand = new XElement(original.Name);
			root.WriteXml(comparand);
			comparand.Trim();
			if (XElement.DeepEquals(original, comparand))
				return true;

			switch (MessageBox.Show("Would you like to save your changes to " + (CurrentPath ?? "this unsaved file") + "?",
									"Shomrei Torah Configurator", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)) {
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
			if (!ConfirmClose())
				return;
			CurrentPath = null;
			root = new Schema.ConfigRoot();
			SetOriginal();
			Rebind();
		}

		private void Open_Click(object sender, RoutedEventArgs e) {
			if (!ConfirmClose())
				return;
			var dialog = new OpenFileDialog { Filter = "XML Files|*.xml|All Files|*", Title = "Open ShomreiTorah Config File" };
			if (dialog.ShowDialog(this) != true)
				return;
			OpenFile(dialog.FileName);
		}
		private void New_Click(object sender, RoutedEventArgs e) { CreateNew(); }
		private void Save_Click(object sender, RoutedEventArgs e) { DoSave(); }

		private void OpenDebug_Click(object sender, RoutedEventArgs e) {
			if (!ConfirmClose())
				return;
			OpenFile(Path.Combine(App.SourceRoot, @"Config\Debug\ShomreiTorahConfig.xml"));
		}
		private void OpenProduction_Click(object sender, RoutedEventArgs e) {
			if (!ConfirmClose())
				return;
			OpenFile(Path.Combine(App.SourceRoot, @"Config\Production\ShomreiTorahConfig.xml"));
		}

		private void SaveDebug_Click(object sender, RoutedEventArgs e) {
			SaveFile(Path.Combine(App.SourceRoot, @"Config\Debug\ShomreiTorahConfig.xml"));
			root.CreateDebugDoc().SaveIndent(Path.Combine(App.SourceRoot, @"Config\Debug\Web.ConnectionStrings.config"));
		}
		private void SaveProduction_Click(object sender, RoutedEventArgs e) {
			SaveFile(Path.Combine(App.SourceRoot, @"Config\Production\ShomreiTorahConfig.xml"));
			root.CreateProductionDoc().SaveIndent(Path.Combine(App.SourceRoot, @"Config\Production\Web.Release.config"));
		}
		#endregion

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			e.Cancel = !ConfirmClose();
		}
	}
}
