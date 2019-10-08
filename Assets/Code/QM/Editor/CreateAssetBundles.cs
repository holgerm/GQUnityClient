using UnityEditor;
using System.IO;
using UnityEngine;

namespace QM.EditUtils
{
	public class CreateAssetBundles
	{
		[MenuItem("Assets/Build AssetBundles")]
		static void BuildAllAssetBundles()
		{
			BuildPipeline.BuildAssetBundles(
                Path.Combine(Application.streamingAssetsPath),
                BuildAssetBundleOptions.None,
                BuildTarget.Android);
		}
	}
}