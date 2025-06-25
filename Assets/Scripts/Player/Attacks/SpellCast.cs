using UnityEngine;
using System.Collections.Generic;
using System;

public class SpellCast : MonoBehaviour
{
    public Transform startPos;
    public GameObject shootSprite;
    public GameObject coilSprite;

    private bool lookRight;

    private Dictionary<string, Action> spellsDict;

    void Start()
    {
        spellsDict = new Dictionary<string, Action>
    {
        {"Fire1", () => ShootingSpell(new Color(1f, 0.6f, 0f))},
        {"Fire2", () => SelfSpell(3, "SpeedBoost")},
        {"Fire3", () => ShootingSpell(new Color(1f, 0.6f, 0f), "Burn")},
        {"Fire1+Fire2", () => SelfSpell(3, "ExtraDamage")},
        {"Fire1+Fire3", () => CoilSpell(1, "Burn")},
        {"Fire2+Fire3", () => ShootingSpell(new Color(1f, 0.6f, 0f), "BurnLong")},
        {"Fire1+Fire2+Fire3", () => ShootingSpell(new Color(1f, 0.6f, 0f), "PercentDamage")}
    };
    }

    void Update()
    {
        lookRight = gameObject.GetComponent<PlayerAttack>().lookRight;
    }

    public void HandleSpell(string combo)
    {
        if (spellsDict.TryGetValue(combo, out Action spell))
        {
            spell();  // Каст заклинания, если существует такое комбо
        }
        else
        {
            Debug.Log("Заклинание не изучено!");
        }
}

    private void ShootingSpell(Color colorOfShoot, string effectType="No effect")
    {
        GameObject obj = Instantiate(shootSprite, startPos.position, Quaternion.identity);
        obj.GetComponent<ShootSpell>().lookRight = lookRight;
        obj.GetComponent<ShootSpell>().effectType = effectType;
        obj.GetComponent<SpriteRenderer>().color = colorOfShoot;
    }

    private void CoilSpell(int range, string effectType="No effect")
    {
        GameObject obj = Instantiate(coilSprite, startPos.position, Quaternion.identity);
        obj.GetComponent<CoilSpell>().lookRight = lookRight;
        obj.GetComponent<CoilSpell>().range = range;
        obj.GetComponent<CoilSpell>().effectType = effectType;
    }

    private void SelfSpell(float amount, string effectType)
    {
        EffectsManager.Instance.effect.ApplyEffect(gameObject, gameObject, effectType, amount);
    }
}
