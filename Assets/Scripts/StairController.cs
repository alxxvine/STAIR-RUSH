using UnityEngine;

// Перечисление всех возможных типов ступенек
public enum StairBehaviorType
{
    Normal,
    Broken,
    Coin,
    FakeCoin
}

public class StairController : MonoBehaviour
{
    [Tooltip("Задает поведение этой ступеньки")]
    public StairBehaviorType behaviorType = StairBehaviorType.Normal;

    // Этот метод будет вызываться, когда игрок приземляется на ступеньку
    public void OnPlayerLand()
    {
        switch (behaviorType)
        {
            case StairBehaviorType.Normal:
                // Ничего не делаем, это безопасная ступенька
                Debug.Log("Landed on a Normal stair.");
                break;

            case StairBehaviorType.Broken:
                // Здесь будет логика проигрыша
                Debug.Log("Landed on a BROKEN stair! Game Over.");
                // GameManager.Instance.GameOver(); // Пример вызова
                break;

            case StairBehaviorType.Coin:
                // Здесь будет логика получения монеты
                Debug.Log("Landed on a Coin stair! +1 Coin.");
                // GameManager.Instance.AddCoin(1); // Пример вызова
                // Можно добавить эффект и скрыть монетку
                break;

            case StairBehaviorType.FakeCoin:
                // Выглядит как денежная, но на самом деле ломается
                Debug.Log("Landed on a FAKE COIN stair! It's a trap! Game Over.");
                // GameManager.Instance.GameOver(); // Пример вызова
                break;
        }
    }
}
