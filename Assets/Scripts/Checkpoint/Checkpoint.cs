using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int maxHill = 3;
    private int hillAttempt = 0;
    
    public int index;

    void Start()
    {
        hillAttempt = 0;
    }

    // void Awake()
    // {
    //     player = GameObject.Find("mage");
    //     if (DataContainer.checkpointIndex == index)
    //     {
    //         player.transform.position = transform.position;
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            if (hillAttempt < maxHill && !other.GetComponent<Player>().IsMaxHP())
            {
                other.GetComponent<Player>().IncreaseHPToFull();
                hillAttempt++;
            }
            if (transform.position.x > DataContainer.checkpointIndex.x)
            {
                DataContainer.checkpointIndex = transform.position;
            }
        }
    }
}
