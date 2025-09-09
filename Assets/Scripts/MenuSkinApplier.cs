using UnityEngine;
using System.Collections.Generic;

public class MenuSkinApplier : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private List<RewardSkin> rewardSkins = new List<RewardSkin>();
    private const string LastUnlockedKey = "SkinReward_LastUnlockedStep";

    [System.Serializable]
    public struct RewardSkin
    {
        public int stepsToUnlock;
        public Sprite newSkin;
    }

    private void Awake()
    {
        int lastUnlocked = PlayerPrefs.GetInt(LastUnlockedKey, int.MinValue);
        Sprite lastSkin = null;
        for (int i = 0; i < rewardSkins.Count; i++)
        {
            if (lastUnlocked >= rewardSkins[i].stepsToUnlock && rewardSkins[i].newSkin != null)
            {
                lastSkin = rewardSkins[i].newSkin;
            }
        }
        Debug.Log($"MenuSkinApplier: lastUnlocked={lastUnlocked}, lastSkin={(lastSkin ? lastSkin.name : "null")}");
        if (playerRenderer != null && lastSkin != null)
        {
            playerRenderer.sprite = lastSkin;
        }
        else if (playerRenderer == null)
        {
            Debug.LogWarning("MenuSkinApplier: playerRenderer не назначен!");
        }
        else if (lastSkin == null)
        {
            Debug.LogWarning("MenuSkinApplier: не найден подходящий скин для lastUnlocked=" + lastUnlocked);
        }
    }
}
