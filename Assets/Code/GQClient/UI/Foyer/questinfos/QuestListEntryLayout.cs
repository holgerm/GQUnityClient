using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GQ.Client.Conf;

namespace GQ.Client.UI
{

	public class QuestListEntryLayout : LayoutConfig
	{

		public override void layout ()
		{
			// set entry background color:
			Image image = GetComponent<Image> ();
			if (image != null) {
				image.color = ConfigurationManager.Current.listEntryColor;
			}

			// set heights of text and image:
			FoyerListLayoutConfig.SetListEntryHeight (gameObject);
			FoyerListLayoutConfig.SetListEntryHeight (gameObject, "InfoButton", sizeScaleFactor: 0.65f);
			FoyerListLayoutConfig.SetListEntryHeight (gameObject, "Name");
			FoyerListLayoutConfig.SetListEntryHeight (gameObject, "DownloadButton");
			FoyerListLayoutConfig.SetListEntryHeight (gameObject, "StartButton");
			FoyerListLayoutConfig.SetListEntryHeight (gameObject, "DeleteButton");
			FoyerListLayoutConfig.SetListEntryHeight (gameObject, "UpdateButton");
		}

	}
}
