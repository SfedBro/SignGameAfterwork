using UnityEngine;

public enum UIScreen
{
    None,
    MainCanvas,
    Settings,
    Shop,
    Map
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private GameObject mapCanvas;

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
                settingsCanvas.SetActive(true);
                break;
            case UIScreen.Shop:
                shopCanvas.SetActive(true);
                break;
            case UIScreen.Map:
                mapCanvas.SetActive(true);
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
    }
}
