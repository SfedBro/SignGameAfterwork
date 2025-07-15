using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LayerMask), typeof(NavMeshAgent))]
[RequireComponent(typeof(IAttack))]
public class FlyingEnemyMovement : MonoBehaviour
{
    [SerializeField]
    private EnemyInteractionCharacteristics stats;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private IAttack attackScript;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float attackPeriod;
    [SerializeField]
    private bool isRanged;
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private float projSpeed;
    [SerializeField]
    private LayerMask consideredMasks;
    private string playerTag;
    private NavMeshAgent agent;
    [SerializeField, Range(0f, 100f)]
    private float speed;
    [SerializeField, Range(0f, 50f)]
    private float acceleration;
    [SerializeField]
    private float stoppingDistance;
    [SerializeField]
    private float visionRange;
    //Patrolling
    [SerializeField]
    private float untilPatrolTime;
    [SerializeField]
    private float untilChangeTime;
    [SerializeField]
    private float patrolRange;
    [SerializeField]
    private bool isPatrolRunning;
    [SerializeField]
    private bool isWaitingForPlayer;
    private Coroutine waitForPlayerCoroutine;
    private Coroutine patrolCoroutine;
    //private Coroutine shootingCoroutine;
    private Vector2 initPatrolPosition;
    public Transform Target
    {
        set
        {
            target = value;
        }
        get
        {
            return target;
        }
    }
    public NavMeshAgent EnemyAgent
    {
        get
        {
            return agent;
        }
    }
    private void OnValidate()
    {
        if (stats != null)
        {
            visionRange = stats.visionRange;
            speed = stats.speed;
            acceleration = stats.acceleration;
            stoppingDistance = stats.stoppingDistance;
            damage = stats.damage;
            if (stats.isGround)
            {
                Debug.Log("Wrong enemy type! :: FlyingEnemyMovement; OnValidate");
            }
            if (GetComponent<Enemy>())
            {
                GetComponent<Enemy>().maxHp = stats.health;
            }
        }
    }
    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (target == null)
        {
            target = transform;
            if (FindFirstObjectByType<Player>()?.transform != null)
            {
                target = FindFirstObjectByType<Player>().transform;
            }
        }
        if (attackScript == null)
        {
            attackScript = GetComponent<IAttack>();
        }
    }
    void Start()
    {
        playerTag = target.gameObject.tag;
        SetAgentParameters();
        isRanged = attackScript is RangedAttack;
        if (isRanged)
        {
            if (projectile == null)
            {
                Debug.LogWarning("No projectile on ranged enemy: " + this.name.ToString());
            }
        }
        if (damage < 0)
        {
            damage = 0;
            Debug.LogWarning("Damage on enemy " + this.name.ToString() + " was nullified due to negative value");
        }
    }
    private void SetAgentParameters()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
        agent.acceleration = acceleration;
        agent.stoppingDistance = stoppingDistance;
    }

    void Update()
    {
        if (GeneralEnemyBehaviour.LookingDirectlyAtPlayer(agent.transform.position, target.position, visionRange, consideredMasks, target))
        {
            if (waitForPlayerCoroutine != null)
            {
                StopCoroutine(waitForPlayerCoroutine);
                waitForPlayerCoroutine = null;
                isWaitingForPlayer = false;
            }
            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
                patrolCoroutine = null;
                isPatrolRunning = false;
            }
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(target.position);
            if ((agent.transform.position - target.position).magnitude <= stoppingDistance)
            {
                if (attackScript != null)
                {
                    if (isRanged)
                    {
                        attackScript.Attack(target, damage, attackPeriod, projSpeed, projectile);
                    }
                    else
                    {
                        attackScript.Attack(target, damage, attackPeriod);
                    }
                }
            }
        }
        else
        {
            agent.stoppingDistance = 0;
            if (!isPatrolRunning && !isWaitingForPlayer)
            {
                waitForPlayerCoroutine = StartCoroutine(WaitBeforePatrol(untilPatrolTime));
            }
        }
    }
    private IEnumerator WaitBeforePatrol(float time)
    {
        isWaitingForPlayer = true;
        yield return new WaitForSeconds(time);
        initPatrolPosition = agent.transform.position;
        isWaitingForPlayer = false;
        isPatrolRunning = true;
        patrolCoroutine = StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        bool isAtPlace = false;
        Vector2 newPos;
        while (isPatrolRunning)
        {
            isAtPlace = agent.remainingDistance <= agent.stoppingDistance;
            if (isAtPlace)
            {
                newPos = initPatrolPosition + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * patrolRange;
                agent.SetDestination(newPos);
                yield return new WaitForSeconds(untilChangeTime);
            }
            else
            {
                yield return null;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        float circleSegments = 36;
        float radius = visionRange;
        Vector2 center = transform.position;
        float angleStep = 360f / circleSegments;

        Vector2 prevPoint = center + new Vector2(Mathf.Cos(0), Mathf.Sin(0)) * radius;

        for (int i = 1; i <= circleSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 nextPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        if (isPatrolRunning)
        {
            Gizmos.color = Color.red;
            float radius2 = patrolRange;
            Vector2 center2 = initPatrolPosition;
            prevPoint = center2 + new Vector2(Mathf.Cos(0), Mathf.Sin(0)) * radius2;

            for (int i = 1; i <= circleSegments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector2 nextPoint = center2 + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius2;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
    }

    public void SpeedChange(float amount)
    {
        agent.speed = speed + speed*amount;
    }
}
