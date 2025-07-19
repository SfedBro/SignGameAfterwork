using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHp = 100f;

    [Header("Death Effects")]
    [SerializeField] private int coinReward = 3;
    [SerializeField] private float deathEffectDuration = 0.5f;
    [SerializeField] private GameObject explosionControllerPrefab;
    [SerializeField] private ParticleSystem blowEffect;

    [Header("Debug")]
    [SerializeField] private float hp;
    [SerializeField] private EnemySpawn spawn;
    [SerializeField] private float flashDuration = 0.06f;
    private ImpactFlash impactFlash;
    [SerializeField] private List<SpriteRenderer> spriteRenderers;
    private DamageParticles damageParticles;
    private ExplosionController explosionController;

    private bool isDying = false;

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

        if (explosionControllerPrefab != null)
        {
            GameObject explosionObject = Instantiate(explosionControllerPrefab, transform);
            explosionController = explosionObject.GetComponent<ExplosionController>();

            if (explosionController != null)
            {
                explosionController.SetCoinCount(coinReward);
            }
        }
    }

    void Update()
    {
    }

    public void TakeDamage(float amount, Vector3 direction = default)
    {
        if (isDying) return;

        hp -= amount;
        Debug.Log($"{name} получил {amount} урона. Осталось HP: {hp}.");
        ScreenShake.Instance.ShakeVeryLight();

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
        if (isDying) return;

        isDying = true;
        Debug.Log($"Объект {name} был уничтожен!");

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        if (impactFlash != null)
        {
            foreach (var sprite in spriteRenderers)
            {
                if (sprite != null)
                    impactFlash.Flash(sprite, 0.2f);
            }
        }

        if (damageParticles != null)
        {
            damageParticles.PlayMediumSparkEffect(transform.position);
        }

        yield return new WaitForSeconds(0.1f);

        ScreenShake.Instance.ShakeLight();

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        BlowEffect();
        if (explosionController != null)
        {
            explosionController.TriggerExplosion();
        }
        yield return new WaitForSeconds(deathEffectDuration - 0.1f);


        //foreach (var sprite in spriteRenderers)
        //{
         //   if (sprite != null)
//sprite.color = Color.clear;
       // }
        //BlowEffect();

        yield return new WaitForSeconds(0.2f);
        if (spawn != null)
        {
            spawn.DeleteEnemy();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void BlowEffect()
    {
        if (!gameObject.scene.isLoaded) return;
        ParticleSystem effect = Instantiate(blowEffect, transform.position, Quaternion.identity);
        effect.Play();
    }

    public void ReturnToOrig()
    {
        GetComponent<SpriteRenderer>().color = originalColor;
    }
}