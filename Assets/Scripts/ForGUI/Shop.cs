using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    // Variable for specifying a price and inserting it into a TextMeshProUGUI
    [SerializeField] private int price;
    [SerializeField] private string objectName;
    [SerializeField] private GameObject block;
    [SerializeField] private TextMeshProUGUI objectPrice;
    [SerializeField] private TextMeshProUGUI coinCounter;
    private DropHPController hpDrop;
    private int access;
    // for test
    public bool fl = false;
    // for test

    void Awake()
    {
        // for test
        if (fl == false)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("coins", 1000);
            fl = true;
        }
        // for test
        hpDrop = GetComponent<DropHPController>();
        AccessUpdate();
    }

    void AccessUpdate()
    {
        access = PlayerPrefs.GetInt(objectName + "Access");
        objectPrice.text = price.ToString();

        if (access == 1)
        {
            block.SetActive(true);
            objectPrice.gameObject.SetActive(false);
        }
        else
        {
            block.SetActive(false);
            objectPrice.gameObject.SetActive(true);
        }
    }

    public void OnButtonDown()
    {
        int coins = PlayerPrefs.GetInt("coins");

        if (access == 0)
        {
            if (coins >= price)
            {
                PlayerPrefs.SetInt(objectName + "Access", 1);
                PlayerPrefs.SetInt("coins", coins - price);
                coinCounter.text = PlayerPrefs.GetInt("coins").ToString();
                hpDrop.DropHP();
                AccessUpdate();
            }
            else
            {
                Debug.Log("Нет денег");
            }
        }
    }
}
