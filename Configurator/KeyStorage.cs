using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Configurator {
	static class KeyStorage {
		const int BlockSize = 256 / 8;
		const int KeySaltSize = 16;

		public static RSA ReadKey(Stream file, String password) {
			byte[] key = Unsalt(file, password);
			byte[] iv = Unsalt(file, password);

			using (var aes = new RijndaelManaged { Key = key, IV = iv })
			using (var transform = aes.CreateDecryptor())
			using (var stream = new CryptoStream(file, transform, CryptoStreamMode.Read))
			using (var reader = new StreamReader(stream)) {
				var rsa = new RSACryptoServiceProvider();
				rsa.FromXmlString(reader.ReadToEnd());
				return rsa;
			}
		}
		static byte[] Unsalt(this Stream source, string password) {
			byte[] salt = new byte[KeySaltSize];
			source.ReadFill(salt);
			var deriver = new Rfc2898DeriveBytes(password, salt);
			return deriver.GetBytes(BlockSize);
		}


		public static void WriteKey(Stream file, String password, RSA rsa) {
			byte[] key = DeriveBytes(file, password);
			byte[] iv = DeriveBytes(file, password);

			using (var aes = new RijndaelManaged { Key = key, IV = iv })
			using (var transform = aes.CreateEncryptor())
			using (var stream = new CryptoStream(file, transform, CryptoStreamMode.Write))
			using (var writer = new StreamWriter(stream)) {
				writer.Write(rsa.ToXmlString(true));
			}
		}
		static byte[] DeriveBytes(Stream target, string password) {
			var rng = new RNGCryptoServiceProvider();
			var salt = new byte[KeySaltSize];
			rng.GetBytes(salt);

			target.WriteAllBytes(salt);

			var deriver = new Rfc2898DeriveBytes(password, salt);
			return deriver.GetBytes(BlockSize);
		}
	}
}
