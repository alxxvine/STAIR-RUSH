using System.Collections.Generic;
using UnityEngine;

// Этот класс мы уже создавали, он нужен для удобства в инспекторе.
// Повторяем его здесь, так как он теперь относится к этому скрипту.
[System.Serializable]
public class StairType
{
    public string name; // Просто для удобства в инспекторе
    public GameObject prefab;
    [Range(0f, 100f)]
    public float spawnChance;
    [Tooltip("Если true, этот тип ступеньки не может появиться два раза подряд.")]
    public bool preventImmediateRepeat = false;
}

public class StairSelector : MonoBehaviour
{
    [Header("Stair Variations")]
    [Tooltip("Список всех возможных типов ступенек и их шансы появления")]
    [SerializeField] private List<StairType> stairTypes;

    private float totalSpawnChance;
    private StairType lastSelectedType = null; // "Память" селектора

    private void Awake()
    {
        // Считаем сумму всех шансов один раз при старте для оптимизации
        totalSpawnChance = 0f;
        foreach (var type in stairTypes)
        {
            totalSpawnChance += type.spawnChance;
        }
    }

    /// <summary>
    /// Возвращает случайный префаб ступеньки на основе заданных шансов.
    /// </summary>
    public GameObject GetRandomStairPrefab()
    {
        // 1. Создаем временный список доступных для спауна ступенек
        List<StairType> availableTypes = new List<StairType>();
        float currentTotalChance = 0f;

        foreach (var type in stairTypes)
        {
            // Проверяем, нужно ли исключить этот тип
            if (lastSelectedType != null && lastSelectedType.preventImmediateRepeat && type == lastSelectedType)
            {
                continue; // Пропускаем этот тип, он запрещен для повторения
            }
            availableTypes.Add(type);
            currentTotalChance += type.spawnChance;
        }

        // Если вдруг все отфильтровались, спавним ступеньку по умолчанию
        if (availableTypes.Count == 0)
        {
            Debug.LogWarning("Не нашлось доступных ступенек для спауна (все были отфильтрованы). Спавним обычную.");
            return GetDefaultStairPrefab();
        }

        // 2. Проводим лотерею среди доступных ступенек
        float randomValue = Random.Range(0, currentTotalChance);
        float cumulativeChance = 0f;

        foreach (var type in availableTypes)
        {
            cumulativeChance += type.spawnChance;
            if (randomValue <= cumulativeChance)
            {
                lastSelectedType = type; // Запоминаем, что выбрали
                return type.prefab;
            }
        }
        
        lastSelectedType = availableTypes[0]; // Запасной вариант
        return availableTypes[0].prefab;
    }

    /// <summary>
    /// Возвращает префаб ступеньки по умолчанию (первый в списке).
    /// Используется для начальной генерации.
    /// </summary>
    public GameObject GetDefaultStairPrefab()
    {
        if (stairTypes != null && stairTypes.Count > 0)
        {
            lastSelectedType = stairTypes[0]; // Запоминаем, что использовали обычную ступеньку
            return stairTypes[0].prefab;
        }
        
        Debug.LogError("В StairSelector не добавлено ни одного типа ступенек!", this);
        return null;
    }
}
