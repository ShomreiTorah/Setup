using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configurator.Schema {
	class DatabaseConfig : ConfigObject {
		[ConfigProperty("@Type")]
		public string Type { get; set; }
		[ConfigProperty("@ConnectionString")]
		public string ConnectionString { get; set; }

		[ConfigProperty("Backup/@Path")]
		public string BackupPath { get; set; }
	}
}
