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

    void Start()
    {
        gameObject.tag = "Projectile";
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            collision.gameObject.GetComponent<Player>().TakeDamage(damage, direction.normalized);
        }
        if (!(collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.gameObject.name.Contains(this.name)))
        {
            Destroy(gameObject);
        }
    }
}
