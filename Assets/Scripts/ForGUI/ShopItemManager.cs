using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopItemManager : MonoBehaviour
{
    [SerializeField] private List<Shop> items;
    [SerializeField] private TextMeshProUGUI descriptionField;
    private int access = 0;


    public void OnItemClicked(Shop item, string description)
    {
        int itemAccess = PlayerPrefs.GetInt(item.objectName + "Access");
        if (itemAccess == 0)
        {
            foreach (var other in items)
            {
                access = PlayerPrefs.GetInt(other.objectName + "Access");
                Debug.Log(access);
                if (access == 0)
                {
                    if (other != item)
                        other.Deselect();
                }
            }

            item.Select();
            descriptionField.text = description;
        }
    }
}
