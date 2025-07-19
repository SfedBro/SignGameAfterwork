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
        transform.Translate(flyingDirection.normalized * speed * Time.deltaTime, Space.World);
    }
    private void ChangeSize()
    {
        transform.localScale *= (1f + growMultiplier * Time.deltaTime);
    }
    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
