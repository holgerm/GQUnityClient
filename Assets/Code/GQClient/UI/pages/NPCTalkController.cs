using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.UI {
	
	public class NPCTalkController : PageController {

		#region Fields

		public Transform image;
		private string IMAGE_PATH = "Viewport/Panel/Image";

		public Transform text;
		private string Text_PATH = "Viewport/Panel/Text";

		#endregion


		#region Runtime API

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		#endregion


		#region Editor Setup

		void Reset()
		{
			image = EnsurePrefabVariableIsSet<Transform> (image, "Image", IMAGE_PATH);
			text = EnsurePrefabVariableIsSet<Transform> (text, "Text", Text_PATH);
		}	

		#endregion

	}
}