using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/AoE Spell")]
public class AoeSpell : Spell
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float radius;

    private void Awake() {
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
}
