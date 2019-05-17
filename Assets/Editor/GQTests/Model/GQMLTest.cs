using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using GQ.Client.Model;
using GQ.Client.Err;
using GQ.Editor.Util;
using NUnit.Framework;

namespace GQTests.Model
{

	public abstract class GQMLTest
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

		protected string parseXmlFromFile (string relativeFilePath)
		{
			return File.ReadAllText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, relativeFilePath));
		}

		protected string xmlRoot = GQML.QUEST;

		protected string XmlRoot {
			get {
				if (xmlRoot == null) {
					Log.SignalErrorToDeveloper ("Test class {0} does not specify xml root element name.", GetType ().FullName);
					return null;
				}
				return xmlRoot;
			}
			set {
				xmlRoot = value;
			}
		}

		protected string xml;
		protected QuestManager qm;

		[SetUp]
		public void InitQuestManager ()
		{ 

			QuestManager.Reset ();
			qm = QuestManager.Instance;
			Variables.Clear ();
		}


	}
}
