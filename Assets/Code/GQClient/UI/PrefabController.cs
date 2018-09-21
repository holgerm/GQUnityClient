using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;
using GQ.Client.Util;

namespace GQ.Client.UI
{
	
	public abstract class PrefabController : UIController
	{

		#region Runtime API

		public static GameObject Create (string prefabName, GameObject root = null)
		{
			Object prefab = Resources.Load (prefabName);
			if (prefab == null) {
				Log.SignalErrorToDeveloper ("Resource for prefab '{0}' could not be loaded.", prefabName);
				return null;
			}

			if (root == null) {
				root = GameObject.FindGameObjectWithTag (Tags.ROOT_CANVAS);
				if (root == null)
					Log.SignalErrorToDeveloper ("No root game object found for prefab {0}", prefabName);
			}

			GameObject go = (GameObject)Instantiate (
				                prefab,
				                root.transform,
				                false
			                );
			go.SetActive (false);
			return go;
		}


		/// <summary>
		/// Shows the prefab for at least one frame duration.
		/// </summary>
		public virtual void Show ()
        {
			Base.Instance.StartCoroutine (showAsCoroutine (true));
		}

		/// <summary>
		/// Hides the prefab for at least one frame duration.
		/// </summary>
		public virtual void Hide ()
		{
			Base.Instance.StartCoroutine (showAsCoroutine (false));
		}

		protected virtual IEnumerator showAsCoroutine (bool show)
		{
			yield return new WaitForEndOfFrame ();
			gameObject.SetActive (show);
		}

		/// <summary>
		/// Deletes the prefab from the hierarchy.
		/// </summary>
		public virtual void Destroy ()
		{
			Base.Instance.StartCoroutine (destroyAsCoroutine ());
		}

		private IEnumerator destroyAsCoroutine ()
		{
			yield return new WaitForEndOfFrame ();
			yield return new WaitForEndOfFrame ();

			if (this != null && gameObject != null) {
				gameObject.SetActive (false);
				Destroy (gameObject);
			}
		}

		#endregion

	}
}
