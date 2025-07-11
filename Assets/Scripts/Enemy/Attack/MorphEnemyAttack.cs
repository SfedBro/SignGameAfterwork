using UnityEngine;

public class MorphEnemyAttack : MonoBehaviour
{
    //[SerializeField]
    //private EnemyInteractionCharacteristics stats;
    //[SerializeField]
    //private Transform target;
    //[SerializeField]
    //private int damage;
    //[SerializeField]
    //private float stoppingDistance;
    //void Update()
    //{
        
    //}
}
public interface IAttack
{
    public void Attack(Transform target, int damage, float period, float speed = 0f, GameObject projectile = null);
}

