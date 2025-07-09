using UnityEngine;

public class MorphEnemyAttack : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
public interface IAttack
{
    public void Attack(Transform target, int damage, float speed = 0f, GameObject projectile = null);
}
public class CombatAttack : MonoBehaviour, IAttack
{
    [SerializeField]
    private EnemyInteractionCharacteristics stats;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float stoppingDistance;
    public void Attack(Transform target, int damage, float speed = 0f, GameObject projectile = null)
    {
        target.GetComponent<Player>().TakeDamage(damage);
    }
}
public class RangedAttack : MonoBehaviour, IAttack
{
    public void Attack(Transform target, int damage, float speed = 0f, GameObject projectile = null)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 position = transform.position + direction;
        GameObject proj = Instantiate(projectile, position, Quaternion.LookRotation(direction));
        proj.GetComponent<EnemyProjectile>().Damage = damage;
        proj.GetComponent<EnemyProjectile>().Speed = speed;
    }
}
