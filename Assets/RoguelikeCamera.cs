using UnityEngine;
using System.Collections;

public class RoguelikeCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector2 roomOffset = new Vector2(20f, 15f);

    [Header("Movement Settings")]
    [SerializeField] private float smoothSpeed = 2f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Time Scale Settings")]
    [SerializeField] private bool useTimeScale = false;
    [SerializeField] private float timeScaleMultiplier = 0.3f;
    [SerializeField] private float timeScaleTransitionSpeed = 3f;

    private Vector3 initialCameraPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float originalTimeScale;

    void Awake() {
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager) {
            roomOffset.x = levelManager.roomOffsetX;
            roomOffset.y = levelManager.roomOffsetY;
        }
    }

    private void Start()
    {
        initialCameraPosition = transform.position;
        targetPosition = initialCameraPosition;
        originalTimeScale = Time.timeScale;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        CheckPlayerPosition();
    }

    private void CheckPlayerPosition()
    {
        if (isMoving) return;

        Vector3 currentCameraPos = transform.position;
        Vector3 playerPos = playerTransform.position;

        float horizontalDistance = playerPos.x - currentCameraPos.x;
        if (Mathf.Abs(horizontalDistance) > roomOffset.x * 0.5f)
        {
            Vector3 newTargetPos = currentCameraPos;
            newTargetPos.x += horizontalDistance > 0 ? roomOffset.x : -roomOffset.x;
            StartCameraTransition(newTargetPos);
            return;
        }

        float verticalDistance = playerPos.y - currentCameraPos.y;
        if (Mathf.Abs(verticalDistance) > roomOffset.y * 0.5f)
        {
            Vector3 newTargetPos = currentCameraPos;
            newTargetPos.y += verticalDistance > 0 ? roomOffset.y : -roomOffset.y;
            StartCameraTransition(newTargetPos);
        }
    }

    private void StartCameraTransition(Vector3 newTargetPosition)
    {
        if (isMoving) return;

        targetPosition = newTargetPosition;
        StartCoroutine(MoveCameraCoroutine());
    }

    private IEnumerator MoveCameraCoroutine()
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        float duration = 1f / smoothSpeed;

        if (useTimeScale)
        {
            StartCoroutine(ChangeTimeScale(timeScaleMultiplier));
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float normalizedTime = elapsedTime / duration;
            float curveValue = movementCurve.Evaluate(normalizedTime);

            transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

            yield return null;
        }

        transform.position = targetPosition;

        if (useTimeScale)
        {
            StartCoroutine(ChangeTimeScale(originalTimeScale));
        }

        isMoving = false;
    }

    private IEnumerator ChangeTimeScale(float targetTimeScale)
    {
        float startTimeScale = Time.timeScale;
        float elapsedTime = 0f;
        float duration = 1f / timeScaleTransitionSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float normalizedTime = elapsedTime / duration;

            Time.timeScale = Mathf.Lerp(startTimeScale, targetTimeScale, normalizedTime);

            yield return null;
        }

        Time.timeScale = targetTimeScale;
    }

    public void SetRoomOffset(Vector2 offset)
    {
        roomOffset = offset;
    }

    public void SetRoomOffsetX(float offsetX)
    {
        roomOffset.x = offsetX;
    }

    public void SetRoomOffsetY(float offsetY)
    {
        roomOffset.y = offsetY;
    }

    public void SetSmoothSpeed(float speed)
    {
        smoothSpeed = Mathf.Max(0.1f, speed);
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }

    public void MoveCameraTo(Vector3 newPosition, bool smooth = true)
    {
        if (smooth && !isMoving)
        {
            StartCameraTransition(newPosition);
        }
        else
        {
            transform.position = newPosition;
            targetPosition = newPosition;
        }
    }

    public void ResetToInitialPosition(bool smooth = true)
    {
        MoveCameraTo(initialCameraPosition, smooth);
    }

    public void SetUseTimeScale(bool useTimeScale)
    {
        this.useTimeScale = useTimeScale;
    }

    public bool IsMoving()
    {
        return isMoving;
    }



    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;

        Gizmos.color = Color.yellow;
        Vector3 currentPos = transform.position;

        Gizmos.DrawLine(
            new Vector3(currentPos.x - roomOffset.x * 0.5f, currentPos.y - 5f, currentPos.z),
            new Vector3(currentPos.x - roomOffset.x * 0.5f, currentPos.y + 5f, currentPos.z)
        );
        Gizmos.DrawLine(
            new Vector3(currentPos.x + roomOffset.x * 0.5f, currentPos.y - 5f, currentPos.z),
            new Vector3(currentPos.x + roomOffset.x * 0.5f, currentPos.y + 5f, currentPos.z)
        );

        Gizmos.DrawLine(
            new Vector3(currentPos.x - 5f, currentPos.y - roomOffset.y * 0.5f, currentPos.z),
            new Vector3(currentPos.x + 5f, currentPos.y - roomOffset.y * 0.5f, currentPos.z)
        );
        Gizmos.DrawLine(
            new Vector3(currentPos.x - 5f, currentPos.y + roomOffset.y * 0.5f, currentPos.z),
            new Vector3(currentPos.x + 5f, currentPos.y + roomOffset.y * 0.5f, currentPos.z)
        );

        if (isMoving)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetPosition, 0.5f);
        }
    }
}