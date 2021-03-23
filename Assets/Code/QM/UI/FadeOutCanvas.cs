using System.Collections;
using UnityEngine;

namespace Code.QM.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeOutCanvas : MonoBehaviour
    {
        private static bool _done;

        public float timeBeforeFade = 2;
        public float timeFading = 1.3f;
        public bool onlyOnFirstUse = true;
        public CanvasGroup canvasGroup;

        public void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            if (onlyOnFirstUse && _done)
            {
                yield break;
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            var timeSinceStart = 0f;

            while (timeSinceStart < timeBeforeFade + timeFading)
            {
                timeSinceStart += Time.deltaTime;
                if (canvasGroup != null && timeSinceStart > timeBeforeFade)
                {
                    canvasGroup.alpha =
                        Mathf.Lerp(1f, 0f, (timeSinceStart - timeBeforeFade) / timeFading);
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