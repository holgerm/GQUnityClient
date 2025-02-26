using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using GQ.Editor.Util;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace GQ.Editor.Building
{
    /// <summary>
    /// The ProductSpec class represents a product specifiation of our app at edit time (not runtime!). 
    /// 
    /// Each ProductSpec instance refers to image files and resources directly 
    /// and to all textual parameters via a Config object. Product instances are used by the ProductManager
    /// and can be edited in the ProductEditor view.
    /// 
    /// A product is backed on file by diverse graphic files and a configuration file (Product.json). 
    /// These files reside in one folder (the product folder) which can have an arbitrary name. 
    /// 
    /// You create a ProductSpec instance by calling the Constructor with the product folder path as argument.
    /// 
    /// The ProductSpec is NOT the Build Setting, instead you use a ProductSpec to create the current build setting.
    /// </summary>
    public class ProductSpec
    {
        #region Product Configuration Properties

        public string Id
        {
            get { return Config.id; }
            private set
            {
                Config.id = value; // TODO
            }
        }

        private string _dir;

        /// <summary>
        /// Directory path for this product.
        /// </summary>
        /// <value>The dir.</value>
        public string Dir
        {
            get => _dir;
            private set => _dir = value;
        }

        internal const string APP_ICON = "AppIcon.png";

        public string AppIconPath
        {
            get { return Files.CombinePath(Dir, APP_ICON); }
        }

        internal const string SPLASH_SCREEN = "SplashScreen.jpg";

        public string SplashScreenPath
        {
            get { return Files.CombinePath(Dir, SPLASH_SCREEN); }
        }

        internal const string TOP_LOGO = "TopLogo.jpg";

        public string TopLogoPath
        {
            get { return Files.CombinePath(Dir, TOP_LOGO); }
        }

        internal const string ANDROID_MANIFEST = "AndroidManifest.xml";

        public string AndroidManifestPath
        {
            get { return Files.CombinePath(Dir, ANDROID_MANIFEST); }
        }

        internal const string STREAMING_ASSETS = "StreamingAssets";

        public string StreamingAssetPath
        {
            get { return Files.CombinePath(Dir, STREAMING_ASSETS); }
        }

        internal const string GQ_BUNDLE_ID_PREFIX = "com.questmill.geoquest";

        internal const string ONLINEMAPS_CONFIG = "OnlineMapsConfig.json";

        /// <summary>
        /// Gets the path to the config file Product.json.
        /// </summary>
        /// <value>The config path.</value>
        public string ConfigPath
        {
            get
            {
                return Files.CombinePath(Dir, ConfigurationManager.CONFIG_FILE);
            }
        }

        private Config _config;

        public Config Config
        {
            get
            {
                if (_config == null)
                {
                    if (EditorApplication.isPlaying)
                        // Play Mode
                        InitConfig();
                    else
                        // Edit Mode (app not running in editor right now)
                        _config = Config.Current;
                }

                return _config;
            }
            set => _config = value;
        }

        public RTConfig RTConfig;

        private string RTConfigPath => 
            Code.GQClient.FileIO.Files.CombinePath(Dir, RTConfig.RT_CONFIG_FILE);

/*
		private RTConfig _rtConfig;
*/

        #endregion


        #region Creating Product Intances

        /// <summary>
        /// Initializes a new Product instance by the given directory. 
        /// It expects that all product files are contained in the directory. 
        /// Among referring to image files etc. it also deserializes a Config object internally to read the Product.json specification of all textual parameters.
        /// 
        /// Throws ArgumentException when the folder does not contain all necessary stuff correctly, i.e. branding files and a matching config file.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="dir">Dir.</param>
        public ProductSpec(string dirPath)
        {
            // Check path:
            if (!Directory.Exists(dirPath))
                throw new ArgumentException("Invalid path: Product directory not found: " + dirPath);

            // Init Dir:
            if (dirPath.EndsWith("/"))
                dirPath = dirPath.Substring(0, dirPath.Length - 1);
            _dir = dirPath;

            InitConfig();
        }

        internal void InitConfig()
        {
            // init and check Config:
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    Log.SignalErrorToDeveloper("Invalid product definition. Product.json file missing.");
                    throw new ArgumentException("Invalid product definition. Product.json file missing.");
                }

                string configJSON = File.ReadAllText(ConfigPath);
                Config = JsonConvert.DeserializeObject<Config>(configJSON);
                string rtConfigJson = File.ReadAllText(RTConfigPath);
                Config.rt = JsonConvert.DeserializeObject<RTConfig>(rtConfigJson);
            }
            catch (Exception exc)
            {
                Log.SignalErrorToDeveloper($"Invalid product definition. Reading Product.json: {exc.Message}");
                throw new ArgumentException("Invalid product definition. Product.json file could not be read.", exc);
            }

            // init and check RTConfig:
            try
            {
                if (!File.Exists(RTConfigPath))
                {
                    Log.SignalErrorToDeveloper(
                        $"Invalid product definition. RTProduct.json file missing in folder {_dir}.");
                    throw new ArgumentException("Invalid product definition. RTProduct.json file missing.");
                }
            }
            catch (Exception exc)
            {
                Log.SignalErrorToDeveloper($"Invalid product definition. Reading RTProduct.json: {exc.Message}");
                throw new ArgumentException(
                    "Invalid product definition. RTProduct.json file could not be read.", exc);
            }
        }

        internal static bool IsValidProductName(string name)
        {
            // TODO do we need to restrict the product names somehow?
            return true;
        }

        #endregion

        public override string ToString()
        {
            return String.Format("product {0}", Id);
        }

        #region Validity and Errors

        protected List<GQError> _errors = new List<GQError>();

        public List<GQError> Errors
        {
            get { return _errors; }
        }

        protected void StoreError(string message)
        {
            Errors.Add(new GQError(message));
        }

        public string AllErrorsAsString()
        {
            StringBuilder errorString = new StringBuilder();
            foreach (var error in Errors)
            {
                errorString.AppendLine(error.ToString());
            }

            return errorString.ToString();
        }

        /// <summary>
        /// Validates this product.
        /// </summary>
        /// <returns><c>true</c>, if product was validated, <c>false</c> otherwise.</returns>
        public bool IsValid()
        {
            bool isValid = true;
            bool productJSONFound = false;
            bool appIconFound = false;
            bool appIconBGFound = false;
            bool appIconFGFound = false;
            bool topLogoFound = false;
            bool androidManifestFound = false;

            // Directory must exist:
            DirectoryInfo productDir = new DirectoryInfo(Dir);
            isValid &= productDir.Exists;

            // Checking some basic files:
            FileInfo[] files = productDir.GetFiles();
            foreach (FileInfo file in files)
            {
                // Product.json
                if ("Product.json".Equals(file.Name))
                {
                    productJSONFound = true;
                    // TODO do more detailed checks here (e.g. marker images)
                    continue;
                }

                // AppIcon:
                if ("AppIcon.png".Equals(file.Name))
                {
                    appIconFound = true;
                    continue;
                }

                if ("AppIconFG.png".Equals(file.Name))
                {
                    appIconFGFound = true;
                    continue;
                }

                if ("AppIconBG.png".Equals(file.Name) || "AppIconBG.jpg".Equals(file.Name))
                {
                    appIconBGFound = true;
                    continue;
                }

                // TopLogo.jpg
                if ("TopLogo.png".Equals(file.Name))
                {
                    topLogoFound = true;
                    continue;
                }

                // AndroidManifest.xml
                if ("AndroidManifest.xml".Equals(file.Name))
                {
                    androidManifestFound = true;
                    string foundID = ProductManager.Extract_ID_FromXML_Watermark(file.FullName);
                    if (foundID == null)
                    {
                        StoreError("Android Manifest misses a product watermark.");
                    }
                    else
                    {
                        if (!Id.Equals(foundID))
                            StoreError("Android Manifest watermark (" + foundID +
                                       ") does not correspond to this product (" + Id + ").");
                        continue;
                    }
                }
            } // end foreach file

            if (!productJSONFound)
            {
                StoreError("No Product.json file found.");
            }

            if (!appIconFound)
            {
                StoreError("No AppIcon.png file found.");
            }

            if (!appIconBGFound)
            {
                StoreError("No AppIconBG.png file found.");
            }

            if (!appIconFGFound)
            {
                StoreError("No AppIconFG.png file found.");
            }

            if (!topLogoFound)
            {
                StoreError("No TopLogo.png file found.");
            }

            if (!androidManifestFound)
            {
                StoreError("No AndroidManifest.xml file found.");
            }

            isValid &= Errors.Count == 0;

            return isValid;
        }

        #endregion
    }
}