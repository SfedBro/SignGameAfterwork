using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private float flashDuration = 0.06f;
    private ImpactFlash impactFlash;
    [SerializeField] private List<SpriteRenderer> spriteRenderers;
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
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
        damageParticles = GetComponent<DamageParticles>();
    }

    void Update()
    {

    }

    public void TakeDamage(float amount, Vector3 direction = default)
    {
        hp -= amount;
        Debug.Log($"{name} получил {amount} урона. Осталось HP: {hp}.");
        // не просто так я не использую его, включать на свой страх и риск
        // ScreenShake.Instance.ShakeVeryLight();

        if (impactFlash != null)
        {
            foreach (var sprite in spriteRenderers)
            {
                if (sprite != null)
                    impactFlash.Flash(sprite, flashDuration);
            }
        }
        if (direction == Vector3.zero)
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
        if (spawn != null)
        {
            spawn.DeleteEnemy();
        } else
        {
            Destroy(gameObject);
        }
    }

    public void ReturnToOrig()
    {
        GetComponent<SpriteRenderer>().color = originalColor;
    }
}
