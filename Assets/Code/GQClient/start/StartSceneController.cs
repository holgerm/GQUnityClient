// #define DEBUG_LOG

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.GQClient.start
{
    public class StartSceneController : MonoBehaviour
    {
        private void Start()
        {
            var startCanvasPrefab = Resources.Load<GameObject>("prefabs/StartCanvas");
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