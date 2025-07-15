using System.Collections.Generic;
using UnityEngine;

public class EffectsHandler : MonoBehaviour
{
    private Dictionary<string, Coroutine> activeEffects = new Dictionary<string, Coroutine>();
    private Dictionary<string, string> counterElements;
    private SpellEffect spells;

    private void Awake()
    {
        counterElements = new Dictionary<string, string>
        {
            {"Water", "Burn"}
        };
    }

    public void TakeDamage(GameObject effectCaster, float damage)
    {
        if (!effectCaster.GetComponent<SpellEffect>())
        {
            effectCaster.AddComponent<SpellEffect>();
        }
        spells = effectCaster.GetComponent<SpellEffect>();

        spells.ApplyEffect(effectCaster, gameObject, "No effect", damage);
    }
    public void HandleEffect(GameObject effectCaster, string element, string effectName = "No effect", float effectAmount = 0f, float effectDuration = 0f, float effectChance = 0f)
    {
        // Обращаемся всегда к спелл эффектам кастера, т.к. там хранятся данные об изменениях для следующих заклинаний
        if (!effectCaster.GetComponent<SpellEffect>())
        {
            effectCaster.AddComponent<SpellEffect>();
        }
        spells = effectCaster.GetComponent<SpellEffect>();

        // Если эффект уже висит, мы его обновим
        if (activeEffects.ContainsKey(effectName))
        {
            RemoveEffect(effectName);
        }

        // Если наложенный эффект должен снять существующий, это произойдёт
        if (counterElements.ContainsKey(element))
        {
            if (activeEffects.ContainsKey(counterElements[element]))
            {
                RemoveEffect(counterElements[element]);
                Debug.Log($"Эффект {counterElements[element]} был снят");
            }
        }

        ApplyEffect(effectCaster, effectName, effectAmount, effectDuration, effectChance);
    }

    private void ApplyEffect(GameObject effectCaster, string effectName, float effectAmount, float effectDuration, float effectChance)
    {
        Coroutine newEffect = spells.ApplyEffect(effectCaster, gameObject, effectName, effectAmount, effectDuration, effectChance, () => OnEffectComplete(effectName));
        if (newEffect != null)
        {
            activeEffects.Add(effectName, newEffect);
        }
    }

    void RemoveEffect(string effectName)
    {
        StopCoroutine(activeEffects[effectName]);

        if (gameObject.CompareTag("Enemy"))
        {
            gameObject.GetComponent<Enemy>().ReturnToOrig();
        }
        
        activeEffects.Remove(effectName);
    }

    private void OnEffectComplete(string effectName)
    {
        if (activeEffects.ContainsKey(effectName))
        {
            activeEffects.Remove(effectName);
            Debug.Log($"Эффект {effectName} завершен");
        }
    }
}
