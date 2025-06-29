using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell", order = 51)]
public class Spell : ScriptableObject
{
    [Header("Основные настройки")]
    [SerializeField] private string spellName;
    [SerializeField] private string mainElement;
    [SerializeField] private string combo;
    private string spellType = "Shoot";
    [SerializeField] private GameObject spellPrefab;

    [Header("Специальные настройки")]
    [SerializeField] private float damage;
    [SerializeField] private string effect;
    [SerializeField] private float effectDuration;

    public string Name
    {
        get
        {
            return spellName;
        }
    }

    public string Combo
    {
        get
        {
            return combo;
        }
    }

    public GameObject Prefab
    {
        get
        {
            return spellPrefab;
        }
    }
    public string MainElement
    {
        get
        {
            return mainElement;
        }
    }

    public float Damage
    {
        get
        {
            return damage;
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
