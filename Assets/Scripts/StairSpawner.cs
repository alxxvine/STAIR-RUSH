using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class StairSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("Ссылка на скрипт, который выбирает, какую ступеньку спавнить")]
    [SerializeField] private StairSelector stairSelector;

    [Header("Spawning Settings")]
    [SerializeField] private Transform stairsContainer;
    [SerializeField] private float spawnOffsetX = 2f;
    [SerializeField] private float spawnOffsetY = 1f;

    [Header("Appearance Animation Settings")]
    [Tooltip("How far below their final position stairs will start.")]
    [SerializeField] private float appearYOffset = -15f;
    [Tooltip("How long the 'rise up' animation takes for each stair.")]
    [SerializeField] private float appearDuration = 0.5f;
    [Tooltip("Delay between spawning/appearing initial stairs (seconds), like in StairAnimationManager")] 
    [SerializeField] private float delayBetweenSteps = 0.05f;

    [Header("Initial Generation")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private int initialStairsUp = 5;
    [SerializeField] private int initialStairsDown = 5;

    [Header("Cleanup")]
    [SerializeField] private float destroyPointX = -10f;

    // Cache of the current last right stair to maintain exact spacing
    private Transform lastRightStair;

    private void OnEnable()
    {
        PlayerJump.OnSingleJump += HandleSingleJump;
        PlayerJump.OnDoubleJump += HandleDoubleJump;
    }

    private void OnDisable()
    {
        PlayerJump.OnSingleJump -= HandleSingleJump;
        PlayerJump.OnDoubleJump -= HandleDoubleJump;
    }
    
    private void HandleSingleJump()
    {
        SpawnStairs(1, false);
        CleanupOldStairs();
    }

    private void HandleDoubleJump()
    {
        SpawnStairs(2, false);
        CleanupOldStairs();
    }

    private void SpawnStairs(int count, bool isInitial)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject prefabToSpawn = isInitial 
                ? stairSelector.GetDefaultStairPrefab() 
                : stairSelector.GetRandomStairPrefab();

            if (prefabToSpawn == null || stairsContainer == null) return;

            Transform lastStair = FindRightmostStair();
            if (lastStair == null) return;

            Vector3 spawnPosition = new Vector3(
                lastStair.position.x + spawnOffsetX,
                lastStair.position.y + spawnOffsetY,
                lastStair.position.z
            );
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, stairsContainer);
        }
    }

    #region Initial Generation and Cleanup
    private void Start()
    {
        StartCoroutine(GenerateInitialStairs());
    }

    private IEnumerator GenerateInitialStairs()
    {
        if (stairSelector == null || stairsContainer == null || startPoint == null) yield break;
        
        GameObject defaultPrefab = stairSelector.GetDefaultStairPrefab();
        if (defaultPrefab == null) yield break;
        
        foreach (Transform child in stairsContainer) Destroy(child.gameObject);
        
        // Create the start stair and immediately use IT as the anchor for all subsequent stairs.
        // This ensures consistent spacing from the very beginning.
        GameObject startStairObject = Instantiate(defaultPrefab, startPoint.position, Quaternion.identity, stairsContainer);
        Transform anchorStair = startStairObject.transform;

        // Generate stairs to the left first, using a stable target position
        Vector3 lastPosDown = anchorStair.position;
        for (int i = 0; i < initialStairsDown; i++)
        {
            lastPosDown = new Vector3(
                lastPosDown.x - spawnOffsetX,
                lastPosDown.y - spawnOffsetY,
                lastPosDown.z
            );
            GameObject newStair = Instantiate(stairSelector.GetRandomStairPrefab(), lastPosDown, Quaternion.identity, stairsContainer);
            var animDown = newStair.AddComponent<StairAppearAnimation>();
            animDown.Play(appearYOffset, appearDuration);
            if (delayBetweenSteps > 0f) yield return new WaitForSeconds(delayBetweenSteps);
        }

        // Then generate stairs to the right
        Vector3 lastPosUp = anchorStair.position;
        for (int i = 0; i < initialStairsUp; i++)
        {
            lastPosUp = new Vector3(
                lastPosUp.x + spawnOffsetX,
                lastPosUp.y + spawnOffsetY,
                lastPosUp.z
            );
            GameObject newStair = Instantiate(stairSelector.GetRandomStairPrefab(), lastPosUp, Quaternion.identity, stairsContainer);
            var animUp = newStair.AddComponent<StairAppearAnimation>();
            animUp.Play(appearYOffset, appearDuration);
            if (delayBetweenSteps > 0f) yield return new WaitForSeconds(delayBetweenSteps);
        }
    }
    
    private Transform FindRightmostStair()
    {
        Transform rightmost = null;
        if (stairsContainer.childCount == 0) return null;
        foreach (Transform stair in stairsContainer)
        {
            if (rightmost == null || stair.position.x > rightmost.position.x)
            {
                rightmost = stair;
            }
        }
        return rightmost;
    }

    private void CleanupOldStairs()
    {
        if (stairsContainer == null) return;
        List<Transform> stairsToDestroy = new List<Transform>();
        foreach (Transform stair in stairsContainer)
        {
            if (stair.position.x < destroyPointX)
            {
                stairsToDestroy.Add(stair);
            }
        }
        foreach(var stair in stairsToDestroy) Destroy(stair.gameObject);
    }
    #endregion
}
