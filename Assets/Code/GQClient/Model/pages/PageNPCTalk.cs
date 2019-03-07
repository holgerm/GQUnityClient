using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using GQ.Client.Err;
using System;

namespace GQ.Client.Model
{

	[XmlRoot (GQML.PAGE)]
	public class PageNPCTalk : Page
	{

		#region State
		public string EndButtonText { get; set ; }

		public string ImageUrl { get; set; }

        private string text;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                text = value;
                // adapt to NPCTalk: set the text as dialog item:
                DialogItem d = new DialogItem();
                d.Id = -1; // not applicable
                d.IsBlocking = false;
                d.AudioURL = null;
                d.Speaker = null;
                d.Text = text;
                dialogItems.Add(d);
            }
        }

        public string NextDialogButtonText { get; set; }

		public string DisplayMode { get; set; }

		public bool SkipWordTicker { get; set; }

		public int TextSize { get; set; }

		public int TickerSpeed { get; set; }

		protected List<DialogItem> dialogItems = new List<DialogItem> ();

		public int NumberOfDialogItems ()
		{
			return dialogItems.Count;
		}

		public DialogItem CurrentDialogItem {
			get {
				if (CurDialogItemNo == 0) 
				// cannot be negative or beyond limit cf. property setter.
					return DialogItem.Null;
				else
					return dialogItems [CurDialogItemNo - 1];
			}
		}

		protected int curDialogItemNo = 0;

		/// <summary>
		/// The (1-based) index of the current dialog item. 
		/// Limited by the available dialog items: If no dialog items are present it will always be zero.
		/// </summary>
		/// <value>The current dialog item no.</value>
		public int CurDialogItemNo {
			get {
				return curDialogItemNo;
			}
			protected set {
				curDialogItemNo = Math.Max (0, Math.Min (value, dialogItems.Count));
			}
		}
		#endregion


		#region Runtime API

		public override void Start ()
		{
			CurDialogItemNo = 1;
			base.Start ();
		}

		public bool Next ()
		{
			if (dialogItems.Count > CurDialogItemNo) {
				CurDialogItemNo++;
				return true;
			} else {
				return false;
			}
		}

		public bool HasMoreDialogItems ()
		{
			return (dialogItems.Count > CurDialogItemNo);
		}

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			EndButtonText = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_ENDBUTTONTEXT, reader);

			ImageUrl = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_IMAGEURL, reader);
			QuestManager.CurrentlyParsingQuest.AddMedia (ImageUrl, "NPCTalk." + GQML.PAGE_NPCTALK_IMAGEURL);

            Text = GQML.GetStringAttribute(GQML.PAGE_NPCTALK_TEXT, reader);

            DisplayMode = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_DISPLAYMODE, reader);

			NextDialogButtonText = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_NEXTBUTTONTEXT, reader);

			SkipWordTicker = GQML.GetRequiredBoolAttribute (GQML.PAGE_NPCTALK_SKIPWORDTICKER, reader);

			TextSize = GQML.GetIntAttribute (GQML.PAGE_NPCTALK_TEXTSIZE, reader);

			TickerSpeed = GQML.GetIntAttribute (GQML.PAGE_NPCTALK_TICKERSPEED, reader);
		}

		protected override void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			switch (reader.LocalName) {
			case GQML.PAGE_NPCTALK_DIALOGITEM:
				xmlRootAttr.ElementName = GQML.PAGE_NPCTALK_DIALOGITEM;
				XmlSerializer serializer = new XmlSerializer (typeof(DialogItem), xmlRootAttr);
				DialogItem d = (DialogItem)serializer.Deserialize (reader);
				dialogItems.Add (d);
				break;
			default:
				base.ReadContent (reader, xmlRootAttr);
				break;
			}
		}

		#endregion

	}

	public class DialogItem : IXmlSerializable
	{

		#region State

		public int Id {
			get;
			set;
		}

		public bool IsBlocking {
			get;
			set;
		}

		public string Speaker {
			get;
			set;
		}

		public string AudioURL {
			get;
			set;
		}

		public string Text {
			get;
			set;
		}

		#endregion


		#region XML Serialization

		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		public void WriteXml (System.Xml.XmlWriter writer)
		{
			Debug.LogWarning ("WriteXML not implemented for " + GetType ().Name);
		}

		public void ReadXml (XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.PAGE_NPCTALK_DIALOGITEM);

			// Read Attributes:
			Id = GQML.GetIntAttribute (GQML.ID, reader);
			IsBlocking = GQML.GetRequiredBoolAttribute (GQML.PAGE_NPCTALK_DIALOGITEM_BLOCKING, reader);
			Speaker = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_DIALOGITEM_SPEAKER, reader);
			AudioURL = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_DIALOGITEM_AUDIOURL, reader);
            QuestManager.CurrentlyParsingQuest.AddMedia(AudioURL, "NPCTalk#DialogItem." + GQML.PAGE_NPCTALK_DIALOGITEM_AUDIOURL);

            // Content: Read and implicitly proceed the reader so that this node is completely consumed:
            Text = reader.ReadInnerXml ();
		}

		#endregion

		#region Null

		public static NullDialogItem Null = new NullDialogItem ();

		public class NullDialogItem : DialogItem
		{

			internal NullDialogItem ()
			{

			}
		}

		#endregion

	}
}
