using UnityEngine;


namespace QM.Util {

	public static class TransformE {

		public static string GetPath(this Transform transform) {
			string myPath = "/" + transform.name;
			if (transform.parent == null)
				return myPath;

			return transform.parent.GetPath() + myPath;
		}

	}
}
