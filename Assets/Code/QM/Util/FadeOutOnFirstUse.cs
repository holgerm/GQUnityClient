using System;
using System.Collections;
using UnityEngine;

namespace Code.QM.Util
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeOutOnFirstUse : MonoBehaviour
    {
        private static bool _done;

        public float timeToShow = 2;
        public float timeToFade = 1.3f;
        public CanvasGroup canvasGroup;

        public void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            if (_done)
            {
                yield break;
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            var timeSinceStart = 0f;

            while (timeSinceStart < timeToShow + timeToFade)
            {
                timeSinceStart += Time.deltaTime;
                if (canvasGroup != null && timeSinceStart > timeToShow)
                {
                    canvasGroup.alpha =
                        Mathf.Lerp(1f, 0f, (timeSinceStart - timeToShow) / timeToFade);
                }

                yield return null;
            }

            gameObject.SetActive(false); 
            _done = true;
        }

        public void OnDisable()
        {
            if (canvasGroup != null)
                canvasGroup.alpha = 1;
        }
    }
}