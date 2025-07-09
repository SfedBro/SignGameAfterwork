using System.Collections;
using UnityEngine;

public class ItemPurchase : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float changeSpeed = 0.7f;
    // [SerializeField] private float speedBoostDuration = 5f;

    public void Purchase(string objectName)
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
                break;
            case "Сапоги-скороходы":
                Debug.Log("сапоги-скороходы");
                {
                    if (player != null && player.CompareTag("Player"))
                    {
                        Debug.Log($"Игрок получил ускорение на {changeSpeed} навсегда");
                        player.GetComponent<PlayerController>().SpeedChange(changeSpeed);
                    }

                    // float timer = 0f;

                    // while (timer < speedBoostDuration)
                    // {
                    //     timer += 1f;
                    //     yield return new WaitForSeconds(1f);
                    // }

                    // if (player != null && player.CompareTag("Player"))
                    // {
                    //     player.GetComponent<PlayerController>().SpeedChange(-changeSpeed);
                    // }
                }
                break;
        }
    }
}
