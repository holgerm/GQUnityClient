using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using GQ.Client.Model.XML;

namespace GQTests.Model.XML
{

	public abstract class XMLTest
	{

		protected virtual T parseXML<T> (string xml)
		{
			using (TextReader reader = new StringReader (xml)) {
				XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
				xmlRootAttr.ElementName = XmlRoot;
				xmlRootAttr.IsNullable = true;
				XmlSerializer serializer = new XmlSerializer (typeof(T), xmlRootAttr);

				return (T)serializer.Deserialize (reader);
			}
		}

		protected abstract string XmlRoot {
			get;
		}

	}
}
