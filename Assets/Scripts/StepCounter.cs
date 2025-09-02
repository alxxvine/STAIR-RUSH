using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class StepCounter : MonoBehaviour
{
    [Header("UI (optional)")]
    [Tooltip("TMP text in gameplay to show current steps. Optional.")]
    [SerializeField] private TMP_Text stepsText;

    [Header("Counting")]
    [Tooltip("Ignore the very first valid landing on a stair (starting platform).")]
    [SerializeField] private bool ignoreFirstLanding = true;

    private int currentSteps = 0;
    private int highScore;
    private Rigidbody2D rb;
    private readonly HashSet<int> countedStairIds = new HashSet<int>();
    private bool awardDoubleOnNextLanding = false;

    private const string HighScoreKey = "HighScore_Steps";

    private void Awake()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        rb = GetComponent<Rigidbody2D>();
        if (stepsText == null)
        {
            stepsText = GetComponent<TMP_Text>();
        }
        UpdateStepsUI();
    }

    private void OnEnable()
    {
        PlayerJump.OnSingleJump += HandleSingleJump;
        PlayerJump.OnDoubleJump += HandleDoubleJump;
        // Ensure UI is reset correctly when level scene loads
        ResetRunCounter();
        SceneManager.sceneLoaded += HandleSceneLoaded;
        // Force label update next frame to override any other UI initializers
        StartCoroutine(ForceLabelNextFrame());
    }

    private void OnDisable()
    {
        PlayerJump.OnSingleJump -= HandleSingleJump;
        PlayerJump.OnDoubleJump -= HandleDoubleJump;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetRunCounter();
        StartCoroutine(ForceLabelNextFrame());
    }

    private void HandleSingleJump() { awardDoubleOnNextLanding = false; }
    private void HandleDoubleJump() { awardDoubleOnNextLanding = true; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Attach this script to the Player. Count only landings on objects that have a StairController
        // and where the contact normal points up (player landed from above).
        var stair = collision.gameObject.GetComponent<StairController>();
        if (stair == null) return;
        if (collision.contactCount == 0) return;
        if (collision.contacts[0].normal.y <= 0.75f) return; // be stricter about upward normal

        // Only count when actually landing (moving downward or nearly stopped vertically)
        if (rb != null && rb.linearVelocity.y > 0f) return;

        int stairId = stair.GetInstanceID();
        if (countedStairIds.Contains(stairId)) return; // prevent double-counting the same stair

        // Skip counting the first valid landing (initial platform)
        if (ignoreFirstLanding)
        {
            ignoreFirstLanding = false;
            countedStairIds.Add(stairId);
            return;
        }

        int increment = awardDoubleOnNextLanding ? 2 : 1;
        currentSteps += increment;
        awardDoubleOnNextLanding = false;
        if (currentSteps > highScore)
        {
            highScore = currentSteps;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }
        countedStairIds.Add(stairId);
        UpdateStepsUI();
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public static void ResetHighScore()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
    }

    public void ResetRunCounter()
    {
        currentSteps = 0;
        ignoreFirstLanding = true;
        countedStairIds.Clear();
        awardDoubleOnNextLanding = false;
        UpdateStepsUI();
    }

    private void UpdateStepsUI()
    {
        if (stepsText != null)
        {
            stepsText.text = $"SCORE {currentSteps}";
        }
    }

    private System.Collections.IEnumerator ForceLabelNextFrame()
    {
        yield return null;
        UpdateStepsUI();
    }
}


