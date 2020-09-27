using Code.GQClient.Model;
using UnityEngine;

namespace Code.GQClient.UI.map
{

	public class HotspotMarker : Marker
	{
        public const string SERVER_DEFAULT_MARKER_URL = "https://quest-mill.intertech.de/assets/img/erzbistummarker.png";
        public const string QUEST_SPECIFIC_MARKER_MDKEY = "gq.questspecific.marker";

        public Texture2D DefaultTexture;

		public void Awake ()
		{
			DefaultTexture = Resources.Load<Texture2D> (DEFAULT_MARKER_PATH);
		}

		public Hotspot Hotspot { get; set; }

		public override Texture2D Texture {
			get {
				return DefaultTexture;
			}
		}

		public override void OnTouchOMM ()
		{
            if (Hotspot.Active)
            {
                Hotspot.Tap();
            }
		}
		public override void OnTouchOMM(OnlineMapsMarkerBase marker)
		{
			OnTouchOMM();
		}


	}
}
