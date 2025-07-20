using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/AoE from self Spell")]
public class AoeFromSelf : AoeSpell
{
    private void Awake()
    {
        spellType = "AoEFromSelf";
    }
}
