using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    [SerializeField] ParticleSystem destroyEffect;
    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        ParticleSystem effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        effect.Play();
    }
}
