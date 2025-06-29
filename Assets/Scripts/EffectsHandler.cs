using System.Collections.Generic;
using UnityEngine;

public class EffectsHandler : MonoBehaviour
{
    private Dictionary<string, Coroutine> activeEffects = new Dictionary<string, Coroutine>();
    private Dictionary<string, string> counterEffects;
    private SpellEffect spells;

    void Start()
    {
        counterEffects = new Dictionary<string, string>
        {
            {"Water", "Burn"}
        };
    }
    void HandleEffect(string effectName, float amount)
    {
        if (activeEffects.ContainsKey(effectName))
        {
            RemoveEffect(effectName);
        }

        if (counterEffects.ContainsKey(effectName))
        {
            if (activeEffects.ContainsKey(counterEffects[effectName]))
            {
                RemoveEffect(counterEffects[effectName]);
            }
        }

        ApplyEffect(effectName, amount);
    }

    void ApplyEffect(string effectName, float amount)
    {
        spells.ApplyEffect(gameObject, gameObject, effectName, amount);
        //activeEffects.Add(effectName, af);
    }

    void RemoveEffect(string effectName)
    {
        activeEffects.Remove(effectName);
        StopCoroutine(effectName);
    }
}
