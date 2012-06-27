using System;
using System.Collections.Generic;
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
using System.IO;
using System.Xml.Linq;

namespace Configurator {
	/// <summary>
	/// Interaction logic for HelpBox.xaml
	/// </summary>
	partial class HelpBox : UserControl {
		public HelpBox() {
			InitializeComponent();
		}



		public string MemberName {
			get { return (string)GetValue(MemberNameProperty); }
			set { SetValue(MemberNameProperty, value); }
		}

		public static readonly DependencyProperty MemberNameProperty =
			DependencyProperty.Register("MemberName", typeof(string), typeof(HelpBox), new PropertyMetadata(MemberNameChanged));

		static readonly XDocument docComments = XDocument.Load(Path.ChangeExtension(App.ExePath, ".xml"));

		static void MemberNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var box = (HelpBox)d;
			var memberName = e.NewValue as string;
			if (string.IsNullOrEmpty(memberName))
				box.text.Text = null;
			else {
				if (!memberName.StartsWith("Configurator."))
					memberName = "Configurator.Schema." + memberName;
				box.text.Text = docComments.Root.Element("members")
												.Elements("member")
												.First(m => m.Attribute("name").Value.EndsWith(":" + memberName))
												.Element("summary")
												.Value;
			}
		}
	}
}
