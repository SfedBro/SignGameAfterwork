using UnityEngine;
using System.Collections;

public class AreaSpellActions : MonoBehaviour
{
    public GameObject effectCaster;
    public float duration;
    public bool lookRight;
    public string element;
    public string effectType;
    public float effectDuration;

    private void Start()
    {
        StartCoroutine(CreateForTime(duration));
    }

    private IEnumerator CreateForTime(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            if (!other.gameObject.GetComponent<EffectsHandler>())
            {
                other.gameObject.AddComponent<EffectsHandler>();
            }
            other.gameObject.GetComponent<EffectsHandler>().HandleEffect(effectCaster, element, effectType, effectDuration, 0);
        }
    }
}
