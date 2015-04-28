using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Product
{
	public class Builder
	{
		private static string PRODUCT_SHORT_NAME = "wcc"; // TODO make an environment var etc.
		private static string PRODUCT_DISPLAY_NAME = "WCC Regio Bonn"; // TODO make an environment var etc.
		private static string BUILD_TARGET = "Android"; // TODO make an environment var etc.

		private static string PRODUCTS_DIR = "Production/products/";
		private static string PRODUCT_ASSETS_DIR = "Assets/Editor/products/";
		private static Dictionary<BuildTarget, List<int>> appIconSizes = new Dictionary<BuildTarget, List<int>> ()
		{
			{ 
				BuildTarget.Android, 
				new List<int>() { 192, 144, 96, 72, 48, 36 }
			},
			{ 
				BuildTarget.iOS, 
				new List<int>() { 180, 152, 144, 120, 114, 76, 72, 57 }
			}
		};

		public static void BuildAndroidPlayer ()
		{
			PlayerSettings.bundleIdentifier = GetBundleIdentifier ();
			PlayerSettings.productName = PRODUCT_DISPLAY_NAME;

			Debug.Log ("ICONS: set before: " + PlayerSettings.GetIconsForTargetGroup (BuildTargetGroup.Android).Length);
			Debug.Log ("ICONS: own icons: " + GetAppIcons (BuildTarget.Android).Length);
			Debug.Log ("ICONS: own icon height: " + GetAppIcons (BuildTarget.Android) [0].height);

			PlayerSettings.SetIconsForTargetGroup (BuildTargetGroup.Android, GetAppIcons (BuildTarget.Android));

			Debug.Log ("ICONS: set after: " + PlayerSettings.GetIconsForTargetGroup (BuildTargetGroup.Android).Length);

			BuildPipeline.BuildPlayer (GetScenes (), GetAndroidOutputPath (), BuildTarget.Android, BuildOptions.None);
		}

		/// <summary>
		/// Gets the scenes to be included into the build.
		/// 
		/// TODO should be read from some config file.
		/// </summary>
		/// <returns>The scenes.</returns>
		static string[] GetScenes ()
		{
			return new string[] {
				"Assets/Scenes/questlist.unity",
				"Assets/Scenes/Pages/page_npctalk.unity",
				"Assets/Scenes/Pages/page_fullscreen.unity",
				"Assets/Scenes/Pages/page_multiplechoicequestion.unity",
				"Assets/Scenes/Pages/page_videoplay.unity",
				"Assets/Scenes/Pages/page_qrcodereader.unity",
				"Assets/Scenes/Pages/page_imagecapture.unity",
				"Assets/Scenes/Pages/page_textquestion.unity",
				"Assets/Scenes/Pages/page_audiorecord.unity",
				"Assets/Scenes/Pages/page_map.unity"
			};
		}

		static string GetBundleIdentifier ()
		{
			return "com.questmill.geoquest." + PRODUCT_SHORT_NAME;
		}

		static Texture2D[] GetAppIcons (BuildTarget target)
		{
			string appIconPathTrunk = PRODUCT_ASSETS_DIR + PRODUCT_SHORT_NAME + "/appIcon_";
			List<int> sizes;
			if (!appIconSizes.TryGetValue (target, out sizes)) {
				Debug.LogError ("No app icon sizes defined for build target " + target.GetType ().Name);
			}

			Texture2D[] appIcons = new Texture2D[sizes.Count];
			int i = 0;
			foreach (int size in sizes) {
				// TODO: check that file exists or log error.
				appIcons [i++] =
					AssetDatabase.LoadMainAssetAtPath (appIconPathTrunk + size + ".png") as Texture2D;

			}
			return appIcons;
		}

		/// <summary>
		/// Gets the Android apk output path - relative to the project dir.
		/// </summary>
		/// <returns>The relative output path for Android apk.</returns>
		static string GetAndroidOutputPath ()
		{
			return PRODUCTS_DIR + PRODUCT_SHORT_NAME + "/" + BUILD_TARGET + "/gq_" + PRODUCT_SHORT_NAME + ".apk";
		}
	}
}

