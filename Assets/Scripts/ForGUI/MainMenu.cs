using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string startSceneName = "LevelCave000";

    public void StartGame()
    {
        DataContainer.checkpointIndex = new Vector3(-55, 25, 0);
        PlayerPrefs.DeleteAll();
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
}