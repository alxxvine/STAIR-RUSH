using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles the behavior of a trap stair that falls when the player lands on it.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class TrapStair : MonoBehaviour
{
    [Tooltip("The name of the scene to load when the player loses.")]
    public string menuSceneName = "Menu";

    [Tooltip("Time in seconds to wait before loading the game over scene after the fall starts.")]
    public float delayBeforeLoad = 2f;

    [Tooltip("Controls how fast the stair falls. Higher values mean faster fall.")]
    public float fallGravityScale = 1f;

    private Rigidbody2D rb;
    private bool isTriggered = false;
    
    // We will create this script next
    private StairAnimationManager animationManager;

    void Start()
    {
        // The Rigidbody2D should be kinematic initially so it doesn't fall.
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        
        // Find the manager in the scene
        animationManager = FindFirstObjectByType<StairAnimationManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player has landed on the stair and it hasn't been triggered yet.
        if (!isTriggered && collision.gameObject.CompareTag("Player"))
        {
            // Check if the player is landing from above
            if (collision.contacts[0].normal.y < -0.5)
            {
                TriggerFall(collision.gameObject);
            }
        }
    }

    private void TriggerFall(GameObject player)
    {
        isTriggered = true;
        
        // Stop the camera from following the player
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.StopFollowing();
        }

        // Disable the player's controls to prevent jitter.
        PlayerJump playerJumpScript = player.GetComponent<PlayerJump>();
        if (playerJumpScript != null)
        {
            playerJumpScript.enabled = false;
        }

        // Make the player a child of the stair so they fall together.
        // player.transform.SetParent(transform);
        
        // Trigger the collapse of all other stairs
        if (animationManager != null)
        {
            // We will implement this method in the next script
            animationManager.CollapseAllStairs();
        }
        else
        {
            Debug.LogWarning("StairAnimationManager not found in the scene.");
        }

        // Enable physics for this stair to fall.
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravityScale; // Use the configurable gravity scale

        // Start the countdown to load the game over scene.
        StartCoroutine(LoadGameOverScene());
    }

    private IEnumerator LoadGameOverScene()
    {
        yield return new WaitForSeconds(delayBeforeLoad);

        // Set the flag in the GameManager before loading the menu
        /* if (GameManager.Instance != null)
        {
            GameManager.Instance.SetReturnedFromGameOver();
        } */

        SceneManager.LoadScene(menuSceneName);
    }
}
