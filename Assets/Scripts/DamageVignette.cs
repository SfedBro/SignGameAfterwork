using UnityEngine.Rendering;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class DamageVignette : MonoBehaviour
{
    [Header("Vignette Settings")]
    public float damageIntensity = 0.8f;
    public float fadeSpeed = 2f;

    private Volume postProcessVolume;
    private Vignette vignette;
    private float originalIntensity;
    private bool hasVignette = false;

    void Start()
    {
        FindVignetteVolume();
    }

    void FindVignetteVolume()
    {
        Volume[] volumes = FindObjectsOfType<Volume>();

        foreach (Volume volume in volumes)
        {
            if (volume.profile.TryGet(out Vignette tempVignette))
            {
                postProcessVolume = volume;
                vignette = tempVignette;
                originalIntensity = vignette.intensity.value;
                hasVignette = true;
                Debug.Log($"Found vignette in volume: {volume.name}");
                break;
            }
        }

        if (!hasVignette)
        {
            Debug.LogWarning("No vignette found in any Volume objects!");
        }

    }
    public void TriggerVignetteLight() => TriggerDamageVignette(0.5f);
    public void TriggerVignetteMedium() => TriggerDamageVignette(0.65f);
    public void TriggerVignetteHeavy() => TriggerDamageVignette(0.8f);
    public void TriggerDamageVignette(float damageIntensityPower = -1)
    {
        if (!hasVignette)
        {
            Debug.LogWarning("Cannot trigger damage vignette - no vignette found!");
            return;
        }

        if (damageIntensityPower == -1)
            damageIntensityPower = damageIntensity;
        vignette.intensity.value = damageIntensityPower;

        StartCoroutine(DamageVignetteEffect(damageIntensityPower));
    }

    IEnumerator DamageVignetteEffect(float damageIntensityPower)
    {
        while (vignette.intensity.value > originalIntensity)
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, originalIntensity, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        vignette.intensity.value = originalIntensity;
    }
}