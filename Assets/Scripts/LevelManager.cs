using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Single Jump Movement")]
    [Tooltip("На какое расстояние сдвигать контейнер при обычном прыжке (ЛКМ)")]
    [SerializeField] private Vector3 singleJumpMoveDistance = new Vector3(-2f, 0, 0);
    [Tooltip("За какое время контейнер должен переместиться при обычном прыжке")]
    [SerializeField] private float singleMoveDuration = 0.8f;

    [Header("Double Jump Movement")]
    [Tooltip("На какое расстояние сдвигать контейнер при двойном прыжке (ПКМ)")]
    [SerializeField] private Vector3 doubleJumpMoveDistance = new Vector3(-4f, 0, 0);
    [Tooltip("За какое время контейнер должен переместиться при двойном прыжке")]
    [SerializeField] private float doubleMoveDuration = 1.2f; // Потребуется подбор

    [Header("Setup")]
    [Tooltip("Родительский объект, содержащий все ступеньки")]
    [SerializeField] private Transform stairsContainer;

    private bool isMoving = false;

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
        Move(singleJumpMoveDistance, singleMoveDuration);
    }

    private void HandleDoubleJump()
    {
        Move(doubleJumpMoveDistance, doubleMoveDuration);
    }
    
    private void Move(Vector3 distance, float duration)
    {
        if (stairsContainer != null && !isMoving)
        {
            StartCoroutine(MoveContainerSmoothly(distance, duration));
        }
    }

    private IEnumerator MoveContainerSmoothly(Vector3 distance, float duration)
    {
        isMoving = true;
        float elapsedTime = 0f;

        Vector3 startPosition = stairsContainer.position;
        Vector3 endPosition = startPosition + distance;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            stairsContainer.position = Vector3.Lerp(startPosition, endPosition, progress);
            yield return null;
        }

        stairsContainer.position = endPosition;
        isMoving = false;
    }
}
