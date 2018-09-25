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
using UnityEngine.EventSystems;

namespace GQ.Client.UI
{

	public class QuestMarker : Marker
	{

		public QuestInfo Data { get; set; }

		public override void OnTouch ()
		{
			if (!Data.IsOnDevice && Data.IsOnServer) {
				Task download = Data.DownloadTask ();
				Task play = Data.Play ();
				TaskSequence t = new TaskSequence (download);
				t.AppendIfCompleted (play);
				t.Start ();
			}
			if (Data.IsOnDevice) {
				Data.Play ().Start ();
			}
		}

		protected void Play ()
		{
			if (Data == null) {
				Log.SignalErrorToDeveloper ("Tried to play quest for QuestMarker without QuestInfo data.");
				return;
			}
			
			// Load quest data: game.xml
			LocalFileLoader loadGameXML = 
				new LocalFileLoader (
					filePath: QuestManager.GetLocalPath4Quest (Data.Id) + QuestManager.QUEST_FILE_NAME
				);
			new DownloadDialogBehaviour (
				loadGameXML, 
				string.Format ("Loading {0}", ConfigurationManager.Current.nameForQuestSg)
			);

			QuestStarter questStarter = new QuestStarter ();

			TaskSequence t = 
				new TaskSequence (loadGameXML, questStarter);

			t.Start ();
		}

		public override Texture Texture {
			get {
				string categoryID = Data.CurrentCategoryId;
				string textureID = "marker." + categoryID;
				Texture2D t = TextureManager.Instance.GetTexture (textureID);

				if (t == null) {
					// load basic marker texture and white alpha background template:
					Texture2D markerOutline = Resources.Load<Texture2D> (ConfigurationManager.Current.marker.path);
					t = new Texture2D (markerOutline.width, markerOutline.height);

					Texture2D symbol = null;
					try {
						Category cat = ConfigurationManager.Current.categoryDict [categoryID];
						symbol = Resources.Load<Texture2D> (cat.symbol.path);
						if (symbol == null) {
							Log.SignalErrorToDeveloper ("Symbol Texture not found for category {0}. Using default symbol.", categoryID);
						} else if (symbol.width > t.width) {
							Log.SignalErrorToDeveloper ("Smybol Texture too wide. Must not be wider than marker outline. Using default symbol.");
							symbol = null;
						} else if (symbol.height > t.width) {
							Log.SignalErrorToDeveloper ("Smybol Texture too high. Must not be higher than marker outline width. Using default symbol.");
							symbol = null;
						}
					} catch (KeyNotFoundException) {
						Log.SignalErrorToAuthor ("Quest Category {0} not found. Using default symbol.", categoryID);
					}

					Color32[] outlineColors = markerOutline.GetPixels32 ();

					Texture2D alphaBG = Resources.Load<Texture2D> (MARKER_ALPHA_BG_PATH);
					Color32[] alphaColors = alphaBG.GetPixels32 ();
					Color32[] symbolColors = null;

					int symbolXMin = 0;
					int symbolXMax = 0;
					int symbolYMin = 0;
					int symbolYMax = 0;

					if (symbol == null) {
						symbol = new Texture2D (1, 1);
						symbol.SetPixels32 (new Color32[] { new Color32 (0, 0, 0, 0) });
					}

					symbolColors = symbol.GetPixels32 ();
					symbolXMin = (t.width - symbol.width) / 2; // before this column there are no symbol pixels
					symbolXMax = (t.width + symbol.width) / 2 - 1; // after this line there are no symbol pixels
					symbolYMin = t.height - (t.width + symbol.height) / 2; // below this line there are no symbol pixels
					symbolYMax = t.height - (t.width - symbol.height) / 2 - 1; // above this line there are no symbol pixels
						
					int i = 0; // counter for fast access in flat color arrays for marker outline and alpha circle
					int j = 0; // counter for symbol colors array (which is often smaller
					for (int y = 0; y < markerOutline.height; y++) {
						for (int x = 0; x < markerOutline.width; x++) {
							
							if (symbolYMin <= y && y <= symbolYMax && symbolXMin <= x && x <= symbolXMax) {
								if (alphaColors [i].a == 255) {
									if (symbolColors [j].a > 0) {
                                        // replace white base color with fg color:
                                        symbolColors[j].r = ConfigurationManager.Current.markerSymbolFGColor.r;
                                        symbolColors[j].g = ConfigurationManager.Current.markerSymbolFGColor.g;
                                        symbolColors[j].b = ConfigurationManager.Current.markerSymbolFGColor.b;
 
                                        // we take symbol pixels if we find them above the opaque white circle:
                                        alphaColors[i] = symbolColors [j];
									} else {
										// we take the marker background alpha as specified for this product:
										alphaColors [i].a = ConfigurationManager.Current.markerBGAlpha;
									}
								}
								j++;
							} else if (alphaColors [i].a == 255) {
								// outside of the symbol but inside the white circle, we also use only the specified transparency:
								alphaColors [i].a = ConfigurationManager.Current.markerBGAlpha;
							}

							// colorize marker outline (keeping alpha channel):
							outlineColors [i].r = ConfigurationManager.Current.markerColor.r;
							outlineColors [i].g = ConfigurationManager.Current.markerColor.g;
							outlineColors [i].b = ConfigurationManager.Current.markerColor.b;

							// blend outline above alpha cirlce (already evtually including symbol):
							outlineColors [i] = TextureManager.Blend (outlineColors [i], alphaColors [i]);

							i++;
						}
					}
						
					t.SetPixels32 (outlineColors);
					t.Apply ();
					// cache this marker texture:
					TextureManager.Instance.Add (textureID, t);
				}
				return t;
			}
		}

	}

}
