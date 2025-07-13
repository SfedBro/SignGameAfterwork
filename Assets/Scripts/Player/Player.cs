using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    public float iSeconds = 2f;

    [Header("Debug")]
    [SerializeField] private int hp;
    [SerializeField] private int maxHP = 10;
    [SerializeField] float iSecondsCount = 2;
    [Header("DamageEffect")]
    [SerializeField] private float flashDuration = 1f;
    private ImpactFlash impactFlash;
    private SpriteRenderer spriteRenderer;
    private DamageParticles damageParticles;
    [SerializeField] private GameObject deathScreenPrefab;
    private GameObject deathScreenInstance;
    private CharacterController characterControl;
    private Player Instance;
    private bool isDead = false;
    private bool isDeathScreenUsing = true;
    private Color originalColor;

    void Awake()
    {
        Instance = this;

        transform.position = DataContainer.checkpointIndex;

        if (deathScreenPrefab != null)
        {
            deathScreenInstance = Instantiate(deathScreenPrefab);
            deathScreenInstance.SetActive(false);
        }
        else
        {
            Debug.Log("DeathScreenPrefab не найден");
        }
        originalColor = this.GetComponent<SpriteRenderer>().color;
    }

    void Start()
    {
        isDead = false;
        hp = maxHP;
        impactFlash = GetComponent<ImpactFlash>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        damageParticles = GetComponent<DamageParticles>();
        characterControl = GetComponent<CharacterController>();
        isDeathScreenUsing = true;
        AddDScreenButtonFunctions();
    }
    private void AddDScreenButtonFunctions()
    {
        if (deathScreenPrefab != null)
        {
            var buttons = deathScreenInstance.GetComponentsInChildren<Button>();
            if (buttons.Length >= 2)
            {
                buttons[0].onClick.RemoveAllListeners();
                buttons[1].onClick.RemoveAllListeners();
                buttons[0].onClick.AddListener(GameManager.I.RestartGame);
                buttons[1].onClick.AddListener(GameManager.I.ToMainMenu);
            }
        }
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

    public void TakeDamage(int damage, Vector3 direction = default)
    {
        if (hp <= 0 || iSecondsCount > 0) return;
        hp = Mathf.Max(hp - damage, 0);
        iSecondsCount = iSeconds;
        Debug.Log($"HP {hp}");
        if (hp <= 0) GameManager.I.PlayerDied();
        if (impactFlash != null)
        {
            impactFlash.Flash(spriteRenderer, flashDuration);
        }
        if (direction == default)
        {
            damageParticles.PlayMediumSparkEffect(transform.position);
        }
        else
        {
            Vector2 fixedDirection = new Vector2(direction.x, direction.y);
            damageParticles.PlayMediumSparkEffect(transform.position, fixedDirection);
        }
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        // GameManager.I.PlayerDied();
        Time.timeScale = 0;

        if (characterControl != null)
            characterControl.enabled = false;
        if (isDeathScreenUsing)
        {
            if (deathScreenInstance != null)
            {
                deathScreenInstance.SetActive(true);
            }
        }
        else
        {
            DataContainer.checkpointIndex = transform.position;
            Time.timeScale = 1f;
            Debug.Log("перерождаемся на том же месте");
            GameManager.I.RespawnWithouDeathScreen();
        }
    }

    public void ChangeDeathScreenBool()
    {
        isDeathScreenUsing = false;
    }
    
    public void ReturnToOrig()
    {
        GetComponent<SpriteRenderer>().color = originalColor;
    }
}
