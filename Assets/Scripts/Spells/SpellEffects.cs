using UnityEngine;
using System.Collections;
using System;

public class SpellEffect : MonoBehaviour
{
    public float knockbackForce = 200f;

    private float damageMultiplier = 1f;

    System.Random rnd = new System.Random();

    public Coroutine ApplyEffect(GameObject self, GameObject target, string type = "No effect", float amount = 0f, float duration = 0f, float chance = 1f, Action onComplete = null)
    {
        if (type == "No effect")
        {
            if (amount > 0)
            {
                MakeDamage(target, amount);
            }
        }
        else if (type == "PercentDamage")
        {
            MakePercentDamage(target, amount);
        }
        else if (type == "Burn")
        {
            if (rnd.Next(100) <= chance * 100)
            {
                return StartCoroutine(Burn(target, (int)amount, duration, damageMultiplier, onComplete));
            }
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
        else if (type == "Knockback")
        {
            ApplyKnockback(self, target);
        }
        else if (type == "Slowness")
        {
            if (rnd.Next(100) <= chance * 100)
            {
                return StartCoroutine(SpeedBoost(target, duration, -amount, onComplete));
            }
        }
            else
            {
                Debug.Log("Неизвестный тип эффекта!");
            }

        return null;
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
    }

    private void MakePercentDamage(GameObject obj, float percent)
    {
        MakeDamage(obj, (float)Math.Round(obj.GetComponent<Enemy>().GetHp * percent));
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

    private IEnumerator Burn(GameObject obj, int avgDamage, float duration, float dmgMultiplier, Action onComplete)
    {
        float timer = 0f;
        damageMultiplier = 1f;

        if (obj.CompareTag("Enemy"))
        {
            Debug.Log($"{obj.name} был подожжен");
            obj.GetComponent<SpriteRenderer>().color = new Color(0.75f, 0f, 0f);
            while (timer < duration && obj != null)
            {
                int damage = UnityEngine.Random.Range((avgDamage + 1) / 2, 2 * avgDamage - avgDamage / 2);

                MakeDamage(obj, damage * dmgMultiplier);

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
            Debug.Log($"{obj.name} получил {(change > 0 ? "ускорение" : "замедление")} на {(int)(change * (change > 0 ? 100 : -100))}% на {duration} секунд");
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
        if (multiplier > 0)
        {
            Debug.Log($"Следующее заклинание {obj.name} нанесет на {100 * multiplier - 100}% больше урона");
            damageMultiplier = multiplier;
        }
        else
        {
            damageMultiplier = 1f;
        }
    }

    private void ReturnToOriginal(GameObject obj)
    {
        if (obj.CompareTag("Enemy"))
        {
            obj.GetComponent<Enemy>().ReturnToOrig();
        }
    }
}