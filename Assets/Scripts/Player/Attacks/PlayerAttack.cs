using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public string enemyTag = "Enemy";
    public float timeAvailableForCombo = 1f;
    public bool lookRight;

    private GameObject shootPos;
    private SpellCast spell; // реализация разных видов заклинаний
    private List<string> actualCombo = new List<string>();
    private float timer;
    private bool inputForTimer = false;
    private bool timerIsOn = false;

    
    private void Start()
    {
        spell = GetComponent<SpellCast>();
        shootPos = transform.Find("ShootPos").gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1 was pressed");
            HandleInput("Fire1");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2 was pressed");
            HandleInput("Fire2");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("3 was pressed");
            HandleInput("Fire3");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("4 was pressed");
            HandleInput("Earth1");
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("5 was pressed");
            HandleInput("Venom1");
        }

        if (gameObject.GetComponent<SpriteRenderer>().flipX == true)
        {
            shootPos.transform.position = gameObject.transform.position + new Vector3(0.3f, -0.3f, 0f);
            lookRight = true;
        }
        else
        {
            shootPos.transform.position = gameObject.transform.position + new Vector3(-0.3f, -0.3f, 0f);
            lookRight = false;
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
                Debug.Log("Timer started!");
            }
            else
            {
                Debug.Log($"timer: {timer}");
                timer += 0.1f;
            }
            yield return new WaitForSeconds(0.1f);
        }

        MakeAttack(actualCombo);
        Debug.Log("Timer is out!");
        actualCombo.Clear();
        timer = 0f;
        timerIsOn = false;
    }

    private void MakeAttack(List<string> combo)
    {
        spell.HandleSpell(string.Join("+", combo));
        Debug.Log($"Использована комбинация: {string.Join("+", combo)}");
    }
}