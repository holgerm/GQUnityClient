// #define DEBUG_LOG

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.GQClient.start
{
    public class StartSceneController : MonoBehaviour
    {
        private void Awake()
        {
            var _ = Migration.Migration.CurrentAppVersion; // just update it
        }

        private void Start()
        {
            var startCanvasPrefab = Resources.Load<GameObject>("ImportedPackage/prefabs/StartCanvas");
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