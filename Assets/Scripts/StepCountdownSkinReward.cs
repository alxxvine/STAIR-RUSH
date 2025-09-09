using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class StepCountdownSkinReward : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text stepsText;
    [Header("Игрок и награды")]
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private List<RewardSkin> rewardSkins = new List<RewardSkin>();

    [System.Serializable]
    public struct RewardSkin
    {
        public int stepsToUnlock;
        public Sprite newSkin;
        [HideInInspector] public bool triggered;
    }

    private void OnEnable()
    {
        StepCounter.OnStepsChanged += OnStepsChanged;
        OnStepsChanged(StepCounter.CurrentSteps); // обновить UI сразу
    }

    private void OnDisable()
    {
        StepCounter.OnStepsChanged -= OnStepsChanged;
    }

    private void OnStepsChanged(int currentSteps)
    {
        int minStepsLeft = int.MaxValue;
        foreach (var reward in rewardSkins)
        {
            int stepsLeft = reward.stepsToUnlock - currentSteps;
            if (stepsLeft < minStepsLeft && stepsLeft >= 0)
                minStepsLeft = stepsLeft;
        }
        if (stepsText != null && minStepsLeft != int.MaxValue)
            stepsText.text = minStepsLeft.ToString();
        else if (stepsText != null)
            stepsText.text = "0";

        for (int i = 0; i < rewardSkins.Count; i++)
        {
            if (!rewardSkins[i].triggered && currentSteps >= rewardSkins[i].stepsToUnlock)
            {
                if (playerRenderer != null && rewardSkins[i].newSkin != null)
                    playerRenderer.sprite = rewardSkins[i].newSkin;
                rewardSkins[i] = MarkTriggered(rewardSkins[i]);
            }
        }
    }

    private RewardSkin MarkTriggered(RewardSkin skin)
    {
        skin.triggered = true;
        return skin;
    }
}
