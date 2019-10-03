using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
	public Image FadeInImage;
    public float waitTime;
    public float fadeInTime;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Debug.Log("StartScreen: starting ...");

        FadeInImage.color = new Color(1f, 1f, 1f, 0f);

        Debug.Log("StartScreen: logo transparency set.");

        yield return new WaitForSeconds(waitTime);

        Debug.Log("StartScreen: starting to fade in the logo.");

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeInTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(0.0f, 1.0f, t));
            FadeInImage.color = newColor;
            Debug.Log("StartScreen: FadeIn a = " + newColor.a);
            yield return null;
        }

        Debug.Log("StartScreen: Ready to start");

        SceneManager.LoadScene("Foyer");

        Debug.Log("StartScreen: End.");

    }

}
