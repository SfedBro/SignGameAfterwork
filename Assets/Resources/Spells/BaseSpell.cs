using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Base Spell")]
public class Spell : ScriptableObject
{
    [Header("Основные настройки")]
    [SerializeField] private string description;
    [SerializeField] private string mainElement;
    [SerializeField] private string combo;
    protected string spellType;

    [Header("Специальные настройки")]

    [SerializeField] private string effect;
    [SerializeField] private float effectAmount;
    [SerializeField] private float effectDuration;
    [SerializeField] private float effectChance;

    public string Description
    {
        get
        {
            return description;
        }
    }

    public string Combo
    {
        get
        {
            return combo;
        }
    }

    public string MainElement
    {
        get
        {
            return mainElement;
        }
    }

    public float EffectDuration
    {
        get
        {
            return effectDuration;
        }
    }

    public float EffectAmount
    {
        get
        {
            return effectAmount;
        }
    }

    public string Effect
    {
        get
        {
            return effect;
        }
    }

    public float EffectChance
    {
        get
        {
            return effectChance;
        }
    }

    public string Type
    {
        get
        {
            return spellType;
        }
    }
} 
