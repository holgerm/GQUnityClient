// #define DEBUG_LOG

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneController : MonoBehaviour
{
	public Image FadeInImage;
    public float waitTime;
    public float fadeInTime;

    // Start is called before the first frame update
    IEnumerator Start()
    {
#if DEBUG_LOG
        Debug.Log("StartScreen: starting ...");
#endif

        FadeInImage.color = new Color(1f, 1f, 1f, 0f);

#if DEBUG_LOG
        Debug.Log("StartScreen: logo transparency set.");
#endif

        yield return new WaitForSeconds(waitTime);

#if DEBUG_LOG
        Debug.Log("StartScreen: starting to fade in the logo.");
#endif

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeInTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(0.0f, 1.0f, t));
            FadeInImage.color = newColor;
#if DEBUG_LOG
            Debug.Log("StartScreen: FadeIn a = " + newColor.a);
#endif
            yield return null;
        }

#if DEBUG_LOG
        Debug.Log("StartScreen: Ready to start");
#endif

        SceneManager.LoadScene("Foyer");

#if DEBUG_LOG
        Debug.Log("StartScreen: End.");
#endif

    }

}
