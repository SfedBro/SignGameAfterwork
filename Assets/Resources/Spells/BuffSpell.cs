using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Buff Spell")]
public class BuffSpell : Spell
{
    private void Awake() {
        spellType = "Buff";
    }
}
