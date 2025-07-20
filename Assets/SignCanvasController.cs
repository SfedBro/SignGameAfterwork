using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SignData
{
    public string signName;
    public Sprite signSprite;
    public Sprite lightSprite;
    public Color signColor = Color.white;
}

public class SignCanvasController : MonoBehaviour
{
    public static SignCanvasController Instance { get; private set; }

    [Header("Sign Settings")]
    [SerializeField] private GameObject signSlotPrefab;
    [SerializeField] private Transform signContainer;

    [Header("Sign Data Dictionary")]
    [SerializeField] private List<SignData> signDataList = new List<SignData>();

    [Header("Animation Settings")]
    [SerializeField] private float signAppearDuration = 0.5f;
    [SerializeField] private float signDisappearDuration = 0.3f;
    [SerializeField] private float signScaleMultiplier = 1.5f;
    [SerializeField] private AnimationCurve appearCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve disappearCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private List<GameObject> signSlots = new List<GameObject>();
    private List<string> currentSigns = new List<string>();
    private Dictionary<string, SignData> signDataDict = new Dictionary<string, SignData>();
    private bool isAnimating = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitializeSignDataDictionary();
        ClearExistingSigns();
    }

    private void InitializeSignDataDictionary()
    {
        signDataDict.Clear();
        foreach (var signData in signDataList)
        {
            if (!string.IsNullOrEmpty(signData.signName) && !signDataDict.ContainsKey(signData.signName))
            {
                signDataDict[signData.signName] = signData;
            }
        }
    }

    private void ClearExistingSigns()
    {
        for (int i = 0; i < signContainer.childCount; i++)
        {
            GameObject childObject = signContainer.GetChild(i).gameObject;
            Destroy(childObject);
        }
        signSlots.Clear();
        currentSigns.Clear();
    }

    public void SetSigns(List<string> newSigns)
    {
        if (isAnimating) return;

        StartCoroutine(UpdateSignsCoroutine(newSigns));
    }

    private IEnumerator UpdateSignsCoroutine(List<string> newSigns)
    {
        isAnimating = true;

        List<string> targetSigns = new List<string>(newSigns ?? new List<string>());

        for (int i = currentSigns.Count - 1; i >= 0; i--)
        {
            string currentSign = currentSigns[i];
            if (!targetSigns.Contains(currentSign))
            {
                yield return StartCoroutine(RemoveSignSlot(i));
            }
        }

        foreach (string newSign in targetSigns)
        {
            if (!currentSigns.Contains(newSign))
            {
                yield return StartCoroutine(AddSignSlot(newSign));
            }
        }

        yield return StartCoroutine(ReorderSigns(targetSigns));

        isAnimating = false;
    }

    private IEnumerator AddSignSlot(string signName)
    {
        if (!signDataDict.ContainsKey(signName))
        {
            Debug.LogWarning($"Sign data not found for: {signName}");
            yield break;
        }

        SignData signData = signDataDict[signName];
        GameObject newSlot = Instantiate(signSlotPrefab, signContainer);
        signSlots.Add(newSlot);
        currentSigns.Add(signName);

        Transform signTransform = newSlot.transform.Find("Sign");
        Transform lightTransform = signTransform.Find("Light");

        if (signTransform == null)
        {
            Debug.LogError($"Sign child object not found in prefab for: {signName}");
            yield break;
        }

        if (lightTransform == null)
        {
            Debug.LogError($"Light child object not found in prefab for: {signName}");
            yield break;
        }

        UnityEngine.UI.Image signImage = signTransform.GetComponent<UnityEngine.UI.Image>();
        UnityEngine.UI.Image lightImage = lightTransform.GetComponent<UnityEngine.UI.Image>();

        if (signImage != null && signData.signSprite != null)
        {
            signImage.sprite = signData.signSprite;
            signImage.color = signData.signColor;
        }

        if (lightImage != null && signData.lightSprite != null)
        {
            lightImage.sprite = signData.lightSprite;
            lightImage.color = signData.signColor;
        }

        yield return StartCoroutine(AnimateSignAppear(newSlot));
    }

    private IEnumerator RemoveSignSlot(int index)
    {
        if (index < 0 || index >= signSlots.Count)
            yield break;

        GameObject slotToRemove = signSlots[index];
        signSlots.RemoveAt(index);
        currentSigns.RemoveAt(index);

        yield return StartCoroutine(AnimateSignDisappear(slotToRemove));
    }

    private IEnumerator ReorderSigns(List<string> targetOrder)
    {
        List<GameObject> newSignSlots = new List<GameObject>();
        List<string> newCurrentSigns = new List<string>();

        for (int i = 0; i < targetOrder.Count; i++)
        {
            string targetSign = targetOrder[i];
            int currentIndex = currentSigns.IndexOf(targetSign);

            if (currentIndex >= 0 && currentIndex < signSlots.Count)
            {
                newSignSlots.Add(signSlots[currentIndex]);
                newCurrentSigns.Add(currentSigns[currentIndex]);

                signSlots[currentIndex].transform.SetSiblingIndex(i);
            }
        }

        signSlots = newSignSlots;
        currentSigns = newCurrentSigns;

        yield return null;
    }

    private IEnumerator AnimateSignAppear(GameObject signSlot)
    {
        signSlot.transform.localScale = Vector3.zero;
        float elapsedTime = 0f;

        while (elapsedTime < signAppearDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / signAppearDuration;
            float curveValue = appearCurve.Evaluate(t);

            signSlot.transform.localScale = Vector3.one * curveValue;
            yield return null;
        }

        signSlot.transform.localScale = Vector3.one;
    }

    private IEnumerator AnimateSignDisappear(GameObject signSlot)
    {
        if (signSlot == null) yield break;

        CanvasGroup canvasGroup = signSlot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = signSlot.AddComponent<CanvasGroup>();

        Vector3 originalScale = signSlot.transform.localScale;
        Vector3 targetScale = originalScale * signScaleMultiplier;

        float elapsedTime = 0f;

        float phase1Duration = signDisappearDuration * 0.3f;
        while (elapsedTime < phase1Duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / phase1Duration;

            signSlot.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        elapsedTime = 0f;
        float phase2Duration = signDisappearDuration * 0.7f;

        while (elapsedTime < phase2Duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / phase2Duration;
            float curveValue = disappearCurve.Evaluate(t);

            signSlot.transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, t);
            canvasGroup.alpha = 1f - t;
            yield return null;
        }

        if (signSlot != null)
            DestroyImmediate(signSlot);
    }

    public void PlaySignActivationAnimation(string signName)
    {
        if (isAnimating) return;

        int signIndex = currentSigns.IndexOf(signName);
        if (signIndex >= 0 && signIndex < signSlots.Count)
        {
            StartCoroutine(SignActivationCoroutine(signSlots[signIndex]));
        }
    }

    private IEnumerator SignActivationCoroutine(GameObject signSlot)
    {
        isAnimating = true;

        StartCoroutine(PunchScale(signSlot.transform, Vector3.one * 0.3f, 0.6f));
        StartCoroutine(FlashSign(signSlot, Color.yellow, 0.6f));

        yield return new WaitForSeconds(0.6f);
        isAnimating = false;
    }

    private IEnumerator PunchScale(Transform signTransform, Vector3 punchAmount, float duration)
    {
        Vector3 originalScale = signTransform.localScale;
        Vector3 targetScale = originalScale + punchAmount;

        float elapsedTime = 0f;

        while (elapsedTime < duration * 0.5f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / (duration * 0.5f);

            signTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < duration * 0.5f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / (duration * 0.5f);

            signTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        signTransform.localScale = originalScale;
    }

    private IEnumerator FlashSign(GameObject signSlot, Color flashColor, float duration)
    {
        UnityEngine.UI.Image[] images = signSlot.GetComponentsInChildren<UnityEngine.UI.Image>();
        Color[] originalColors = new Color[images.Length];

        for (int i = 0; i < images.Length; i++)
        {
            originalColors[i] = images[i].color;
        }

        float elapsedTime = 0f;
        int flashCount = 8;
        float flashInterval = duration / flashCount;

        for (int flash = 0; flash < flashCount; flash++)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = flashColor;
            }

            yield return new WaitForSeconds(flashInterval * 0.5f);

            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = originalColors[i];
            }

            yield return new WaitForSeconds(flashInterval * 0.5f);
        }

        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = originalColors[i];
        }
    }

    public List<string> GetCurrentSigns()
    {
        return new List<string>(currentSigns);
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }

    public void AddSignData(string signName, Sprite signSprite, Sprite lightSprite, Color signColor = default)
    {
        SignData newSignData = new SignData
        {
            signName = signName,
            signSprite = signSprite,
            lightSprite = lightSprite,
            signColor = signColor == default ? Color.white : signColor
        };

        signDataList.Add(newSignData);
        signDataDict[signName] = newSignData;
    }
}