using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/AoE Spell")]
public class AoeSpell : Spell
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float radius;
    [SerializeField] private float areaLifetime;

    private void Awake()
    {
        spellType = "AoE";
    }

    public GameObject Prefab
    {
        get
        {
            return spellPrefab;
        }
    }
    public float Radius
    {
        get
        {
            return radius;
        }
    }

    public float AreaLifetime
    {
        get
        {
            return areaLifetime;
        }
    }
}
