using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configurator.Schema {
	class ConfigRoot : ConfigObject {
		[ConfigProperty("Names/@OrgName")]
		public string OrgName { get; set; }
		[ConfigProperty("Names/@DomainName")]
		public string DomainName { get; set; }

		[ConfigProperty("Databases/Default")]
		public DatabaseConfig DefaultDB { get; private set; }
		[ConfigProperty("Databases/Test")]
		public DatabaseConfig TestDB { get; private set; }

		//TODO: Updates

		[ConfigProperty("FTP")]
		public FtpConfig Ftp { get; private set; }

		[ConfigProperty("SMTP/Default")]
		public string DefaultSmtp { get; set; }

		[ConfigProperty("SMTP/Gmail")]
		public SmtpConfig Gmail { get; private set; }
		[ConfigProperty("SMTP/Hosted")]
		public SmtpConfig Hosted { get; private set; }

		[ConfigProperty("POP3/Gmail")]
		public PopConfig Pop { get; private set; }
	}
}
