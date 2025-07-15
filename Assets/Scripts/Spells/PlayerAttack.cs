using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float timeAvailableForCombo = 0.5f;

    private SpellCast spellCaster; // Реализация заклинаний
    private Spell newSpell; // Объект заклинания
    private SpellsManager allSpells; // Все заклинания

    private List<string> actualCombo = new();
    private float timer;
    private bool inputForTimer = false;
    private bool timerIsOn = false;


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
    }

    private void HandleInput(string symb)
    {
        inputForTimer = true;
        if (timerIsOn == false)
        {
            timerIsOn = true;
            StartCoroutine(ComboTimer(timeAvailableForCombo));
        }
        actualCombo.Add(symb);   
    }

    private IEnumerator ComboTimer(float timeAvailableForCombo)
    {
        while (timer < timeAvailableForCombo)
        {
            if (inputForTimer)
            {
                timer = 0f;
                inputForTimer = false;
            }
            else
            {
                timer += 0.1f;
            }
            yield return new WaitForSeconds(0.1f);
        }

        MakeAttack(actualCombo);
        actualCombo.Clear();
        timer = 0f;
        timerIsOn = false;
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
}