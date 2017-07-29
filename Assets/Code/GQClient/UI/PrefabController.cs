using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Err;
using GQ.Util;

namespace GQ.Client.UI {
	
	public abstract class PrefabController : MonoBehaviour {

		#region Runtime API

		protected static GameObject Create(string prefabName, GameObject root = null) {
			Object prefab = Resources.Load (prefabName);
			if (prefab == null) {
				Log.SignalErrorToDeveloper ("Resource for prefab '{0}' could not be loaded.", prefabName);
				return null;
			}

			if (root == null) {
				root = GameObject.FindGameObjectWithTag(Tags.ROOT_CANVAS);
			}

			GameObject go = (GameObject) Instantiate (
				prefab,
				root.transform,
				false
			);
			go.SetActive (false);
			return go;
		}


		/// <summary>
		/// Shows the dialog for at least one frame duration.
		/// </summary>
		public void Show() {
			Base.Instance.StartCoroutine (showAsCoroutine(true));
		}

		/// <summary>
		/// Hides the dialog for at least one frame duration.
		/// </summary>
		public void Hide() {
			Base.Instance.StartCoroutine (showAsCoroutine(false));
		}

		private IEnumerator showAsCoroutine(bool show) {
			yield return new WaitForEndOfFrame ();
			gameObject.SetActive (show);
			yield return new WaitForEndOfFrame ();
		}

		#endregion


		#region Initialization in Editor

		protected T EnsurePrefabVariableIsSet<T>(T variable, string goName, string goPath) 
			where T : Component
		{
			if (variable == null)
			{
				Transform textGo = transform.Find (goPath);
				if (textGo == null) {
					Debug.LogErrorFormat (
						"Dialog must contain a {0} GameObject \"{1}\" inside (at path {2}).", 
						variable.GetType().Name,
						goName,
						goPath);
					return null;
				}

				variable = textGo.GetComponent<T> ();
			}
			return variable;
		}

		#endregion

	}
}
