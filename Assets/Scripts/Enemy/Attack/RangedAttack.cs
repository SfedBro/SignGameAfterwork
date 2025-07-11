using System.Collections;
using UnityEngine;

public class RangedAttack : MonoBehaviour, IAttack
{
    private int dmg;
    private float spd;
    private GameObject proj;
    private Vector3 position;
    private Vector3 direction;
    private Coroutine attackCoroutine;
    public void Attack(Transform target, int damage, float period, float speed = 0f, GameObject projectile = null)
    {
        direction = (target.position - transform.position).normalized;
        position = transform.position + direction;
        //Debug.Log(position.ToString());
        dmg = damage;
        spd = speed;
        proj = projectile;
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackDelay(period));
        }
    }
    private IEnumerator AttackDelay(float time)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject projInstance = Instantiate(proj, position, Quaternion.Euler(0, 0, angle));
        projInstance.GetComponent<EnemyProjectile>().Damage = dmg;
        projInstance.GetComponent<EnemyProjectile>().Speed = spd;
        yield return new WaitForSecondsRealtime(time);
        attackCoroutine = null;
    }
}
