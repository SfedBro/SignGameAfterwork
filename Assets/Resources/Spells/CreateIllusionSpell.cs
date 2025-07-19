using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Illusion Spell")]
public class CreateIllusionSpell : Spell
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float lifetime;

    private void Awake()
    {
        spellType = "Illusion";
    }

    public GameObject Prefab
    {
        get
        {
            return spellPrefab;
        }
    }

    public float Lifetime
    {
        get
        {
            return lifetime;
        }
    }
}
