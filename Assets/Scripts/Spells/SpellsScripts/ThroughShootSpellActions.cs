using System;
using UnityEngine;

public class ThroughShootSpellActions : MonoBehaviour
{
    private GameObject effectCaster;
    private float lifeDistance = 20f;
    private float damage;
    private string element;
    private string effectType;
    private float effectAmount;
    private float effectDuration;
    private float effectChance;

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
            if (damage != 0)
            {
                collider.gameObject.GetComponent<EffectsHandler>().HandleEffect(effectCaster, element, "No effect", damage);
            }
            collider.gameObject.GetComponent<EffectsHandler>().HandleEffect(effectCaster, element, effectType, effectAmount, effectDuration, effectChance);
        }
    }

    private void CheckIfAlive()
    {
        if (Math.Abs((transform.position - origPos).magnitude) >= lifeDistance)
        {
            // Снятие множителя урона с игрока
            if (!effectCaster.GetComponent<EffectsHandler>())
            {
                effectCaster.AddComponent<EffectsHandler>();
            }
            effectCaster.GetComponent<EffectsHandler>().HandleEffect(effectCaster, element, "NextSpellDamageBoost", -1);
            
            Destroy(gameObject);
        }
    }

    public void SetSettings(GameObject caster, string elem, float dmg, string effType, float effAmount, float effDuration, float effChance)
    {
        effectCaster = caster;
        element = elem;
        damage = dmg;
        effectType = effType;
        effectAmount = effAmount;
        effectDuration = effDuration;
        effectChance = effChance;
    }
}
