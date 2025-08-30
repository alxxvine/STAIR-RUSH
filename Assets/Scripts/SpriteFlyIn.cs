using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Animates 2D objects to fly into the screen from above when the scene starts.
/// Works with any object that has a Transform component (e.g., SpriteRenderers).
/// </summary>
public class SpriteFlyIn : MonoBehaviour
{
    [System.Serializable]
    public class FlyInObject
    {
        [Tooltip("The object to animate.")]
        public GameObject targetObject;

        [Tooltip("How long the flight animation takes in seconds.")]
        public float flyInDuration = 1f;

        [Tooltip("Delay in seconds before this object starts flying in.")]
        public float delay = 0f;
    }

    [Header("Animation Settings")]
    [Tooltip("The list of objects to animate.")]
    public List<FlyInObject> objectsToAnimate;

    [Tooltip("The fixed Y position from which all objects will start their flight.")]
    public float startYPosition = 15f;

    void Start()
    {
        foreach (var obj in objectsToAnimate)
        {
            if (obj.targetObject != null)
            {
                StartCoroutine(AnimateFlyIn(obj));
            }
        }
    }

    private IEnumerator AnimateFlyIn(FlyInObject obj)
    {
        // Store the final position where the object should end up.
        Vector3 endPosition = obj.targetObject.transform.position;

        // Calculate the starting position using the fixed startYPosition.
        Vector3 startPosition = new Vector3(endPosition.x, startYPosition, endPosition.z);

        // Instantly move the object to its starting position.
        obj.targetObject.transform.position = startPosition;
        
        // Wait for the specified delay.
        yield return new WaitForSeconds(obj.delay);

        // Animate the object flying into place.
        float elapsedTime = 0f;
        while (elapsedTime < obj.flyInDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / obj.flyInDuration;
            // Optional: Add easing for a smoother effect, e.g., progress = Mathf.SmoothStep(0, 1, progress);
            obj.targetObject.transform.position = Vector3.Lerp(startPosition, endPosition, progress);
            yield return null;
        }

        // Ensure the object is exactly at its final destination.
        obj.targetObject.transform.position = endPosition;
    }
}
