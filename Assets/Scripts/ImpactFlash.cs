using System.Collections;
using UnityEngine;

public class ImpactFlash : MonoBehaviour
{
    // Название материала во встроенной папке Resources (Resources/Flash.mat)
    [SerializeField] private string flashMaterialName = "Flash";

    private Material flashMaterial;

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

        StartCoroutine(DoFlash(spriteRenderer, duration));
    }

    private IEnumerator DoFlash(SpriteRenderer spriteRenderer, float duration)
    {
        Material originalMaterial = spriteRenderer.sharedMaterial;
        spriteRenderer.material = flashMaterial;

        yield return new WaitForSeconds(duration);

        spriteRenderer.material = originalMaterial;
    }
}
