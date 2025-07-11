using System.Collections;
using UnityEngine;

public class CombatAttack : MonoBehaviour, IAttack
{
    private int dmg;
    private Coroutine attackCoroutine;
    public void Attack(Transform target, int damage, float period, float speed = 0f, GameObject projectile = null)
    {
        dmg = damage;
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackDelay(period, target));
        }
    }
    private IEnumerator AttackDelay(float time, Transform target)
    {
        target.GetComponent<Player>().TakeDamage(dmg);
        yield return new WaitForSecondsRealtime(time);
        attackCoroutine = null;
    }
}