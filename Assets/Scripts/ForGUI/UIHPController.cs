using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Hierarchy;

public class UIHPController : MonoBehaviour
{
    [Header("Heart Settings")]
    [SerializeField] private GameObject heartSlotPrefab;
    [SerializeField] private Transform heartContainer;
    [SerializeField] private Player playerScript;

    [Header("Animation Settings")]
    [SerializeField] private float heartAppearDuration = 0.5f;
    [SerializeField] private float heartDisappearDuration = 0.3f;
    [SerializeField] private float heartScaleMultiplier = 1.5f;
    [SerializeField] private AnimationCurve appearCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve disappearCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private List<GameObject> heartSlots = new List<GameObject>();
    private DamageVignette damageVignette;
    private int currentHP;
    private int previousHP;
    private bool isAnimating = false;

    void Start()
    {
        if (playerScript != null)
        {
            currentHP = playerScript.GetHP();
            previousHP = currentHP;
        }
        for (int i = 0; i < heartContainer.childCount; i++)
        {
            GameObject childObject = heartContainer.GetChild(i).gameObject;
            Destroy(childObject);
        }
        damageVignette = GetComponent<DamageVignette>();
        UpdateHearts();
    }

    void Update()
    {
        if (playerScript != null)
        {
            currentHP = playerScript.GetHP();

            if (currentHP != previousHP)
            {
                if (currentHP < previousHP && damageVignette != null)
                {
                    damageVignette.TriggerDamageVignette();
                    ScreenShake.Instance.ShakeVeryLight();
                }
                UpdateHearts();
                previousHP = currentHP;
            }
        }
    }

    private void UpdateHearts()
    {
        int requiredSlots = Mathf.CeilToInt(currentHP / 2f);

        while (heartSlots.Count > requiredSlots)
        {
            RemoveHeartSlot(heartSlots.Count - 1);
        }

        while (heartSlots.Count < requiredSlots)
        {
            AddHeartSlot();
        }

        UpdateHeartStates();
    }

    private void AddHeartSlot()
    {
        GameObject newSlot = Instantiate(heartSlotPrefab, heartContainer);
        heartSlots.Add(newSlot);

        GameObject fullHeart = newSlot.transform.Find("fullHeart").gameObject;
        GameObject halfHeart = newSlot.transform.Find("halfHeart").gameObject;

        StartCoroutine(AnimateHeartAppear(fullHeart));
        StartCoroutine(AnimateHeartAppear(halfHeart));
    }

    private void RemoveHeartSlot(int index)
    {
        if (index >= 0 && index < heartSlots.Count)
        {
            GameObject slotToRemove = heartSlots[index];
            heartSlots.RemoveAt(index);

            StartCoroutine(AnimateHeartDisappear(slotToRemove));
        }
    }

    private void UpdateHeartStates()
    {
        for (int i = 0; i < heartSlots.Count; i++)
        {
            GameObject slot = heartSlots[i];
            GameObject fullHeart = slot.transform.Find("fullHeart").gameObject;
            GameObject halfHeart = slot.transform.Find("halfHeart").gameObject;

            int heartIndex = i * 2;

            if (currentHP >= heartIndex + 2)
            {
                SetHeartState(fullHeart, true);
                SetHeartState(halfHeart, true);
            }
            else if (currentHP == heartIndex + 1)
            {
                SetHeartState(fullHeart, false);
                SetHeartState(halfHeart, true);
            }
            else
            {
                SetHeartState(fullHeart, false);
                SetHeartState(halfHeart, false);
            }
        }
    }

    private void SetHeartState(GameObject heart, bool isActive)
    {
        if (heart.activeSelf != isActive)
        {
            heart.SetActive(isActive);

            if (isActive)
            {
                StartCoroutine(AnimateHeartPartAppear(heart));
            }
        }
    }

    private IEnumerator AnimateHeartAppear(GameObject heart)
    {
        heart.transform.localScale = Vector3.zero;
        float elapsedTime = 0f;

        while (elapsedTime < heartAppearDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / heartAppearDuration;
            float curveValue = appearCurve.Evaluate(t);

            heart.transform.localScale = Vector3.one * curveValue;
            yield return null;
        }

        heart.transform.localScale = Vector3.one;
    }

    private IEnumerator AnimateHeartPartAppear(GameObject heart)
    {
        heart.transform.localScale = Vector3.zero;
        float duration = heartAppearDuration * 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / duration;
            float curveValue = appearCurve.Evaluate(t);

            heart.transform.localScale = Vector3.one * curveValue;
            yield return null;
        }

        heart.transform.localScale = Vector3.one;
    }

    private IEnumerator AnimateHeartDisappear(GameObject heartSlot)
    {
        GameObject fullHeart = heartSlot.transform.Find("fullHeart").gameObject;
        GameObject halfHeart = heartSlot.transform.Find("halfHeart").gameObject;

        CanvasGroup canvasGroup = heartSlot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = heartSlot.AddComponent<CanvasGroup>();

        Vector3 originalScale = heartSlot.transform.localScale;
        Vector3 targetScale = originalScale * heartScaleMultiplier;

        float elapsedTime = 0f;

        float phase1Duration = heartDisappearDuration * 0.3f;
        while (elapsedTime < phase1Duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / phase1Duration;

            heartSlot.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        elapsedTime = 0f;
        float phase2Duration = heartDisappearDuration * 0.7f;

        while (elapsedTime < phase2Duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / phase2Duration;
            float curveValue = disappearCurve.Evaluate(t);

            heartSlot.transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, t);
            canvasGroup.alpha = 1f - t;
            yield return null;
        }

        if (heartSlot != null)
            DestroyImmediate(heartSlot);
    }

    public void PlayHurtAnimation(int damageAmount)
    {
        if (isAnimating) return;

        StartCoroutine(HurtAnimationCoroutine(damageAmount));
    }

    private IEnumerator HurtAnimationCoroutine(int damageAmount)
    {
        isAnimating = true;

        List<Coroutine> shakeCoroutines = new List<Coroutine>();
        foreach (var slot in heartSlots)
        {
            shakeCoroutines.Add(StartCoroutine(ShakeHeart(slot.transform, 0.3f, 5f)));
        }

        int damagedHeartIndex = Mathf.FloorToInt((previousHP - 1) / 2f);
        if (damagedHeartIndex >= 0 && damagedHeartIndex < heartSlots.Count)
        {
            var damagedSlot = heartSlots[damagedHeartIndex];
            StartCoroutine(FlashHeart(damagedSlot, Color.red, 0.5f));
        }

        yield return new WaitForSeconds(0.5f);
        isAnimating = false;
    }

    public void PlayHealAnimation()
    {
        if (isAnimating) return;

        StartCoroutine(HealAnimationCoroutine());
    }

    private IEnumerator HealAnimationCoroutine()
    {
        isAnimating = true;

        // Зеленое свечение для восстановленного сердца
        int healedHeartIndex = Mathf.FloorToInt((currentHP - 1) / 2f);
        if (healedHeartIndex >= 0 && healedHeartIndex < heartSlots.Count)
        {
            var healedSlot = heartSlots[healedHeartIndex];
            StartCoroutine(PunchScale(healedSlot.transform, Vector3.one * 0.2f, 0.5f));
            StartCoroutine(FlashHeart(healedSlot, Color.green, 0.5f));
        }

        yield return new WaitForSeconds(0.5f);
        isAnimating = false;
    }

    private IEnumerator ShakeHeart(Transform heartTransform, float duration, float strength)
    {
        Vector3 originalPos = heartTransform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            Vector3 randomOffset = Random.insideUnitSphere * strength;
            randomOffset.z = 0f; // Только в 2D

            heartTransform.localPosition = originalPos + randomOffset;
            yield return null;
        }

        heartTransform.localPosition = originalPos;
    }

    private IEnumerator PunchScale(Transform heartTransform, Vector3 punchAmount, float duration)
    {
        Vector3 originalScale = heartTransform.localScale;
        Vector3 targetScale = originalScale + punchAmount;

        float elapsedTime = 0f;

        // Увеличение
        while (elapsedTime < duration * 0.5f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / (duration * 0.5f);

            heartTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Уменьшение
        elapsedTime = 0f;
        while (elapsedTime < duration * 0.5f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / (duration * 0.5f);

            heartTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        heartTransform.localScale = originalScale;
    }

    private IEnumerator FlashHeart(GameObject heartSlot, Color flashColor, float duration)
    {
        // Получаем все Image компоненты в слоте
        UnityEngine.UI.Image[] images = heartSlot.GetComponentsInChildren<UnityEngine.UI.Image>();
        Color[] originalColors = new Color[images.Length];

        // Сохраняем оригинальные цвета
        for (int i = 0; i < images.Length; i++)
        {
            originalColors[i] = images[i].color;
        }

        float elapsedTime = 0f;
        int flashCount = 6;
        float flashInterval = duration / flashCount;

        for (int flash = 0; flash < flashCount; flash++)
        {
            // Устанавливаем цвет вспышки
            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = flashColor;
            }

            yield return new WaitForSeconds(flashInterval * 0.5f);

            // Возвращаем оригинальный цвет
            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = originalColors[i];
            }

            yield return new WaitForSeconds(flashInterval * 0.5f);
        }

        // Убеждаемся, что цвета восстановлены
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = originalColors[i];
        }
    }
}