using System.Collections.Generic;
using UnityEngine;

public class EffectsHandler : MonoBehaviour
{
    private Dictionary<string, Coroutine> activeEffects = new();
    private Dictionary<string, float> activeEffectsDamage = new();
    private Dictionary<string, string> counterElements;
    private SpellEffect spells;

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

        // Если наложенный эффект должен снять существующий, это произойдёт
        if (counterElements.ContainsKey(element))
        {
            if (activeEffects.ContainsKey(counterElements[element]))
            {
                RemoveEffect(counterElements[element]);
                Debug.Log($"Эффект {counterElements[element]} был снят");
            }
        }

        // Если эффект уже висит, мы его обновим, если новый не слабее, если эффект не висит, повесим
        if (activeEffects.ContainsKey(effectName))
        {
            if (effectAmount >= activeEffectsDamage[effectName])
            {
                RemoveEffect(effectName);
                ApplyEffect(effectCaster, effectName, effectAmount, effectDuration, effectChance);
            }
        }
        else
        {
            ApplyEffect(effectCaster, effectName, effectAmount, effectDuration, effectChance);
        }
    }

    private void Awake()
    {
        counterElements = new Dictionary<string, string>
        {
            {"Water", "Burn"},
            {"Air", "Slowness"}
        };
    }

    private void ApplyEffect(GameObject effectCaster, string effectName, float effectAmount, float effectDuration, float effectChance)
    {
        Coroutine newEffect = spells.ApplyEffect(effectCaster, gameObject, effectName, effectAmount, effectDuration, effectChance, () => OnEffectComplete(effectName));
        if (gameObject != null && newEffect != null)
        {
            activeEffects.Add(effectName, newEffect);
            activeEffectsDamage.Add(effectName, effectAmount);
        }
    }

    private void RemoveEffect(string effectName)
    {
        spells.StopCoroutine(activeEffects[effectName]);

        activeEffects.Remove(effectName);
        activeEffectsDamage.Remove(effectName);
    }

    private void OnEffectComplete(string effectName)
    {
        if (activeEffects.ContainsKey(effectName))
        {
            activeEffects.Remove(effectName);
            activeEffectsDamage.Remove(effectName);
            Debug.Log($"Эффект {effectName} завершен");
        }
    }
}
