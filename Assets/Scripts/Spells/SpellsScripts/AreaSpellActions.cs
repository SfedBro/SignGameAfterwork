using UnityEngine;
using System.Collections;

public class AreaSpellActions : MonoBehaviour
{
    private GameObject effectCaster;
    private float maxLifeTime;
    private string element;
    private string effectType;
    private float effectAmount;
    private float effectDuration;
    private float effectChance;

    private void Start()
    {
        StartCoroutine(CreateForTime(maxLifeTime));
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
            other.gameObject.GetComponent<EffectsHandler>().HandleEffect(effectCaster, element, effectType, effectAmount, effectDuration, effectChance);
        }
    }

    public void SetSettings(GameObject caster, string elem, string effType, float effAmount, float effDuration, float effChance, float lifetime)
    {
        effectCaster = caster;
        element = elem;
        effectType = effType;
        effectAmount = effAmount;
        effectDuration = effDuration;
        effectChance = effChance;
        maxLifeTime = lifetime;
    }
}
