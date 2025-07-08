using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float iSeconds = 2f;

    [Header("Debug")]
    [SerializeField] private int hp;
    [SerializeField] private int maxHP = 10;
    [SerializeField] float iSecondsCount = 2;
    [SerializeField] private GameObject deathScreenPrefab;
    private GameObject deathScreenInstance;
    private Player Instance;
    private bool isDead = false;


    void Awake()
    {
        Instance = this;

        if (deathScreenPrefab != null)
        {
            deathScreenInstance = Instantiate(deathScreenPrefab);
            deathScreenInstance.SetActive(false);

            var buttons = deathScreenInstance.GetComponentsInChildren<Button>();
            if (buttons.Length >= 2)
            {
                buttons[0].onClick.RemoveAllListeners();
                buttons[1].onClick.RemoveAllListeners();

                buttons[0].onClick.AddListener(GameManager.I.RestartGame);
                buttons[1].onClick.AddListener(GameManager.I.ToMainMenu);
            }
        }
        else
        {
            Debug.Log("DeathScreenPrefab не найден");
        }
    }

    void Start()
    {
        isDead = false;
        hp = maxHP;
    }

    // for test
    void Update()
    {
        iSecondsCount = Mathf.Max(iSecondsCount - Time.deltaTime, 0);
        if (Input.GetKeyDown(KeyCode.T))
        {
            hp--;
            Debug.Log($"Player HP: {hp}");
        }
        // for test
        if (hp <= 0 && !isDead)
        {
            Die();
        }
    }

    public int GetHP()
    {
        return hp;
    }

    public void IncreaseHP(int plusHP)
    {
        hp += plusHP;
        if (hp > maxHP)
            hp = maxHP;
    }

    public void IncreaseHPToFull()
    {
        hp = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (hp <= 0 || iSecondsCount > 0) return;
        hp = Mathf.Max(hp - damage, 0);
        iSecondsCount = iSeconds;
        Debug.Log($"HP {hp}");
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        // GameManager.I.PlayerDied();
        
        Debug.Log("Die");
        Time.timeScale = 0;

        var characterControl = GetComponent<CharacterController>();
        if (characterControl != null)
            characterControl.enabled = false;

        if (deathScreenInstance != null)
        {
            deathScreenInstance.SetActive(true);
        }
    }
}
