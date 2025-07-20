using UnityEngine;

public class DoorClosing : MonoBehaviour
{
    public PhaseTransition pt;
    [SerializeField]
    private GameObject doorL;
    [SerializeField]
    private GameObject doorU;
    private bool playerInside = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            doorL.SetActive(true);
            doorU.SetActive(true);
            pt.StartBattle();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && (doorL.activeSelf || doorU.activeSelf))
        {
            playerInside = false;
            doorL.SetActive(false);
            doorU.SetActive(false);
        }
    }
    private void Update()
    {
        if (!playerInside && (doorL.activeSelf || doorU.activeSelf))
        {
            doorL.SetActive(false);
            doorU.SetActive(false);
        }
    }
}
