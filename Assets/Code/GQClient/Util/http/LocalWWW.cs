using Code.QM.Util;
using UnityEngine;

namespace Code.GQClient.Util.http {

	public static class LocalWWW {

		/// <summary>
		/// Use LocalWWW.Create("path/to/my.file") instead of new WWW(pre + Application.persistentDataPath + "path/to/my.file").
		/// </summary>
		/// <param name="localFilePath">Local file path.</param>
		public static WWW Create (string localFilePath) {
			string pre = "file: /";

			if ( Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ) {
				pre = "file:";
			}

			return new WWW(pre + Device.GetPersistentDatapath() + localFilePath);
		}

	}

}
