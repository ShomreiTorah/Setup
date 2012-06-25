using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configurator.Schema {
	class CryptoKey : ConfigObject {
		[ConfigProperty("@Algorithm")]
		public string Algorithm { get; set; }
		[ConfigProperty("Key")]
		public string Key { get; set; }
		[ConfigProperty("IV")]
		public string IV { get; set; }
	}
}
