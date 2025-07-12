using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] private List<Talker> talkers = new List<Talker> { new Talker { name = "Маг", sprite = null } };
    private List<Message> currentDialog = new List<Message>();
    private int currentMessageID = 0;
    [SerializeField] private GameObject dialogGameObject;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private GameObject leftNamePanel;
    [SerializeField] private GameObject rightNamePanel;
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text leftName;
    [SerializeField] private TMP_Text rightName;
    [SerializeField] private Image leftCharacterImage;
    [SerializeField] private Image rightCharacterImage;
    void Start()
    {
        if (textPanel == null)
        {
            Debug.Log("Не настроены все поля DialogSystem");
            Destroy(gameObject);
        }
        if (leftNamePanel == null)
        {
            Debug.Log("Не настроены все поля DialogSystem");
            Destroy(gameObject);
        }
        if (rightNamePanel == null)
        {
            Debug.Log("Не настроены все поля DialogSystem");
            Destroy(gameObject);
        }
        if (leftCharacterImage == null)
        {
            Debug.Log("Не настроены все поля DialogSystem");
            Destroy(gameObject);
        }
        if (rightCharacterImage == null)
        {
            Debug.Log("Не настроены все поля DialogSystem");
            Destroy(gameObject);
        }
        if (dialogGameObject == null)
        {
            Debug.Log("Не настроены все поля DialogSystem");
            Destroy(gameObject);
        }
    }

    public void SetDialog(List<Message> dialog)
    {
        currentDialog = dialog;
    }
    public void StartDialog()
    {
        if (currentDialog == null) return;
        dialogGameObject.SetActive(true);
        currentMessageID = 0;
        ShowMessageByID(currentMessageID);
    }
    public void endDialog()
    {
        if (currentDialog == null) return;
        dialogGameObject.SetActive(false);
        currentMessageID = 0;
    }
    public void ShowNextMessage()
    {
        currentMessageID++;
        if (currentMessageID >= currentDialog.Count)
        {
            endDialog();
        } 
        else
        {
            ShowMessageByID(currentMessageID);
        }
    }
    public void ShowMessageByID(int id)
    {
        if (currentDialog[currentMessageID].talkerID < 0 || currentDialog[currentMessageID].talkerID >= talkers.Count) return;
        int talkerID = currentDialog[currentMessageID].talkerID;
        if (currentDialog[currentMessageID].isTalkerLeft)
        {
            leftCharacterImage.gameObject.SetActive(true);
            leftCharacterImage.sprite = talkers[talkerID].sprite;
            leftCharacterImage.SetNativeSize();

            rightCharacterImage.gameObject.SetActive(false);
            rightCharacterImage.sprite = null;


            leftNamePanel.gameObject.SetActive(true);
            leftName.text = talkers[talkerID].name;

            rightNamePanel.gameObject.SetActive(false);
            rightName.text = "";
        }
        else
        {
            rightCharacterImage.gameObject.SetActive(true);
            rightCharacterImage.sprite = talkers[talkerID].sprite;
            rightCharacterImage.SetNativeSize();

            leftCharacterImage.gameObject.SetActive(false);
            leftCharacterImage.sprite = null;


            rightNamePanel.gameObject.SetActive(true);
            rightName.text = talkers[talkerID].name;

            leftNamePanel.gameObject.SetActive(false);
            leftName.text = "";
        }
        text.text = currentDialog[currentMessageID].message;
    }

    void Update()
    {
        if (dialogGameObject.activeInHierarchy && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))) {
            ShowNextMessage();
        }
    }
}
[System.Serializable]
public class Talker
{
    public string name;
    public Sprite sprite;

    public Talker()
    {
        name = "[None]";
        sprite = null;
    }
    public Talker(string name)
    {
        this.name = name;
    }
    public Talker(string name, Sprite sprite)
    {
        this.name = name;
        this.sprite = sprite;
    }
}
[System.Serializable]
public class Message
{
    public string message;
    public bool isTalkerLeft = true;
    public int talkerID;

    private static int nextId = 1;

    public Message(string message, bool isTalkerLeft, int talkerID)
    {
        this.message = message;
        this.isTalkerLeft = isTalkerLeft;
        this.talkerID = talkerID;
    }
    public Message()
    {
        this.message = "...";
        this.isTalkerLeft = true;
        this.talkerID = -1;
    }
}
