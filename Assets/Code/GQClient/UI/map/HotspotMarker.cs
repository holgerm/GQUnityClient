using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;

namespace GQ.Client.UI
{

	public class HotspotMarker : Marker
	{
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
