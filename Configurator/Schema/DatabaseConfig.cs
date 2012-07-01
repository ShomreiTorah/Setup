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

		///<summary>Enter the format string for the file to store local backups of the database (as gzip-ed XML).  The current time will be substituted as "{0}".  This optional setting is used by ShomreiTorah.Backup in Utilities.</summary>
		[ConfigProperty("Backup/@Path")]
		public string BackupPath { get; set; }
	}
}
