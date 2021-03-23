using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class AutoScroll : MonoBehaviour
{
    public bool scrollOnStart = true;

    public float waitBeforeScrollStarts = 0f;
    public float scrollDuration = 3f;

    private void Start()
    {
        if (scrollOnStart)
        {
            StartAutoScroll();
        }
    }
    
    // starts scrolling this component down to the bottom:
    public void StartAutoScroll()
    {
        StartCoroutine(AdjustScrollRect());
    }

    private IEnumerator AdjustScrollRect()
    {
        yield return new WaitForSeconds(waitBeforeScrollStarts);
        yield return new WaitForEndOfFrame();

        var usedTime = 0f;
        var startPosition = GetComponent<ScrollRect>().verticalNormalizedPosition;
        float newPos;

        do
        {
            usedTime += Time.deltaTime;
            var share = scrollDuration <= usedTime ? 1f : usedTime / scrollDuration;
            newPos = Mathf.SmoothStep(startPosition, 0f, share);
            GetComponent<ScrollRect>().verticalNormalizedPosition = newPos;

            yield return null;
            if (this == null)
                // if gameobject already disabled, e.g. page left:
                yield break;
        } while (newPos > 0.0001 && Input.touchCount == 0 && !Input.GetMouseButtonDown(0));

        // when it was not touched scroll to the perfect button:
        if (Input.touchCount == 0 && !Input.GetMouseButtonDown(0))
            GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
    }
}