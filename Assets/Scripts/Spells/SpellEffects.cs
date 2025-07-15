using UnityEngine;
using System.Collections;
using System;

public class SpellEffect : MonoBehaviour
{
    public float knockbackForce = 200f;

    private float damageMultiplier = 1f;

    public Coroutine ApplyEffect(GameObject self, GameObject target, string type = "No effect", float duration = 0f, float amount = 0f, Action onComplete = null)
    {
        Coroutine toBeReturned = null;
        if (type == "No effect")
        {
            MakeDamage(target, amount);
        }
        else if (type == "PercentDamage")
        {
            MakePercentDamage(target, duration);
        }
        else if (type == "Burn")
        {
            toBeReturned = StartCoroutine(Burn(target, duration, damageMultiplier, onComplete));
        }
        else if (type == "SpeedBoost")
        {
            return StartCoroutine(SpeedBoost(self, duration, amount, onComplete));
        }
        else if (type == "NextSpellDamageBoost")
        {
            DamageBoost(self, amount);
            return null;
        }
        else if (type == "ThroughShot")
        {
            MakeDamage(target, amount);
        }
        else if (type == "Knockback")
        {
            ApplyKnockback(self, target);
        }
        else if (type == "Slowness")
        {
            return StartCoroutine(SpeedBoost(target, duration, -amount, onComplete));
        }
        else
        {
            Debug.Log("Неизвестный тип эффекта!");
        }

        damageMultiplier = 1f;
        return toBeReturned;
    }

    private void MakeDamage(GameObject obj, float damage)
    {
        if (obj.CompareTag("Enemy"))
        {
            obj.GetComponent<Enemy>().TakeDamage(damage * damageMultiplier);
        }
        else if (obj.CompareTag("Boss"))
        {

        }
        damageMultiplier = 1f;
    }

    private void MakePercentDamage(GameObject obj, float percent)
    {
        MakeDamage(obj, (float)Math.Round(obj.GetComponent<Enemy>().GetHp * percent / 100f));
    }

    private void ApplyKnockback(GameObject self, GameObject obj)
    {
        // Определяем направление от объекта к цели, чтобы отбрасывание было правильным
        Vector3 direction = obj.transform.position - self.transform.position;
        Rigidbody2D physic = obj.GetComponent<Rigidbody2D>();
        direction.Normalize();

        // Применяем отбрасывание
        physic.AddForce(direction * knockbackForce);

        Debug.Log($"Объект {obj.name} отброшен с силой {knockbackForce}");
    }

    private IEnumerator Burn(GameObject obj, float duration, float dmgMultiplier, Action onComplete)
    {
        float timer = 0f;
        damageMultiplier = 1f;

        if (obj.CompareTag("Enemy"))
        {
            obj.GetComponent<SpriteRenderer>().color = new Color(0.75f, 0f, 0f);
            while (timer < duration && obj != null)
            {
                int damage = UnityEngine.Random.Range(1, 4);

                obj.GetComponent<Enemy>().TakeDamage(damage * dmgMultiplier);

                timer += 1f;
                yield return new WaitForSeconds(1f);
            }
        }

        // Возвращаем в исходное состояние
            if (obj != null)
            {
                ReturnToOriginal(obj);
            }
        onComplete?.Invoke();
    }

    private IEnumerator SpeedBoost(GameObject obj, float duration, float change, Action onComplete)
    {
        if (obj != null)
        {
            Debug.Log($"{obj.name} получил {(change >0 ? "ускорение" : "замедление")} на {(int)(change*(change>0 ? 100 : -100))}% на {duration} секунд");
            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<PlayerController>().SpeedChange(change);
            }
            else if (obj.CompareTag("Enemy"))
            {
                // if (obj.GetComponent<FlyingEnemyMovement>())
                // {
                //     obj.GetComponent<FlyingEnemyMovement>().SpeedChange(change);
                // }
                // else if (obj.GetComponent<LandEnemyMovement>())
                // {
                //     obj.GetComponent<LandEnemyMovement>().SpeedChange(change);
                // }
            }
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += 1f;
            yield return new WaitForSeconds(1f);
        }

        // Возвращаем в исходное состояние
        if (obj.CompareTag("Player"))
        {
            obj.GetComponent<PlayerController>().SpeedChange(0);
        }
        else if (obj.CompareTag("Enemy"))
        {
            // if (obj.GetComponent<FlyingEnemyMovement>())
            // {
            //     obj.GetComponent<FlyingEnemyMovement>().SpeedChange(0);
            // }
            // else if (obj.GetComponent<LandEnemyMovement>())
            // {
            //     obj.GetComponent<LandEnemyMovement>().SpeedChange(0);
            // }
        }
        onComplete?.Invoke();
    }

    private void DamageBoost(GameObject obj, float multiplier)
    {
        Debug.Log($"Следующее заклинание {obj.name} нанесет на {100 * multiplier - 100}% больше урона");
        damageMultiplier = multiplier;
    }
    
    private void ReturnToOriginal(GameObject obj)
    {
        if (obj.CompareTag("Enemy"))
        {
            obj.GetComponent<Enemy>().ReturnToOrig();
        }
        // else if (obj.CompareTag("Player"))
        // {
        //     obj.GetComponent<Player>().ReturnToOrig();
        // }
    }
}