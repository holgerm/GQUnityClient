using UnityEngine;
using GQ.Client.Conf;

namespace GQ.Client.UI
{

	public class ShadowLayoutConfig : LayoutConfig
	{

		public GameObject showOnlyWhenActive;

		public override void layout ()
		{
			gameObject.SetActive (ConfigurationManager.Current.showShadows && (showOnlyWhenActive == null || showOnlyWhenActive.activeInHierarchy));
		}
	}
}
