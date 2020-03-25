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
            Debug.Log($"START @{Time.frameCount}");
            var startCanvas = Instantiate(Resources.Load<GameObject>("prefabs/StartCanvas"));
            startCanvas.SetActive(true);
        }
    }
}