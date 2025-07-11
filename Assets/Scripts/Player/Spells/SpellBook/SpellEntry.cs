using UnityEngine;
using UnityEngine.UI;

public class SpellEntry : MonoBehaviour
{
    private Text nameText;
    private Text comboText;
    private Text descriptionText;

    public void SetSpell(Spell someSpell)
    {
        nameText.text = someSpell.name;
        comboText.text = someSpell.Combo;
        descriptionText.text = someSpell.Description;
    }

    public string GetName()
    {
        return nameText.text;
    }
}