using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.UI {

	public class UIController : MonoBehaviour {

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