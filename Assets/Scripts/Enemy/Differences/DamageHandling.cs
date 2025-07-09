using UnityEngine;

public class DamageHandling : MonoBehaviour
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private EnemyInteractionCharacteristics stats;

    private void Start()
    {
        if (stats != null)
        {
            damage = stats.damage;
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Player>().TakeDamage(damage);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Player>().TakeDamage(damage);
    }
}
