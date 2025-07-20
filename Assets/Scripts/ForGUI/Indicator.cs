using System.Collections;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    private float value = 0;

    [SerializeField] float maxpos;
    [SerializeField] RectTransform mask;
    [SerializeField] RectTransform fill;
    [SerializeField] GameObject background;
    private bool isActive = true;

    void Update()
    {
        if (!isActive) return;
        
        mask.localPosition = new Vector2(maxpos - maxpos * value, mask.localPosition.y);
        fill.localPosition = new Vector2(-mask.localPosition.x, fill.localPosition.y);
    }

    public void SetProgress(float val)
    {
        if (!isActive) return;

        value = Mathf.Clamp01(val);

        if (background != null)
        {
            background.SetActive(value > 0f);
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if (!active && background != null)
        {
            background.SetActive(false);
        }

        if (active && background != null)
        {
            background.SetActive(true);
            value = 0;
        }
    }
}
