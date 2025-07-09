using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{
    private GameObject player;
    private int maxHP;
    public int index;

    void Awake()
    {
        player = GameObject.Find("mage");
        if (DataContainer.checkpointIndex == index)
        {
            player.transform.position = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            player.GetComponent<Player>().IncreaseHPToFull();
            if (index > DataContainer.checkpointIndex)
            {
                DataContainer.checkpointIndex = index;
            }
            
        }
    }
}
