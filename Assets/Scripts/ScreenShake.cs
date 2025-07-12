using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [Header("Screen Shake Settings")]
    public bool isScreenShakeEnabled = true;

    [Header("Shake Parameters")]
    public float shakeDuration = 0.3f;
    public float shakeIntensity = 1f;
    public AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Vector3 originalPosition;
    private float shakeTimer;
    private bool isShaking;

    public static ScreenShake Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            originalPosition = transform.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isShaking && isScreenShakeEnabled)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer > 0)
            {
                float normalizedTime = 1f - (shakeTimer / shakeDuration);
                float curveValue = shakeCurve.Evaluate(normalizedTime);

                Vector3 shakeOffset = Random.insideUnitCircle * shakeIntensity * curveValue;
                transform.position = originalPosition + shakeOffset;
            }
            else
            {
                isShaking = false;
                transform.position = originalPosition;
            }
        }
    }

    public void Shake(float intensity, float duration = -1f)
    {
        if (!isScreenShakeEnabled) return;

        shakeIntensity = intensity;
        shakeDuration = duration > 0 ? duration : this.shakeDuration;
        shakeTimer = shakeDuration;
        isShaking = true;

        originalPosition = transform.position;
    }

    public void ShakeVeryLight() => Shake(0.1f, 0.075f);
    public void ShakeLight() => Shake(0.3f, 0.2f);
    public void ShakeMedium() => Shake(0.8f, 0.3f);
    public void ShakeHeavy() => Shake(1.5f, 0.5f);

    public void ToggleScreenShake(bool enabled)
    {
        isScreenShakeEnabled = enabled;
        if (!enabled && isShaking)
        {
            isShaking = false;
            transform.position = originalPosition;
        }
    }

    public void StopShake()
    {
        isShaking = false;
        transform.position = originalPosition;
    }
}