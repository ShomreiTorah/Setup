using System;
using System.Collections.Generic;
using System.IO;
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

		///<summary>Writes raw bytes to a stream.</summary>
		///<param name="stream">The stream to write to.</param>
		///<param name="data">The bytes to write.</param>
		///<remarks>This method removes the need for temporary variables to get the length of the byte array.</remarks>
		public static void WriteAllBytes(this Stream stream, byte[] data) { stream.Write(data, 0, data.Length); }

		///<summary>Fills a byte array from a stream.</summary>
		///<returns>The number of bytes read.  If the end of the stream was reached, this will be less than the size of the array.</returns>
		///<remarks>Stream.Read is not guaranteed to read length bytes even if it doesn't hit the end of the stream, so I wrote this method, which is.</remarks>
		public static int ReadFill(this Stream stream, byte[] buffer) { return stream.ReadFill(buffer, buffer.Length); }
		///<summary>Reads a given number of bytes into a byte array from a stream.</summary>
		///<returns>The number of bytes read.  If the end of the stream was reached, this will be less than the length.</returns>
		///<remarks>Stream.Read is not guaranteed to read length bytes even if it doesn't hit the end of the stream, so I wrote this method, which is.</remarks>
		public static int ReadFill(this Stream stream, byte[] buffer, int length) {
			if (stream == null) throw new ArgumentNullException("stream");
			if (buffer == null) throw new ArgumentNullException("buffer");

			int position = 0;
			while (position < length) {
				var bytesRead = stream.Read(buffer, position, length - position);
				if (bytesRead == 0) break;
				position += bytesRead;
			}
			return position;
		}


		public static void SaveIndent(this XDocument doc, string path, string indent = "\t") {
			using (var writer = XmlWriter.Create(path, new XmlWriterSettings { IndentChars = indent, Indent = true, NewLineOnAttributes = true })) {
				doc.Save(writer);
			}
		}
	}
}
