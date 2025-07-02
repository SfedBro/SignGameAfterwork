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
    public void HandleEffect(GameObject effectCaster, string effectName, float effectDuration, float amount)
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
        // Пока недореализовано
        if (counterElements.ContainsKey(effectName))
        {
            if (activeEffects.ContainsKey(counterElements[effectName]))
            {
                RemoveEffect(counterElements[effectName]);
            }
        }

        ApplyEffect(effectCaster, effectName, effectDuration, amount);
    }

    private void ApplyEffect(GameObject effectCaster, string effectName, float effectDuration, float amount)
    {
        Coroutine newEffect = spells.ApplyEffect(effectCaster, gameObject, effectName, effectDuration, amount, () => OnEffectComplete(effectName));
        if (newEffect != null)
        {
            Debug.Log($"На {gameObject.name} наложен эффект {effectName}");
            activeEffects.Add(effectName, newEffect);
        }
    }

    void RemoveEffect(string effectName)
    {
        StopCoroutine(activeEffects[effectName]);
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
