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
    public void Attack(int damage, Transform target);
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
    public void Attack(int damage, Transform target)
    {
        target.GetComponent<Player>().TakeDamage(damage);
    }
}
public class RangedAttack : MonoBehaviour, IAttack
{
    [SerializeField]
    private EnemyInteractionCharacteristics stats;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float stoppingDistance;
    public void Attack(int damage, Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 position = transform.position + direction;
        GameObject proj = Instantiate(projectile, position, Quaternion.LookRotation(direction));
        proj.GetComponent<EnemyProjectile>().
    }
}
