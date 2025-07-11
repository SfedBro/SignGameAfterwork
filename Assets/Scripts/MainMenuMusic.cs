using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip menuMusic;
    [SerializeField] private float fadeDuration = 1f;

    private bool isFading = false;

    void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = menuMusic;
        musicSource.loop = true;
        musicSource.volume = 1f;
        musicSource.Play();
    }

    public void FadeOutMusic()
    {
        if (!isFading)
        {
            isFading = true;
            StartCoroutine(FadeOut());
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
        isFading = false;
    }
}
