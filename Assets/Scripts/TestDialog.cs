using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestDialog : MonoBehaviour
{
    private DialogSystem dialogSystem;
    [SerializeField] private List<Message> testDialog = new List<Message>();

    void Start()
    {
        dialogSystem = FindObjectOfType<DialogSystem>();

        if (dialogSystem == null)
        {
            Debug.LogWarning("DialogSystem не найден");
        }
    }

    void Update()
    {
        if (dialogSystem != null && Input.GetKeyDown(KeyCode.E))
        {
            dialogSystem.SetDialog(testDialog);
            dialogSystem.StartDialog();
        }
    }
}