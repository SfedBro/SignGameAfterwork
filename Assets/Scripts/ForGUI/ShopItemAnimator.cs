using UnityEngine;
using System.Collections;

public class ShopItemAnimator : MonoBehaviour
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        rectTransform.localScale = new Vector3(1f, 0.1f, 1f);
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    public void PlayOpenAnimation()
    {
        StartCoroutine(AnimateItem());
    }

    private IEnumerator AnimateItem()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        Quaternion startRot = Quaternion.Euler(0, 90, 0);
        Quaternion endRot = Quaternion.Euler(0, 0, 0);

        Vector3 startScale = new Vector3(1f, 0.1f, 1f);
        Vector3 endScale = new Vector3(1f, 1f, 1f);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            canvasGroup.alpha = t;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;
        rectTransform.localScale = endScale;
        canvasGroup.alpha = 1f;
    }
}
