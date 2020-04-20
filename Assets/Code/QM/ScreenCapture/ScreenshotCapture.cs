using System.Collections;
using System.IO;
using UnityEngine;

namespace QM.SC

{
    [RequireComponent(typeof(Camera))]
    public class ScreenshotCapture : MonoBehaviour
    {

       public void Take()
        {
           ScreenCapture.CaptureScreenshot(
               $"{Application.dataPath}/../Screenshots/Test_{Screen.width}-{Screen.height}.png");
        }

        // static readonly private string SCREENSHOT_DIR_DEFAULT = "SCREENSHOT_DIR_DEFAULT";
        // static string ScreenshotDir {
        //     get
        //     {
        //         if (!EditorPrefs.HasKey(SCREENSHOT_DIR_DEFAULT))
        //         {
        //             EditorPrefs.SetString(SCREENSHOT_DIR_DEFAULT, Application.dataPath + "/Screenshots");
        //         }
        //         return EditorPrefs.GetString(SCREENSHOT_DIR_DEFAULT);
        //     }
        //     set
        //     {
        //         Debug.Log("Trying to set screenshot dir to: " + value);
        //         EditorPrefs.SetString(SCREENSHOT_DIR_DEFAULT, value);
        //     }
        // } 
        //
        // static string ProductScreenshotDir
        // {
        //     get
        //     {
        //         return ProductManager.PRODUCT_ADDON_PATH + "/" + ConfigurationManager.Current.id + "/_AppScreenshots";
        //     }
        // }
        //
        // static private void SaveToFile(Texture2D texture)
        // {
        //     string scene = SceneManager.GetActiveScene().name;
        //     string filename = scene + "_" + DateTime.Now.ToString("yyMMdd_HHmmss") + "_" + texture.width + "x" + texture.height + ".jpg";
        //     string path = ProductScreenshotDir + "/" + filename;
        //
        //     // Save the screnshot.
        //     Directory.CreateDirectory(ScreenshotDir);
        //     byte[] png = texture.EncodeToJPG();
        //     File.WriteAllBytes(path, png);
        //
        //     EditorUtility.DisplayDialog(
        //             "Screenshot for scene: " + scene,
        //             "Saved to " + path, 
        //             "Ok");
        // }
        //
        //
        // [MenuItem("Edit/Set Screenshot dir")]
        // static void SetScreenshotDir()
        // {
        //     string newDir = EditorUtility.OpenFolderPanel("Sreenshot directory", ScreenshotDir, "");
        //     if (!string.IsNullOrEmpty(newDir)) {
        //         ScreenshotDir = newDir;
        //     }
        // }
    }
}
