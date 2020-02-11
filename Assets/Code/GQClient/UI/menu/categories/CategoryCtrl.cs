using TMPro;
using UnityEngine;

namespace Code.GQClient.UI.menu.categories
{

    public abstract class CategoryCtrl : MonoBehaviour
	{

		public TextMeshProUGUI categoryName;
		public TextMeshProUGUI categoryCount;

		protected CategoryTreeCtrl treeCtrl;

		protected bool unfolded = false;

		/// <summary>
		/// Is true if the corresponding folder is open, so that this item should eventually be shown.
		/// </summary>
		/// <value><c>true</c> if unfolded; otherwise, <c>false</c>.</value>
		public virtual bool Unfolded { 
			get {
				return unfolded;
			}
			set {
				unfolded = value;
			}
		}

		abstract protected bool showMenuItem ();

	}
}