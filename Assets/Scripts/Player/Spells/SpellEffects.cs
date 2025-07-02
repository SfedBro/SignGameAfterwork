using UnityEngine;
using System.Collections;
using System;

public class SpellEffect : MonoBehaviour
{
    public float knockbackForce = 200f;

    public float speedBoostDuration = 5f;

    private bool extraDamageApplied = false;
    private float extraDamageAmount = 0f;

    public Coroutine ApplyEffect(GameObject self, GameObject target, string type = "No effect", float duration = 0f, float amount = 0f, Action onComplete = null)
    {
        if (extraDamageApplied && type != "SpeedBoost" && type != "ExtraDamage")
        {
            MakeDamage(target, extraDamageAmount);
            extraDamageApplied = false;
            extraDamageAmount = 0f;
        }

        if (type == "No effect")
        {
            Debug.Log("Заклинание не имело никаких эффектов");
        }
        else if (type == "PercentDamage")
        {
            MakePercentDamage(target, duration);
        }
        else if (type == "Poison")
        {
            return StartCoroutine(Poison(target, duration, onComplete));
        }
        else if (type == "Burn")
        {
            return StartCoroutine(Burn(target, duration, onComplete));
        }
        else if (type == "Knockback")
        {
            ApplyKnockback(self, target);
        }
        else if (type == "SpeedBoost")
        {
            return StartCoroutine(SpeedBoost(self, amount, onComplete));
        }
        else if (type == "ExtraDamage")
        {
            ExtraDamage(self, amount);
        }
        else
        {
            Debug.Log("Неизвестный тип эффекта!");
        }
        return null;
    }

    private void MakeDamage(GameObject obj, float damage)
    {
        // Сделать общий класс персонажа, а энеми и плеер от него? Звучит как необходимость
        if (obj.CompareTag("Enemy"))
        {
            obj.GetComponent<Enemy>().TakeDamage(damage);
        }
        else if (obj.CompareTag("Boss"))
        {

        }
    }

    private void MakePercentDamage(GameObject obj, float percent)
    {
        if (obj.CompareTag("Enemy"))
        {
            obj.GetComponent<Enemy>().TakeDamage((float)Math.Round(obj.GetComponent<Enemy>().GetHp * percent / 100f));
        }
        else if (obj.CompareTag("Boss"))
        {
            
        }
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

    private IEnumerator Burn(GameObject obj, float duration, Action onComplete)
    {
        float timer = 0f;

        while (timer < duration)
        {
            if (obj != null)
            {
                int damage = UnityEngine.Random.Range(1, 4);
                obj.GetComponent<SpriteRenderer>().color = new Color(0.75f, 0f, 0f);
                Debug.Log($"Объект {obj.name} горит. Нанесен урон {damage}.");
                MakeDamage(obj, damage);
            }
            timer += 1f;
            yield return new WaitForSeconds(1f);
        }

        // Возвращаем в исходное состояние
        if (obj != null)
        {
            ReturnToOriginal(obj);
        }
        onComplete?.Invoke();
    }

    private IEnumerator Poison(GameObject obj, float duration, Action onComplete)
    {
        float timer = 0f;

        while (timer < duration)
        {
            int damage = UnityEngine.Random.Range(1, 4);
            obj.GetComponent<SpriteRenderer>().color = new Color(0f, 0.5f, 0f);
            Debug.Log($"Объект {obj.name} отравлен. Нанесен урон {damage}.");
            MakeDamage(obj, damage);

            timer += 1f;
            yield return new WaitForSeconds(1f);
        }

        // Возвращаем в исходное состояние
        if (obj != null)
        {
            ReturnToOriginal(obj);
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

    private IEnumerator SpeedBoost(GameObject obj, float change, Action onComplete)
    {
        if (obj.CompareTag("Player"))
        {
            Debug.Log($"{obj.name} получил ускорение на {change} на {speedBoostDuration} секунд");
            obj.GetComponent<PlayerController>().SpeedChange(change);
        }

        float timer = 0f;

        while (timer < speedBoostDuration)
        {
            timer += 1f;
            yield return new WaitForSeconds(1f);
        }

        // Возвращаем в исходное состояние
        if (obj.CompareTag("Player"))
        {
            obj.GetComponent<PlayerController>().SpeedChange(-change);
        }
        onComplete?.Invoke();
    }

    private void ExtraDamage(GameObject obj, float amount)
    {
        // Да, это не совсем то. В будущем изменю на + % урон следующего заклинания мб
        Debug.Log($"Следующее заклинание {obj.name} дополнительно нанесет {amount} урона");
        extraDamageApplied = true;
        extraDamageAmount += amount;
    }
}