using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Rendering.Universal.Internal;
using System.Linq;

public class PlayerAttack : MonoBehaviour
{
    private SpellCast spellCaster; // Реализация заклинаний
    private Spell newSpell; // Объект заклинания
    private SpellsManager allSpells; // Все заклинания
    private List<string> actualCombo = new();


    private void Start()
    {
        if (!GetComponent<SpellCast>())
        {
            gameObject.AddComponent<SpellCast>();
        }
        if (!GetComponent<SpellsManager>())
        {
            gameObject.AddComponent<SpellsManager>();
        }
        spellCaster = GetComponent<SpellCast>();
        allSpells = GetComponent<SpellsManager>();
    }

    private void Update()
    {
        if (Time.timeScale == 1f)
        {
            KeyInput();
        }
    }
    private void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HandleInput("Fire1");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HandleInput("Fire2");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HandleInput("Fire3");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            HandleInput("Water1");
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            HandleInput("Water2");
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            HandleInput("Water3");
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            HandleInput("Air1");
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            HandleInput("Air2");
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            HandleInput("Air3");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            HandleInput("Earth1");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            HandleInput("Earth2");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            HandleInput("Earth3");
        }
    }

    public void HandleInput(string symb)
    {
        List<string> newCombo = actualCombo.ToList();
        newCombo.Add(symb);
        if (allSpells.GetSpellByCombo(string.Join("+", newCombo)) == null)
        {

            Debug.Log("Вы пытаетесь задать несуществующую комбинацию!");
        }
        else
        {
            actualCombo.Add(symb);
            ShowSigns(actualCombo);
            MakeAttack(actualCombo);
        }
    }

    private void MakeAttack(List<string> combo)
    {
        if (allSpells.GetSpellByCombo(string.Join("+", combo)) == null)
        {
            Debug.Log("Заклинание не изучено!");
        }
        else
        {
            newSpell = allSpells.GetSpellByCombo(string.Join("+", combo));
            Debug.Log($"Использована комбинация: {string.Join("+", combo)}. Заклинание: {newSpell.name}");
            spellCaster.SetSpell(newSpell);
        }
    }

    public void ClearCombo()
    {
        actualCombo.Clear();
        ShowSigns(actualCombo);
    }

    public void LeaveLast()
    {
        List<string> newCombo = new();
        newCombo.Add(actualCombo[actualCombo.Count - 1]);
        actualCombo.Clear();
        actualCombo = newCombo;
        ShowSigns(actualCombo);
        MakeAttack(actualCombo);
    }

    private void ShowSigns(List<string> signs)
    {
        if (SignCanvasController.Instance == null)
        {
            Debug.LogError("SignCanvasController instance not found!");
            return;
        }

        // if (SignCanvasController.Instance.IsAnimating())
        // {
        //     Debug.Log("SignCanvasController is currently animating, skipping test.");
        //     return;
        // }

        SignCanvasController.Instance.SetSigns(signs);
    }
}