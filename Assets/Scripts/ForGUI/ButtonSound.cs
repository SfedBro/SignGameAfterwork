using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioSource sfxSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioMixerGroup soundsMixerGroup;
    [SerializeField] private float hoverDelay = 0.2f; // �������� ����� ���������������� ����� ��������� (� ��������)

    private bool isPointerOver = false;
    private float pointerEnterTime;

    void Start()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = 0.8f;
        sfxSource.outputAudioMixerGroup = soundsMixerGroup;
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