using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private UIManager ui_manager;

    [Header("Позиции")]
    [SerializeField] private Vector2 shownPosition = Vector2.zero;

    [Header("Сторонние объекты")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject settings;
    private Vector2 hiddenPosition;
    private RectTransform bookRect;

    private bool isOpen = false;
    private bool isAnimating = false;

    private void Start()
    {
        GameObject manager = GameObject.FindGameObjectWithTag("UIManager");
        scrollRect.scrollSensitivity = 10f;
        if (manager != null)
        {
            ui_manager = manager.GetComponent<UIManager>();
        }

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
                    if (ui_manager != null)
                    {
                        ui_manager.ShowScreen(UIScreen.MainCanvas);
                    }
                    else
                    {
                        spellBookUI.SetActive(false);
                    }
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
            if (ui_manager != null)
            {
                spellBookUI.SetActive(true);
                ui_manager.ShowScreen(UIScreen.SpellBook);
            }
            else
            {
                spellBookUI.SetActive(true);
            }

            Time.timeScale = 0f;
            player.GetComponent<SpellCast>().enabled = false;
            settings.SetActive(false);
            foreach (Transform child in Camera.main.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            Time.timeScale = 1f;
            player.GetComponent<SpellCast>().enabled = true;
            settings.SetActive(true);
            foreach (Transform child in Camera.main.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
