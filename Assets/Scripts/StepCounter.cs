using UnityEngine;
using TMPro;

public class StepCounter : MonoBehaviour
{
    [Header("UI (optional)")]
    [Tooltip("TMP text in gameplay to show current steps. Optional.")]
    [SerializeField] private TMP_Text stepsText;

    private int currentSteps;
    private int highScore;

    private const string HighScoreKey = "HighScore_Steps";

    private void Awake()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        UpdateStepsUI();
    }

    private void OnEnable()
    {
        PlayerJump.OnSingleJump += HandleAnyJump;
        PlayerJump.OnDoubleJump += HandleAnyJump;
    }

    private void OnDisable()
    {
        PlayerJump.OnSingleJump -= HandleAnyJump;
        PlayerJump.OnDoubleJump -= HandleAnyJump;
    }

    private void HandleAnyJump()
    {
        // We count progress by jumps that successfully land on a stair.
        // Increment will happen on landing via collision callback below.
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Attach this script to the Player. Count only landings on objects that have a StairController
        // and where the contact normal points up (player landed from above).
        if (collision.gameObject.GetComponent<StairController>() == null) return;
        if (collision.contactCount == 0) return;
        if (collision.contacts[0].normal.y <= 0.5f) return;

        currentSteps += 1;
        if (currentSteps > highScore)
        {
            highScore = currentSteps;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }
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
        UpdateStepsUI();
    }

    private void UpdateStepsUI()
    {
        if (stepsText != null)
        {
            stepsText.text = $"Steps: {currentSteps}";
        }
    }
}


