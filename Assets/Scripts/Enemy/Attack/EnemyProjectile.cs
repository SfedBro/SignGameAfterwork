using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
//[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private float speed;
    [SerializeField]
    private Transform target;
    public int Damage
    {
        set
        {
            if (value >= 0)
            {
                damage = value;
            }
        }
    }
    public float Speed
    {
        set
        {
            if (value > 0)
            {
                speed = value;
            }
        }
    }
    public Transform Target
    {
        set
        {
            if (value != null)
            {
                target = value;
            }
        }
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage, transform.forward);
        }
        if (!(collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.gameObject.name.Contains(this.name)))
        {
            Destroy(gameObject);
        }
    }
}
