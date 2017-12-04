using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.UI;
using GQ.Client.Err;
using GQ.Client.Util;
using GQ.Client.Model;
using GQ.Client.UI.Dialogs;
using GQ.Client.Conf;
using System;

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

		private const string MARKER_ALPHA_BG_PATH = "defaults/readable/defaultMarkerBG";

		public override Texture Texture {
			get {
				string category = Data.Categories.Count > 0 ? Data.Categories [0] : "base";
				string textureID = "marker." + category;
				Texture2D t = TextureManager.Instance.GetTexture (textureID);
				if (t == null) {
					// load basic marker texture and white alpha background template:
					Texture2D markerOutline = Resources.Load<Texture2D> (ConfigurationManager.Current.marker.path);
					Texture2D alphaBG = Resources.Load<Texture2D> (MARKER_ALPHA_BG_PATH);

					t = new Texture2D(markerOutline.width, markerOutline.height);
					Color[] alphaColors = alphaBG.GetPixels ();
					// colorize solid white parts of the basic marker texture and blend above alpha background:
					Color[] colors = markerOutline.GetPixels();

					for (int i=0; i< colors.Length; i++) {
						if (colors[i].a >= 0.99f) {
							// replace solid white in marker with marker color:
							colors[i].r = ConfigurationManager.Current.markerColor.r;
							colors[i].g = ConfigurationManager.Current.markerColor.g;
							colors[i].b = ConfigurationManager.Current.markerColor.b;
						}

						if (alphaColors [i].a >= 0.99f) {
							// for all pixels of solid white in alpha circle we blend the marker pixel above the alpha circle:
							colors [i] = TextureManager.Blend (colors [i], new Color (1f, 1f, 1f, ConfigurationManager.Current.markerBGAlpha));
						}
					}

					t.SetPixels (colors);

					string categoryID = QuestInfoManager.Instance.CurrentCategoryId (Data);
					try {
						Category cat = ConfigurationManager.Current.categoryDict [categoryID];
						Texture2D symbol = Resources.Load<Texture2D> (cat.symbol.path);
						if (symbol == null) {
							Log.SignalErrorToDeveloper("Symbol Texture not found for category {0}. Using default symbol.", categoryID);
							t.Apply();
							TextureManager.Instance.Add (textureID, t);
							return t;
						}
						if (symbol.width > t.width) {
							Log.SignalErrorToDeveloper("Smybol Texture too wide. Must not be wider than marker outline. Using default symbol.");
							t.Apply();
							TextureManager.Instance.Add (textureID, t);
							return t;
						}
						if (symbol.height > t.height) {
							Log.SignalErrorToDeveloper("Smybol Texture too high. Must not be higher than marker outline. Using default symbol.");
							t.Apply();
							TextureManager.Instance.Add (textureID, t);
							return t;
						}
						Color[] symbolColors = symbol.GetPixels();
						int deltaX = (t.width - symbol.width) / 2;
						int deltaY = t.height - symbol.height;

						Color[] tBelowSymbol = t.GetPixels(deltaX, deltaY, symbol.width, symbol.height);

						for (int i = 0; i< symbolColors.Length; i++) {
							tBelowSymbol[i] = TextureManager.Blend (symbolColors[i], tBelowSymbol[i]);
						}

						t.SetPixels(deltaX, deltaY, symbol.width, symbol.height, tBelowSymbol);
					}
					catch (KeyNotFoundException)
					{
						Log.SignalErrorToAuthor ("Quest Category {0} not found. Using default symbol.", categoryID);
					}

					t.Apply ();
					TextureManager.Instance.Add (textureID, t);
				}
				return t;
			}
		}

	}

}
