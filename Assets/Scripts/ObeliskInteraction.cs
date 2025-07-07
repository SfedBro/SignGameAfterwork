using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ObeliskInteraction : MonoBehaviour
{
    // [SerializeField] private GameObject letterE;
    [SerializeField] private GameObject blackPentagram;
    [SerializeField] private GameObject whitePentagram; 
    [SerializeField] private ParticleSystem spiralParticles;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float detectionRadius = 3f;
    [Header("Light Settings")]
    [SerializeField] private Light2D spotLight;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Light Properties")]
    [SerializeField] private float targetIntensity = 1.5f;

    private bool isPlayerNearby = false;
    private bool isActivated = false;
    private SpriteRenderer blackPentagramRenderer;
    private SpriteRenderer whitePentagramRenderer;
    private GameObject player;
    private float originalIntensity;
    private float originalRange;
    private float originalSpotAngle;
    private bool isAnimating = false;

    private void Start()
    {
        blackPentagramRenderer = blackPentagram.GetComponent<SpriteRenderer>();
        whitePentagramRenderer = whitePentagram.GetComponent<SpriteRenderer>();

        SetAlpha(whitePentagramRenderer, 0f);

        // letterE.SetActive(false); 

        spiralParticles.Stop();


        if (spotLight != null)
        {
            originalIntensity = spotLight.intensity;
            if (targetIntensity <= 0) targetIntensity = originalIntensity;
            spotLight.intensity = 0f;
            spotLight.enabled = false;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);
        isPlayerNearby = distance <= detectionRadius;
        
        // letterE.SetActive(isPlayerNearby); 

        // if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isActivated)
        if (isPlayerNearby && !isActivated)
        {
            isActivated = true;
            TurnOnLight();
            StartCoroutine(FadePentagrams());
        }
    }

    private System.Collections.IEnumerator FadePentagrams()
    {
        float elapsedTime = 0f;
        spiralParticles.Play();

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            
            SetAlpha(blackPentagramRenderer, Mathf.Lerp(1f, 0f, t));
            
            SetAlpha(whitePentagramRenderer, Mathf.Lerp(0f, 1f, t)); 
            yield return null;
        }

        SetAlpha(blackPentagramRenderer, 0f);
        SetAlpha(whitePentagramRenderer, 1f);
    }

    private void SetAlpha(SpriteRenderer renderer, float alpha)
    {
        Color color = renderer.color;
        color.a = alpha;
        renderer.color = color;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    public void TurnOnLight()
    {
        if (spotLight != null && !isAnimating)
        {
            StartCoroutine(FadeLight(true));
        }
    }

    public void TurnOffLight()
    {
        if (spotLight != null && !isAnimating)
        {
            StartCoroutine(FadeLight(false));
        }
    }

    public void ToggleLight()
    {
        if (spotLight != null && !isAnimating)
        {
            if (spotLight.enabled && spotLight.intensity > 0)
            {
                TurnOffLight();
            }
            else
            {
                TurnOnLight();
            }
        }
    }

    private IEnumerator FadeLight(bool turnOn)
    {
        isAnimating = true;

        if (turnOn)
        {
            spotLight.enabled = true;
            yield return StartCoroutine(FadeIn());
        }
        else
        {
            yield return StartCoroutine(FadeOut());
            spotLight.enabled = false;
        }

        isAnimating = false;
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        float startIntensity = spotLight.intensity;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            float curveValue = fadeCurve.Evaluate(t);
            spotLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, curveValue);

            yield return null;
        }
        spotLight.intensity = targetIntensity;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        float startIntensity = spotLight.intensity;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            float curveValue = fadeCurve.Evaluate(1f - t);
            spotLight.intensity = Mathf.Lerp(0f, startIntensity, curveValue);

            yield return null;
        }

        spotLight.intensity = 0f;
    }
}