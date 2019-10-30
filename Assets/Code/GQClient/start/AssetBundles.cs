#define DEBUG_LOG

using System.Collections.Generic;
using System.IO;
using GQ.Client.Err;
using UnityEngine;

namespace GQ.Client.Util
{
    public class AssetBundles : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        static Dictionary<string, AssetBundle> Bundles = new Dictionary<string, AssetBundle>();

        internal static Object Asset(string assetBundleName, string assetName)
        {
            if (!Bundles.ContainsKey(assetBundleName))
            {
                AssetBundle newBundle = AssetBundle.LoadFromFile(
                        Path.Combine(Application.streamingAssetsPath, assetBundleName));
                if (newBundle == null)
                {
                    Log.SignalErrorToDeveloper("Failed to load AssetBundle '{0}'!", assetBundleName);
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

            Object obj = Bundles[assetBundleName].LoadAsset(assetName);
            if (obj == null)
            {
                Log.SignalErrorToDeveloper("Failed to load Asset '{0}' from AssetBundle '{1}'!", assetName, assetBundleName);
                return null;
            }

#if DEBUG_LOG
            Debug.LogFormat("Asset {0} from Bundle {1} loaded.", assetName, assetBundleName);
#endif

            return obj;
        }
    }
}
