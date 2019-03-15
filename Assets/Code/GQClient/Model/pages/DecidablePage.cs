using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using GQ.Client.Err;
using System;

namespace GQ.Client.Model
{
	abstract public class DecidablePage : Page
	{

		#region XML Serialization

		protected override void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			XmlSerializer serializer; 

			switch (reader.LocalName) {
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

		public override void Start (bool canReturnToPrevious = false)
		{
			base.Start (canReturnToPrevious);
		}

		public void Succeed (bool alsoEnd = true)
		{
            State = GQML.STATE_SUCCEEDED;

            if (!alsoEnd)
                return;

            if (SuccessTrigger != Trigger.Null) {
				SuccessTrigger.Initiate ();
				End (false);
			} else {
                // end this page after succeeding:
                End(true);
			}
		}

		public void Fail (bool alsoEnd = true)
		{
			State = GQML.STATE_FAILED;

            if (!alsoEnd)
                return;

			if (FailTrigger != Trigger.Null) {
				FailTrigger.Initiate ();
                End(false);
			} else {
                // end this page after failing:
                End(true);
			}
		}

		public override void End() {
			End (true);
		}

		void End(bool needsContinuation) {
			if (EndTrigger == Trigger.Null) {
				if (needsContinuation) {
					Log.SignalErrorToAuthor (
						"Quest {0} ({1}, page {2} has no actions onEnd defined, hence we end the quest here.",
						Quest.Name, Quest.Id,
						Id
					);
					Quest.End ();
				}
			} else {
				EndTrigger.Initiate ();
			}
			Resources.UnloadUnusedAssets ();
		}

        protected List<IText> ExpectedCodes = new List<IText>();

        public bool AnswerCorrect(string input)
        {
            if (input == null)
                return false;

            foreach (ExpectedCode a in ExpectedCodes)
            {
                // TODO: Extract this into a reusable method that will be called from here and from TextQuestionPage as well.
                string aText = a.Text.Trim();

                // Text comparison:
                if (aText == input.Trim())
                    return true;

                // Number Ranges:
                if (aText.StartsWith("[[") && aText.EndsWith("]]"))
                {
                    string[] rangeBounds = aText.Substring(2, aText.Length - 4).Split('-');
                    if (rangeBounds.Length == 2)
                    {
                        try
                        {
                            double lowerBound = Convert.ToDouble(rangeBounds[0]);
                            double upperBound = Convert.ToDouble(rangeBounds[1]);

                            if (upperBound < lowerBound)
                            {
                                double swapTmp = upperBound;
                                upperBound = lowerBound;
                                lowerBound = swapTmp;
                            }

                            // bounds are ok:
                            double number;
                            try
                            {
                                number = Convert.ToDouble(input.Trim());

                                // now we test wether input is in range:
                                return (lowerBound <= number && number <= upperBound);
                            }
                            catch (FormatException)
                            {
                                Log.SignalErrorToUser("Eingabe '{0}' kann nicht als Zahl erkannt werden.", input);
                                return false;
                            }
                            catch (OverflowException)
                            {
                                Log.SignalErrorToUser("Eingabe '{0}' zu groß oder zu klein um als Zahl benutzt zu werden.", input);
                                return false;
                            }
                        }
                        catch (Exception)
                        {
                            Log.SignalErrorToAuthor(
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
}
