using UnityEngine;
using System.Collections.Generic;
using System;

public class SpellCast : MonoBehaviour
{
    public GameObject shootingSpellPrefab;
    public GameObject areaSpellPrefab;
    public float shootingSpellForce = 100f;

    private Dictionary<string, Action> spellsDict;
    private Camera mainCamera;
    private Vector3 cursorWorldPos;

    void Start()
    {
        mainCamera = Camera.main;
        spellsDict = new Dictionary<string, Action>
        {
            {"Fire1", () => ShootingSpell(new Color(1f, 0.6f, 0f))},
            {"Fire2", () => SelfSpell(3, "SpeedBoost")},
            {"Fire3", () => ShootingSpell(new Color(1f, 0.6f, 0f), "Burn")},
            {"Fire1+Fire2", () => SelfSpell(3, "ExtraDamage")},
            {"Fire1+Fire3", () => AreaSpell(1, "Burn")},
            {"Fire2+Fire3", () => ShootingSpell(new Color(1f, 0.6f, 0f), "BurnLong")},
            {"Fire1+Fire2+Fire3", () => ShootingSpell(new Color(1f, 0.6f, 0f), "PercentDamage")}
        };
    }

    public void HandleSpell(string combo)
    {
        if (spellsDict.TryGetValue(combo, out Action spell))
        {
            cursorWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            cursorWorldPos.z = 0;
            spell();  // Каст заклинания, если существует такое комбо
        }
        else
        {
            Debug.Log("Заклинание не изучено!");
        }
}

    private void ShootingSpell(Color colorOfShoot, string effectType="No effect")
    {
        GameObject obj = Instantiate(shootingSpellPrefab, transform.position, Quaternion.identity);
        obj.AddComponent<ShootSpell>();
        obj.GetComponent<ShootSpell>().cursorPos = cursorWorldPos;
        obj.GetComponent<ShootSpell>().force = shootingSpellForce;
        obj.GetComponent<ShootSpell>().effectType = effectType;
        obj.GetComponent<SpriteRenderer>().color = colorOfShoot;
    }

    private void AreaSpell(int range, string effectType="No effect")
    {
        GameObject obj = Instantiate(areaSpellPrefab, cursorWorldPos, Quaternion.identity);
        obj.AddComponent<AreaSpell>();
        obj.GetComponent<AreaSpell>().effectType = effectType;
    }

    private void SelfSpell(float amount, string effectType)
    {
        EffectsManager.Instance.effect.ApplyEffect(gameObject, gameObject, effectType, amount);
    }
}
