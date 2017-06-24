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

		#region Null

		public static NullDialogItem Null = new NullDialogItem ();

		public class NullDialogItem : DialogItem
		{

			internal NullDialogItem ()
			{
				
			}
		}

		#endregion

		#region State

		protected string endButtonText;

		public string EndButtonText {
			get {
				return endButtonText;
			}
			protected set {
				endButtonText = value;
			}
		}

		protected string imageUrl;

		public string ImageUrl {
			get {
				return imageUrl;
			}
			protected set {
				imageUrl = value;
			}
		}

		protected string nextDialogButtonText;

		public string NextDialogButtonText {
			get {
				return nextDialogButtonText;
			}
			protected set {
				nextDialogButtonText = value;
			}
		}

		protected string displayMode;

		public string DisplayMode {
			get {
				return displayMode;
			}
			protected set {
				displayMode = value;
			}
		}

		protected bool skipWordTicker;

		public bool SkipWordTicker {
			get {
				return skipWordTicker;
			}
			protected set {
				skipWordTicker = value;
			}
		}

		protected int textSize;

		public int TextSize {
			get {
				return textSize;
			}
			protected set {
				textSize = value;
			}
		}

		protected int tickerSpeed;

		public int TickerSpeed {
			get {
				return tickerSpeed;
			}
			protected set {
				tickerSpeed = value;
			}
		}

		protected List<DialogItem> dialogItems = new List<DialogItem> ();

		public DialogItem CurrentDialogItem {
			get {
				if (CurDialogItemNo == 0) 
				// cannot be negative or beyond limit cf. property setter.
				return Null;
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

		public override void Start (Quest quest)
		{
			base.Start (quest);
			CurDialogItemNo++;
		}

		public void Next ()
		{
			CurDialogItemNo++;
		}

		public override void End ()
		{
			State = GQML.STATE_SUCCEEDED;
			// TODO: Invoke onEnd Trigger ...

		}


		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			EndButtonText = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_ENDBUTTONTEXT, reader);
			ImageUrl = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_IMAGEURL, reader);
			DisplayMode = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_DISPLAYMODE, reader);
			NextDialogButtonText = GQML.GetStringAttribute (GQML.PAGE_NPCTALK_NEXTBUTTONTEXT, reader);
			SkipWordTicker = GQML.GetBoolAttribute (GQML.PAGE_NPCTALK_SKIPWORDTICKER, reader);
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
