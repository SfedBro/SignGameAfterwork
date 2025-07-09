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
                if (access == 0)
                {
                    if (other != item)
                        other.Deselect();
                }
            }

            item.Select();
            int length = description.Length;

            if (length < 30)
                descriptionField.fontSize = 500;
            else if (length < 60)
                descriptionField.fontSize = 400;
            else
                descriptionField.fontSize = 350;
            descriptionField.text = description;
        }
    }
}
