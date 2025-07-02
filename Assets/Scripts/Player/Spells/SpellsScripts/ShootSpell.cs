using System;
using UnityEngine;

public class ShootSpellActions : MonoBehaviour
{
    public GameObject effectCaster;
    public float force;
    public float lifeDistance = 20f;
    public float damage;
    public string effectType;
    public float effectDuration;
    public float amount;
    public Vector3 cursorPos;

    private Rigidbody2D physic;
    private Vector3 origPos;

    private void Start()
    {
        origPos = transform.position;
        if (!GetComponent<Rigidbody2D>())
        {
            gameObject.AddComponent<Rigidbody2D>();
        }

        physic = GetComponent<Rigidbody2D>();
        physic.AddForce(force * (cursorPos - origPos).normalized);
    }

    void Update()
    {
        CheckIfAlive();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Enemy")
        {
            Debug.Log("Collision!");
            if (!collider.gameObject.GetComponent<EffectsHandler>())
            {
                collider.gameObject.AddComponent<EffectsHandler>();
            }
            if (damage != 0)
            {
                collider.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            }
            collider.gameObject.GetComponent<EffectsHandler>().HandleEffect(effectCaster, effectType, effectDuration, 0);
            //EffectsManager.Instance.effect.ApplyEffect(gameObject, collider.gameObject, effectType, effectDuration);
            
            Destroy(gameObject);
        }
    }

    private void CheckIfAlive()
    {
        if (Math.Abs((transform.position - origPos).magnitude) >= lifeDistance)
        {
            Destroy(gameObject);
        }
    }
}
