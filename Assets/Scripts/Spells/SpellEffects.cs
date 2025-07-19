using UnityEngine;
using System.Collections;
using System;
using UnityEngine.AI;

public class SpellEffect : MonoBehaviour
{
    private float knockbackForce = 12000f;

    private float damageMultiplier = 1f;
    private int dodgingAttacks = 0;
    private float dodgingChance = 1f;
    private float extraKnockback = -1f;
    private float extraKnockbackAmount = 0f;

    private System.Random rnd = new();

    public Coroutine ApplyEffect(GameObject self, GameObject target, string type = "No effect", float amount = 0f, float duration = 0f, float chance = 1f, Action onComplete = null)
    {
        if (extraKnockback > 0f && type != "SpeedBoost" && type != "AttackDodge" && type != "NextSpellDamageBoost")
        {
            ApplyKnockback(self, target, extraKnockbackAmount);
            extraKnockback = -1f;
            extraKnockbackAmount = 0f;
        }

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
            if (rnd.Next(100) <= chance * 100)
            {
                return StartCoroutine(SpeedBoost(self, duration, amount, onComplete));
            }
        }
        else if (type == "AttackDodge")
        {
            if (amount >= 1f)
            {
                DodgeAttacks((int)amount);
            }
            else if (duration > 0f)
            {
                return StartCoroutine(DodgeAttacks(duration, chance, onComplete));
            }
        }
        else if (type == "NextSpellDamageBoost")
        {
            if (rnd.Next(100) <= chance * 100)
            {
                DamageBoost(self, amount);
            }
        }
        else if (type == "Knockback")
        {
            if (rnd.Next(100) <= chance * 100)
            {
                ApplyKnockback(self, target, amount);
            }
        }
        else if (type == "NextSpellKnockback")
        {
            extraKnockback = 1f;
            extraKnockbackAmount = amount;
            Debug.Log("Следующее заклинание оттолкнет врага");
        }
        else if (type == "Slowness")
        {
            if (rnd.Next(100) <= chance * 100)
            {
                return StartCoroutine(SpeedBoost(target, duration, -amount, onComplete));
            }
        }
        else if (type == "Stun")
        {
            if (rnd.Next(100) <= chance * 100)
            {
                return StartCoroutine(StunEffect(target, duration, onComplete));
            }
        }
        else if (type == "SizeDecrease")
        {
            return StartCoroutine(SizeChange(self, amount, duration, onComplete));
        }
        else if (type == "NextSpellDuplicate")
        {
            if (rnd.Next(100) <= chance * 100)
            {
                gameObject.GetComponent<SpellCast>().DuplicateNextSpell();
                Debug.Log("Следующее заклинание можно будет использовать дважды");
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

    private void ApplyKnockback(GameObject self, GameObject obj, float amount)
    {
        // Определяем направление от объекта к цели, чтобы отталкивание было правильным
        Vector3 direction = obj.transform.position - self.transform.position;
        direction.Normalize();

        // Настраиваем физику отталкиваемого объекта
        if (!obj.GetComponent<Rigidbody2D>())
        {
            obj.AddComponent<Rigidbody2D>();
        }
        Rigidbody2D physic = obj.GetComponent<Rigidbody2D>();
        physic.bodyType = RigidbodyType2D.Dynamic;
        physic.mass = 10f;
        physic.gravityScale = 0;
        physic.freezeRotation = true;
        physic.linearDamping = 5f;

        direction.Normalize();

        // Применяем отталкивание
        physic.AddForce(direction * amount * knockbackForce);

        Debug.Log($"Объект {obj.name} отброшен");
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
                if (obj.GetComponent<FlyingEnemyMovement>())
                {
                    obj.GetComponent<FlyingEnemyMovement>().SpeedChange(change);
                }
                else if (obj.GetComponent<LandEnemyMovement>())
                {
                    obj.GetComponent<LandEnemyMovement>().SpeedChange(change);
                }
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
            if (obj.GetComponent<FlyingEnemyMovement>())
            {
                obj.GetComponent<FlyingEnemyMovement>().SpeedChange(0);
            }
            else if (obj.GetComponent<LandEnemyMovement>())
            {
                obj.GetComponent<LandEnemyMovement>().SpeedChange(0);
            }
        }
        onComplete?.Invoke();
    }

    private IEnumerator StunEffect(GameObject obj, float duration, Action onComplete)
    {
        bool flying = true;

        if (obj.CompareTag("Enemy"))
        {
            if (obj.GetComponent<FlyingEnemyMovement>())
            {
                obj.GetComponent<FlyingEnemyMovement>().enabled = false;
            }
            else if (obj.GetComponent<LandEnemyMovement>())
            {
                obj.GetComponent<LandEnemyMovement>().enabled = false;
                flying = false;
            }
            obj.GetComponent<NavMeshAgent>().enabled = false;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }

        if (obj.CompareTag("Enemy"))
        {
            if (flying)
            {
                obj.GetComponent<FlyingEnemyMovement>().enabled = true;
            }
            else
            {
                obj.GetComponent<LandEnemyMovement>().enabled = true;
            }
            obj.GetComponent<NavMeshAgent>().enabled = true;
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

    private void DodgeAttacks(int amount)
    {
        dodgingAttacks = amount;
    }

    private IEnumerator DodgeAttacks(float duration, float chance, Action onComplete)
    {
        float timer = 0f;
        dodgingAttacks = 1000;
        dodgingChance = chance;

        while (timer < duration)
        {
            timer += 1f;
            yield return new WaitForSeconds(1f);
        }

        dodgingAttacks = 0;
        dodgingChance = 1f;
        onComplete?.Invoke();
    }

    private IEnumerator SizeChange(GameObject obj, float amount, float duration, Action onComplete)
    {
        float timer = 0f;

        if (obj.CompareTag("Player"))
        {
            obj.GetComponent<Transform>().localScale *= amount;
            obj.GetComponent<SpellCast>().MoveWand(amount);
        }

        while (timer < duration)
        {
            timer += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }

        if (obj.CompareTag("Player"))
        {
            obj.GetComponent<Transform>().localScale /= amount;
            obj.GetComponent<SpellCast>().MoveWand(1/amount);
        }

        onComplete?.Invoke();
    }

    private void ReturnToOriginal(GameObject obj)
    {
        if (obj.CompareTag("Enemy"))
        {
            obj.GetComponent<Enemy>().ReturnToOrig();
        }
    }

    public bool DodgeAttack()
    {
        if (dodgingAttacks > 0 && rnd.Next(100) <= dodgingChance * 100)
        {
            dodgingAttacks -= 1;
            return true;
        }
        else
        {
            return false;
        }
    }
}