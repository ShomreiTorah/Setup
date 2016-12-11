using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using DialogResult = System.Windows.Forms.DialogResult;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace Configurator.Controls {
	/// <summary>
	/// Interaction logic for PathBox.xaml
	/// </summary>
	partial class PathBox : UserControl {
		public PathBox() {
			InitializeComponent();
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			"Text", typeof(string), typeof(PathBox),
			new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)
		);
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(PathType), typeof(PathBox));
		public PathType Type {
			get { return (PathType)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}

		public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(PathBox));
		public string Filter {
			get { return (string)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}


		class AnyWindow : IWin32Window {
			public AnyWindow(IntPtr handle) { Handle = handle; }

			public IntPtr Handle { get; private set; }
		}

		private void Browse_Click(object sender, RoutedEventArgs e) {
			DependencyObject parent = this;
			Window window;

			while (null == (window = parent as Window))
				parent = VisualTreeHelper.GetParent(parent);

			switch (Type) {
				case PathType.File: {
						var dialog = new OpenFileDialog { FileName = Text, Filter = Filter };
						if (dialog.ShowDialog(window) != true)
							return;
						Text = dialog.FileName;
					}
					break;

				case PathType.Folder:
					using (var dialog = new FolderBrowserDialog {
						SelectedPath = Text,
						ShowNewFolderButton = true
					}) {
						if (DialogResult.Cancel == dialog.ShowDialog(new AnyWindow(new WindowInteropHelper(window).Handle)))
							return;

						Text = dialog.SelectedPath;
						break;
					}
			}
			GetBindingExpression(TextProperty).UpdateSource();
		}
	}

	enum PathType {
		File,
		Folder
	}
}
