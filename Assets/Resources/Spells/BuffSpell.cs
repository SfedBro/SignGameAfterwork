using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Buff Spell")]
public class BuffSpell : Spell
{
    [SerializeField] private float amount;
    private void Awake()
    {
        spellType = "Buff";
    }

    public float Amount
    {
        get
        {
            return amount;
        }
    }
}
