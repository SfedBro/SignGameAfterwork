using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour
{
    [SerializeField] private GameObject spellBookUI;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject spellEntryPrefab;
    [SerializeField] private GameObject dividerPrefab;

    private SpellsManager spellsManager;
    private bool isOpen = false;

    private void Start()
    {
        if (!GetComponent<SpellsManager>())
        {
            gameObject.AddComponent<SpellsManager>();
        }
        spellBookUI.SetActive(false);
        spellsManager = GetComponent<SpellsManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleSpellBook();
        }
    }
    private void ToggleSpellBook()
    {
        isOpen = !isOpen;
        spellBookUI.SetActive(isOpen);

        if (isOpen)
        {
            OpenSpellBook();
        }
    }

    private void OpenSpellBook()
    {
        // Очищаем старые записи
        foreach (Transform child in scrollRect.content)
        {
            Destroy(child.gameObject);
        }

        // Добавляем все заклинания
        foreach (Spell spell in spellsManager.GetAllSpells())
        {
            GameObject divider = Instantiate(dividerPrefab, scrollRect.content);
            GameObject entry = Instantiate(spellEntryPrefab, scrollRect.content);
            
            TMP_Text nameText = entry.transform.Find("Name").GetComponent<TMP_Text>();
            TMP_Text comboText = entry.transform.Find("Combo").GetComponent<TMP_Text>();
            TMP_Text descText = entry.transform.Find("Description").GetComponent<TMP_Text>();

            nameText.text = spell.name;
            comboText.text = spell.Combo;
            descText.text = spell.Description;
        }
    }

    public void SetSpellsManager(SpellsManager someSM)
    {
        spellsManager = someSM;
    }
}
