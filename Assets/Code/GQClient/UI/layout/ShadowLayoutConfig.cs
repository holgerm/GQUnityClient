using Code.GQClient.Conf;
using UnityEngine;

namespace Code.GQClient.UI.layout
{

	public class ShadowLayoutConfig : LayoutConfig
	{

		public GameObject showOnlyWhenActive;

		public override void layout ()
		{
			gameObject.SetActive (Config.Current.showShadows && (showOnlyWhenActive == null || showOnlyWhenActive.activeInHierarchy));
		}
	}
}
