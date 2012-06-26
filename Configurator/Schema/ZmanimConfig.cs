using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configurator.Schema {
	class ZmanimConfig : ConfigObject {
		[ConfigProperty("@DefaultProvider")]
		public string DefaultProvider { get; set; }

		[ConfigProperty("Xml/@Path")]
		public string XmlPath{ get; set; }

		[ConfigProperty("FastCsv/@Path")]
		public string FastCsvPath { get; set; }

	}
}
