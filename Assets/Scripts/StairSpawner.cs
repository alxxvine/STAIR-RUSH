using System.Collections.Generic;
using UnityEngine;

public class StairSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("Префаб ступеньки, который нужно создавать")]
    [SerializeField] private GameObject stairPrefab;
    [Tooltip("Родительский объект для всех ступенек")]
    [SerializeField] private Transform stairsContainer;
    [Tooltip("Горизонтальное расстояние между ступеньками")]
    [SerializeField] private float spawnOffsetX = 2f;
    [Tooltip("Вертикальное расстояние между ступеньками")]
    [SerializeField] private float spawnOffsetY = 1f;

    [Header("Initial Generation")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private int initialStairsUp = 5;
    [SerializeField] private int initialStairsDown = 5;

    [Header("Cleanup")]
    [Tooltip("Позиция по X, левее которой ступеньки будут удаляться")]
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
        // При обычном прыжке спавним одну ступеньку
        SpawnStairs(1);
        CleanupOldStairs();
    }

    private void HandleDoubleJump()
    {
        // При двойном прыжке спавним две ступеньки
        SpawnStairs(2);
        CleanupOldStairs();
    }

    private void SpawnStairs(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (stairPrefab == null || stairsContainer == null) return;

            Transform lastStair = FindRightmostStair();
            if (lastStair == null)
            {
                Debug.LogError("Не найдено ни одной ступеньки для спауна следующей!", this);
                return;
            }

            Vector3 spawnPosition = new Vector3(
                lastStair.position.x + spawnOffsetX,
                lastStair.position.y + spawnOffsetY,
                lastStair.position.z
            );
            Instantiate(stairPrefab, spawnPosition, Quaternion.identity, stairsContainer);
        }
    }

    #region Initial Generation and Cleanup
    private void Start()
    {
        GenerateInitialStairs();
    }

    private void GenerateInitialStairs()
    {
        if (stairPrefab == null || stairsContainer == null || startPoint == null)
        {
            Debug.LogError("Не все поля для начальной генерации назначены в StairSpawner!", this);
            return;
        }

        foreach (Transform child in stairsContainer) Destroy(child.gameObject);
        Instantiate(stairPrefab, startPoint.position, Quaternion.identity, stairsContainer);

        for (int i = 1; i <= initialStairsUp; i++)
        {
            Vector3 pos = new Vector3(startPoint.position.x + spawnOffsetX * i, startPoint.position.y + spawnOffsetY * i, startPoint.position.z);
            Instantiate(stairPrefab, pos, Quaternion.identity, stairsContainer);
        }

        for (int i = 1; i <= initialStairsDown; i++)
        {
            Vector3 pos = new Vector3(startPoint.position.x - spawnOffsetX * i, startPoint.position.y - spawnOffsetY * i, startPoint.position.z);
            Instantiate(stairPrefab, pos, Quaternion.identity, stairsContainer);
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
