using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Configurator {
	///<summary>A base class that can serialize its properties to/from a ShomreiTorahConfig.xml.</summary>
	abstract class ConfigObject {
		class Property {
			public Property(PropertyInfo prop) {
				Info = prop;
				XPath = ((ConfigPropertyAttribute)prop.GetCustomAttributes(typeof(ConfigPropertyAttribute), true)[0]).XPath;
			}

			public string XPath { get; private set; }
			public PropertyInfo Info { get; private set; }
		}
		readonly ICollection<Property> properties;

		protected ConfigObject() {
			properties = Array.ConvertAll(GetType().GetProperties(), p => new Property(p));
		}

		///<summary>Populates this instance from an XML element.</summary>
		public void ReadXml(XElement elem) {
			foreach (var prop in properties) {
				XObject node = elem.XPath<XObject>(prop.XPath).SingleOrDefault();

				//If the node is null, clear the property.
				//If it's a complex property, recursively clear it.
				if (typeof(ConfigObject).IsAssignableFrom(prop.Info.PropertyType))
					((ConfigObject)prop.Info.GetValue(this, null)).ReadXml((XElement)node ?? new XElement("Null"));
				else if (node == null)
					prop.Info.SetValue(this, null, null);
				else
					prop.Info.SetValue(this, Convert.ChangeType(GetValue(node), prop.Info.PropertyType), null);
			}
		}
		static object GetValue(object xValue) {
			var attr = xValue as XAttribute;
			if (attr != null)
				return attr.Value;
			else
				return ((XElement)xValue).Value;
		}
		public void WriteXml(XElement elem) {
			foreach (var prop in properties) {
				XObject target = elem.XPath<XObject>(prop.XPath).SingleOrDefault();

				object value = prop.Info.GetValue(this, null);
				if (value == null)
					Remove(target);
				else {
					if (target == null)	//If we actually have a value, make sure the target element/attribute actually exists.
						target = EnsureNode(elem, prop.XPath);

					if (typeof(ConfigObject).IsAssignableFrom(prop.Info.PropertyType))
						((ConfigObject)prop.Info.GetValue(this, null)).WriteXml((XElement)target);
					else
						SetValue(target, value.ToString());
				}
			}
		}
		static void SetValue(XObject target, string value) {
			var attr = target as XAttribute;
			if (attr != null)
				attr.Value = value;
			else
				((XElement)target).Value = value;
		}
		static void Remove(XObject target) {
			if (target == null)
				return;
			var attr = target as XAttribute;
			if (attr != null)
				attr.Remove();
			else
				((XElement)target).Remove();
		}

		static XObject EnsureNode(XElement parent, string xpath) {
			XObject result = parent.XPath<XObject>(xpath).SingleOrDefault();
			if (result != null)
				return result;

			var b = xpath.LastIndexOf('/');
			if (b > 0) {	//If we're not at the root of the path, recurse to find/create the parent element
				parent = (XElement)EnsureNode(parent, xpath.Remove(b));
				xpath = xpath.Substring(b + 1);
			}

			if (xpath[0] == '@')
				result = new XAttribute(xpath.Remove(1), "Something is wrong; this value was not set.");
			else
				result = new XElement(xpath);

			parent.Add(result);

			return result;
		}
	}

	static class XPathExtensions {
		public static IEnumerable<T> XPath<T>(this XNode node, string xpath) {
			return (IEnumerable<T>)node.XPathEvaluate(xpath);
		}
	}

	///<summary>Specifies the location in ShomreiTorahConfig.xml that a config property is stored.</summary>
	sealed class ConfigPropertyAttribute : Attribute {
		///<summary>Gets the XPath to the XML element/attribute that stores that value.</summary>
		public string XPath { get; private set; }

		public ConfigPropertyAttribute(string xpath) { XPath = xpath; }
	}
}
