using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public int damage = 3;

    void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 otherPosition = collision.transform.position;
            Vector2 contactPoint = contact.point;
            Vector2 direction = (otherPosition - contactPoint).normalized;

            collision.gameObject.GetComponent<Player>().TakeDamage(damage, direction);
        }
    }
}
