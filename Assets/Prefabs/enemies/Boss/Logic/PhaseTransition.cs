using UnityEngine;

public class PhaseTransition : MonoBehaviour
{
    public Phases phases;
    public Collider2D col1;
    public Collider2D col2;

    public void StartBattle() {
        phases.isPhaseInProcess = true;
        phases.StartPhaseOne();
    }
    public void ChangePhase()
    {
        phases.isPhaseInProcess = false;
        phases.StartPhaseTwo();
    }
}
