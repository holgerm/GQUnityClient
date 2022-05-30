// #define DEBUG_LOG

using Code.GQClient.Err;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.GQClient.start
{
    public class StartSceneController : MonoBehaviour
    {
        public string canvasControllerName = "StartCanvas";
        private void Awake()
        {
            var _ = Migration.Migration.CurrentAppVersion; // just update it
        }

        private void Start()
        {
            var startCanvasPrefab = Resources.Load<GameObject>($"ImportedPackage/prefabs/{canvasControllerName}");
            if (startCanvasPrefab == null)
            {
                // skip start canvas since it was not defined
                SceneManager.LoadScene("Foyer");
            }
            else
            {
                var startCanvas = Instantiate(startCanvasPrefab);
                startCanvas.SetActive(true);
            }
        }
    }
}