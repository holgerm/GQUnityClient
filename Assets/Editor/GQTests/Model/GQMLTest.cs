using System.IO;
using GQ.Editor.Util;
using NUnit.Framework;
using System.Reflection;
using System;
using System.Xml;
using Code.GQClient.Err;
using Code.GQClient.Model.expressions;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;

namespace GQTests.Model
{

    public abstract class GQMLTest
	{

		protected virtual T parseXML<T> (string xml)
		{
			using (TextReader reader = new StringReader (xml)) {
                ConstructorInfo constructorInfoObj = typeof(T).GetConstructor(new Type[] { typeof(XmlReader) });
                if (constructorInfoObj == null)
                {
                    Log.SignalErrorToDeveloper("Type {0} misses a Constructor for creating the model from XmlReader.", typeof(T).Name);
                }
                return (T) constructorInfoObj.Invoke(new object[] { reader });
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
