using UnityEngine;

namespace QM.UI
{

    public class MenuRadioGroup : MonoBehaviour
	{

		public GameObject MenuCanvas;

		public void HideMenu ()
		{
			if (MenuCanvas != null)
				MenuCanvas.gameObject.SetActive (false);
		}

	}
}