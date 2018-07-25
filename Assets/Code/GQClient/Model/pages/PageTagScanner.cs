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
	public class PageTagScanner : DecidablePage
	{
		
		#region State

		public bool ShowTagContent { get; set; }

		public string Prompt { get; set ; }

		private List<ExpectedCode> ExpectedCodes = new List<ExpectedCode> ();

		#endregion


		#region XML Serialization

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			ShowTagContent = GQML.GetRequiredBoolAttribute (GQML.PAGE_TYPE_QRTAGSCANNER_SHOWTAGCONTENT, reader);

			Prompt = GQML.GetStringAttribute (GQML.PAGE_TYPE_QRTAGSCANNER_PROMPT, reader);
		}

		protected override void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			XmlSerializer serializer; 

			switch (reader.LocalName) {
			case GQML.PAGE_TYPE_QRTAGSCANNER_EXPECTEDCODE:
				xmlRootAttr.ElementName = GQML.PAGE_TYPE_QRTAGSCANNER_EXPECTEDCODE;
				serializer = new XmlSerializer (typeof(ExpectedCode), xmlRootAttr);
				ExpectedCode a = (ExpectedCode)serializer.Deserialize (reader);
				ExpectedCodes.Add (a);
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
			foreach (ExpectedCode a in ExpectedCodes) {
				// TODO: Extract this into a reusable method that will be called from here and from TextQuestionPage as well.
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

	public class ExpectedCode : IXmlSerializable
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
			GQML.AssertReaderAtStart (reader, GQML.PAGE_TYPE_QRTAGSCANNER_EXPECTEDCODE);

			// Read Attributes:
			Id = GQML.GetIntAttribute (GQML.ID, reader);
			// Content: Read and implicitly proceed the reader so that this node is completely consumed:
			Text = reader.ReadInnerXml ();
		}

		#endregion

		#region Null

		public static NullExpectedCode Null = new NullExpectedCode ();

		public class NullExpectedCode : ExpectedCode
		{

			internal NullExpectedCode ()
			{

			}
		}

		#endregion

	}
}
