using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("Объект, за которым должна следовать камера (наш игрок)")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("Насколько плавно камера будет догонять игрока. Меньше значение - более плавно.")]
    [SerializeField] private float smoothSpeed = 0.125f;

    [Tooltip("Вертикальное смещение камеры относительно игрока.")]
    [SerializeField] private float yOffset = 1f;

    private float initialX;
    private float initialZ;

    private void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Не назначен объект для слежения (playerTransform) в инспекторе!", this);
            return;
        }

        initialX = transform.position.x;
        initialZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (playerTransform == null) return;

        // Вычисляем новую желаемую позицию Y для камеры, добавляя смещение
        float desiredY = playerTransform.position.y + yOffset;
        
        Vector3 desiredPosition = new Vector3(initialX, desiredY, initialZ);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
