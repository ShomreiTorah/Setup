using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configurator.Schema {
	class ZmanimConfig : ConfigObject {
		[ConfigProperty("@DefaultProvider")]
		public string DefaultProvider { get; set; }

		[ConfigProperty("@Latitude")]
		public string Latitude { get; set; }
		[ConfigProperty("@Longitude")]
		public string Longitude { get; set; }

		[ConfigProperty("Xml/@Path")]
		public string XmlPath{ get; set; }

		[ConfigProperty("FastCsv/@Path")]
		public string FastCsvPath { get; set; }
	}
}
