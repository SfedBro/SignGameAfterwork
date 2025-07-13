using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LayerMask), typeof(NavMeshAgent))]
public class FlyingEnemyMovement : MonoBehaviour
{
    [SerializeField]
    private EnemyInteractionCharacteristics stats;
    [SerializeField]
    private Transform target;
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
            if (stats.isGround)
            {
                Debug.Log("Wrong enemy type! :: FlyingEnemyMovement; OnValidate");
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
            target = FindFirstObjectByType<Player>().transform;
        }
    }
    void Start()
    {
        playerTag = target.gameObject.tag;
        SetAgentParameters();
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
        if (GeneralEnemyBehaviour.LookingDirectlyAtPlayer(agent.transform.position, target.position, visionRange, consideredMasks, playerTag))
        {
            if (isPatrolRunning)
            {
                isPatrolRunning = false;
                if (patrolCoroutine != null)
                {
                    StopCoroutine(patrolCoroutine);
                }
            }
            if (isWaitingForPlayer)
            {
                isWaitingForPlayer = false;
                if (waitForPlayerCoroutine != null)
                {
                    StopCoroutine(waitForPlayerCoroutine);
                }
            }
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.stoppingDistance = 0;
            if (!isPatrolRunning && !isWaitingForPlayer)
            {
                waitForPlayerCoroutine = StartCoroutine(WaitBeforePatrol());
            }
        }
    }
    private IEnumerator WaitBeforePatrol()
    {
        isWaitingForPlayer = true;
        yield return new WaitForSeconds(untilPatrolTime);
        initPatrolPosition = agent.transform.position;
        isWaitingForPlayer = false;
        isPatrolRunning = true;
        patrolCoroutine = StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        while (isPatrolRunning)
        {
            Vector2 newPos = initPatrolPosition + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * patrolRange;

            agent.SetDestination(newPos);

            while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            yield return new WaitForSeconds(untilChangeTime);
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
}
