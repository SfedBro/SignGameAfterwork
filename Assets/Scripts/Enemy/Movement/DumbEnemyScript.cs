using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D), typeof(LayerMask))]
public class DumbEnemyScript : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private bool isMovingRight;
    [SerializeField]
    private LayerMask consideredMasks;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private int damage;
    private CapsuleCollider2D enemyCollider;
    private Transform target;
    private int multiplier;
    private void Awake()
    {
        if (target == null)
        {
            target = FindFirstObjectByType<Player>().transform;
        }
    }
    private void Start()
    {
        isMovingRight = Random.Range(0, 2) == 0;
        enemyCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        multiplier = isMovingRight ? 1 : -1;
        if (ShouldRotate())
        {
            isMovingRight = !isMovingRight;
        }
        if (!ShouldStop())
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + multiplier * Vector3.right, speed * Time.deltaTime);
        }
        else
        {
            Attack();
        }
    }

    private bool ShouldRotate()
    {
        bool isObstacleAhead = Physics2D.CapsuleCast(enemyCollider.bounds.center, enemyCollider.size, enemyCollider.direction, 0f, new Vector2(multiplier, 0), attackRange, consideredMasks);
        bool isGroundAhead = Physics2D.CapsuleCast(enemyCollider.bounds.center + Vector3.right * multiplier, enemyCollider.size, enemyCollider.direction, 0f, Vector2.down, attackRange, consideredMasks);
        return !isGroundAhead || isObstacleAhead;
    }
    //Stop, not rotate!
    private bool ShouldStop()
    {
        RaycastHit2D hit = Physics2D.CapsuleCast(enemyCollider.bounds.center, enemyCollider.size, enemyCollider.direction, 0f, new Vector2(multiplier, 0), attackRange, consideredMasks);
        if (hit)
        {
            if (hit.transform == target)
            {
                return true;
            }
        }
        return false;
    }
    private void Attack()
    {
        target.gameObject.GetComponent<Player>().TakeDamage(damage);
        //Attack animation Here
    }
}
