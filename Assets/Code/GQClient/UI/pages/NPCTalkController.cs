using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;
using UnityEngine.UI;
using Candlelight.UI;
using GQ.Client.Util;

namespace GQ.Client.UI
{
	
	public class NPCTalkController : PageController
	{

		#region Fields

		public Transform image;
		private string IMAGE_PATH = "Viewport/Panel/Image";

		public HyperText text;
		private string Text_PATH = "Viewport/Panel/Text";

		protected PageNPCTalk npcPage;

		#endregion


		#region Runtime API

		// Use this for initialization
		public override void Start ()
		{
			base.Start ();
			Debug.Log (
				"Here NPCTAlkController @quest: " + QuestManager.Instance.CurrentQuest.Name +
				"@page: " + QuestManager.Instance.CurrentPage.Id
			);

			if (page != null) {
				npcPage = (PageNPCTalk)page;
				text.text = TextHelper.Decode4HyperText (npcPage.CurrentDialogItem.Text);
			}
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}

		#endregion


		#region Editor Setup

		void Reset ()
		{
			image = EnsurePrefabVariableIsSet<Transform> (image, "Image", IMAGE_PATH);
			text = EnsurePrefabVariableIsSet<HyperText> (text, "Text", Text_PATH);
		}

		#endregion

	}
}