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
using System.Windows.Shapes;

namespace Configurator {
	/// <summary>
	/// Interaction logic for PasswordPrompt.xaml
	/// </summary>
	partial class PasswordPrompt : Window {
		public static string Prompt() {
			var dialog = new PasswordPrompt();
			if (dialog.ShowDialog() != true)
				return null;
			return dialog.password.Password;
		}

		PasswordPrompt() {
			InitializeComponent();
			password.Focus();
		}

		private void Ok_Click(object sender, RoutedEventArgs e) {
			DialogResult = true;
		}

		private void password_PasswordChanged(object sender, RoutedEventArgs e) {
			ok.IsEnabled = !string.IsNullOrEmpty(password.Password);
		}
	}
}
