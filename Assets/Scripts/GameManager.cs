using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Player player;
    [SerializeField] private float respawnDelay = 1f;

    public static GameManager I;

    void Awake() {
        if (I != null) return;
        I = this;
    }

    void Start()
    {
        player = GameObject.Find("mage").GetComponent<Player>();
    }

    void Update()
    {
        if (player.GetHP() <= 0f)
        {
            Invoke(nameof(RespawnPlayer), respawnDelay);
        }
    }

    public void PlayerDied()
    {
        Invoke(nameof(RespawnPlayer), respawnDelay);
    }

    private void RespawnPlayer()
    {
        SceneManager.LoadScene("LevelCave000");
    }
}
