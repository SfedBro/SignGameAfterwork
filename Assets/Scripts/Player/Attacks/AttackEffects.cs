using UnityEngine;
using System.Collections;
using System;

public class AttackEffect : MonoBehaviour
{
    public float burnDuration = 3f;
    public float poisonDuration = 5f;
    public float knockbackForce = 200f;
    public float hpPercentDamage = 0.1f;
    public float speedBoostDuration = 5f;

    private bool extraDamageApplied = false;
    private float extraDamageAmount = 0f;

    public void ApplyEffect(GameObject self, GameObject target, string type = "No effect", float amount = 1)
    {
        if (extraDamageApplied && type != "SpeedBoost" && type != "ExtraDamage")
        {
            MakeDamage(target, extraDamageAmount);
            extraDamageApplied = false;
            extraDamageAmount = 0f;
        }

        if (type == "No effect")
        {
            MakeDamage(target, amount);
        }
        else if (type == "PercentDamage")
        {
            MakePercentDamage(target);
        }
        else if (type == "Poison")
        {
            StartPoisoning(target);
        }
        else if (type == "Burn")
        {
            StartBurning(target);
        }
        else if (type == "BurnLong")
        {
            StartBurning(target, 3f);
        }
        else if (type == "Knockback")
        {
            ApplyKnockback(self, target);
        }
        else if (type == "SpeedBoost")
        {
            StartCoroutine(SpeedBoost(self, amount));
        }
        else if (type == "ExtraDamage")
        {
            ExtraDamage(self, amount);
        }
    }

    private void MakeDamage(GameObject obj, float damage)
    {
        // Сделать общий класс персонажа, а энеми и плеер от него? Звучит как необходимость
        if (obj.CompareTag("Enemy"))
        {
            obj.GetComponent<Enemy>().TakeDamage(damage);
        }
        else if (obj.CompareTag("Player"))
        {
            //obj.GetComponent<Player>().takeDamage(damage);
        }
    }

    private void MakePercentDamage(GameObject obj)
    {
        if (obj.CompareTag("Enemy"))
        {
            obj.GetComponent<Enemy>().TakeDamage((float)Math.Round(obj.GetComponent<Enemy>().GetHP() * hpPercentDamage));
        }
        else if (obj.CompareTag("Player"))
        {
            //obj.GetComponent<Player>().takeDamage(damage);
        }
    }

    private void StartBurning(GameObject obj, float extraDuration = 0f)
    {
        Debug.Log($"{obj.name} был подожжен!");
        StartCoroutine(Burn(obj, extraDuration));
    }

    private void StartPoisoning(GameObject obj, float extraDuration = 0f)
    {
        Debug.Log($"{obj.name} был отравлен!");
        StartCoroutine(Poison(obj, extraDuration));
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

    private IEnumerator Burn(GameObject obj, float extraDuration)
    {
        float timer = 0f;

        while (timer < burnDuration + extraDuration)
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
        ReturnToOriginal(obj);
    }

    private IEnumerator Poison(GameObject obj, float extraDuration)
    {
        float timer = 0f;

        while (timer < poisonDuration + extraDuration)
        {
            int damage = UnityEngine.Random.Range(1, 4);
            obj.GetComponent<SpriteRenderer>().color = new Color(0f, 0.5f, 0f);
            Debug.Log($"Объект {obj.name} отравлен. Нанесен урон {damage}.");
            MakeDamage(obj, damage);

            timer += 1f;
            yield return new WaitForSeconds(1f);
        }

        // Возвращаем в исходное состояние
        ReturnToOriginal(obj);
    }

    private void ReturnToOriginal(GameObject obj)
    {
        if (obj.CompareTag("Enemy"))
        {
            obj.GetComponent<Enemy>().ReturnToOrig();
        }
    }

    private IEnumerator SpeedBoost(GameObject obj, float change)
    {
        if (obj != null && obj.CompareTag("Player"))
        {
            Debug.Log($"{obj.name} получил ускорение на {change} на {speedBoostDuration} секунд");
            obj.GetComponent<PlayerController>().SpeedChange(change);
            obj.GetComponent<PlayerMovement>().SpeedChange(change);
        }

        float timer = 0f;

        while (timer < speedBoostDuration)
        {
            timer += 1f;
            yield return new WaitForSeconds(1f);
        }

        // Возвращаем в исходное состояние
        if (obj != null && obj.CompareTag("Player"))
        {
            obj.GetComponent<PlayerController>().SpeedChange(-change);
            obj.GetComponent<PlayerMovement>().SpeedChange(-change);
        }
    }

    private void ExtraDamage(GameObject obj, float amount)
    {
        // Да, это не совсем то. В будущем изменю на + % урон следующего заклинаиня
        Debug.Log($"Следующее заклинание {obj.name} дополнительно нанесет {amount} урона");
        extraDamageApplied = true;
        extraDamageAmount += amount;
    }
}