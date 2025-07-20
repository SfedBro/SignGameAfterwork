using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Game/EnemyStats")]
public class EnemyInteractionCharacteristics : ScriptableObject
{
    public int health;
    public int damage;
    public float visionRange;
    public float speed;
    public float acceleration;
    public float stoppingDistance;
    public bool isGround;

    [Header("Ground Enemy")]
    public float minJumpHeight;
    public float maxJumpHeight;
    public float jumpTime;
    public float gravity;
}