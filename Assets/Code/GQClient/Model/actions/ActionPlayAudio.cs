using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Xml.Serialization;
using GQ.Client.Err;
using System.Xml;
using GQ.Client.Util;

namespace GQ.Client.Model
{

	public class ActionPlayAudio : ActionAbstract
	{
		#region State

		public string AudioUrl { get; set; }

		public bool Loop { get; set; }

		public bool StopOthers { get; set; }

		#endregion


		#region Structure

		protected override void ReadAttributes (XmlReader reader)
		{
			Loop = GQML.GetOptionalBoolAttribute (GQML.ACTION_PLAYAUDIO_LOOP, reader, false);
			StopOthers = GQML.GetOptionalBoolAttribute (GQML.ACTION_PLAYAUDIO_STOPOTHERS, reader, true);

			AudioUrl = GQML.GetStringAttribute (GQML.ACTION_PLAYAUDIO_FILE, reader);
			if (AudioUrl != "")
				QuestManager.CurrentlyParsingQuest.AddMedia (AudioUrl);
		}

		#endregion


		#region Functions

		public override void Execute ()
		{
			MediaInfo audioInfo = null;
			if (QuestManager.Instance.CurrentQuest.MediaStore.TryGetValue(AudioUrl, out audioInfo)) {
				Audio.PlayFromFile (AudioUrl, Loop, StopOthers);
			}
			else {
				Log.SignalErrorToAuthor ("Audio file refenrenced at {0} not locally stored.", AudioUrl);
			}
		}

		#endregion
	}
}
