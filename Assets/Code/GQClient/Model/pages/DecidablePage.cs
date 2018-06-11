using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using GQ.Client.Err;

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

		public override void Start ()
		{
			base.Start ();
		}

		public void Succeed ()
		{
			State = GQML.STATE_SUCCEEDED;
			if (SuccessTrigger != Trigger.Null) {
				SuccessTrigger.Initiate ();
				End (false);
			} else {
				// end this page after succeeding:
				End (true);
			}
		}

		public void Fail ()
		{
			State = GQML.STATE_FAILED;
			if (FailTrigger != Trigger.Null) {
				FailTrigger.Initiate ();
				End (false);
			} else {
				// end this page after failing:
				End (true);
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

		#endregion

	}
}
