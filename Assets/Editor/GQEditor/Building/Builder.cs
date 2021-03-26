using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace GQ.Editor.Building
{
    public class Builder
    {
        const string UNITY_IOS_ProjectFileName = "Unity-iPhone.xcodeproj";

        static string productID = "default";
        static bool replaceProduct = false;

        public static string ProductID
        {
            get
            {
                return productID;
            }
            set
            {
                productID = value;
            }
        }

        private static string PRODUCTS_DIR = "Production/products/";
        private static string PRODUCT_ASSETS_DIR = "Assets/Editor/products/";
        private static Dictionary<BuildTarget, List<int>> appIconSizes = new Dictionary<BuildTarget, List<int>>() { {
                BuildTarget.Android,
                new List<int>() {
                    192,
                        144,
                    96,
                        72,
                    48,
                        36
                }
            }, {
                BuildTarget.iOS,
                new List<int>() {
                    180,
                        152,
                    144,
                        120,
                    114,
                        76,
                    72,
                        57
                }
            }
        };

        public static void BuildPlayers()
        {
            string[] args = Environment.GetCommandLineArgs();
            bool productIDFound = false;
            int i = 0;
            while (i < args.Length && !productIDFound)
            {
                if (args[i].Equals("--gqproduct") && args.Length > i + 1)
                {
                    productIDFound = true;
                    productID = args[++i];
                }
                if (args[i].Equals("--gqreplace"))
                {
                    replaceProduct = true;
                }
                i++;
            }

            if (productIDFound)
            {
                Debug.Log("Producing: " + productID);
                //				ProductEditorOld.load(productID);
            }
            else
            {
                Debug.LogError("ERROR: No product ID specified. Use --gqproduct <productID> to build your product!");
                return; // TODO how should we exit in error cases like this?
            }

            changeAndSavePlayerSettings();

            Debug.Log("Building product " + PlayerSettings.productName + " (" + PlayerSettings.applicationIdentifier + ")");

            BuildAndroidPlayer();
            BuildIOSPlayer();

            restoreSavedPlayerSettings();
        }

        static void BuildAndroidPlayer()
        {
            string errorMsg;

            // Build Android:
            Debug.Log("Building Android player ...".Yellow());
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, GetAppIcons(BuildTarget.Android));
            string outDir = PRODUCTS_DIR + productID + "/Android";
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir + "/");
            }
            string outPath = outDir + "/gq_" + productID + ".apk";
            BuildReport buildReport = BuildPipeline.BuildPlayer(GetScenes(), outPath, BuildTarget.Android, BuildOptions.None);
            errorMsg = "Errors during build: " + outPath + " # " + buildReport.summary.totalErrors;
            if (errorMsg != null && !errorMsg.Equals(""))
            {
                Debug.LogError("ERROR while trying to build Android player: " + errorMsg);
            }
            else
            {
                Debug.Log("Build done for Android.");
            }
        }

        static void BuildIOSPlayer()
        {
            string errorMsg;

            // Build iOS:
            Debug.Log("Building iOS player ...");
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, GetAppIcons(BuildTarget.iOS));
            string outDir = PRODUCTS_DIR + productID + "/iOS/gq_" + productID;
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir + "/");
            }
            // should we replace the project? otherwise we append
            BuildOptions buildOptions = replaceProduct ?
                BuildOptions.None :
                    BuildOptions.AcceptExternalModificationsToPlayer;
            // in case we did not build that project vefore, we can not append to it:
            if (!File.Exists(outDir + "/" + UNITY_IOS_ProjectFileName))
                buildOptions = BuildOptions.None;
            BuildReport buildReport = BuildPipeline.BuildPlayer(GetScenes(), outDir, BuildTarget.iOS, buildOptions);
            errorMsg = "Errors during build: " + outDir + " # " + buildReport.summary.totalErrors;
            if (errorMsg != null && !errorMsg.Equals(""))
            {
                Debug.LogError("ERROR while trying to build iOS player: " + errorMsg);
                Debug.LogError("  args follow:");
                foreach (string arg in Environment.GetCommandLineArgs())
                {
                    Debug.LogError("    " + arg);
                }
            }
            else
            {
                Debug.Log("Build done for iOS.");
            }
        }

        static string savedSettingsBundleIdentifier;
        static string savedSettingsProductName;
        static Texture2D[] savedSettingsIcons4Android;
        static Texture2D[] savedSettingsIcons4iOS;

        static void changeAndSavePlayerSettings()
        {
            savedSettingsBundleIdentifier = PlayerSettings.applicationIdentifier;
            PlayerSettings.applicationIdentifier = GetBundleIdentifier();

            savedSettingsProductName = PlayerSettings.productName;
            PlayerSettings.productName = Config.Current.name;

            savedSettingsIcons4Android = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Android);
            savedSettingsIcons4iOS = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.iOS);
        }

        static void restoreSavedPlayerSettings()
        {
            PlayerSettings.applicationIdentifier = savedSettingsBundleIdentifier;
            PlayerSettings.productName = savedSettingsProductName;
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, savedSettingsIcons4Android);
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, savedSettingsIcons4iOS);
        }

        /// <summary>
        /// Gets the scenes to be included into the build.
        /// 
        /// TODO should be read from some config file.
        /// </summary>
        /// <returns>The scenes.</returns>
        static string[] GetScenes()
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

        static string GetBundleIdentifier()
        {
            return "com.questmill.geoquest." + Config.Current.id;
        }

        static Texture2D[] GetAppIcons(BuildTarget target)
        {
            string appIconPathTrunk = PRODUCT_ASSETS_DIR + productID + "/appIcon_";
            List<int> sizes;
            if (!appIconSizes.TryGetValue(target, out sizes))
            {
                Debug.LogError("No app icon sizes defined for build target " + target.GetType().Name);
            }

            Texture2D[] appIcons = new Texture2D[sizes.Count];
            int i = 0;
            foreach (int size in sizes)
            {
                // TODO: check that file exists or log error.
                appIcons[i++] =
                    AssetDatabase.LoadMainAssetAtPath(appIconPathTrunk + size + ".png") as Texture2D;
                Debug.Log("Loaded app icon from path: " + appIconPathTrunk + size + ".png");
            }
            return appIcons;
        }


#if UNITY_IOS

        [PostProcessBuild(1)]
        public static void AmendUsageRightsInfosToPList_IOS_Only(BuildTarget target, string pathToBuiltProject)
        {
            Debug.Log("Build done. Target was " + target.ToString() + "; build path is: " + pathToBuiltProject);
            if (target != BuildTarget.iOS)
            {
                Debug.Log("Non iOS Build.");
                // we only do this for iOS builds:
                return;
            }
            PlistDocument infoPlist = new PlistDocument();
            string infoPath = pathToBuiltProject + "/Info.plist";
            infoPlist.ReadFromFile(infoPath);
            infoPlist.root.SetString("NSAppleMusicUsageDescription", "In interactive quests audio that you recorded may be stored on your phone.");
            infoPlist.root.SetString("NSCameraUsageDescription", "Photos can be used in interactive quests.");
            infoPlist.root.SetString("NSPhotoLibraryUsageDescription", "In interactive quests photos that you take may be stored on your phone.");
            infoPlist.root.SetString("NSMicrophoneUsageDescription", "In interactive quests microphone can be used to let you record audio.");
            infoPlist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            infoPlist.WriteToFile(infoPath);

            Debug.Log("Build for iOS: PList enhanced by usage strings about access rights.");
        }

#endif
        private static Dictionary<BuildTarget, string> BuildFolderNames = new Dictionary<BuildTarget, string>()
        {
            [BuildTarget.iOS] = "iOS",
            [BuildTarget.Android] = "Android"
        };
        private static Dictionary<BuildTarget, string> BuildNamePostfix = new Dictionary<BuildTarget, string>()
        {
            [BuildTarget.iOS] = @"/",
            [BuildTarget.Android] = ".apk"
        };

        [MenuItem("Build/Build GeoQuest")]
        public static void BuildGQ()
        {
            BuildGQ(EditorUserBuildSettings.activeBuildTarget);
        }

        private static void BuildGQ(BuildTarget target)
        {
            BuildPipeline.BuildAssetBundles(
                Path.Combine(Application.streamingAssetsPath),
                BuildAssetBundleOptions.None,
                target
            );

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new string[Config.Current.scenePaths.Length];
            Config.Current.scenePaths.CopyTo(buildPlayerOptions.scenes, 0);
            buildPlayerOptions.locationPathName =
                Path.Combine(
                    "Production/builds/",
                    Config.Current.id,
                    BuildFolderNames[target],
                    "gq_" + Config.Current.id + BuildNamePostfix[target]);
            buildPlayerOptions.target = target;
            buildPlayerOptions.options = BuildOptions.None;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
        }


    }

}