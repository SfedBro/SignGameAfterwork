using System.Collections.Generic;
using UnityEngine;

public class SpellsManager : MonoBehaviour
{
    private Dictionary<string, Spell> allSpells;
    private Dictionary<string, Spell>.ValueCollection values;
    void Start()
    {
        allSpells = new Dictionary<string, Spell>();

        foreach (Spell spell in Resources.LoadAll<Spell>("Spells"))
        {
            //Debug.Log($"Изучено заклинание {spell.Name}, комбинация: {spell.Combo}");
            allSpells.Add(spell.Combo, spell);
        }

        values = allSpells.Values;
    }

    public Spell GetSpellByCombo(string combo)
    {
        if (allSpells.ContainsKey(combo))
        {
            return allSpells[combo];
        }
        else
        {
            return null;
        }
    }

    public Dictionary<string, Spell>.ValueCollection GetAllSpells()
    {
        return values;
    }
}
