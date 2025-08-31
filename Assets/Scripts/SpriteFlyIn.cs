using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Animates 2D world objects to fly in from above to their scene-placed positions at scene start.
/// </summary>
[DefaultExecutionOrder(-100)]
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

    [Tooltip("Absolute world Y from which objects will start their flight (X/Z preserved from end position).")]
    public float startYPosition = 15f;

    [Tooltip("Disable physics on objects during animation to prevent jitter, then restore.")]
    public bool disablePhysicsDuringAnimation = true;

    private readonly Dictionary<GameObject, Vector3> cachedEndPositions = new Dictionary<GameObject, Vector3>();

    private void Awake()
    {
        // Cache final positions set in the editor before other Start() methods can move them
        cachedEndPositions.Clear();
        if (objectsToAnimate == null) return;
        foreach (var entry in objectsToAnimate)
        {
            if (entry == null || entry.targetObject == null) continue;
            cachedEndPositions[entry.targetObject] = entry.targetObject.transform.position;
        }
    }

    private void Start()
    {
        if (objectsToAnimate == null) return;
        foreach (var obj in objectsToAnimate)
        {
            if (obj != null && obj.targetObject != null)
            {
                StartCoroutine(AnimateFlyIn(obj));
            }
        }
    }

    private IEnumerator AnimateFlyIn(FlyInObject obj)
    {
        // Determine the final end position from cache (fallback to current if missing)
        Vector3 endPosition;
        if (!cachedEndPositions.TryGetValue(obj.targetObject, out endPosition))
        {
            endPosition = obj.targetObject.transform.position;
        }

        Vector3 startPosition = new Vector3(endPosition.x, startYPosition, endPosition.z);

        // Optionally disable physics to avoid jitter while we animate Transform
        Rigidbody2D rb2d = null;
        Rigidbody rb3d = null;
        bool rb2dHad = false, rb3dHad = false;
        RigidbodyType2D rb2dPrevBodyType = RigidbodyType2D.Dynamic;
        float rb2dPrevGravity = 0f;
        Vector2 rb2dPrevVelocity = Vector2.zero;
        bool rb3dWasKinematic = false;
        Vector3 rb3dPrevVelocity = Vector3.zero;

        if (disablePhysicsDuringAnimation)
        {
            rb2d = obj.targetObject.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2dHad = true;
                rb2dPrevBodyType = rb2d.bodyType;
                rb2dPrevGravity = rb2d.gravityScale;
                rb2dPrevVelocity = rb2d.linearVelocity;
                rb2d.bodyType = RigidbodyType2D.Kinematic;
                rb2d.gravityScale = 0f;
                rb2d.linearVelocity = Vector2.zero;
            }

            rb3d = obj.targetObject.GetComponent<Rigidbody>();
            if (rb3d != null)
            {
                rb3dHad = true;
                rb3dWasKinematic = rb3d.isKinematic;
                rb3dPrevVelocity = rb3d.linearVelocity;
                rb3d.isKinematic = true;
                rb3d.linearVelocity = Vector3.zero;
            }
        }

        // Instantly move the object to its starting position.
        obj.targetObject.transform.position = startPosition;

        // Wait for the specified delay.
        if (obj.delay > 0f)
        {
            yield return new WaitForSeconds(obj.delay);
        }

        // Animate the object flying into place.
        float elapsedTime = 0f;
        while (elapsedTime < obj.flyInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / Mathf.Max(0.0001f, obj.flyInDuration));
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            obj.targetObject.transform.position = Vector3.Lerp(startPosition, endPosition, smooth);
            yield return null;
        }

        // Ensure the object is exactly at its final destination.
        obj.targetObject.transform.position = endPosition;

        // Restore physics
        if (disablePhysicsDuringAnimation)
        {
            if (rb2dHad && rb2d != null)
            {
                rb2d.bodyType = rb2dPrevBodyType;
                rb2d.gravityScale = rb2dPrevGravity;
                rb2d.linearVelocity = rb2dPrevVelocity;
            }
            if (rb3dHad && rb3d != null)
            {
                rb3d.isKinematic = rb3dWasKinematic;
                rb3d.linearVelocity = rb3dPrevVelocity;
            }
        }
    }
}
