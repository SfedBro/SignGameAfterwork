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
    [SerializeField] private float effectDuration;

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

    public string Effect
    {
        get
        {
            return effect;
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
