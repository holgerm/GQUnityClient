using UnityEditor;
using System.IO;
using UnityEngine;

namespace QM.EditUtils
{
	public class CreateAssetBundles
	{
		[MenuItem("Assets/Build AssetBundles")]
		static void BuildAllAssetBundles4Android()
		{
			BuildPipeline.BuildAssetBundles(
                Path.Combine(Application.streamingAssetsPath),
                BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget);
		}
    }
}