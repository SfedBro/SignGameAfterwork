using UnityEngine;
using System.Collections;

public class AreaSpell : MonoBehaviour
{
    public float duration = 0.1f;
    public bool lookRight;
    public string effectType;

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
            EffectsManager.Instance.effect.ApplyEffect(gameObject, other.gameObject, effectType);
        }
    }
}
