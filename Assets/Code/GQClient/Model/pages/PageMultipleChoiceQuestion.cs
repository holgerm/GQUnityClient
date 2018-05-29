using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using GQ.Client.Err;

namespace GQ.Client.Model
{

	[XmlRoot (GQML.PAGE)]
	public class PageMultipleChoiceQuestion : Page
	{

		#region State

		public string LoopButtonText { get; set ; }

		public string LoopText { get; set ; }

		public string LoopImage { get; set; }

		public bool LoopUntilSuccess { get; set; }

		public string Question { get; set ; }

		public bool ShowOnlyImages { get; set; }

		public bool Shuffle { get; set; }

		public string BackGroundImage { get; set; }

		public List<Answer> Answers = new List<Answer> ();

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			LoopButtonText = GQML.GetStringAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_LOOP_BUTTON_TEXT, reader);

			LoopText = GQML.GetStringAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_LOOP_TEXT, reader);

			LoopImage = GQML.GetStringAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_LOOP_IMAGE, reader);
			if (LoopImage != "")
				QuestManager.CurrentlyParsingQuest.AddMedia (LoopImage);

			LoopUntilSuccess = GQML.GetOptionalBoolAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_LOOP_UNTIL_SUCCESS, reader);

			Question = GQML.GetStringAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_QUESTION, reader);

			ShowOnlyImages = GQML.GetOptionalBoolAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_SHOW_ONLY_IMAGES, reader);

			Shuffle = GQML.GetOptionalBoolAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_SHUFFLE, reader);

			BackGroundImage = GQML.GetStringAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_BACKGROUND_IMAGE, reader);
			if (BackGroundImage != "")
				QuestManager.CurrentlyParsingQuest.AddMedia (BackGroundImage);
		}

		protected override void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			switch (reader.LocalName) {
			case GQML.PAGE_MULTIPLECHOICEQUESTION_ANSWER:
				xmlRootAttr.ElementName = GQML.PAGE_MULTIPLECHOICEQUESTION_ANSWER;
				XmlSerializer serializer = new XmlSerializer (typeof(Answer), xmlRootAttr);
				Answer a = (Answer)serializer.Deserialize (reader);
				Answers.Add (a);
				break;
			case GQML.ON_SUCCESS:
				xmlRootAttr.ElementName = GQML.ON_SUCCESS;
				serializer = new XmlSerializer (typeof(Trigger), xmlRootAttr);
				SuccessTrigger = (Trigger)serializer.Deserialize (reader);
				SuccessTrigger.Parent = this;
				break;
			case GQML.ON_FAIL:
				xmlRootAttr.ElementName = GQML.ON_FAIL;
				serializer = new XmlSerializer (typeof(Trigger), xmlRootAttr);
				FailTrigger = (Trigger)serializer.Deserialize (reader);
				FailTrigger.Parent = this;
				break;
			default:
				base.ReadContent (reader, xmlRootAttr);
				break;
			}
		}

		protected Trigger SuccessTrigger = Trigger.Null;
		protected Trigger FailTrigger = Trigger.Null;

		#endregion


		#region Runtime API

		public override void Start ()
		{
			base.Start ();
		}

		public void Succeed ()
		{
			State = GQML.STATE_SUCCEEDED;
			if (SuccessTrigger != Trigger.Null) {
				SuccessTrigger.Initiate ();
			}

			// end this page after succeeding:
			End ();
		}

		public void Fail ()
		{
			State = GQML.STATE_FAILED;
			if (FailTrigger != Trigger.Null) {
				FailTrigger.Initiate ();
			}

			// end this page after failing:
			End ();
		}

		#endregion
	}

	public class Answer : IXmlSerializable
	{

		#region State

		public int Id {
			get;
			set;
		}

		public bool Correct {
			get;
			set;
		}

		public string Text {
			get;
			set;
		}


		public string Image {
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
			GQML.AssertReaderAtStart (reader, GQML.PAGE_MULTIPLECHOICEQUESTION_ANSWER);

			// Read Attributes:
			Id = GQML.GetIntAttribute (GQML.PAGE_ID, reader);
			Correct = GQML.GetRequiredBoolAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_ANSWER_CORRECT, reader);
			Image = GQML.GetStringAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_ANSWER_IMAGE, reader);
			if (Image != "")
				QuestManager.CurrentlyParsingQuest.AddMedia (Image);

			// Content: Read and implicitly proceed the reader so that this node is completely consumed:
			Text = reader.ReadInnerXml ();
		}

		#endregion

		#region Null

		public static NullAnswer Null = new NullAnswer ();

		public class NullAnswer : Answer
		{

			internal NullAnswer ()
			{

			}
		}

		#endregion

	}
}
