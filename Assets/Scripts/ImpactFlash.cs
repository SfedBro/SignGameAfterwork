using System.Collections;
using UnityEngine;

public class ImpactFlash : MonoBehaviour
{
    // Название материала во встроенной папке Resources (Resources/Flash.mat)
    [SerializeField] private string flashMaterialName = "Flash";
    private Material flashMaterial;
    private Material originalMaterial; // Сохраняем оригинальный материал

    public void Flash(SpriteRenderer spriteRenderer, float duration)
    {
        if (flashMaterial == null)
        {
            flashMaterial = Resources.Load<Material>(flashMaterialName);
            if (flashMaterial == null)
            {
                Debug.LogError("Flash material not found in Resources: " + flashMaterialName);
                return;
            }
        }

        // Сохраняем оригинальный материал только один раз
        if (originalMaterial == null)
        {
            originalMaterial = spriteRenderer.sharedMaterial;
        }

        StartCoroutine(DoFlash(spriteRenderer, duration));
    }

    private IEnumerator DoFlash(SpriteRenderer spriteRenderer, float duration)
    {
        spriteRenderer.material = flashMaterial;
        var saveColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = saveColor;
        spriteRenderer.material = originalMaterial;
    }
}