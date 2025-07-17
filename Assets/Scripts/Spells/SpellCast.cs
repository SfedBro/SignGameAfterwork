using UnityEngine;

public class SpellCast : MonoBehaviour
{
    [Header("Staff Settings")]
    [SerializeField] private Transform wandTip; // точка на посохе для эффектов и каста заклинаний
    [SerializeField] private float wandOffset = 1f; // расстояние нижнего конца посоха от центра персонажа
    [SerializeField] private Vector3 wandPlayerCenterOffset = new Vector3(0, -0.36f, 0);
    [SerializeField] private Transform wandTransform;
    [SerializeField] private SpriteRenderer wandSpriteRenderer;
    // положения посоха
    [SerializeField] private Vector3 idleStaffPositionLeft = new Vector3(-0.34f, 0.16f, 0f);
    [SerializeField] private Vector3 idleStaffPositionRight = new Vector3(0.34f, 0.16f, 0f);
    [SerializeField] private Vector3 idleWandRotationLeft = new Vector3(0f, 0f, 45f);
    [SerializeField] private Vector3 idleWandRotationRight = new Vector3(0f, 0f, -45f);

    [Header("Spell Settings")]
    [SerializeField] private GameObject aim;
    [SerializeField] private float spellSpeed = 15f;
    [SerializeField] private ParticleSystem redWandParticles;
    [SerializeField] private ParticleSystem castEffectParticles;
    [SerializeField] private float spellDuration = 3f;
    [SerializeField] private float aimRotationSpeed = 200f;
    [SerializeField] private float timeSlowFactor = 1f;

    private float currentSpellTime;
    private bool isCasting;
    private GameObject activeAim;
    private Vector3 targetPosition;
    private Camera mainCamera;
    private float lastHorizontalInput;
    private Spell spellToCast;

    public void SetSpell(Spell someSpell)
    {
        spellToCast = someSpell;
    }

    public void MoveWand(float amount)
    {
        wandOffset *= amount;
        wandPlayerCenterOffset *= amount;
    }
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
        if (spellToCast && Input.GetMouseButtonDown(1) && !isCasting)
        {
            StartCasting();
        }
        if (Input.GetMouseButtonUp(1) && isCasting)
        {
            if (spellToCast)
            {
                HandleSpell();
            }
            else
            {
                Debug.Log("Заклинание не задано");
            }
            isCasting = false;
            Destroy(activeAim);
            redWandParticles.Pause();
            redWandParticles.Clear();
        }

        if (isCasting)
        {
            UpdateCasting();
        }
        else
        {
            UpdateIdleStaff();
        }

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

    private void HandleSpell()
    {
        isCasting = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        redWandParticles.Pause();
        wandSpriteRenderer.color = Color.black;

        if (spellToCast.Type == "Shoot")
        {
            ShootingSpell((ShootSpell)spellToCast);
        }
        else if (spellToCast.Type == "ThroughShoot")
        {
            ThroughShootSpelling((ThroughShootSpell)spellToCast);
        }
        else if (spellToCast.Type == "DistanceWeakeningShoot")
        {
            DistanceWeakeningShootSpelling((DistanceWeakeningShootSpell)spellToCast);
        }
        else if (spellToCast.Type == "AoE")
        {
            AreaSpell((AoeSpell)spellToCast);
        }
        else if (spellToCast.Type == "Buff")
        {
            SelfSpell((BuffSpell)spellToCast);
        }
        else if (spellToCast.Type == "FindNearest")
        {
            NearestEnemySpell((NearestEnemySpell)spellToCast);
        }
        else if (spellToCast.Type == "AoEFromSelf")
        {
            SelfAreaSpell((AoeFromSelf)spellToCast);
        }
        else if (spellToCast.Type == "Illusion")
        {
            IllusionSpell((CreateIllusionSpell)spellToCast);
        }
        else
        {
            Debug.Log($"{spellToCast.Type} - неизвестный тип заклинания");
        }

        Destroy(activeAim);

        wandSpriteRenderer.color = Color.black;
        spellToCast = null;
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

    private void ShootingSpell(ShootSpell someSpell)
    {
        Vector3 direction = (targetPosition - wandTip.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector3 selfAngles = someSpell.Prefab.GetComponent<Transform>().eulerAngles;

        GameObject obj = Instantiate(someSpell.Prefab, wandTip.position, Quaternion.Euler(selfAngles.x, selfAngles.y, selfAngles.z + angle - 90f));
        obj.AddComponent<ShootSpellActions>();
        obj.GetComponent<ShootSpellActions>().SetSettings(gameObject, someSpell.MainElement, someSpell.Damage, someSpell.Effect,
                                                        someSpell.EffectAmount, someSpell.EffectDuration, someSpell.EffectChance);

        obj.GetComponent<Rigidbody2D>().linearVelocity = direction * spellSpeed;
    }

    private void AreaSpell(AoeSpell someSpell)
    {
        GameObject obj = Instantiate(someSpell.Prefab, targetPosition, Quaternion.identity);
        obj.AddComponent<AreaSpellActions>();
        obj.GetComponent<AreaSpellActions>().SetSettings(gameObject, someSpell.MainElement, someSpell.Effect, someSpell.EffectAmount,
                                                        someSpell.EffectDuration, someSpell.EffectChance, someSpell.AreaLifetime);
        obj.GetComponent<Transform>().localScale = new Vector3(someSpell.Radius * 2, someSpell.Radius * 2, 0);
    }

    private void SelfAreaSpell(AoeFromSelf someSpell)
    {
        GameObject obj = Instantiate(someSpell.Prefab, transform.position, Quaternion.identity);
        obj.AddComponent<AreaSpellActions>();
        obj.GetComponent<AreaSpellActions>().SetSettings(gameObject, someSpell.MainElement, someSpell.Effect, someSpell.EffectAmount,
                                                        someSpell.EffectDuration, someSpell.EffectChance, someSpell.AreaLifetime);
        obj.GetComponent<Transform>().localScale = new Vector3(someSpell.Radius * 2, someSpell.Radius * 2, 0);
    }

    private void NearestEnemySpell(NearestEnemySpell someSpell)
    {
        GameObject obj = Instantiate(someSpell.Prefab, targetPosition, Quaternion.identity);
        obj.AddComponent<NearestEnemyActions>();
        obj.GetComponent<NearestEnemyActions>().SetSettings(gameObject, someSpell.MainElement, someSpell.Effect, someSpell.EffectAmount,
                                                            someSpell.EffectDuration, someSpell.EffectChance, someSpell.AreaLifetime);

        obj.GetComponent<Transform>().localScale = new Vector3(someSpell.Radius * 2, someSpell.Radius * 2, 0);
    }

    private void ThroughShootSpelling(ThroughShootSpell someSpell)
    {
        Vector3 direction = (targetPosition - wandTip.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject obj = Instantiate(someSpell.Prefab, wandTip.position, Quaternion.Euler(0, 0, angle));
        obj.AddComponent<ThroughShootSpellActions>();
        obj.GetComponent<ThroughShootSpellActions>().SetSettings(gameObject, someSpell.MainElement, someSpell.Damage, someSpell.Effect,
                                                                someSpell.EffectAmount, someSpell.EffectDuration, someSpell.EffectChance);

        obj.GetComponent<Rigidbody2D>().linearVelocity = direction * spellSpeed;
    }
    
    private void DistanceWeakeningShootSpelling(DistanceWeakeningShootSpell someSpell)
    {
        Vector3 direction = (targetPosition - wandTip.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject obj = Instantiate(someSpell.Prefab, wandTip.position, Quaternion.Euler(0, 0, angle));
        obj.AddComponent<DistanceWeakeningShootSpellActions>();
        obj.GetComponent<DistanceWeakeningShootSpellActions>().SetSettings(gameObject, someSpell.MainElement, someSpell.Damage, someSpell.Distance,
                                                                        someSpell.Effect, someSpell.EffectAmount, someSpell.EffectChance, someSpell.EffectDuration);

        obj.GetComponent<Rigidbody2D>().linearVelocity = direction * spellSpeed;
    }

    private void IllusionSpell(CreateIllusionSpell someSpell)
    {
        GameObject obj = Instantiate(someSpell.Prefab, transform.position, Quaternion.identity);
        obj.GetComponent<SpriteRenderer>().flipX = gameObject.GetComponent<SpriteRenderer>().flipX;

        obj.AddComponent<IllusionActions>();
        obj.GetComponent<IllusionActions>().SetSettings(gameObject, someSpell.MainElement, someSpell.Effect, someSpell.EffectAmount,
                                                        someSpell.EffectDuration, someSpell.EffectChance, someSpell.Lifetime);

    }

    private void SelfSpell(BuffSpell someSpell)
    {
        if (!GetComponent<EffectsHandler>())
        {
            gameObject.AddComponent<EffectsHandler>();
        }
        GetComponent<EffectsHandler>().HandleEffect(gameObject, someSpell.MainElement, someSpell.Effect,
                                                    someSpell.EffectAmount, someSpell.EffectDuration, someSpell.EffectChance);
    }
}