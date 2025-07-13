using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private GameObject spellBookUI;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject spellEntryPrefab;
    [SerializeField] private GameObject dividerPrefab;
    [SerializeField] private float animationSpeed = 8f;

    [Header("Позиции")]
    [SerializeField] private Vector2 shownPosition = Vector2.zero;
    private Vector2 hiddenPosition;
    private RectTransform bookRect;

    private SpellsManager spellsManager;
    private bool isOpen = false;
    private bool isAnimating = false;

    private void Start()
    {
        if (!GetComponent<SpellsManager>())
        {
            gameObject.AddComponent<SpellsManager>();
        }
        spellsManager = GetComponent<SpellsManager>();
        bookRect = spellBookUI.GetComponent<RectTransform>();

        hiddenPosition = new Vector2(
            shownPosition.x,
            -Screen.height * 1.5f
        );
        
        bookRect.anchoredPosition = hiddenPosition;
        spellBookUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !isAnimating)
        {
            ToggleSpellBook();
        }

        if (isAnimating)
        {
            Vector2 targetPos = isOpen ? shownPosition : hiddenPosition;
            bookRect.anchoredPosition = Vector2.Lerp(
                bookRect.anchoredPosition,
                targetPos,
                Time.unscaledDeltaTime * animationSpeed
            );

            // Проверка завершения анимации
            if (Vector2.Distance(bookRect.anchoredPosition, targetPos) < 1f)
            {
                bookRect.anchoredPosition = targetPos;
                isAnimating = false;
                
                // Скрываем UI после анимации закрытия
                if (!isOpen)
                {
                    spellBookUI.SetActive(false);
                }
            }
        }
    }
    private void ToggleSpellBook()
    {
        isOpen = !isOpen;
        isAnimating = true;

        if (isOpen)
        {
            spellBookUI.SetActive(true);
            OpenSpellBook();
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
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
