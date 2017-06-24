using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using GQ.Client.Model;
using System.Xml;
using System.IO;

namespace GQTests.Model
{

	public class XMLReaderExperiment : GQMLTest
	{

		[Test]
		public void XMLReader_Experiment_1 ()
		{
			// Arrange:
			xml = Files.ReadText (Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "XML/Quests/QuestWith1NPCTalk/game.xml"));
			XmlRoot = GQML.QUEST;

			// Act:
			XmlReader reader = XmlReader.Create (new StringReader (xml));


			// Display:
			int i = 0;
			while (reader.Read ()) {
				reader.MoveToContent ();
				i++;
				Debug.Log ("-------------------- Read() --------------------");
				Debug.Log (i + ". node type: " + reader.NodeType);

				switch (reader.NodeType) {
				case XmlNodeType.Element:
					Debug.Log (reader.LocalName + ":");
					if (reader.HasAttributes) {
						Debug.Log ("Attributes of <" + reader.Name + ">");
						while (reader.MoveToNextAttribute ()) {
							Debug.Log (string.Format (" {0}={1} ({2})", reader.Name, reader.Value, reader.NodeType));
						}
						// Move the reader back to the element node.
						reader.MoveToElement ();
					}
					break;
				case XmlNodeType.EndElement:
					Debug.Log ("Ended: " + reader.LocalName);
					break;
				
				case XmlNodeType.Text:
					Debug.Log (reader.ReadInnerXml ());
					break;

				default:
					Debug.Log ("FORGOTTEN");
					break;
				}

			}
			Debug.Log ("Out of while.");
		}
	}
}
