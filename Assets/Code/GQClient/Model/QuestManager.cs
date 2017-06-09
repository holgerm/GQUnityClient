using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using GQ.Client.Model;
using System.IO;
using GQ.Client.Err;
using System.Collections.Generic;

namespace GQ.Client.Model
{

	public class QuestManager
	{

		#region quest management functions

		private Dictionary<string, Quest> quests;

		protected Quest currentQuest;

		public Quest CurrentQuest {
			get {
				return currentQuest;
			}
			set {
				currentQuest = value;
			}
		}

		/// <summary>
		/// Add the given quest to the internal dictionary of quests. In case there was already a quest with the given ID, it returns false.
		/// 
		/// </summary>
		/// <param name="quest">Quest.</param>
		public bool Add (Quest quest)
		{
			Quest q;
			if (quests.TryGetValue (quest.Id, out q)) {
				return false;
			} else {
				quests.Add (quest.Id, quest);
			}
		}

		public Quest Import (string xml)
		{
			
			// Creates an instance of the XmlSerializer class;
			// specifies the type of object to be deserialized.
			XmlSerializer serializer = new XmlSerializer (typeof(Quest));

			// If the XML document has been altered with unknown 
			// nodes or attributes, handles them with the 
			// UnknownNode and UnknownAttribute events.
			serializer.UnknownNode += new 
				XmlNodeEventHandler (serializer_UnknownNode);
			serializer.UnknownAttribute += new 
				XmlAttributeEventHandler (serializer_UnknownAttribute);

			Quest quest;

			using (TextReader reader = new StringReader (xml)) {
				quest = (Quest)serializer.Deserialize (reader);
			}

			return quest;
		}

		#endregion


		#region singleton

		public static QuestManager Instance {
			get {
				if (_instance == null) {
					_instance = new QuestManager ();
				} 
				return _instance;
			}
			set {
				_instance = value;
			}
		}

		public static void Reset ()
		{
			_instance = null;
		}

		private static QuestManager _instance = null;

		private QuestManager ()
		{
			quests = new Dictionary<string, Quest> ();
		}

		#endregion


		#region Parsing

		protected void serializer_UnknownNode
		(object sender, XmlNodeEventArgs e)
		{
			Log.SignalErrorToDeveloper ("Unknown XML Node found in Quest XML:" + e.Name + "\t" + e.Text);
		}

		protected void serializer_UnknownAttribute
		(object sender, XmlAttributeEventArgs e)
		{
			System.Xml.XmlAttribute attr = e.Attr;
			Log.SignalErrorToDeveloper ("Unknown XML Attribute found in Quest XML:" +
			attr.Name + "='" + attr.Value + "'");
		}

		#endregion
	}
}
