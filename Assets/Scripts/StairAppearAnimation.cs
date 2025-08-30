using UnityEngine;
using System.Collections;

/// <summary>
/// A self-contained animation script that makes the GameObject it's attached to
/// rise up from a lower position when Play is called.
/// </summary>
public class StairAppearAnimation : MonoBehaviour
{
	public void Play(float appearYOffset, float appearDuration)
	{
		StopAllCoroutines();
		StartCoroutine(RiseUp(appearYOffset, appearDuration));
	}

	private IEnumerator RiseUp(float appearYOffset, float appearDuration)
	{
		Vector3 endPosition = transform.position;
		Vector3 startPosition = endPosition + new Vector3(0, appearYOffset, 0);

		// Temporarily disable physics influence, if any
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		bool hadRb = rb != null;
		RigidbodyType2D prevType = RigidbodyType2D.Dynamic;
		bool prevSimulated = true;
		if (hadRb)
		{
			prevType = rb.bodyType;
			prevSimulated = rb.simulated;
			rb.simulated = false;
		}

		transform.position = startPosition;

		float elapsedTime = 0f;
		float dur = Mathf.Max(appearDuration, 0.0001f);
		while (elapsedTime < dur)
		{
			if (this == null) yield break;
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / dur);
			transform.position = Vector3.Lerp(startPosition, endPosition, t);
			yield return null;
		}

		transform.position = endPosition;

		// Restore physics settings
		if (hadRb)
		{
			rb.bodyType = prevType;
			rb.simulated = prevSimulated;
		}
	}
}
