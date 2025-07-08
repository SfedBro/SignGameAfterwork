using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Player player;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string levelCave000 = "LevelCave000";

    public static GameManager I;

    void Awake()
    {
        if (I != null) return;
        I = this;
    }

    void Start()
    {
        player = GameObject.Find("mage").GetComponent<Player>();
    }

    // void Update()
    // {
    //     if (player.GetHP() <= 0f)
    //     {
    //         Invoke(nameof(RespawnPlayer), respawnDelay);
    //     }
    // }

    public void PlayerDied()
    {
        Invoke(nameof(RespawnPlayer), respawnDelay);
    }

    private void RespawnPlayer()
    {
        SceneManager.LoadScene("LevelCave000");
    }

    public void RestartGame()
    {
        Debug.Log("Restart Clicked");
        SceneManager.LoadScene(levelCave000);
        Time.timeScale = 1f;
    }

    public void ToMainMenu()
    {
        Debug.Log("Main Menu Clicked");
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
