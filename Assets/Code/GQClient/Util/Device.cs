using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Threading;
using GQ.Client.Err;
using GQ.Client.UI.Dialogs;
using UnityEngine.SceneManagement;
using GQ.Client.Conf;
using GQ.Client.Model;
using QM.Util;
using GQ.Client.FileIO;

namespace GQ.Client.Util
{

	/// <summary>
	/// Helper class allows mocking for tests.
	/// </summary>
	public class Device
	{
		#region Screen
		public static float dpi = Screen.dpi;
		public static int height = Screen.height; 
		public static int width = Screen.width; 
		#endregion

		#region DataPath
		private static Void2String _getPersistentDatapath = () => {
			return Application.persistentDataPath;
		};

		public static string GetPersistentDatapath() {
			return _getPersistentDatapath();
		}

		public static void SetPersistentDataPathMethod(Void2String method) {
			_getPersistentDatapath = method;
		}
		#endregion
	}

}
