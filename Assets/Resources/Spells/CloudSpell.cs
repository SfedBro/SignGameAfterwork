using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Cloud Spell")]
public class CloudSpell : Spell
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float radius;
    [SerializeField] private float areaLifetime;

    private void Awake()
    {
        spellType = "Cloud";
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
