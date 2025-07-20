using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Room size Spell")]
public class RoomSizeSpell : Spell
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float areaLifetime;

    private void Awake()
    {
        spellType = "Room";
    }

    public GameObject Prefab
    {
        get
        {
            return spellPrefab;
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
