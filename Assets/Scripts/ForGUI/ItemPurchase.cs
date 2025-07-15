using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPurchase : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float changeSpeed = 0.7f;
    // [SerializeField] private float speedBoostDuration = 5f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Purchase(string objectName)
    {
        if (player != null)
        {
            switch (objectName)
            {
                case "ХП":
                    Debug.Log("хп");
                    int hp = player.GetComponent<Player>().GetHP();
                    if (hp < 10)
                    {
                        player.GetComponent<Player>().IncreaseHP(1);
                    }
                    break;
                case "Осколок Жизни":
                    Debug.Log("Осколок жизни");
                    player.GetComponent<Player>().ChangeDeathScreenBool();
                    break;
                case "Сапоги-скороходы":
                    Debug.Log("сапоги-скороходы");
                    {
                        if (player != null && player.CompareTag("Player"))
                        {
                            Debug.Log($"Игрок получил ускорение на {changeSpeed} навсегда");
                            player.GetComponent<PlayerController>().SpeedChange(changeSpeed);
                        }
                    }
                    break;
            }
        }
        else
        {
            Debug.Log("Player не найден ItemPurchase");
        }
    }
}
