﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Configurator.Schema;

namespace Configurator {
	static class ConfigFiles {
		public static XDocument CreateDebugDoc(this ConfigRoot config) {
			return new XDocument(
				new XDeclaration("1.0", "utf-8", "yes"),
				new XComment("This file is generated by the configurator based on the settings specified in ShomreiTorahConfig.xml.\r\n\t " +
							 "It is preferable to not edit this file by hand."),
				new XComment("This file is copied to the website root directories and linked using configSource=\"\" for debug runs.\r\n\t " +
							 "It is replaced using an XDT transform in production."),
				new XElement("connectionStrings",
					new XElement("clear"),
					new XElement("add",
						new XAttribute("name", "DB"),
						new XAttribute("connectionString", config.DefaultDB.ConnectionString)		//TODO: Specify providerName=""
					)
				)
			);
		}

		static readonly XNamespace xdt = "http://schemas.microsoft.com/XML-Document-Transform";
		public static XDocument CreateProductionDoc(this ConfigRoot config) {
			return new XDocument(
				new XDeclaration("1.0", "utf-8", "yes"),
				new XComment(" This file is generated by the configurator based on the settings specified in ShomreiTorahConfig.xml.\r\n\t" +
							 " It is preferable to not edit this file by hand."),
				new XComment(" This file is copied to each web application using a post-build step.\r\n\t" +
							 " I cannot simply link it to the project because Visual Studio doesn't\r\n\t" +
							 " apply config transforms from linked config files.\r\n\t" +
							 " The file does not need to be included in the projects."),

				new XElement("configuration",
					new XAttribute(XNamespace.Xmlns + "xdt", xdt.NamespaceName),
					new XElement("connectionStrings",
						new XAttribute(xdt + "Transform", "RemoveAttributes(configSource)"),

						new XElement("clear"),
						new XElement("add",
							new XAttribute("name", "DB"),
							new XAttribute("connectionString", config.DefaultDB.ConnectionString),	//TODO: Specify providerName=""
							new XAttribute(xdt + "Transform", "Insert")
						)
					),

					new XElement("elmah",
						new XElement("errorMail",
							new XAttribute("from", "Alerts@" + config.DomainName),
							new XAttribute("to", "Dev@" + config.DomainName),
							new XAttribute("async", "true"),
							new XAttribute("useSsl", config.GmailSmtp.SSL),
							new XAttribute("smtpPort", config.GmailSmtp.Port),
							new XAttribute("smtpServer", config.GmailSmtp.Server),
							new XAttribute("userName", "Alerts@" + config.DomainName),
							new XAttribute("password", config.GmailSmtp.Password),
							new XAttribute(xdt + "Transform", "SetAttributes")
						)
					),

					new XElement("system.web",
						new XElement("compilation",
							new XAttribute(xdt + "Transform", "RemoveAttributes(debug)")
						)
					)
				)
			);
		}
	}
}
