using UnityEngine;

public enum UIScreen
{
    None,
    MainCanvas,
    Settings,
    Shop,
    Map,
    SpellBook
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private GameObject mapCanvas;
    [SerializeField] private GameObject spellBookCanvas;

    private UIScreen currentScreen = UIScreen.None;

    private void Start()
    {
        ShowScreen(UIScreen.MainCanvas);
    }

    public void ShowScreen(UIScreen screen)
    {
        HideAll();

        switch (screen)
        {
            case UIScreen.MainCanvas:
                if (mainCanvas != null)
                    mainCanvas.SetActive(true);
                break;
            case UIScreen.Settings:
                if (settingsCanvas != null)
                    settingsCanvas.SetActive(true);
                break;
            case UIScreen.Shop:
                if (shopCanvas != null)
                    shopCanvas.SetActive(true);
                break;
            case UIScreen.Map:
                if (mapCanvas != null)
                    mapCanvas.SetActive(true);
                break;
            case UIScreen.SpellBook:
                if (spellBookCanvas != null)
                    spellBookCanvas.SetActive(true);
                break;
        }

        currentScreen = screen;
    }

    private void HideAll()
    {
        if (mainCanvas != null)
            mainCanvas.SetActive(false);
        if (settingsCanvas != null)
            settingsCanvas.SetActive(false);
        if (shopCanvas != null)
            shopCanvas.SetActive(false);
        if (mapCanvas != null)
            mapCanvas.SetActive(false);
        if (spellBookCanvas != null)
            spellBookCanvas.SetActive(false);
    }
}
