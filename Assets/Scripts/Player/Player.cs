using UnityEngine;
[RequireComponent (typeof(SpriteRenderer))]
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
    void Start()
    {
        hp = maxHP;
        impactFlash = GetComponent<ImpactFlash>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
    }
    // for test

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

    public void TakeDamage(int damage, Vector3 ? direction = null) {
        if (hp <= 0 || iSecondsCount > 0) return;
        hp = Mathf.Max(hp - damage, 0);
        iSecondsCount = iSeconds;
        Debug.Log($"HP {hp}");
        if (hp <= 0) GameManager.I.PlayerDied();
        if (impactFlash != null)
        {
            impactFlash.Flash(spriteRenderer, flashDuration);
        }
        if (direction == null)
        {

        }
        else
        {

        }
    }
}
