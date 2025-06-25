using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;
    public SpellEffect effect;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        effect = GetComponent<SpellEffect>();
    }   
}