using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configurator.Schema {
	class ConfigRoot : ConfigObject {
		//The XML comments on these properties are displayed in the UI by parsing the generated XML file.
		//I hope to also auto-generate standalone documentation from them.

		///<summary>Enter the public name of the organization using the Shomrei Torah system.  This is displayed on the website and email messages.</summary>
		[ConfigProperty("Names/@OrgName")]
		public string OrgName { get; set; }
		///<summary>Enter the organization's domain name.  This is used to construct email addresses and URLs.</summary>
		[ConfigProperty("Names/@DomainName")]
		public string DomainName { get; set; }

		///<summary>Enter the connection information for the default database used by all Shomrei Torah applications.  This database is used by both the web and client applications, and should be accessible over the internet.</summary>
		[ConfigProperty("Databases/Default")]
		public DatabaseConfig DefaultDb { get; private set; }
		///<summary>Enter the connection information for the test database.  This is used by the EmailCommand system to send messages to the test list (forwarded from the Commands+SendTest@ address).  It is not used anywhere else.</summary>
		[ConfigProperty("Databases/Test")]
		public DatabaseConfig TestDb { get; private set; }

		///<summary>Enter the directory on the web server that contains update binaries.  This is also used to upload updates over FTP.  All update files are encrypted.</summary>
		[ConfigProperty("Updates/@Path")]
		public string UpdatePath { get; set; }
		///<summary>Enter the encryption method, key and initialization vector (IV) used to encrypt update files on the web server.  The key and IV should be random bit strings matching the encryption block size (typically 256-bit).</summary>
		[ConfigProperty("Updates/Cryptography/FileDecryptor")]
		public CryptoKey UpdateFilesKey { get; private set; }
		///<summary>Enter the RSA public used to verify update files.  The private key is stored (encrypted) in Updater-Private-Key.xml, and is read by the update publisher.</summary>
		[ConfigProperty("Updates/Cryptography/UpdateVerifier", EmbedXml = true)]
		public string UpdateSigningKey { get; set; }

		///<summary>Enter the connection settings for the FTP server used to publish files to the website.  This is used for updates, schedules, and journal ad blanks.</summary>
		[ConfigProperty("FTP")]
		public FtpConfig Ftp { get; private set; }

		///<summary>Select the SMTP server used to send ordinary messages.</summary>
		[ConfigProperty("SMTP/@Default")]
		public string SmtpDefault { get; set; }

		///<summary>Enter the connection settings for the (typically-) Gmail-based SMTP server.  This server can be used to send ordinary messages and log them in Gmail's Sent folders.</summary>
		[ConfigProperty("SMTP/Gmail")]
		public SmtpConfig GmailSmtp { get; private set; }
		///<summary>Enter the connection settings for the externally hosted SMTP server.  This server is used to send messages and statements to the mailing list; it must be capable of delivering large quantities of mail.  (which Gmail cannot)</summary>
		[ConfigProperty("SMTP/Hosted")]
		public SmtpConfig HostedSmtp { get; private set; }

		///<summary>Enter the connection settings for the POP3 server that receives incoming messages.  This is used by the EmailCommand system (only), which will read &amp; delete mail sent to the address specified in Commands recipient.</summary>
		[ConfigProperty("POP3/Gmail")]
		public PopConfig Pop { get; private set; }

		///<summary>Enter the settings used to calculate זמנים for the schedule.  The "Calculator" provider, which uses latitude and longitude, is recommended.</summary>
		[ConfigProperty("Zmanim")]
		public ZmanimConfig Zmanim { get; private set; }

		///<summary>Enter the paths of the various schedule files.</summary>
		[ConfigProperty("Schedules")]
		public ScheduleConfig Schedules { get; private set; }
	}
}
