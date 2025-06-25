using UnityEngine;
using System.Collections;

public class CoilSpell : MonoBehaviour
{
    public float duration = 0.1f;
    public float coilRange = 4f;
    public int range;
    public bool lookRight;
    public string effectType;

    private void Start()
    {
        if (lookRight)
        {
            transform.position += new Vector3(range * coilRange, 0, 0);
        }
        else
        {
            transform.position += new Vector3(-range * coilRange, 0, 0);
        }
        StartCoroutine(CreateForTime(duration));
    }

    void Update()
    {

    }

    private IEnumerator CreateForTime(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            //Debug.Log("Coil timer:" + timer);
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            EffectsManager.Instance.effect.ApplyEffect(gameObject, other.gameObject, effectType);
        }
    }
    
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.tag == "Enemy")
    //     {
    //         Debug.Log("Collision!");
    //         EffectsManager.Instance.effect.ApplyEffect(gameObject, collision.gameObject, effectType);
    //         Destroy(gameObject);
    //     }
    // }
}
