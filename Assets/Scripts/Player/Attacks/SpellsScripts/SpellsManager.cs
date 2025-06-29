using System.Collections.Generic;
using UnityEngine;

public class SpellsManager : MonoBehaviour
{
    private Dictionary<string, Spell> allSpells;
    void Start()
    {
        allSpells = new Dictionary<string, Spell>();

        foreach (Spell spell in Resources.LoadAll<Spell>("Spells"))
        {
            //Debug.Log($"Изучено заклинание {spell.Name}, комбинация: {spell.Combo}");
            allSpells.Add(spell.Combo, spell);
        }
    }

    public Spell getSpellByCombo(string combo)
    {
        return allSpells[combo];
    }
}
