using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Shooting Spell")]
public class ShootSpell : Spell
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float damage;

    private void Awake() {
        spellType = "Shoot";
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
