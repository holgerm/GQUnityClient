// #define DEBUG_LOG

using System.Collections;
using Code.GQClient.Err;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.GQClient.start
{
    public class StartCanvasController : MonoBehaviour
    {
        public float waitTimeBeforeFadeIn;
        public float fadeInTime;
        public float waitTimeAfterFadeIn;

        public Image BG;
        public Image FG;
        public Image FadeIn;


        private void Awake()
        {
            var _ = Migration.Migration.CurrentAppVersion; // just update it
        }

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            loadImage(BG, "BG/ImageBG", "SplashScreenBG");
            loadImage(FG, "BG/ImageFG", "SplashScreenFG");
            loadImage(FadeIn, "BG/FadeInBG", "SplashScreenFadeIn");

            if (FadeIn)
            {
                FadeIn.color = new Color(1f, 1f, 1f, 0f);

                yield return new WaitForSeconds(waitTimeBeforeFadeIn);

                for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeInTime)
                {
                    var newColor = new Color(1, 1, 1, Mathf.Lerp(0.0f, 1.0f, t));
                    FadeIn.color = newColor;
                    yield return new WaitForEndOfFrame();
                }
            }

            yield return new WaitForSeconds(waitTimeAfterFadeIn);

            SceneManager.LoadScene("Foyer");
        }

        private void loadImage(Image image, string path, string resourceName)
        {
            if (!image)
            {
                image = transform.Find(path)?.GetComponent<Image>();
            }

            if (!image)
            {
                Log.WarnDeveloper("StartScene Image missing for path: " + path);
            }
            else
            {
                if (!image.sprite)
                {
                    image.sprite = Resources.Load<Sprite>(resourceName);
                    image.preserveAspect = true;
                }
            }
        }
    }
}