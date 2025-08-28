using System;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Single Jump (LMB)")]
    [Tooltip("Сила обычного прыжка.")]
    [SerializeField] private float singleJumpForce = 5.2f;

    [Header("Double Jump (RMB)")]
    [Tooltip("Сила двойного прыжка (через ступеньку).")]
    [SerializeField] private float doubleJumpForce = 7.5f; // Потребуется подбор

    public static event Action OnSingleJump;
    public static event Action OnDoubleJump;

    private Rigidbody2D rb;
    private bool canJump = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("На игроке не найден компонент Rigidbody2D!", this);
        }
    }

    private void Update()
    {
        if (canJump)
        {
            if (Input.GetMouseButtonDown(0)) // ЛКМ
            {
                Jump(singleJumpForce, OnSingleJump);
            }
            else if (Input.GetMouseButtonDown(1)) // ПКМ
            {
                Jump(doubleJumpForce, OnDoubleJump);
            }
        }
    }

    private void Jump(float force, Action jumpEvent)
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            canJump = false;
            jumpEvent?.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        canJump = true;
    }
}
