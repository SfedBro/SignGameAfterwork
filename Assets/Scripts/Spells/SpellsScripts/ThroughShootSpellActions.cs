using System;
using UnityEngine;

public class ThroughShootSpellActions : MonoBehaviour
{
    private GameObject effectCaster;
    private float lifeDistance = 20f;
    private float damage;
    private string element;
    private string effectType;
    private float effectDuration;

    private Vector3 origPos;

    private void Start()
    {
        origPos = transform.position;
        if (!GetComponent<Rigidbody2D>())
        {
            gameObject.AddComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        CheckIfAlive();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Enemy")
        {
            if (!collider.gameObject.GetComponent<EffectsHandler>())
            {
                collider.gameObject.AddComponent<EffectsHandler>();
            }
            collider.gameObject.GetComponent<EffectsHandler>().HandleEffect(effectCaster, element, effectType, effectDuration, damage);
        }
    }

    private void CheckIfAlive()
    {
        if (Math.Abs((transform.position - origPos).magnitude) >= lifeDistance)
        {
            Destroy(gameObject);
        }
    }

    public void SetSettings(GameObject caster, string elem, float dmg, string effType)
    {
        effectCaster = caster;
        element = elem;
        damage = dmg;
        effectType = effType;
    }
}
