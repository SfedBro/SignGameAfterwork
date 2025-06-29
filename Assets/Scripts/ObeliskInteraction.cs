using UnityEngine;

public class ObeliskInteraction : MonoBehaviour
{
    // [SerializeField] private GameObject letterE;
    [SerializeField] private GameObject blackPentagram;
    [SerializeField] private GameObject whitePentagram; 
    [SerializeField] private ParticleSystem spiralParticles;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float detectionRadius = 3f;

    private bool isPlayerNearby = false;
    private bool isActivated = false;
    private SpriteRenderer blackPentagramRenderer;
    private SpriteRenderer whitePentagramRenderer;
    private GameObject player;

    private void Start()
    {
        blackPentagramRenderer = blackPentagram.GetComponent<SpriteRenderer>();
        whitePentagramRenderer = whitePentagram.GetComponent<SpriteRenderer>();

        SetAlpha(whitePentagramRenderer, 0f);

        // letterE.SetActive(false); 

        spiralParticles.Stop(); 
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);
        isPlayerNearby = distance <= detectionRadius;
        
        // letterE.SetActive(isPlayerNearby); 

        // if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isActivated)
        if (isPlayerNearby && !isActivated)
        {
            isActivated = true;
            StartCoroutine(FadePentagrams());
        }
    }

    private System.Collections.IEnumerator FadePentagrams()
    {
        float elapsedTime = 0f;
        spiralParticles.Play();

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            
            SetAlpha(blackPentagramRenderer, Mathf.Lerp(1f, 0f, t));
            
            SetAlpha(whitePentagramRenderer, Mathf.Lerp(0f, 1f, t)); 
            yield return null;
        }

        SetAlpha(blackPentagramRenderer, 0f);
        SetAlpha(whitePentagramRenderer, 1f);
    }

    private void SetAlpha(SpriteRenderer renderer, float alpha)
    {
        Color color = renderer.color;
        color.a = alpha;
        renderer.color = color;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); 
    }
}