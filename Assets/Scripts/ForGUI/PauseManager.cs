using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject mainSettingsPanel;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button pauseIcon;
    [SerializeField] private Button toMainMenuButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainSettingsIcon;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private UIManager ui_manager;

    [Header("Objects to disable")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject spellBook;

    private bool isPaused = false;

    [Header("Volume")]
    public AudioSource audioSource;
    public AudioMixer audioMixer;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    

    void Start()
    {
        GameObject manager = GameObject.FindGameObjectWithTag("UIManager");
        if (manager != null)
        {
            ui_manager = manager.GetComponent<UIManager>();
        }
        exitButton.onClick.AddListener(ExitGame);
        pauseIcon.onClick.AddListener(OpenGameSettingsPanel);
        mainSettingsIcon.onClick.AddListener(OpenLevelSettings);
        continueButton.onClick.AddListener(OpenCloseSettings);
        if (toMainMenuButton != null)
            toMainMenuButton.onClick.AddListener(OpenMainMenu);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenCloseSettings);
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenCloseSettings();

        }

        if (isPaused && Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIObject())
            {
                OpenCloseSettings();
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        if (EventSystem.current == null) return false;

        return EventSystem.current.IsPointerOverGameObject();
    }

    private void OpenLevelSettings()
    {
        mainSettingsPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    private void OpenMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            settingsPanel.SetActive(false);
        }
    }

    private void OpenGameSettingsPanel()
    {
        pausePanel.SetActive(true);
        mainSettingsPanel.SetActive(false);
    }

    private void OpenCloseSettings()
    {
        if (!isPaused)
        {
            player.GetComponent<SpellCast>().enabled = false;
            spellBook.SetActive(false);
            foreach (Transform child in Camera.main.transform)
            {
                child.gameObject.SetActive(false);
            }

            if (ui_manager != null)
            {
                ui_manager.ShowScreen(UIScreen.Settings);
            }
            else
            {
                settingsPanel.SetActive(true);
            }
        }
        else
        {
            player.GetComponent<SpellCast>().enabled = true;
            spellBook.SetActive(true);
            foreach (Transform child in Camera.main.transform)
            {
                child.gameObject.SetActive(true);
            }

            if (ui_manager != null)
            {
                ui_manager.ShowScreen(UIScreen.MainCanvas);
            }
            else
            {
                settingsPanel.SetActive(false);
            }
        }
        TogglePause();
    }

    private void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void OnMasterVolumeChanged(float value) {
        audioMixer.SetFloat("masterVolume", value);
    }
    public void OnMusicVolumeChanged(float value) {
        audioMixer.SetFloat("musicVolume", value);
    }
    public void OnSoundsVolumeChanged(float value) {
        audioMixer.SetFloat("soundsVolume", value);
    }

    public void PlayHoverSound() {
        audioSource.PlayOneShot(hoverSound);
    }
    public void PlayClickSound() {
        audioSource.PlayOneShot(clickSound);
    }
}
