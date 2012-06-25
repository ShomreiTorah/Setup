using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configurator.Schema {
	class FtpConfig :ConfigObject{
		[ConfigProperty("@Server")]
		public string Server { get; set; }

		[ConfigProperty("@SSL")]
		public bool SSL { get; set; }

		[ConfigProperty("@Username")]
		public string Username { get; set; }

		[ConfigProperty("@Password")]
		public string Password { get; set; }
	}
}
