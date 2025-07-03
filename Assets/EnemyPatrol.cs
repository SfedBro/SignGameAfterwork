using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolDistance = 3f; // Дистанция патрулирования (влево и вправо от начальной точки)
    [SerializeField] private float moveSpeed = 2f; // Скорость движения врага

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private bool movingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position; // Запоминаем начальную позицию
        Flip();
    }

    private void FixedUpdate()
    {
        Patrol();
    }

    private void Patrol()
    {
        // Определяем целевую позицию
        float targetX = movingRight ? startPosition.x + patrolDistance : startPosition.x - patrolDistance;

        // Двигаем врага
        Vector2 targetVelocity = new Vector2(movingRight ? moveSpeed : -moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = targetVelocity;

        // Проверяем, достиг ли враг границы патруля
        if (movingRight && transform.position.x >= startPosition.x + patrolDistance)
        {
            movingRight = false;
            Flip();
        }
        else if (!movingRight && transform.position.x <= startPosition.x - patrolDistance)
        {
            movingRight = true;
            Flip();
        }
    }

    private void Flip()
    {
        // Переворачиваем спрайт врага по оси X
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}