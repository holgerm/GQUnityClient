using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using GQTests;
using GQ.Editor.UI;

namespace GQ.Editor.Building
{

	class GQAssetChangePostprocessor : AssetPostprocessor
	{

		static bool productDirHasChanges;
		static bool configHasChanged;
		static bool buildTimeChanged;

		static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
	
			productDirHasChanges = false;
			configHasChanged = false;
			buildTimeChanged = false;
	
			// if the only change is buildtime.txt we ignore it:
			if (importedAssets.Length == 1 && importedAssets [0].Equals (ConfigurationManager.BUILD_TIME_FILE_PATH))
				return;

			foreach (string str in importedAssets) {
                if (ProductManager.Instance.IsImportingPackage &&
                    str.StartsWith(Config.RUNTIME_PRODUCT_DIR + "/" + "ImportedPackage", StringComparison.CurrentCulture))
                {
                    // ignore "changes" due to importing the package file:
                    DateTime time = DateTime.Now;
                    Debug.Log("IMPORTED PACKAGE FILE ignored " + str + 
                              " at " + time.Hour + ":" + time.Minute + ":" + time.Second + "." + time.Millisecond);
                    continue;
                }

                setChangeFlags(str);
			}

			foreach (string str in deletedAssets) {

				setChangeFlags (str);
			}

			foreach (string str in movedAssets) {

				setChangeFlags (str);
			}

			foreach (string str in movedFromAssetPaths) {

				setChangeFlags (str);
			}

			if (buildTimeChanged && !ProductEditor.IsCurrentlyPreparingProduct) {
				writeBuildDate ();
			}

			if (configHasChanged) {
				ProductManager.Instance.ConfigFilesHaveChanges = true;
				ConfigurationManager.Reset ();
				configHasChanged = false;
			}

			ProductEditor.BuildIsDirty = productDirHasChanges; 
			if (ProductEditor.Instance != null)
				ProductEditor.Instance.Repaint ();
		}

        private static bool isMonitoredAsset (string assetPath)
		{
			if (assetPath.StartsWith (GQAssert.TEST_DATA_BASE_DIR, StringComparison.CurrentCulture))
				// test assets are NOT REAL assets:
				return false;

			if (assetPath.Equals (ConfigurationManager.BUILD_TIME_FILE_PATH))
				// buildtime.txt is NOT a REAL asset:
				return false;
                
			return true;
		}

		private static void setChangeFlags (string str)
		{

			if (productDirHasChanges == false && str.StartsWith (ProductManager.ProductsDirPath)) {
				// a product might have changed: refresh product list:
				productDirHasChanges = true;
			}

			if (!buildTimeChanged && isMonitoredAsset (str)) {
				buildTimeChanged = true;
			}

			if (!configHasChanged &&
			    !ProductEditor.IsCurrentlyPreparingProduct &&
			    isMonitoredAsset (str) &&
			    str.StartsWith (ProductManager.Instance.BuildExportPath)) {
				configHasChanged = true;
			}
		}

		/// <summary>
		/// Writes the current build date into a tiny file in the ConfigAssets. 
		/// It will be read by the application on start and used as additional version number.
		/// </summary>
		internal static void writeBuildDate ()
		{
			try {
				CultureInfo culture = new CultureInfo ("de-DE"); 
				if (File.Exists (ConfigurationManager.BUILD_TIME_FILE_PATH)) {
					AssetDatabase.DeleteAsset (ConfigurationManager.BUILD_TIME_FILE_PATH);
				} 
				string buildtime = DateTime.Now.ToString ("G", culture);

				int mainVersionNumber = EditorPrefs.HasKey ("mainVersionNumber") ? EditorPrefs.GetInt ("mainVersionNumber") : 0;
				int yearVersionNumber = EditorPrefs.HasKey ("yearVersionNumber") ? EditorPrefs.GetInt ("yearVersionNumber") : DateTime.Now.Year;
				int monthVersionNumber = EditorPrefs.HasKey ("monthVersionNumber") ? EditorPrefs.GetInt ("monthVersionNumber") : DateTime.Now.Month;
				int buildVersionNumber = EditorPrefs.HasKey ("buildVersionNumber") ? EditorPrefs.GetInt ("buildVersionNumber") : 1;
				string version = String.Format (
					                 "{0}.{1}.{2:D2}.{3:D2}", 
					                 mainVersionNumber,
					                 yearVersionNumber - 2000, 
					                 monthVersionNumber,
					                 buildVersionNumber
				                 );
				string versionString = String.Format ("{0} ({1})", version, buildtime);

				File.WriteAllText (ConfigurationManager.BUILD_TIME_FILE_PATH, versionString);
				AssetDatabase.Refresh ();
			} catch (Exception exc) {
				Log.SignalErrorToDeveloper ("Could not write build time file at " + ConfigurationManager.BUILD_TIME_FILE_PATH + "\n" + exc.Message);
				return;
			} 
		}

		void OnPreprocessTexture()
		{
			if (assetPath.Contains ("/readable/")) {
				TextureImporter textureImporter  = (TextureImporter)assetImporter;

				TextureImporterPlatformSettings standaloneSettings = textureImporter.GetPlatformTextureSettings ("Standalone");
				standaloneSettings.format = TextureImporterFormat.RGBA32;
				standaloneSettings.overridden = true;
				textureImporter.ClearPlatformTextureSettings("Standalone");
				textureImporter.SetPlatformTextureSettings (standaloneSettings);

				TextureImporterPlatformSettings iPhoneSettings = textureImporter.GetPlatformTextureSettings ("iPhone");
				iPhoneSettings.format = TextureImporterFormat.RGBA32;
				iPhoneSettings.overridden = true;
				textureImporter.ClearPlatformTextureSettings("iPhone");
				textureImporter.SetPlatformTextureSettings (iPhoneSettings);

				TextureImporterPlatformSettings androidSettings = textureImporter.GetPlatformTextureSettings ("Android");
				androidSettings.format = TextureImporterFormat.RGBA32;
				androidSettings.overridden = true;
				textureImporter.ClearPlatformTextureSettings ("Android");
				textureImporter.SetPlatformTextureSettings (androidSettings);

				TextureImporterPlatformSettings webGLSettings = textureImporter.GetPlatformTextureSettings ("WebGL");
				webGLSettings.format = TextureImporterFormat.RGBA32;
				webGLSettings.overridden = true;
				textureImporter.ClearPlatformTextureSettings("WebGL");
				textureImporter.SetPlatformTextureSettings (webGLSettings);

				textureImporter.isReadable = true;
				textureImporter.mipmapEnabled = false;
			}
		}
	}
}
