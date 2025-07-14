using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(CapsuleCollider2D), typeof(LayerMask), typeof(NavMeshAgent))]
public class LandEnemyMovement : MonoBehaviour
{
    //ScriptableObject
    [SerializeField]
    private EnemyInteractionCharacteristics stats;
    //Target detection
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float visionRange;
    [SerializeField]
    private LayerMask consideredMasks;
    [SerializeField]
    private LayerMask notGroundMasks;
    private string playerTag;
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
    //NavMesh parameters
    private NavMeshAgent agent;
    [SerializeField, Range(0f, 100f)]
    private float speed;
    [SerializeField, Range(0f, 50f)]
    private float acceleration;
    [SerializeField]
    private float stoppingDistance;
    //Physics
    [SerializeField]
    private float groundDetectionOffset;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float minJumpHeight;
    [SerializeField]
    private float jumpTime;
    [SerializeField]
    private CapsuleCollider2D enemyCollider;
    [SerializeField]
    private bool onGround = false;
    [SerializeField, Range(-50f, 50f)]
    private float verticalSpeed;
    [SerializeField]
    private float gravity;
    //Other
    [SerializeField]
    private bool isJumping;
    [SerializeField]
    private Vector3[] corners;
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
                jumpHeight = stats.maxJumpHeight;
                minJumpHeight = stats.minJumpHeight;
                jumpTime = stats.jumpTime;
                gravity = stats.gravity;
            }
            else
            {
                Debug.Log("Wrong enemy type! :: LandEnemyMovement; OnValidate");
            }
        }
    }
    void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (enemyCollider == null)
        {
            enemyCollider = GetComponent<CapsuleCollider2D>();
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
        if (ShouldJump())
        {
            StartCoroutine(JumpBezier());
        }
        if (!isJumping)
        {
            IsStanding();
            HandleGravity();
            FollowPlayer();
        }
    }
    private IEnumerator JumpBezier()
    {
        isJumping = true;
        Vector2 startPos = corners[0];
        Vector2 endPos = corners[1];

        float peakY = Mathf.Max(startPos.y, endPos.y) * 1.1f;
        Vector2 controlPoint = (startPos + endPos) * 0.5f;
        controlPoint.y = peakY;

        float time = 0f;
        while (time < jumpTime)
        {
            float t = time / jumpTime;
            Vector2 bezierPos = Mathf.Pow(1 - t, 2) * startPos + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * endPos;
            agent.transform.position = bezierPos;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        agent.transform.position = endPos;
        isJumping = false;
    }
    private void IsStanding()
    {
        onGround = (Physics2D.CapsuleCast(enemyCollider.bounds.center, enemyCollider.size, enemyCollider.direction, 0, Vector2.down, groundDetectionOffset, ~notGroundMasks));
    }
    private bool ShouldJump()
    {
        corners = agent.path.corners;
        if (corners.Length >= 2 && onGround && !isJumping)
        {
            Vector3 pos1 = corners[0];
            Vector3 pos2 = corners[1] + agent.radius * Vector3.up;
            float heightDifference = Mathf.Abs(pos1.y - pos2.y);
            if (heightDifference <= jumpHeight && heightDifference >= minJumpHeight)
            {
                //Debug.Log("Wanna jump");
                if (Mathf.Abs(pos1.x - pos2.x) <= speed * jumpTime && (GeneralEnemyBehaviour.LookingDirectlyAtPosition(pos1, pos2, consideredMasks) || GeneralEnemyBehaviour.LookingDirectlyAtPlayer(pos1, pos2, visionRange, consideredMasks, playerTag)))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void HandleGravity()
    {
        if (!onGround)
        {
            verticalSpeed -= gravity * Time.deltaTime;
        }
        else
        {
            verticalSpeed = 0f;
        }
        Vector2 currentPos = agent.transform.position;
        Vector2 nextPos = new Vector2(currentPos.x, currentPos.y + verticalSpeed * Time.deltaTime);
        if (IsPointOnNavMesh(nextPos))
        {
            agent.transform.position = nextPos;
        }
    }
    private bool IsPointOnNavMesh(Vector2 point, float maxDistance = 1.0f)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(point, out hit, maxDistance, NavMesh.AllAreas);
    }
    private void FollowPlayer()
    {
        Vector2 agentPos = agent.transform.position;
        Vector2 targetPos = target.position;
        if (GeneralEnemyBehaviour.LookingDirectlyAtPlayer(agentPos, targetPos, visionRange, consideredMasks, playerTag))
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
            Vector2 goalPosition = (Mathf.Abs(targetPos.y - agentPos.y) >= minJumpHeight) ? targetPos : new Vector2(targetPos.x, agentPos.y);
            if ((agentPos - targetPos).magnitude > stoppingDistance)
            {
                agent.SetDestination(goalPosition);
            }
            else
            {
                agent.SetDestination(agentPos);
            }
        }
        else
        {
            if (((Vector2)agent.destination - agentPos).magnitude < stoppingDistance)
            {
                if (!isPatrolRunning & !isWaitingForPlayer)
                {
                    waitForPlayerCoroutine = StartCoroutine(WaitForPlayer(untilPatrolTime));
                    isWaitingForPlayer = true;
                }
            }
        }
    }
    private IEnumerator WaitForPlayer(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        isWaitingForPlayer = false;
        waitForPlayerCoroutine = null;
        isPatrolRunning = true;
        patrolCoroutine = StartCoroutine(Patrol(untilChangeTime));
    }
    private IEnumerator Patrol(float changePosTime = 1f)
    {
        Vector2 initPos = agent.transform.position;
        initPatrolPosition = initPos;
        while (isPatrolRunning)
        {
            yield return new WaitForSecondsRealtime(changePosTime);
            if ((agent.transform.position - agent.destination).magnitude < stoppingDistance)
            {
                changePosition(initPos, patrolRange);
            }
        }
        patrolCoroutine = null;
    }
    private void changePosition(Vector2 initialPosition, float patrolRange)
    {
        Vector2 offsetPosition = initialPosition + Random.Range(-patrolRange, patrolRange) * Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(offsetPosition, Vector2.down, jumpHeight, ~notGroundMasks);
        while (!hit)
        {
            offsetPosition = initialPosition + Random.Range(-patrolRange, patrolRange) * Vector2.right;
            hit = Physics2D.Raycast(offsetPosition, Vector2.down, jumpHeight, ~notGroundMasks);
        }
        if (hit.collider != null && isPatrolRunning)
        {
            Vector2 newPosition = new(offsetPosition.x, hit.point.y + agent.radius);
            agent.SetDestination(newPosition);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector2 startAbove = transform.position;
        Vector2 endAbove = startAbove + Vector2.up * jumpHeight;
        Gizmos.DrawLine(startAbove, endAbove);

        Vector2 startBelow = transform.position;
        Vector2 endBelow = startBelow - Vector2.up * jumpHeight;
        Gizmos.DrawLine(startBelow, endBelow);

        Gizmos.color = Color.blue;
        startAbove = transform.position;
        endAbove = startAbove + Vector2.up * minJumpHeight;
        Gizmos.DrawLine(startAbove, endAbove);

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