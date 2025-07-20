using UnityEngine;

[RequireComponent (typeof(PhaseTransition))]
public class HealthAndPhases : MonoBehaviour
{
    [SerializeField]
    private float health1;
    [SerializeField]
    private float health2;
    public GameObject phase1Object;
    public GameObject phase2Object;
    [SerializeField]
    private Enemy healthScript;
    [SerializeField, Range(1, 2)]
    private int phase;
    [SerializeField]
    private float currentHealth;
    public PhaseTransition phaseTransition;

    private void Awake()
    {
        if (phase1Object != null && phase1Object.GetComponent<Enemy>())
        {
            phase1Object.SetActive(true);
            phase1Object.GetComponent<Enemy>().maxHp = health1;
        }
        else
        {
            Debug.Log("No phase 1 Boss object or Enemy on it");
            Application.Quit();
        }
        //if (phase2Object != null && phase2Object.GetComponent<Enemy>())
        //{
        //    phase2Object.GetComponent<Enemy>().maxHp = health2;
        //    phase2Object.SetActive(false);
        //}
        //else
        //{
        //    Debug.Log("No phase 2 Boss object or Enemy on it");
        //    Application.Quit();
        //}
        if (phaseTransition == null)
        {
            phaseTransition = GetComponent<PhaseTransition>();
        }
        phase = 1;
    }
    private void Update()
    {
        if (phase == 1)
        {
            health1 = phase1Object.GetComponent<Enemy>().GetHp;
        }
        //else
        //{
        //    health2 = phase2Object.GetComponent<Enemy>().GetHp;
        //}
        if (phase1Object == null || health1 == 0)
        {
            phase = 2;
            //phaseTransition.ChangePhase();
            if (phase1Object != null)
            {
                phase1Object.SetActive(false);
            }
            Debug.LogErrorFormat("You won!");
            //phase2Object.SetActive(true);
        }
        //if (phase2Object == null || health2 == 0)
        //{
        //    if (phase2Object != null)
        //    {
        //        phase2Object.SetActive(false);
        //    }
        //    Debug.Log("Player won!");
        //}
    }
}
