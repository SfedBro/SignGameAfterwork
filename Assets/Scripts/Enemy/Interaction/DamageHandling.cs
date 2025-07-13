using UnityEngine;
using System.Collections.Generic;

public class DamageHandling : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private EnemyInteractionCharacteristics stats;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private LayerMask playerLayer;

    private readonly Dictionary<int, float> lastDamageTime = new Dictionary<int, float>();

    private void Start()
    {
        if (stats != null)
        {
            damage = stats.damage;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ProcessDamage(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ProcessDamage(collision.gameObject);
    }

    private void ProcessDamage(GameObject target)
    {
        if (((1 << target.layer) & playerLayer) == 0)
            return;

        int targetId = target.GetInstanceID();

        if (lastDamageTime.ContainsKey(targetId) &&
            Time.time - lastDamageTime[targetId] < damageInterval)
            return;

        if (target.TryGetComponent(out Player player))
        {
            player.TakeDamage(damage);
            lastDamageTime[targetId] = Time.time;
        }
    }
}