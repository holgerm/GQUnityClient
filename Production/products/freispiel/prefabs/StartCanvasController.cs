// #define DEBUG_LOG

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.GQClient.start
{
    public class StartCanvasController : MonoBehaviour
    {
        public Image FadeInImage;
        public float waitTime;
        public float fadeInTime;
        
                
        // Start is called before the first frame update
        private IEnumerator Start()
        {
            Debug.Log("Prefab Started");
            if (FadeInImage != null)
            {
                FadeInImage.color = new Color(1f, 1f, 1f, 0f);

                yield return new WaitForSeconds(waitTime);

                for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeInTime)
                {
                    var newColor = new Color(1, 1, 1, Mathf.Lerp(0.0f, 1.0f, t));
                    FadeInImage.color = newColor;
                    yield return null;
                }
            }

            Debug.Log($"LOADING FOYER @ {Time.frameCount}");
            SceneManager.LoadScene("Foyer");
        }
    }
}