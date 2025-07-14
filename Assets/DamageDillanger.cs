using UnityEngine;

public class DamageDillanger : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            Vector2 direction = (other.transform.position - transform.position).normalized;
            enemy.TakeDamage(damage, direction);
            Destroy(gameObject);
        }
    }
}
