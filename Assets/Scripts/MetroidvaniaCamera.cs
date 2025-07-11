using UnityEngine;

public class MetroidvaniaCamera : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime = 0.125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lookAheadDistance = 2f;
    [SerializeField] private float lookAheadSpeed = 0.1f;

    [Header("Camera Bounds")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private Transform minBoundsPoint;
    [SerializeField] private Transform maxBoundsPoint;

    [Header("Mouse Control")]
    [SerializeField] private float mouseRadius = 10f;
    [SerializeField] private float mouseOffsetSpeed = 50f;
    [SerializeField] private float returnToOffsetSpeed = 2f; // Скорость возвращения к обычному offset

    private Camera cam;
    private Vector3 lookAheadOffset;
    private Vector3 targetPosition;
    private bool isUsingMouseControl = false;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (player == null)
        {
            enabled = false;
        }

        targetPosition = player.position + offset;
        transform.position = targetPosition;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Проверяем состояние правой кнопки мыши
        if (Input.GetMouseButton(1))
        {
            isUsingMouseControl = true;
            Vector3 mousePos = GetMouseWorldPosition();
            Vector3 direction = (mousePos - transform.position).normalized * mouseRadius;
            lookAheadOffset = Vector3.Lerp(lookAheadOffset, direction, mouseOffsetSpeed * Time.fixedDeltaTime);
        }
        else
        {
            if (isUsingMouseControl)
            {
                // Быстро возвращаемся к стандартному look ahead
                isUsingMouseControl = false;
            }
            UpdateLookAhead();
        }

        targetPosition = player.position + offset + lookAheadOffset;

        Vector3 desiredPosition = Vector3.Lerp(transform.position, targetPosition, smoothTime * Time.fixedDeltaTime * 60f);

        desiredPosition.z = transform.position.z;

        transform.position = desiredPosition;

        if (useBounds)
        {
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBoundsPoint.position.x + camWidth, maxBoundsPoint.position.x - camWidth);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBoundsPoint.position.y + camHeight, maxBoundsPoint.position.y - camHeight);
            transform.position = clampedPosition;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -cam.transform.position.z;
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;
        return worldPos;
    }

    private void UpdateLookAhead()
    {
        Vector3 playerVelocity = player.GetComponent<Rigidbody2D>().linearVelocity;
        Vector3 targetLookAhead = Vector3.zero;

        if (playerVelocity.magnitude > 0.1f)
        {
            targetLookAhead = new Vector3(playerVelocity.x, playerVelocity.y, 0).normalized * lookAheadDistance;
        }

        // Используем более быструю скорость возвращения если мы только что отпустили мышь
        float currentSpeed = isUsingMouseControl ? lookAheadSpeed : returnToOffsetSpeed;
        lookAheadOffset = Vector3.Lerp(lookAheadOffset, targetLookAhead, currentSpeed * Time.fixedDeltaTime);
    }

    public void SetCameraBounds(Vector2 newMinBounds, Vector2 newMaxBounds)
    {
        minBoundsPoint.position = newMinBounds;
        maxBoundsPoint.position = newMaxBounds;
    }

    public void SnapToPlayer()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z) + offset;
            targetPosition = transform.position;
        }
    }
}