using UnityEngine;
using System.Collections.Generic;
using System;

public class SpellCast : MonoBehaviour
{
    public GameObject shootingSpellPrefab;
    public GameObject areaSpellPrefab;
    public float shootingSpellForce = 100f;

    private Camera mainCamera;
    private Vector3 cursorWorldPos;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void castSpell(Spell someSpell)
    {
        // Получаем позицию курсора и разворачиваем игрока в его сторону
        cursorWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        cursorWorldPos.z = 0;
        FixPlayersLook();
        // Кастуем заклинание
        if (someSpell.Type == "Shoot")
        {
            ShootingSpell((ShootSpell)someSpell);
        }
        else if (someSpell.Type == "AoE")
        {
            AreaSpell((AoeSpell)someSpell);
        }
        else if (someSpell.Type == "Buff")
        {
            SelfSpell((BuffSpell)someSpell);
        }
        else
        {
            Debug.Log($"{someSpell.Type} - неизвестный тип заклинания");
        }
    }

    private void ShootingSpell(ShootSpell someSpell)
    {
        GameObject obj = Instantiate(someSpell.Prefab, transform.position, Quaternion.identity);
        obj.AddComponent<ShootSpellActions>();
        obj.GetComponent<ShootSpellActions>().effectCaster = gameObject;
        obj.GetComponent<ShootSpellActions>().cursorPos = cursorWorldPos;
        obj.GetComponent<ShootSpellActions>().force = shootingSpellForce;
        obj.GetComponent<ShootSpellActions>().damage = someSpell.Damage;
        obj.GetComponent<ShootSpellActions>().effectType = someSpell.Effect;
        obj.GetComponent<ShootSpellActions>().effectDuration = someSpell.EffectDuration;
    }

    private void AreaSpell(AoeSpell someSpell)
    {
        GameObject obj = Instantiate(someSpell.Prefab, cursorWorldPos, Quaternion.identity);
        obj.AddComponent<AreaSpellActions>();
        obj.GetComponent<AreaSpellActions>().effectCaster = gameObject;
        obj.GetComponent<AreaSpellActions>().effectType = someSpell.Effect;
        obj.GetComponent<AreaSpellActions>().effectDuration = someSpell.EffectDuration;
        obj.GetComponent<Transform>().localScale = new Vector3(someSpell.Radius*2, someSpell.Radius*2, 0);
    }

    private void SelfSpell(BuffSpell someSpell)
    {
        if (!GetComponent<EffectsHandler>())
        {
            gameObject.AddComponent<EffectsHandler>();
        }
        GetComponent<EffectsHandler>().HandleEffect(gameObject, someSpell.Effect, someSpell.EffectDuration, someSpell.Amount);
    }

    private void FixPlayersLook()
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = cursorWorldPos.x > transform.position.x;
    }
}
