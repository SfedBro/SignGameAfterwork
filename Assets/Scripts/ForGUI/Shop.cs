using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectPrice;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private TextMeshProUGUI coinCounter;
    [SerializeField] private GameObject sourceImage;
    [SerializeField] private GameObject panel;
    // [SerializeField] private GameObject block;
    [SerializeField] private int price;
    [SerializeField] public string objectName;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector3 originalScale;
    private bool isSelected = false;

    private Vector3 dragStartPos;
    private bool isDragging = false;
    private int access;
    private float alpha = 0.7f;

    private void Awake()
    {
        sourceImage.SetActive(false);
        
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        originalScale = rectTransform.localScale;

        rectTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        canvasGroup.alpha = 1f;
        rectTransform.localScale = originalScale;

        AccessUpdate();
    }

    void AccessUpdate()
    {
        access = PlayerPrefs.GetInt(objectName + "Access");
        objectPrice.text = price.ToString();

        if (access == 1)
        {
            // block.SetActive(true);
            SetTransparency(alpha);
            objectPrice.gameObject.SetActive(false);
        }
        else
        {
            // block.SetActive(false);
            SetTransparency(1);
            objectPrice.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (isSelected == true && access == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragStartPos = Input.mousePosition;
                isDragging = true;
            }
            else if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 currentPos = Input.mousePosition;
                float dragDistance = Vector3.Distance(currentPos, dragStartPos);

                if (dragDistance > 5f)
                {
                    SetTransparency(alpha);
                    AttemptToBuy();
                    isDragging = false;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                SetTransparency(1f);
            }
        }
    }

    private void AttemptToBuy()
    {
        int coins = PlayerPrefs.GetInt("coins");

        if (access == 0)
        {
            if (coins >= price)
            {
                PlayerPrefs.SetInt(objectName + "Access", 1);
                PlayerPrefs.SetInt("coins", coins - price);
                coinCounter.text = PlayerPrefs.GetInt("coins").ToString();
                // block.SetActive(true);
                AccessUpdate();
                ItemPurchase purchase = panel.GetComponent<ItemPurchase>();
                purchase.Purchase(objectName);
                descriptionField.text = $"Товар \"{objectName}\" куплен";
                Debug.Log("Куплен товар!");
            }
            else
            {
                if (PlayerPrefs.GetInt(objectName + "Access") == 0)
                {
                    descriptionField.text = "Недостаточно средств";
                    Debug.Log("Нет денег");
                }
            }
        }
    }

    public void AnimateIn()
    {
        StartCoroutine(AnimateSequence());
    }

    private IEnumerator AnimateSequence()
    {
        yield return new WaitForSeconds(1.5f);

        // поворот
        float t = 0f;
        float duration = 0.6f;

        Quaternion startRot = rectTransform.localRotation;
        Quaternion endRot = Quaternion.Euler(0f, 0f, 0f);

        while (t < duration)
        {
            t += Time.deltaTime;
            rectTransform.localRotation = Quaternion.Lerp(startRot, endRot, t / duration);
            yield return null;
        }

        rectTransform.localRotation = endRot;

        yield return new WaitForSeconds(1.2f);

        // увеличение по x
        t = 0f;
        duration = 0.4f;

        float startHeight = rectTransform.sizeDelta.y;
        float targetHeight = 1000f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float newHeight = Mathf.Lerp(startHeight, targetHeight, t / duration);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
            yield return null;
        }

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, targetHeight);
        sourceImage.SetActive(true);
    }

    public void Select()
    {
        isSelected = true;
        StopAllCoroutines();
        StartCoroutine(Expand());
    }

    public void Deselect()
    {
        isSelected = false;
        StopAllCoroutines();
        StartCoroutine(Shrink());
    }

    private IEnumerator Shrink()
    {
        float t = 0f;
        float duration = 0.3f;

        RectTransform rt = rectTransform;
        float startHeight = rt.sizeDelta.y;
        float targetHeight = 300f;

        sourceImage.SetActive(false);

        while (t < duration)
        {
            t += Time.deltaTime;
            float newHeight = Mathf.Lerp(startHeight, targetHeight, t / duration);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, newHeight);
            yield return null;
        }

        rt.sizeDelta = new Vector2(rt.sizeDelta.x, targetHeight);
    }

    private IEnumerator Expand()
    {
        float t = 0f;
        float duration = 0.3f;

        RectTransform rt = rectTransform;
        float startHeight = rt.sizeDelta.y;
        float targetHeight = 1000f;

        sourceImage.SetActive(true);

        while (t < duration)
        {
            t += Time.deltaTime;
            float newHeight = Mathf.Lerp(startHeight, targetHeight, t / duration);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, newHeight);
            yield return null;
        }

        rt.sizeDelta = new Vector2(rt.sizeDelta.x, targetHeight);
    }

    public void SetTransparency(float alpha)
    {
        canvasGroup.alpha = alpha;
    }
}
