using UnityEngine;
using TMPro;

public class HighScoreText : MonoBehaviour
{
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private string prefix = "BEST ";

    private void Awake()
    {
        if (highScoreText == null)
        {
            highScoreText = GetComponent<TMP_Text>();
        }
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        int best = StepCounter.GetHighScore();
        if (highScoreText != null)
        {
            highScoreText.text = prefix + best.ToString();
        }
    }
}


