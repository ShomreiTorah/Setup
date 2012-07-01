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

		///<summary>Enter the folder to store local backups of the database (as gzip-ed XML).  This optional setting is used by ShomreiTorah.Backup in Utilities.</summary>
		[ConfigProperty("Backup/@Path")]
		public string BackupPath { get; set; }
	}
}
