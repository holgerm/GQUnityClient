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

		public string NextDialogButtonText { get; set; }

		public string DisplayMode { get; set; }

		public bool SkipWordTicker { get; set; }

		public int TextSize { get; set; }

		public int TickerSpeed { get; set; }

		protected List<DialogItem> dialogItems = new List<DialogItem> ();

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
		/// Limited by the available dialog items: If not dialog items are present it will always be zero.
		/// </summary>
		/// <value>The current dialog item no.</value>
		public int CurDialogItemNo {
			get {
				return curDialogItemNo;
			}
			protected set {
				curDialogItemNo = Math.Max (0, Math.Min (curDialogItemNo + 1, dialogItems.Count));
			}
		}

		#endregion


		#region Runtime API

		public override void Start ()
		{
			base.Start ();
			CurDialogItemNo++;
		}

		public void Next ()
		{
			CurDialogItemNo++;
		}

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			EndButtonText = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_ENDBUTTONTEXT, reader);

			ImageUrl = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_IMAGEURL, reader);
			if (ImageUrl != "")
				QuestManager.CurrentlyParsingQuest.AddMedia (ImageUrl);

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

}
