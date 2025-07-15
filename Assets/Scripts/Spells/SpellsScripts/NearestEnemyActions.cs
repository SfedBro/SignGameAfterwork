using UnityEngine;
using System.Collections;

public class NearestEnemyActions : MonoBehaviour
{
    private GameObject effectCaster;
    private float maxLifeTime;
    private string element;
    private string effectType;
    private float effectDuration;

    private void Start()
    {
        StartCoroutine(CreateForTime(maxLifeTime));
    }

    private void Update()
    {
        gameObject.GetComponent<Transform>().localScale += new Vector3(0.2f, 0.2f, 0);
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
            Destroy(gameObject);
        }
    }

    public void SetSettings(GameObject caster, string elem, string effType, float effDur, float lifetime)
    {
        effectCaster = caster;
        element = elem;
        effectType = effType;
        effectDuration = effDur;
        maxLifeTime = lifetime;
    }
}
