using System.Collections;
using UnityEngine;

public class SingleSlashScript : MonoBehaviour
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float lifeTime;
    [SerializeField]
    private float growMultiplier;
    [SerializeField]
    private Vector2 flyingDirection;
    [SerializeField]
    private string neededElement;
    private void Start()
    {
        StartCoroutine(DestroyAfterTime(lifeTime));
    }
    private void Update()
    {
        if (Time.timeScale > 0f)
        {
            MoveSlash();
            ChangeSize();
        }
    }
    private void MoveSlash()
    {
        transform.Translate(flyingDirection.normalized * speed * Time.deltaTime, Space.Self);
    }
    private void ChangeSize()
    {
        transform.localScale += new Vector3(1, 1) * growMultiplier * Time.deltaTime;
    }
    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<SpellCast>())
        {
            Debug.Log("Collision");
            string element = collision.gameObject.GetComponent<SpellCast>().LastSpellUsed().MainElement;
            if (element != neededElement)
            {
                if (collision.gameObject.GetComponent<Player>())
                {
                    collision.gameObject.GetComponent<Player>().TakeDamage(damage);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SpellCast>())
        {
            bool shouldTakeDamage = true;
            if (collision.gameObject.GetComponent<SpellCast>().LastSpellUsed() != null)
            {
                string element = collision.gameObject.GetComponent<SpellCast>().LastSpellUsed().MainElement;
                if (element == neededElement)
                {
                    shouldTakeDamage = false;
                }
            }
            if (shouldTakeDamage && collision.gameObject.GetComponent<Player>())
            {
                collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            }
        }
    }
}
