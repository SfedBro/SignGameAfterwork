using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/% Damage Shooting Spell")]
public class PercentDamageShootSpell : Spell
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float percentOfDamage;

    private void Awake() {
        spellType = "PercentDMGShoot";
    }

    public GameObject Prefab
    {
        get
        {
            return spellPrefab;
        }
    }

    public float PercentOfDamage
    {
        get
        {
            return percentOfDamage;
        }
    }
} 
