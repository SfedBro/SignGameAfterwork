using UnityEngine;

public class HealthAndPhases : MonoBehaviour
{
    [SerializeField]
    private float health1;
    [SerializeField]
    private float health2;
    [SerializeField]
    private Enemy healthScript;
    [SerializeField, Range(1, 2)]
    private int phase;
    [SerializeField]
    private float currentHealth;
}
