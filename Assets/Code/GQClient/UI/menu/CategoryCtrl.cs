using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.UI;
using UnityEngine.UI;
using GQ.Client.Conf;
using QM.UI;

namespace GQ.Client.UI {

	public abstract class CategoryCtrl : MonoBehaviour {

		public Text categoryName;
		public Text categoryCount;

		protected CategoryTreeCtrl treeCtrl;

		protected bool unfolded = true;
		/// <summary>
		/// Is true if the corresponding folder is open, so that this item should eventually be shown.
		/// </summary>
		/// <value><c>true</c> if unfolded; otherwise, <c>false</c>.</value>
		public bool Unfolded { 
			get {
				return unfolded;
			}
			set {
				unfolded = value;
			}
		}

		abstract protected bool showMenuItem ();

		abstract public void ToggleSelectedState ();

	}
}