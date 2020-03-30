using UnityEditor;
using System.IO;
using UnityEditor.Build;
using UnityEngine;

namespace QM.EditUtils
{
    public class CreateAssetBundles : IActiveBuildTargetChanged
    {
        [MenuItem("Assets/Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            BuildPipeline.BuildAssetBundles(
                Path.Combine(Application.streamingAssetsPath),
                BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget);
            Debug.Log("Asset bundles built for " + EditorUserBuildSettings.activeBuildTarget.ToString());
        }

        public int callbackOrder => 0;
        
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            BuildAllAssetBundles();
        }
    }
}