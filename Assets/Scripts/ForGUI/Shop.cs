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
    private float alpha = 0.1f;

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("coins", 100);
        // block.SetActive(false);
        sourceImage.SetActive(false);
        
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        originalScale = rectTransform.localScale;

        rectTransform.localRotation = Quaternion.identity;
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
                    // float alpha = Mathf.Lerp(1f, 0.3f, dragDistance / 200f);
                    SetTransparency(alpha);

                    if (dragDistance > 1f)
                    {
                        AttemptToBuy();
                        Debug.Log("Куплен товар!");
                        isDragging = false;
                        // Тут можешь вызвать покупку
                    }
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
        Quaternion endRot = Quaternion.Euler(0f, 0f, 90f);

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
        Vector3 startScale = rectTransform.localScale;
        Vector3 endScale = new Vector3(originalScale.x * 4.5f, originalScale.y, originalScale.z);

        while (t < duration)
        {
            t += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, t / duration);
            yield return null;
        }

        rectTransform.localScale = endScale;
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
        Vector3 start = rectTransform.localScale;
        Vector3 end = new Vector3(originalScale.x / 4.5f, originalScale.y, originalScale.z);

        sourceImage.SetActive(false);

        while (t < duration)
        {
            t += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }

        rectTransform.localScale = end;
    }

    private IEnumerator Expand()
    {
        float t = 0f;
        float duration = 0.3f;
        Vector3 start = rectTransform.localScale;
        Vector3 end = new Vector3(originalScale.x * 5f, originalScale.y, originalScale.z);

        sourceImage.SetActive(true);
        
        while (t < duration)
        {
            t += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }

        rectTransform.localScale = end;
    }

    public void SetTransparency(float alpha)
    {
        canvasGroup.alpha = alpha;
    }
}
