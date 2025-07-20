using UnityEngine;

public class DoorClosing : MonoBehaviour
{
    [SerializeField]
    private GameObject door;
    private bool playerInside = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            door.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && door.activeSelf)
        {
            playerInside = false;
            door.SetActive(false);
        }
    }
    private void Update()
    {
        if (!playerInside && door.activeSelf)
        {
            door.SetActive(false);
        }
    }
}
