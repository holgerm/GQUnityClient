using GQ.Client.Model;
using GQ.Client.Err;
using UnityEngine.SceneManagement;
using System.Xml;
using System.Xml.Serialization;
using System;
using UnityEngine;
using GQ.Client.Util;
using GQ.Client.UI;
using System.Collections.Generic;
using System.IO;
using GQ.Client.FileIO;
using GQ.Client.Conf;

namespace GQ.Client.Model
{
	[XmlRoot (GQML.PAGE)]
	public abstract class Page : IPage
	{

		#region Structure

		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		public void WriteXml (System.Xml.XmlWriter writer)
		{
			Debug.LogWarning ("WriteXML not implemented for " + GetType ().Name);
		}

		public virtual Quest Quest {
			get {
				return Parent;
			}
		}

		/// <summary>
		/// Reader must be at the page element (start). When it returns the reader is position behind the page end element. 
		/// 
		/// This is a template method. Subtypes should only override the ReadAttributes() and ReadContent() methods 
		/// and extend them by calling their base versions.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public void ReadXml (XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.PAGE);

			ReadAttributes (reader);

			if (reader.IsEmptyElement) {
				reader.Read ();
				Log.SignalErrorToDeveloper ("Empty xml page element found.");
				return;
			}

			// consume the Begin Action Element:
			reader.Read (); 

			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;

			while (!GQML.IsReaderAtEnd (reader, GQML.PAGE)) {

				if (reader.NodeType == XmlNodeType.Element)
					ReadContent (reader, xmlRootAttr);
			}

			// consume the closing action tag (if not empty page element)
			if (reader.NodeType == XmlNodeType.EndElement)
				reader.Read ();
		}

		protected virtual void ReadAttributes (XmlReader reader)
		{
			// Id:
			int id;
			if (Int32.TryParse (reader.GetAttribute (GQML.PAGE_ID), out id)) {
				Id = id;
			} else {
				Log.SignalErrorToDeveloper ("Id for a page could not be parsed. We found: " + reader.GetAttribute (GQML.QUEST_ID));
			}

			PageType = GQML.GetStringAttribute (GQML.PAGE_TYPE, reader);

		}

		protected virtual void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			XmlSerializer serializer;

			switch (reader.LocalName) {
			case GQML.ON_START:
				xmlRootAttr.ElementName = GQML.ON_START;
				serializer = new XmlSerializer (typeof(Trigger), xmlRootAttr);
				StartTrigger = (Trigger)serializer.Deserialize (reader);
				StartTrigger.Parent = this;
				break;
			case GQML.ON_END:
				xmlRootAttr.ElementName = GQML.ON_END;
				serializer = new XmlSerializer (typeof(Trigger), xmlRootAttr);
				EndTrigger = (Trigger)serializer.Deserialize (reader);
				EndTrigger.Parent = this;
				break;
			// UNKOWN CASE:
			default:
				Log.WarnDeveloper ("Page has additional unknown {0} element. (Ignored)", reader.LocalName);
				reader.Skip ();
				break;
			}
		}

		protected Trigger StartTrigger = Trigger.Null;
		protected Trigger EndTrigger = Trigger.Null;

		#endregion


		#region State

		public Page ()
		{
			State = GQML.STATE_NEW;

			stateOld = GQML.STATE_NEW;

			result = null;
		}

		public virtual Quest Parent { get; set; }

		public int Id { get; protected set; }

		public string PageType { get; protected set; }

		[XmlAttribute ("type"), Obsolete]
		public string
			type;

		[Obsolete]
		public string stateOld;

		public string State {
			get;
			protected set;
		}

		public string result;

		public string Result {
			get {
				return result;
			}
		}

		#endregion


		#region Runtime API

		public PageController PageCtrl {
			get;
			set;
		}

		private string PageSceneName {
			get {
				return GetType ().Name.Substring (4);
			}
		}

		// called when a scene has been loaded:
		void OnSceneLoaded (Scene scene, LoadSceneMode mode)
		{
			SceneManager.SetActiveScene (scene);
			foreach (Scene sceneToUnload in scenesToUnload) {
				SceneManager.UnloadSceneAsync (sceneToUnload);
			}
			scenesToUnload.Clear ();
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		public static List<Scene> scenesToUnload = new List<Scene> ();

		public virtual void Start ()
		{
			Resources.UnloadUnusedAssets ();

			// ensure that the adequate scene is loaded:
			Scene scene = SceneManager.GetActiveScene ();
			if (!scene.name.Equals (PageSceneName)) {
				SceneManager.sceneLoaded += OnSceneLoaded;
				SceneManager.LoadSceneAsync (PageSceneName, LoadSceneMode.Additive);
				if (scene.name != Base.FOYER_SCENE_NAME) {
					scenesToUnload.Add (scene);
				}
			}

			// set this page as current in QM
			QuestManager.Instance.CurrentQuest = Parent;
			QuestManager.Instance.CurrentPage = this; 
			State = GQML.STATE_RUNNING;

			// Trigger OnStart Actions of this page:
			StartTrigger.Initiate ();
		}

		public virtual void End ()
		{
			State = GQML.STATE_SUCCEEDED;
			if (EndTrigger == Trigger.Null) {
				Log.SignalErrorToAuthor (
					"Quest {0} ({1}, page {2} has no actions onEnd defined, hence we end the quest here.",
					Quest.Name, Quest.Id,
					Id
				);
				Quest.End ();
			} else {
				EndTrigger.Initiate ();
			}
			Resources.UnloadUnusedAssets ();
		}

		#endregion


		#region Null Object

		public static readonly Page Null = new NullPage ();

		private class NullPage : Page
		{

			public NullPage ()
				: base ()
			{
				Id = 0;
				State = GQML.STATE_NEW;
				Parent.CurrentPage = this;
			}

			public override Quest Parent { 
				get {
					return Quest.Null;
				}
			}

			public override void Start ()
			{
				Log.WarnDeveloper ("Null Page started in quest {0} (id: {1})", Parent.Name, Parent.Id);
				Parent.CurrentPage = this;
				State = GQML.STATE_RUNNING;
				StartTrigger.Initiate ();
			}
		}

		#endregion

	}
}
