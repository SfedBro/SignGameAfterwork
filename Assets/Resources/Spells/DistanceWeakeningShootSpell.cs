using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Distance Weakening Shooting Spell")]
public class DistanceWeakeningShootSpell : Spell
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float damage;
    [SerializeField] private float distanceOfDamageReduce;

    private void Awake()
    {
        spellType = "DistanceWeakeningShoot";
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

    public float Distance
    {
        get
        {
            return distanceOfDamageReduce;
        }
    }
} 
