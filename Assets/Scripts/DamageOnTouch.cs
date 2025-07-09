using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public int damage = 3;

    void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) collision.gameObject.GetComponent<Player>().TakeDamage(damage);
    }
}
