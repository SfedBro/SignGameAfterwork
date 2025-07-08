using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;

public class TorgashInteraction : MonoBehaviour
{
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private float playerNearbyTimeMax = 2f;

    private GameObject player;
    private PlayerController playerController;
    private bool isPlayerNearby = false;
    private bool isInteractive = false;
    private float playerNearbyTimer = 0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        isPlayerNearby = distance <= detectionRadius;

        if (isPlayerNearby)
        {
            playerNearbyTimer += Time.deltaTime;

            if (playerNearbyTimer >= playerNearbyTimeMax && !isInteractive)
            {
                Debug.Log("open shop");
                StartCoroutine(OpenShop());
            }
        }
        else
        {
            playerNearbyTimer = 0f;
        }

        // проверяем ЛКМ вне UI
        if (isInteractive && Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIObject())
            {
                StartCoroutine(CloseShop());
            }
        }
    }

    private IEnumerator OpenShop()
    {
        isInteractive = true;
        playerNearbyTimer = 0f;

        GUIManager guiManager = shopCanvas.GetComponent<GUIManager>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        this.GetComponent<Animator>().SetBool("open", true);
        yield return new WaitForSeconds(0.4f);

        guiManager.PanelActivate(true);

        ShopItemManager shopManager = shopCanvas.GetComponentInChildren<ShopItemManager>();
        foreach (var item in shopManager.GetComponentsInChildren<Shop>())
        {
            item.AnimateIn();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            shopCanvas.GetComponent<GUIManager>().PanelActivate(false);
            this.GetComponent<Animator>().SetBool("open", false);
            playerNearbyTimer = 0f;
            isInteractive = false;
        }
    }
    
    private IEnumerator CloseShop()
    {
        GUIManager guiManager = shopCanvas.GetComponent<GUIManager>();

        guiManager.PanelActivate(false);
        yield return new WaitForSeconds(1f);

        Animator animator = this.GetComponent<Animator>();
        animator.SetBool("open", false);
        Debug.Log("open=false");
        yield return new WaitForSeconds(1.3f);
        animator.SetTrigger("disappear");
        Debug.Log("disappear");
        yield return new WaitForSeconds(1.4f);

        if (playerController != null)
        {
            playerController.enabled = true;
        }
        Destroy(gameObject);
        isInteractive = false;
    }

    private bool IsPointerOverUIObject()
    {
        if (EventSystem.current == null) return false;

        return EventSystem.current.IsPointerOverGameObject();
    }
}
