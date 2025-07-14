using UnityEngine;
using System.Collections;

public class AreaSpellActions : MonoBehaviour
{
    private GameObject effectCaster;
    private float duration;
    private string element;
    private string effectType;
    private float effectDuration;
    private float amount = 0;

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
        if (other.tag == "Enemy" || other.tag == "Player" || other.tag == "Boss")
        {
            if (!other.gameObject.GetComponent<EffectsHandler>())
            {
                other.gameObject.AddComponent<EffectsHandler>();
            }
            other.gameObject.GetComponent<EffectsHandler>().HandleEffect(effectCaster, element, effectType, effectDuration, amount);
        }
    }

    public void SetSettings(GameObject caster, string elem, string effType, float effDur, float lifetime, float amt = 0)
    {
        effectCaster = caster;
        element = elem;
        effectType = effType;
        effectDuration = effDur;
        duration = lifetime;
        amount = amt;
    }
}
