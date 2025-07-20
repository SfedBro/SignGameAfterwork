using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DistanceWeakeningShootSpellActions : MonoBehaviour
{
    private GameObject effectCaster;
    private float damage;
    private float damageReduceDistance;
    private float alreadyReduced = 0f;
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

    private void Update()
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
            
            // Снятие множителя урона с игрока
            if (!effectCaster.GetComponent<EffectsHandler>())
            {
                effectCaster.AddComponent<EffectsHandler>();
            }
            effectCaster.GetComponent<EffectsHandler>().HandleEffect(effectCaster, element, "NextSpellDamageBoost", -1);

            Destroy(gameObject);
        }
    }

    private void CheckIfAlive()
    {
        if ((int)Math.Abs((transform.position - origPos).magnitude) % (int)damageReduceDistance - alreadyReduced > 0)
        {
            Debug.Log($"{transform.position} ------- {origPos}");
            damage -= 1;
            alreadyReduced += 1;
        }

        if (damage <= 0)
        {
            damage = 1;
        }
    }

    public void SetSettings(GameObject caster, string elem, float dmg, float distanceReducing, string effType, float effAmount, float effDuration, float effChance)
    {
        effectCaster = caster;
        element = elem;
        damage = dmg;
        damageReduceDistance = distanceReducing;
        effectType = effType;
        effectAmount = effAmount;
        effectDuration = effDuration;
        effectChance = effChance;
    }
}
