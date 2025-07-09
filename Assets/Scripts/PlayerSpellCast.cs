using UnityEngine;

public class PlayerSpellCast : MonoBehaviour
{
    [Header("Staff Settings")]
    [SerializeField] private Transform wandTip; // точка на посохе для эффектов и каста заклинаний
    [SerializeField] private float wandOffset = 1f; // расстояние нижнего конца посоха от центра персонажа
    [SerializeField] private Vector3 wandPlayerCenterOffset = new Vector3(0, 0, 0);
    [SerializeField] private Transform wandTransform;
    [SerializeField] private SpriteRenderer wandSpriteRenderer;
    // положения посоха
    [SerializeField] private Vector3 idleStaffPositionLeft = new Vector3(0.335000008f, -0.555999994f, 0f);
    [SerializeField] private Vector3 idleStaffPositionRight = new Vector3(0.335000008f, -0.555999994f, 0f);
    [SerializeField] private Vector3 idleWandRotationLeft = new Vector3(0f, 0f, 45f);
    [SerializeField] private Vector3 idleWandRotationRight = new Vector3(0f, 0f, -45f);

    [Header("Spell Settings")]
    [SerializeField] private GameObject aim;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float spellSpeed = 30f;
    [SerializeField] private ParticleSystem redWandParticles;
    [SerializeField] private ParticleSystem castEffectParticles;
    [SerializeField] private float spellDuration = 3f;
    [SerializeField] private float aimRotationSpeed = 30f;
    [SerializeField] private float timeSlowFactor = 0.5f;
    [SerializeField] private int spellsPerSecond = 3;
    [SerializeField] private float spellSpread = 5f;

    private float currentSpellTime;
    private bool isCasting;
    private GameObject activeAim;
    private Vector3 targetPosition;
    private Camera mainCamera;
    private float lastHorizontalInput;
    private float lastSpellCastTimer = 0f;

    private void Start()
    {
        mainCamera = Camera.main;
        redWandParticles.Pause();
        redWandParticles.Clear();

        if (aim) aim.SetActive(false);
        lastHorizontalInput = 0f;
        UpdateIdleStaff();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isCasting)
        {
            StartCasting();
            lastSpellCastTimer = 1f / spellsPerSecond;
        }
        if ((Input.GetMouseButtonUp(1)))
        {
            isCasting = false;
            Destroy(activeAim);
        }

        if (isCasting)
        {
            UpdateCasting();
            if (lastSpellCastTimer <= 0f)
            {
                castEffectParticles.Emit(1);

                CastFireball();
                lastSpellCastTimer = 1f / spellsPerSecond;
            }
        }
        else
        {
            UpdateIdleStaff();
            redWandParticles.Pause();
            redWandParticles.Clear();
        }
        lastSpellCastTimer -= Time.deltaTime;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput != 0f)
        {
            lastHorizontalInput = horizontalInput;
        }
    }

    private void StartCasting()
    {
        isCasting = true;
        currentSpellTime = spellDuration;
        Time.timeScale = timeSlowFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // прицел
        targetPosition = GetMouseWorldPosition();
        activeAim = Instantiate(aim, targetPosition, Quaternion.identity);
        activeAim.SetActive(true);
        redWandParticles.Play();
    }

    private void UpdateCasting()
    {
        // прицел к курсору
        targetPosition = GetMouseWorldPosition();
        if (activeAim != null)
        {
            activeAim.transform.position = targetPosition;
            activeAim.transform.Rotate(0, 0, aimRotationSpeed * Time.unscaledDeltaTime);
        }

        Vector3 direction = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        wandTransform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        wandTransform.position = transform.position + direction * wandOffset + wandPlayerCenterOffset;

        // цвет посоха от черного к красному
        float t = 1f - (currentSpellTime / spellDuration);
        wandSpriteRenderer.color = Color.Lerp(Color.black, Color.red, t);

        currentSpellTime -= Time.unscaledDeltaTime;

    }

    private void UpdateIdleStaff()
    {
        Vector3 idlePosition = lastHorizontalInput > 0 ? idleStaffPositionRight : idleStaffPositionLeft;
        Vector3 idleRotation = lastHorizontalInput > 0 ? idleWandRotationRight : idleWandRotationLeft;

        wandTransform.localPosition = idlePosition;
        wandTransform.localRotation = Quaternion.Euler(idleRotation);

        wandSpriteRenderer.color = Color.black;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;
        return worldPos;
    }

    private void CastFireball()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        Vector3 direction = (targetPosition - wandTip.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);
        perpendicular *= Random.Range(-spellSpread, spellSpread);
        Vector3 spellCastPosition = new Vector3(wandTip.position.x + perpendicular.x, wandTip.position.y + perpendicular.y, 0);
        GameObject fireball = Instantiate(fireballPrefab, spellCastPosition, Quaternion.Euler(0, 0, angle - 90f));
        fireball.GetComponent<Rigidbody2D>().linearVelocity = direction * spellSpeed;

        wandSpriteRenderer.color = Color.black;
    }

    private void CancelSpell()
    {
        isCasting = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        redWandParticles.Pause();
        redWandParticles.Clear();
        Destroy(activeAim);
        wandSpriteRenderer.color = Color.black;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}