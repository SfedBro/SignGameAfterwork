using System;
using UnityEngine;

public class ShootSpellActions : MonoBehaviour
{
    public float force;
    public float lifeDistance = 20f;
    public float damage;
    public string effectType;
    public float effectDuration;
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
            if (effectType != "PercentDamage")
            {
                if (damage != 0)
                {
                    collider.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                }
                // Здесь будем на врага накладывать эффект, если он есть. Для этого добавляем врагу новый EffectsManager, если его нет.
                EffectsManager.Instance.effect.ApplyEffect(gameObject, collider.gameObject, effectType, effectDuration);
            }
            else
            {
                EffectsManager.Instance.effect.ApplyEffect(gameObject, collider.gameObject, effectType, 0, damage);
            }
            
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
