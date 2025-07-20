using UnityEngine;

public class SimpleDamagingScript : MonoBehaviour
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private float speed;
    public int Damage
    {
        get
        {
            return damage;
        }
        set
        {
            if (value < 0)
            {
                damage = 0;
            }
            else
            {
                damage = value;
            }
        }
    }
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
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
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
