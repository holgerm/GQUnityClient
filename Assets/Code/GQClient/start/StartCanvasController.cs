// #define DEBUG_LOG

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.GQClient.start
{
    public class StartCanvasController : MonoBehaviour
    {
        public Image fadeInImage;
        public float waitTimeBeforeFadeIn;
        public float fadeInTime;
        public float waitTimeAfterFadeIn;
       
                
        // Start is called before the first frame update
        private IEnumerator Start()
        {
            if (fadeInImage != null)
            {
                fadeInImage.color = new Color(1f, 1f, 1f, 0f);

                yield return new WaitForSeconds(waitTimeBeforeFadeIn);

                for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeInTime)
                {
                    var newColor = new Color(1, 1, 1, Mathf.Lerp(0.0f, 1.0f, t));
                    fadeInImage.color = newColor;
                    yield return new WaitForEndOfFrame();
                }
            }

            yield return new WaitForSeconds(waitTimeAfterFadeIn);

            SceneManager.LoadScene("Foyer");
        }
    }
}