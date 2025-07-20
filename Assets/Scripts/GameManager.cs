using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Player player;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string levelName = "LevelTest";

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

    public void RespawnWithouDeathScreen()
    {
        Invoke(nameof(RespawnPlayerWithPosition), respawnDelay);
    }

    private void RespawnPlayerWithPosition()
    {
        SceneManager.LoadScene(levelName);
        StartCoroutine(SetPlayerPositionAfterLoad());
    }

    private IEnumerator SetPlayerPositionAfterLoad()
    {
        yield return new WaitForSeconds(0.1f);
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = DataContainer.checkpointIndex;
        }
    }

    public void PlayerDied()
    {
        Invoke(nameof(RespawnPlayer), respawnDelay);
    }

    private void RespawnPlayer()
    {
        SceneManager.LoadScene(levelName);
    }

    public void RestartGame()
    {
        Debug.Log("Restart Clicked");
        SceneManager.LoadScene(levelName);
    }

    public void ToMainMenu()
    {
        Debug.Log("Main Menu Clicked");
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
