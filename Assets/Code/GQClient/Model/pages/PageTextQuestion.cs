using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using System;
using GQ.Client.Err;

namespace GQ.Client.Model
{

	[XmlRoot (GQML.PAGE)]
	public class PageTextQuestion : DecidablePage
	{
		
		#region State

		public string LoopButtonText { get; set ; }

		public string LoopText { get; set ; }

		public string LoopImage { get; set; }

		public bool LoopUntilSuccess { get; set; }

		public string Question { get; set ; }

		public string Prompt { get; set ; }

		public string BackGroundImage { get; set; }

		private List<TQAnswer> Answers = new List<TQAnswer> ();

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			LoopButtonText = GQML.GetStringAttribute (GQML.PAGE_QUESTION_LOOP_BUTTON_TEXT, reader);

			LoopText = GQML.GetStringAttribute (GQML.PAGE_QUESTION_LOOP_TEXT, reader);

			LoopImage = GQML.GetStringAttribute (GQML.PAGE_QUESTION_LOOP_IMAGE, reader);
			if (LoopImage != "")
				QuestManager.CurrentlyParsingQuest.AddMedia (LoopImage);

			LoopUntilSuccess = GQML.GetOptionalBoolAttribute (GQML.PAGE_QUESTION_LOOP_UNTIL_SUCCESS, reader);

			Question = GQML.GetStringAttribute (GQML.PAGE_QUESTION_QUESTION, reader);

			Prompt = GQML.GetStringAttribute (GQML.PAGE_TEXTQUESTION_PROMPT, reader);

			BackGroundImage = GQML.GetStringAttribute (GQML.PAGE_QUESTION_BACKGROUND_IMAGE, reader);
			if (BackGroundImage != "")
				QuestManager.CurrentlyParsingQuest.AddMedia (BackGroundImage);
		}

		protected override void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			XmlSerializer serializer; 

			switch (reader.LocalName) {
			case GQML.PAGE_QUESTION_ANSWER:
				xmlRootAttr.ElementName = GQML.PAGE_QUESTION_ANSWER;
				serializer = new XmlSerializer (typeof(TQAnswer), xmlRootAttr);
				TQAnswer a = (TQAnswer)serializer.Deserialize (reader);
				Answers.Add (a);
				break;
			default:
				base.ReadContent (reader, xmlRootAttr);
				break;
			}
		}

		#endregion


		#region Runtime API

		public override void Start ()
		{
			base.Start ();
		}

		public bool AnswerCorrect (string input)
		{
			foreach (TQAnswer a in Answers) {
				string aText = a.Text.Trim ();

				// Text comparison:
				if (aText == input.Trim ())
					return true;
				
				// Number Ranges:
				if (aText.StartsWith ("[[") && aText.EndsWith ("]]")) {
					string[] rangeBounds = aText.Substring (2, aText.Length - 4).Split ('-');
					if (rangeBounds.Length == 2) {
						try {
							double lowerBound = Convert.ToDouble (rangeBounds [0]);
							double upperBound = Convert.ToDouble (rangeBounds [1]);

							if (upperBound < lowerBound) {
								double swapTmp = upperBound;
								upperBound = lowerBound;
								lowerBound = swapTmp;
							}

							// bounds are ok:
							double number;
							try {
								number = Convert.ToDouble (input.Trim ());

								// now we test wether input is in range:
								return (lowerBound <= number && number <= upperBound);
							} catch (FormatException) {
								Log.SignalErrorToUser ("Eingabe '{0}' kann nicht als Zahl erkannt werden.", input);
								return false;
							} catch (OverflowException) {
								Log.SignalErrorToUser ("Eingabe '{0}' zu groß oder zu klein um als Zahl benutzt zu werden.", input);
								return false;
							}
						} catch (Exception) {
							Log.SignalErrorToAuthor (
								"In Quest {0} auf Seite {1} kann Antwort '{2}' nicht als Zahlenbereich erkannt werden.", 
								Quest.Id,
								Id,
								a.Text);
							return false;
						} 
					}
				}

				// TODO RegExp
			}


			return false;
		}

		#endregion

	}

	public class TQAnswer : IXmlSerializable
	{

		#region State

		public int Id {
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
			GQML.AssertReaderAtStart (reader, GQML.PAGE_QUESTION_ANSWER);

			// Read Attributes:
			Id = GQML.GetIntAttribute (GQML.PAGE_ID, reader);
			Image = GQML.GetStringAttribute (GQML.PAGE_MULTIPLECHOICEQUESTION_ANSWER_IMAGE, reader);
			if (Image != "")
				QuestManager.CurrentlyParsingQuest.AddMedia (Image);

			// Content: Read and implicitly proceed the reader so that this node is completely consumed:
			Text = reader.ReadInnerXml ();
		}

		#endregion

		#region Null

		public static NullAnswer Null = new NullAnswer ();

		public class NullAnswer : MCQAnswer
		{

			internal NullAnswer ()
			{

			}
		}

		#endregion

	}
}
