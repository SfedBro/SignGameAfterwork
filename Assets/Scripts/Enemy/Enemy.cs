using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHp = 100f;

    [Header("Debug")]
    [SerializeField]
    private float hp;
    [SerializeField]
    private EnemySpawn spawn;
    [SerializeField] private float flashDuration = 1f;
    private ImpactFlash impactFlash;
    private SpriteRenderer spriteRenderer;
    private DamageParticles damageParticles;
    public float GetHp
    {
        get
        {
            return hp;
        }
    }
    public EnemySpawn Spawn
    {
        get
        {
            return spawn;
        }
        set
        {
            spawn = value;
        }
    }

    private Color originalColor;

    void Start()
    {
        hp = maxHp;
        originalColor = this.GetComponent<SpriteRenderer>().color;
        impactFlash = GetComponent<ImpactFlash>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        damageParticles = GetComponent<DamageParticles>();
    }

    void Update()
    {

    }

    public void TakeDamage(float amount, Vector3 direction = default)
    {
        hp -= amount;
        Debug.Log($"{name} получил {amount} урона. Осталось HP: {hp}.");
        ScreenShake.Instance.ShakeVeryLight();

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
    public void Die()
    {
        Debug.Log($"Объект {name} был уничтожен!");
        spawn.DeleteEnemy();
    }

    public void ReturnToOrig()
    {
        GetComponent<SpriteRenderer>().color = originalColor;
    }
}
