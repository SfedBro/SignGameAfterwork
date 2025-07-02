using UnityEditor;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector2 targetOffset = new(2f, 0);
    public float movingSpeed = 1f;
    
    Vector3 originalPosition;
    Vector3 targetPosition;
    bool movingToTarget = true;
    
    void Start() {
        originalPosition = transform.position;
        targetPosition = transform.position + (Vector3)targetOffset;
    }

    void FixedUpdate() {
        Vector3 currentTarget = movingToTarget ? targetPosition : originalPosition;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, movingSpeed * Time.fixedDeltaTime);
        if (transform.position == currentTarget) movingToTarget = !movingToTarget;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) collision.transform.parent = transform;
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) collision.transform.parent = null;
    }

    void OnDrawGizmosSelected() {
        Vector2 start = EditorApplication.isPlaying ? originalPosition : (Vector2)transform.position, end = start + targetOffset;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(start, 0.25f);
        Gizmos.DrawWireSphere(end, 0.25f);
    }
}
