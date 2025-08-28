using System.Collections.Generic;
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

    [Header("Initial Generation")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private int initialStairsUp = 5;
    [SerializeField] private int initialStairsDown = 5;

    [Header("Cleanup")]
    [SerializeField] private float destroyPointX = -10f;

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
        GenerateInitialStairs();
    }

    private void GenerateInitialStairs()
    {
        if (stairSelector == null || stairsContainer == null || startPoint == null) return;

        // --- Улучшенная логика ---
        // 1. Получаем безопасную ступеньку для старта
        GameObject defaultPrefab = stairSelector.GetDefaultStairPrefab();
        if (defaultPrefab == null) return;
        
        // 2. Очищаем сцену
        foreach (Transform child in stairsContainer) Destroy(child.gameObject);
        
        // 3. Создаем стартовую безопасную ступеньку
        Instantiate(defaultPrefab, startPoint.position, Quaternion.identity, stairsContainer);

        // 4. Генерируем ступеньки вверх-вправо, но уже СЛУЧАЙНЫЕ
        Transform lastStairUp = startPoint;
        for (int i = 0; i < initialStairsUp; i++)
        {
            GameObject randomPrefab = stairSelector.GetRandomStairPrefab();
            Vector3 pos = new Vector3(
                lastStairUp.position.x + spawnOffsetX, 
                lastStairUp.position.y + spawnOffsetY, 
                lastStairUp.position.z
            );
            lastStairUp = Instantiate(randomPrefab, pos, Quaternion.identity, stairsContainer).transform;
        }

        // 5. Генерируем ступеньки вниз-влево, тоже СЛУЧАЙНЫЕ
        Transform lastStairDown = startPoint;
        for (int i = 0; i < initialStairsDown; i++)
        {
            GameObject randomPrefab = stairSelector.GetRandomStairPrefab();
            Vector3 pos = new Vector3(
                lastStairDown.position.x - spawnOffsetX, 
                lastStairDown.position.y - spawnOffsetY, 
                lastStairDown.position.z
            );
            lastStairDown = Instantiate(randomPrefab, pos, Quaternion.identity, stairsContainer).transform;
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
