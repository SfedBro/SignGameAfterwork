using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void PlayHoverSound() {
        audioSource.PlayOneShot(hoverSound);
    }

    public void PlayClickSound() {
        audioSource.PlayOneShot(clickSound);
    }
}
