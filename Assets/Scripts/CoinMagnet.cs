using UnityEngine;
using System.Collections;

public class CoinMagnet : MonoBehaviour
{
    [Header("Attraction")]
    [SerializeField] private float baseAttractionForce = 8f;
    [SerializeField] private float detectionRadius = 4f;
    [SerializeField] private float maxAttractionSpeed = 12f;
    [SerializeField] private float attractionCurve = 1.5f;

    [Header("Behavior")]
    [SerializeField] private float triggerDisableDelay = 0.3f;
    [SerializeField] private float attractionStartDelay = 0.8f;
    [SerializeField] private float upwardBoost = 2f;

    private Rigidbody2D rb;
    private GameObject player;
    private bool isAttracted = false;
    private bool canBeAttracted = false;
    private Collider2D triggerCollider;
    private Vector3 lastPlayerPosition;
    private bool hasAppliedInitialBoost = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        rb.linearDamping = 0.5f;
        rb.angularDamping = 2f;

        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
            StartCoroutine(ActivateTriggerAfterDelay());
        }

        StartCoroutine(EnableAttractionAfterDelay());
    }

    void FixedUpdate()
    {
        if (player == null || !canBeAttracted) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        bool shouldAttract = distance <= detectionRadius;

        if (shouldAttract)
        {
            if (!isAttracted)
            {
                isAttracted = true;
                lastPlayerPosition = player.transform.position;
                hasAppliedInitialBoost = false;

                rb.linearVelocity *= 0.7f;
            }

            ApplyAttractionForce(distance);
        }
        else if (isAttracted)
        {
            isAttracted = false;
        }
    }

    private void ApplyAttractionForce(float distance)
    {
        Vector3 playerVelocity = Vector3.zero;
        if (lastPlayerPosition != Vector3.zero)
        {
            playerVelocity = (player.transform.position - lastPlayerPosition) / Time.fixedDeltaTime;
        }
        Vector3 predictedPlayerPos = player.transform.position + playerVelocity * 0.15f;
        lastPlayerPosition = player.transform.position;

        Vector2 directionToPlayer = (predictedPlayerPos - transform.position).normalized;

        float normalizedDistance = Mathf.Clamp01(distance / detectionRadius);
        float attractionMultiplier = Mathf.Pow(1f - normalizedDistance, attractionCurve);

        float timeMultiplier = Mathf.Min(2f, 1f + (Time.time - Time.fixedTime) * 0.5f);
        float finalForce = baseAttractionForce * attractionMultiplier * timeMultiplier;

        if (!hasAppliedInitialBoost && rb.linearVelocity.y < 1f)
        {
            rb.AddForce(Vector2.up * upwardBoost, ForceMode2D.Impulse);
            hasAppliedInitialBoost = true;
        }

        rb.AddForce(directionToPlayer * finalForce, ForceMode2D.Force);

        if (rb.linearVelocity.magnitude > maxAttractionSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxAttractionSpeed;
        }

        if (distance < detectionRadius * 0.25f)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, directionToPlayer * maxAttractionSpeed, Time.fixedDeltaTime * 8f);
        }
    }

    private IEnumerator EnableAttractionAfterDelay()
    {
        yield return new WaitForSeconds(attractionStartDelay);
        canBeAttracted = true;
    }

    private IEnumerator ActivateTriggerAfterDelay()
    {
        yield return new WaitForSeconds(triggerDisableDelay);

        if (triggerCollider != null)
        {
            triggerCollider.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = canBeAttracted ? Color.green : Color.gray;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (canBeAttracted)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius * 0.25f);
        }
    }
}