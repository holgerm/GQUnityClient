using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Product
{
	public class Builder
	{
//		private static string PRODUCT_SHORT_NAME = "wcc"; // TODO make an environment var etc.
//		private static string PRODUCT_DISPLAY_NAME = "WCC Regio Bonn"; // TODO make an environment var etc.
		static string productID = "default";

		public static string ProductID {
			get {
				return productID;
			}
			set {
				productID = value;
			}
		}

		static Dictionary<string, string> productNames = new Dictionary<string, string> () {
			{
				"default", 
				"GeoQuest"
			},
			{
				"carlbenz", 
				"Carl Benz"
			},
			{
				"wcc",
				"WCC Regio Bonn"
			},
			{
				"ebk",
				"GeoQuest"
			},
			{
				"phka",
				"EduQuest"
			}
		};

		static string getProductName ()
		{
			string name;
			if (!productNames.TryGetValue (ProductID, out name)) {
				Debug.LogError ("ERROR: Unknown product ID: " + ProductID);
			}
			return name;
		}

		// TODO make an environment var etc.

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

		public static void BuildPlayers ()
		{
			string[] args = Environment.GetCommandLineArgs ();
			bool productIDFound = false;
			int i = 0;
			while (i < args.Length && !productIDFound) {
				if (args [i].Equals ("--gqproduct") && args.Length > i + 1) {
					productIDFound = true;
					productID = args [i + 1];
				} else
					i++;
			}

			if (productIDFound) {
				Debug.Log ("Producing: " + productID);
			} else {
				Debug.LogError ("ERROR: No product ID specified. Use --gqproduct <productID> to build your product!");
				return; // TODO how should we exit in error cases like this?
			}

			PlayerSettings.bundleIdentifier = GetBundleIdentifier ();
			PlayerSettings.productName = getProductName ();

			Debug.Log ("Building product " + PlayerSettings.productName + " (" + PlayerSettings.bundleIdentifier + ")");

			BuildAndroidPlayer ();
//			BuildIOSPlayer ();
		}

		static void BuildAndroidPlayer ()
		{
			string errorMsg;

			// Build Android:
			Debug.Log ("Building Android player ...");
			PlayerSettings.SetIconsForTargetGroup (BuildTargetGroup.Android, GetAppIcons (BuildTarget.Android));
			string outDir = PRODUCTS_DIR + productID + "/Android";
			if (!Directory.Exists (outDir)) {
				Directory.CreateDirectory (outDir + "/");
			}
			string outPath = outDir + "/gq_" + productID + ".apk";
			errorMsg = BuildPipeline.BuildPlayer (GetScenes (), outPath, BuildTarget.Android, BuildOptions.None);
			if (errorMsg != null && !errorMsg.Equals ("")) {
				Debug.LogError ("ERROR while trying to build Android player: " + errorMsg);
			} else {
				Debug.Log ("Build done for Android.");
			}
		}

		static void BuildIOSPlayer ()
		{
			string errorMsg;
			
			// Build Android:
			Debug.Log ("Building iOS player ...");
			PlayerSettings.SetIconsForTargetGroup (BuildTargetGroup.Android, GetAppIcons (BuildTarget.Android));
			string outDir = PRODUCTS_DIR + productID + "/iOS/gq_" + productID;
			if (!Directory.Exists (outDir)) {
				Directory.CreateDirectory (outDir + "/");
			}
			errorMsg = BuildPipeline.BuildPlayer (GetScenes (), outDir, BuildTarget.iOS, BuildOptions.None);
			if (errorMsg != null && !errorMsg.Equals ("")) {
				Debug.LogError ("ERROR while trying to build iOS player: " + errorMsg);
			} else {
				Debug.Log ("Build done for iOS.");
			}
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
			return "com.questmill.geoquest." + productID;
		}

		static Texture2D[] GetAppIcons (BuildTarget target)
		{
			string appIconPathTrunk = PRODUCT_ASSETS_DIR + productID + "/appIcon_";
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

	}

}