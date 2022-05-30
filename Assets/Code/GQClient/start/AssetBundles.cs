//#define DEBUG_LOG

using System.Collections.Generic;
using System.IO;
using Code.GQClient.Err;
using UnityEngine;

namespace Code.GQClient.start
{
    public class AssetBundles : MonoBehaviour
    {
        public static readonly string PREFABS = "prefabs";

        // Start is called before the first frame update
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        static Dictionary<string, AssetBundle> Bundles = new Dictionary<string, AssetBundle>();

        internal static Object Asset(string assetBundleName, string assetName)
            // TODO create for iOS and Android at once
        {
            if (!Bundles.ContainsKey(assetBundleName))
            {
                string path = Path.Combine(Application.streamingAssetsPath, assetBundleName);
                var newBundle = AssetBundle.LoadFromFile(path);
                if (newBundle == null)
                {
                    Log.SignalErrorToDeveloper($"Failed to load AssetBundle '{assetBundleName}' from path {path}!");
                    return null;
                }
                else
                {
                    Bundles[assetBundleName] = newBundle;
#if DEBUG_LOG
                    Debug.LogFormat("AssetBundle {0} loaded.", assetBundleName);
#endif
                }
            }

            var obj = Bundles[assetBundleName].LoadAsset(assetName);
            if (obj == null)
            {
                Log.SignalErrorToDeveloper("Failed to load Asset '{0}' from AssetBundle '{1}'!", assetName,
                    assetBundleName);
                return null;
            }

#if DEBUG_LOG
            Debug.LogFormat("Asset {0} from Bundle {1} loaded.", assetName, assetBundleName);
#endif

            return obj;
        }
    }
}