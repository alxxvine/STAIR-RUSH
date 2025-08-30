using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles fading in UI elements every time the menu scene starts.
/// </summary>
public class MenuFadeIn : MonoBehaviour
{
    [System.Serializable]
    public class FadeInObject
    {
        public GameObject targetObject;
        public float fadeDuration = 1f;
        public float delay = 0f;
    }

    [Tooltip("The list of UI elements to fade in.")]
    public List<FadeInObject> fadeInObjects;

    void Start()
    {
        // Set all objects to be invisible at the start.
        foreach (var obj in fadeInObjects)
        {
            if (obj.targetObject == null) continue;

            CanvasGroup cg = obj.targetObject.GetComponent<CanvasGroup>();
            if (cg == null) cg = obj.targetObject.AddComponent<CanvasGroup>();
            
            cg.alpha = 0;
        }

        // Start the fade-in process for all objects.
        StartCoroutine(FadeInAll());
    }

    private IEnumerator FadeInAll()
    {
        foreach (var obj in fadeInObjects)
        {
            if (obj.targetObject != null)
            {
                StartCoroutine(FadeInSingle(obj));
            }
        }
        yield return null;
    }

    private IEnumerator FadeInSingle(FadeInObject obj)
    {
        yield return new WaitForSeconds(obj.delay);

        CanvasGroup cg = obj.targetObject.GetComponent<CanvasGroup>();
        float elapsedTime = 0f;

        while (elapsedTime < obj.fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsedTime / obj.fadeDuration);
            yield return null;
        }

        // Ensure it's fully visible at the end.
        cg.alpha = 1f;
    }
}
