using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;

namespace GQ.Client.UI
{

	public class HotspotMarker : Marker
	{
        public const string SERVER_DEFAULT_MARKER_URL = "https://quest-mill.intertech.de/assets/img/erzbistummarker.png";
        public const string QUEST_SPECIFIC_MARKER_MDKEY = "gq.questspecific.marker";

        public Texture DefaultTexture;

		public void Awake ()
		{
			DefaultTexture = Resources.Load<Texture2D> (DEFAULT_MARKER_PATH);
		}

		public Hotspot Hotspot { get; set; }

		public override Texture Texture {
			get {
				return DefaultTexture;
			}
		}

		public override void OnTouch ()
		{
			Hotspot.Tap ();
		}

	}
}
