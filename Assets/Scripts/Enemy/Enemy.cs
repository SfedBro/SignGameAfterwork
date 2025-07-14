using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHp = 100f;

    [Header("Debug")]
    [SerializeField]
    private float hp;
    [SerializeField]
    private EnemySpawn spawn;
    public float GetHp
    {
        get
        {
            return hp;
        }
    }
    public EnemySpawn Spawn
    {
        get
        {
            return spawn;
        }
        set
        {
            spawn = value;
        }
    }

    private Color originalColor;

    void Start()
    {
        hp = maxHp;
        originalColor = this.GetComponent<SpriteRenderer>().color;
    }

    void Update()
    {

    }

    public void TakeDamage(float amount)
    {
        hp -= amount;
        Debug.Log($"{name} получил {amount} урона. Осталось HP: {hp}.");
        if (hp <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Debug.Log($"Объект {name} был уничтожен!");
        spawn.DeleteEnemy();
    }

    public void ReturnToOrig()
    {
        GetComponent<SpriteRenderer>().color = originalColor;
    }
}
