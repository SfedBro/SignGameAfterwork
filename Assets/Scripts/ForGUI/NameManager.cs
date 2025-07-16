using TMPro;
using UnityEngine;

public class NameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    private const string playerName = "PlayerName";

    private void Start()
    {
        if (PlayerPrefs.HasKey(playerName))
        {
            string savedName = PlayerPrefs.GetString(playerName);
            nameInputField.text = savedName;
        }

        nameInputField.onEndEdit.AddListener(SaveName);
    }

    private void SaveName(string newName)
    {
        PlayerPrefs.SetString(playerName, newName);
        PlayerPrefs.Save();
    }
}
