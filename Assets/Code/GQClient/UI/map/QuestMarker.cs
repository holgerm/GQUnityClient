using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.UI;
using GQ.Client.Err;
using GQ.Client.Util;
using GQ.Client.Model;
using GQ.Client.UI.Dialogs;
using GQ.Client.Conf;

namespace GQ.Client.UI
{

	public class QuestMarker : Marker {

		public QuestInfo Data { get; set; }

		public override void UpdateView () {}

		public override void OnTouch() {
			Debug.Log(string.Format("Marker '{0}' has been touched. (id: {1}, file: {2})", Data.Name, Data.Id, QuestManager.GetLocalPath4Quest (Data.Id)));
			if (!Data.IsOnDevice && Data.IsOnServer) {
				Task download = Data.Download ();
				Task play = Data.Play ();
				TaskSequence t = new TaskSequence (download);
				t.AppendIfCompleted (play);
				t.Start ();
			}
			if (Data.IsOnDevice) {
				Data.Play ().Start();
			}
		}

		protected void Play ()
		{
			if (Data == null) {
				Log.SignalErrorToDeveloper("Tried to play quest for QuestMarker without QuestInfo data.");
				return;
			}
			
			// Load quest data: game.xml
			LocalFileLoader loadGameXML = 
				new LocalFileLoader (
					filePath: QuestManager.GetLocalPath4Quest (Data.Id) + QuestManager.QUEST_FILE_NAME
				);
			new DownloadDialogBehaviour (loadGameXML, "Loading quest");

			QuestStarter questStarter = new QuestStarter ();

			TaskSequence t = 
				new TaskSequence (loadGameXML, questStarter);

			t.Start ();
		}

		public override Texture Texture {
			get {
				string category = Data.Categories.Count > 0 ? Data.Categories [0] : "base";
				string textureID = "marker." + category;
				Texture2D t = TextureManager.Instance.GetTexture (textureID);
				if (t == null) {
					// create Texture and store it in TextureManager:
					t = Resources.Load<Texture2D>(ConfigurationManager.Current.marker.path);
					string categoryID = QuestInfoManager.Instance.CurrentCategoryId (Data);
					try {
						Texture2D symbol = Resources.Load<Texture2D> (ConfigurationManager.Current.categoryDict [categoryID].symbol.path);
					}
					catch (KeyNotFoundException)
					{
						Log.SignalErrorToAuthor ("Quest Category {0} not found.", categoryID);
					}
					TextureManager.Instance.Add (textureID, t);
					Debug.Log("ADDED T to TM: " + textureID + ". Category is: " + QuestInfoManager.Instance.CurrentCategoryId(Data));
				}
				return t;
			}
		}

	}

}
