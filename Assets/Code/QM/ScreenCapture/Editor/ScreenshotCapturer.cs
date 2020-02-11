using System;
using System.Collections;
using System.IO;
using Code.GQClient.Conf;
using Code.GQClient.Util;
using GQ.Editor.Building;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QM.SC

{
    public class ScreenshotCapturer : MonoBehaviour
    {

        [MenuItem("Edit/Take Screenshot")]
        static void Take()
        {
            CoroutineStarter.Instance.StartCoroutine(RecordFrame());
        }

        static private IEnumerator RecordFrame()
        {
            yield return new WaitForEndOfFrame();
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            // do something with texture

            SaveToFile(texture);

            // cleanup
            UnityEngine.Object.DestroyImmediate(texture);
        }

        static readonly private string SCREENSHOT_DIR_DEFAULT = "SCREENSHOT_DIR_DEFAULT";
        static string ScreenshotDir {
            get
            {
                if (!EditorPrefs.HasKey(SCREENSHOT_DIR_DEFAULT))
                {
                    EditorPrefs.SetString(SCREENSHOT_DIR_DEFAULT, Application.dataPath + "/Screenshots");
                }
                return EditorPrefs.GetString(SCREENSHOT_DIR_DEFAULT);
            }
            set
            {
                Debug.Log("Trying to set screenshot dir to: " + value);
                EditorPrefs.SetString(SCREENSHOT_DIR_DEFAULT, value);
            }
        } 

        static string ProductScreenshotDir
        {
            get
            {
                return ProductManager.PRODUCT_ADDON_PATH + "/" + ConfigurationManager.Current.id + "/_AppScreenshots";
            }
        }

        static private void SaveToFile(Texture2D texture)
        {
            string scene = SceneManager.GetActiveScene().name;
            string filename = scene + "_" + DateTime.Now.ToString("yyMMdd_HHmmss") + "_" + texture.width + "x" + texture.height + ".jpg";
            string path = ProductScreenshotDir + "/" + filename;

            // Save the screnshot.
            Directory.CreateDirectory(ScreenshotDir);
            byte[] png = texture.EncodeToJPG();
            File.WriteAllBytes(path, png);

            EditorUtility.DisplayDialog(
                    "Screenshot for scene: " + scene,
                    "Saved to " + path, 
                    "Ok");
        }


        [MenuItem("Edit/Set Screenshot dir")]
        static void SetScreenshotDir()
        {
            string newDir = EditorUtility.OpenFolderPanel("Sreenshot directory", ScreenshotDir, "");
            if (!string.IsNullOrEmpty(newDir)) {
                ScreenshotDir = newDir;
            }
        }
    }
}
