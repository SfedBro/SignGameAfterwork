using UnityEngine;

public class MultipleSlicesScript : MonoBehaviour
{
    [SerializeField]
    private int damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
        }
    }
}
