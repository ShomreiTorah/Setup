using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configurator.Schema {
	class ScheduleConfig : ConfigObject {
		[ConfigProperty("@Path")]
		public string Path { get; set; }

		[ConfigProperty("@Announcements")]
		public string AnnouncementsPath { get; set; }

		[ConfigProperty("@TemplatePath")]
		public string TemplatePath { get; set; }

		[ConfigProperty("@PdfUri")]
		public string PdfUri { get; set; }
	}
}
