using Code.GQClient.Model;
using UnityEngine;

namespace Code.GQClient.UI.map
{

	public class HotspotMarker : Marker
	{
        public const string SERVER_DEFAULT_MARKER_URL = "https://quest-mill.intertech.de/assets/img/erzbistummarker.png";
        public const string QUEST_SPECIFIC_MARKER_MDKEY = "gq.questspecific.marker";

        private Texture2D _defaultTexture;

		public void Awake ()
		{
			_defaultTexture = Resources.Load<Texture2D> (DEFAULT_MARKER_PATH);
		}

		private Hotspot Hotspot { get; set; }

		public override Texture2D Texture {
			get {
				return _defaultTexture;
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
