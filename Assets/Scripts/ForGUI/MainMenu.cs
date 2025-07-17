using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string startSceneName = "LevelCave000";

    void Awake()
    {
        // if (!PlayerPrefs.HasKey("Initialized"))
        // {
            PlayerPrefs.DeleteAll();
        //     PlayerPrefs.SetInt("Initialized", 1);
        //     PlayerPrefs.Save();
        // }
    }

    public void StartGame()
    {
        DataContainer.checkpointIndex = new Vector3(-55, 25, 0);
        //for test
        PlayerPrefs.SetInt("coins", 200);
        // for test
        SceneManager.LoadScene(startSceneName);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}