using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Through Shooting Spell")]
public class ThroughShootSpell : Spell
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float damage;

    private void Awake() {
        spellType = "ThroughShoot";
    }

    public GameObject Prefab
    {
        get
        {
            return spellPrefab;
        }
    }

    public float Damage
    {
        get
        {
            return damage;
        }
    }
} 
