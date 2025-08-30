using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the collapse animation for all stairs in the scene.
/// </summary>
public class StairAnimationManager : MonoBehaviour
{
    [Header("General Settings")]
    [Tooltip("The tag assigned to all stair prefabs.")]
    public string stairTag = "Stair";

    [Header("Collapse Animation")]
    [Tooltip("The delay in seconds between each stair animation step for collapsing.")]
    public float delayBetweenSteps = 0.05f;
    [Tooltip("If true, stairs will collapse from right to left. If false, from left to right.")]
    public bool collapseFromRightToLeft = false;

    private bool hasCollapsed = false;

    /// <summary>
    /// Starts the process of collapsing all stairs. Called by TrapStair.
    /// </summary>
    public void CollapseAllStairs()
    {
        if (hasCollapsed) return;
        hasCollapsed = true;

        List<GameObject> stairs = new List<GameObject>(GameObject.FindGameObjectsWithTag(stairTag));

        // Note: This won't find the "StartStair" if it has a different tag.
        // We should address this if we re-add that feature.

        if (collapseFromRightToLeft)
        {
            stairs = stairs.OrderByDescending(stair => stair.transform.position.x).ToList();
        }
        else
        {
            stairs = stairs.OrderBy(stair => stair.transform.position.x).ToList();
        }

        StartCoroutine(CollapseSequence(stairs));
    }

    private IEnumerator CollapseSequence(List<GameObject> stairs)
    {
        foreach (GameObject stair in stairs)
        {
            if (stair != null)
            {
                Rigidbody2D rb = stair.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    rb = stair.AddComponent<Rigidbody2D>();
                }

                if (rb.bodyType == RigidbodyType2D.Kinematic)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.gravityScale = 1f;
                    yield return new WaitForSeconds(delayBetweenSteps);
                }
            }
        }
    }
}
