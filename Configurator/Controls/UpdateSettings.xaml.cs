using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Configurator.Controls {
	/// <summary>
	/// Interaction logic for UpdateSettings.xaml
	/// </summary>
	partial class UpdateSettings : UserControl {
		public UpdateSettings() {
			InitializeComponent();
		}

		private void SymmetricKeygen_Click(object sender, RoutedEventArgs e) {
			SymmetricAlgorithm alg;
			try {
				alg = SymmetricAlgorithm.Create(algorithm.Text);
			} catch (Exception ex) {
				MessageBox.Show("Couldn't create algorithm: " + ex.Message);
				return;
			}
			alg.BlockSize = alg.KeySize = 256;
			key.Text = Convert.ToBase64String(alg.Key);
			iv.Text = Convert.ToBase64String(alg.IV);

		}

		// TODO: Embed hash in private key file.
		const string Hash = "$2a$10$fH.7kdUxOMTr7rHsoo9ZTuAfmYZk9C4Fp4TpKOQG2DhrfEj9EG0/C";
		string storedPassword;
		string PromptPassword() {
			if (storedPassword != null)
				return storedPassword;
			var password = PasswordPrompt.Prompt();
			if (password == null)
				return null;
			if (!BCrypt.Net.BCrypt.Verify(password, Hash)) {
				MessageBox.Show("Incorrect password");
				return null;
			}
			storedPassword = password;
			return password;
		}

		private void AsymmetricKeygen_Click(object sender, RoutedEventArgs e) {
			var password = PromptPassword();
			if (password == null)
				return;
			var dialog = new SaveFileDialog {
				Title = "Save Private Key",
				Filter = "ShomreiTorah Encrypted Private Key File (*.private-key)|*.private-key",
				FileName = Path.Combine(App.SourceRoot, @"Config", "UpdateKey " + DateTime.Now.ToString("yyyy-MM-dd") + ".private-key")
			};
			if (dialog.ShowDialog() != true)
				return;
			using (var stream = dialog.OpenFile())
			using (var rsa = new RSACryptoServiceProvider(4096)) {
				KeyStorage.WriteKey(stream, password, rsa);
				rsaKey.Text = XElement.Parse(rsa.ToXmlString(false)).ToString();
			}
		}
		private void AsymmetricCompare_Click(object sender, RoutedEventArgs e) {
			XElement parsedKey;
			try {
				parsedKey = XElement.Parse(rsaKey.Text);
			} catch (Exception ex) {
				MessageBox.Show("Couldn't parse public key: " + ex.Message);
				return;
			}

			var password = PromptPassword();
			if (password == null)
				return;
			var dialog = new System.Windows.Forms.FolderBrowserDialog {
				Description = "Select a directory that contains private keys.",
				SelectedPath = Path.Combine(App.SourceRoot, @"Config")
			};
			if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				return;
			var keyPath = Directory.EnumerateFiles(dialog.SelectedPath, "*.private-key")
								.OrderByDescending(Path.GetFileName)
								.FirstOrDefault(p => {
									using (var s = File.OpenRead(p))
									using (var rsa = KeyStorage.ReadKey(s, password))
										return XNode.DeepEquals(parsedKey, XElement.Parse(rsa.ToXmlString(false)));
								});
			if (keyPath == null)
				MessageBox.Show(dialog.SelectedPath + " has no matching private keys.");
			else
				MessageBox.Show("The public key matches " + Path.GetFileName(keyPath));
		}
		private void AsymmetricReadKey_Click(object sender, RoutedEventArgs e) {
			var password = PromptPassword();
			if (password == null)
				return;
			var dialog = new OpenFileDialog {
				Title = "Import Private Key",
				Filter = "ShomreiTorah Encrypted Private Key File (*.private-key)|*.private-key",
				InitialDirectory = Path.Combine(App.SourceRoot, @"Config")
			};
			if (dialog.ShowDialog() != true)
				return;
			using (var stream = dialog.OpenFile())
			using (var rsa = KeyStorage.ReadKey(stream, password)) {
				rsaKey.Text = XElement.Parse(rsa.ToXmlString(false)).ToString();
			}
		}
	}
}
