using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioSource sfxSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    [SerializeField] private float hoverDelay = 0.2f; // Задержка перед воспроизведением звука наведения (в секундах)

    private bool isPointerOver = false;
    private float pointerEnterTime;

    void Start()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = 0.8f;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        pointerEnterTime = Time.time;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
    }

    void Update()
    {
        if (isPointerOver && Time.time - pointerEnterTime >= hoverDelay)
        {
            PlayHoverSound();
            isPointerOver = false;
        }
    }

    public void PlayHoverSound()
    {
        if (hoverSound != null)
        {
            sfxSource.PlayOneShot(hoverSound);
        }
    }

    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }
    }

    private static ButtonSound instance;
}