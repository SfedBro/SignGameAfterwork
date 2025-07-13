using UnityEngine;

public enum UIScreen
{
    None,
    MainMenu,
    Settings
}

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject settingsCanvas;

    private UIScreen currentScreen = UIScreen.None;

    private void Start()
    {
        ShowScreen(UIScreen.MainMenu);
    }

    public void ShowScreen(UIScreen screen)
    {
        HideAll();

        switch (screen)
        {
            case UIScreen.MainMenu:
                mainMenuCanvas.SetActive(true);
                break;
            case UIScreen.Settings:
                settingsCanvas.SetActive(true);
                break;
        }

        currentScreen = screen;
    }

    private void HideAll()
    {
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
    }
}
