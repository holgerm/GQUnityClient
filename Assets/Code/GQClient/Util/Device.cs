using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Threading;
using GQ.Client.Err;
using GQ.Client.UI.Dialogs;
using UnityEngine.SceneManagement;
using GQ.Client.Conf;
using GQ.Client.Model;

namespace GQ.Client.Util
{
	
	public class Device
	{
		public static float dpi = Screen.dpi;
		public static int height = Screen.height; 
		public static int width = Screen.width; 
	}

}
