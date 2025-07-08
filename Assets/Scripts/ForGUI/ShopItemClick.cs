using UnityEngine;

public class ShopItemClick : MonoBehaviour
{
    [SerializeField] private Shop itemUI;
    [SerializeField] private ShopItemManager manager;
    [TextArea] [SerializeField] private string description;

    public void OnClick()
    {
        Debug.Log("OnClick shopitemclick");
        manager.OnItemClicked(itemUI, description);
    }
}

