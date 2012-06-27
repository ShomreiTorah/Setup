using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Configurator {
	static class Extensions {
		///<summary>Recursively removes all empty elements from an XML tree.</summary>
		///<param name="root">The element to modify.  (this is an in-place operation)</param>
		public static void Trim(this XElement root) {
			var empties = root.Descendants().Where(x => x.IsEmpty && !x.HasAttributes).ToList();
			while (empties.Count > 0) {
				var parents = empties.Select(e => e.Parent)
									 .Where(e => e != null)
									 .Distinct()	//In case we have two empty siblings, don't try to remove the parent twice
									 .ToList();

				empties.ForEach(e => e.Remove());

				//Filter the parent nodes to the ones that just became empty.
				parents.RemoveAll(e => !e.IsEmpty && !e.HasAttributes);
				empties = parents;
			}
		}

		public static void SaveIndent(this XDocument doc, string path, string indent = "\t") {
			using (var writer = XmlWriter.Create(path, new XmlWriterSettings { IndentChars = indent, Indent = true, NewLineOnAttributes = true })) {
				doc.Save(writer);
			}
		}
	}
}
