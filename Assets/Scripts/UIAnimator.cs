using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIAnimator : MonoBehaviour
{
    [System.Serializable]
    public class AnimatedObject
    {
        public GameObject targetObject;
        public float fallDistance = 200f;
        public float fadeDuration = 1f;
        public float delay = 0f;
    }

    public List<AnimatedObject> animatedObjects;

    private List<Coroutine> runningCoroutines = new List<Coroutine>();

    public void StartAnimations()
    {
        foreach (var obj in animatedObjects)
        {
            if (obj.targetObject != null)
            {
                Coroutine coroutine = StartCoroutine(AnimateObject(obj));
                runningCoroutines.Add(coroutine);
            }
        }
    }

    private IEnumerator AnimateObject(AnimatedObject obj)
    {
        yield return new WaitForSeconds(obj.delay);

        CanvasGroup canvasGroup = obj.targetObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.targetObject.AddComponent<CanvasGroup>();
        }

        RectTransform rectTransform = obj.targetObject.GetComponent<RectTransform>();
        Vector3 originalPosition = rectTransform.anchoredPosition;
        Vector3 targetPosition = originalPosition - new Vector3(0, obj.fallDistance, 0);

        float elapsedTime = 0f;

        while (elapsedTime < obj.fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / obj.fadeDuration;

            // Fade out
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

            // Fall down
            rectTransform.anchoredPosition = Vector3.Lerp(originalPosition, targetPosition, progress);
            
            yield return null;
        }

        // Ensure final state is set
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = targetPosition;
    }
}
