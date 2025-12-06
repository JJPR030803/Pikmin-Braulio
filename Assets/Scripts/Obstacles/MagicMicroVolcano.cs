using UnityEngine;

/// <summary>
/// Magic micro volcano that shoots weird freezing water projectiles
/// Pikmin that touch the water get frozen
/// </summary>
public class MagicMicroVolcano : ObstacleBase
{
    [Header("Magic Volcano Settings")]
    [SerializeField] private GameObject freezeWaterProjectilePrefab;
    [SerializeField] private Transform[] shootPoints;
    [SerializeField] private float shootInterval = 4f;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float projectileLifetime = 6f;
    [SerializeField] private float freezeRadius = 2.5f;

    [Header("Freeze Settings")]
    [SerializeField] private float freezeDuration = 5f;
    [SerializeField] private float freezeDamagePerSecond = 5f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem magicSmokeEffect;
    [SerializeField] private ParticleSystem waterGeyserEffect;
    [SerializeField] private Color magicWaterColor = new Color(0.3f, 0.8f, 1f, 0.8f);
    [SerializeField] private Material magicMaterial;
    [SerializeField] private float magicGlowIntensity = 1.5f;

    private float lastShootTime;

    protected override void Start()
    {
        base.Start();
        hazardType = "ice";
        vulnerableToPikmin = new PikminColor[] { }; // Cannot be destroyed (magical)
        canBeDestroyed = false;

        // Auto-detect shoot points
        if (shootPoints == null || shootPoints.Length == 0)
        {
            Transform shootPoint = transform.Find("ShootPoint");
            if (shootPoint != null)
            {
                shootPoints = new Transform[] { shootPoint };
            }
            else
            {
                shootPoints = new Transform[] { transform };
            }
        }

        lastShootTime = Time.time;
    }

    protected override void Update()
    {
        base.Update();

        if (!isDestroyed && Time.time - lastShootTime >= shootInterval)
        {
            ShootFreezeWater();
            lastShootTime = Time.time;
        }
    }

    /// <summary>
    /// Shoot freeze water projectiles from the volcano
    /// </summary>
    void ShootFreezeWater()
    {
        if (shootPoints == null || shootPoints.Length == 0) return;

        foreach (var shootPoint in shootPoints)
        {
            if (shootPoint == null) continue;

            // Create freeze water projectile
            if (freezeWaterProjectilePrefab != null)
            {
                GameObject projectile = Instantiate(freezeWaterProjectilePrefab, shootPoint.position, Quaternion.identity);

                // Add velocity
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 shootDirection = shootPoint.forward;
                    shootDirection = (shootDirection + Vector3.up * 0.4f).normalized;
                    rb.linearVelocity = shootDirection * projectileSpeed;
                }

                // Add freeze water behavior
                FreezeWaterProjectile freezeBehavior = projectile.GetComponent<FreezeWaterProjectile>();
                if (freezeBehavior == null)
                {
                    freezeBehavior = projectile.AddComponent<FreezeWaterProjectile>();
                }
                freezeBehavior.Initialize(freezeDamagePerSecond, projectileLifetime, freezeRadius, freezeDuration);

                Destroy(projectile, projectileLifetime);
            }
            else
            {
                // Fallback: create a simple freeze water sphere
                CreateFreezeWaterSphere(shootPoint.position);
            }
        }

        // Play effects
        if (magicSmokeEffect != null)
        {
            magicSmokeEffect.Play();
        }
        if (waterGeyserEffect != null)
        {
            waterGeyserEffect.Play();
        }
    }

    /// <summary>
    /// Create a simple freeze water projectile
    /// </summary>
    void CreateFreezeWaterSphere(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * 0.6f;
        sphere.name = "FreezeWaterProjectile";

        // Add visual
        Renderer renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = magicWaterColor;
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", magicWaterColor * magicGlowIntensity);
        }

        // Add physics
        Rigidbody rb = sphere.AddComponent<Rigidbody>();
        rb.useGravity = true;
        Vector3 shootDirection = (transform.forward + Vector3.up * 0.4f).normalized;
        rb.linearVelocity = shootDirection * projectileSpeed;

        // Add trigger collider
        SphereCollider collider = sphere.GetComponent<SphereCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        // Add freeze water component
        FreezeWaterProjectile freezeBehavior = sphere.AddComponent<FreezeWaterProjectile>();
        freezeBehavior.Initialize(freezeDamagePerSecond, projectileLifetime, freezeRadius, freezeDuration);

        Destroy(sphere, projectileLifetime);
    }

    protected override void UpdateRenderers(float healthRatio)
    {
        base.UpdateRenderers(healthRatio);

        // Update magic glow
        if (magicMaterial != null)
        {
            magicMaterial.EnableKeyword("_EMISSION");
            Color emissionColor = magicWaterColor * magicGlowIntensity;
            magicMaterial.SetColor("_EmissionColor", emissionColor);
        }

        // Update magic color on all renderers
        if (obstacleRenderers != null)
        {
            foreach (var renderer in obstacleRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = magicWaterColor;
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", magicWaterColor * magicGlowIntensity);
                }
            }
        }
    }
}

/// <summary>
/// Component for freeze water projectiles that freeze Pikmin on contact
/// </summary>
public class FreezeWaterProjectile : MonoBehaviour
{
    private float damage;
    private float lifetime;
    private float radius;
    private float freezeDuration;
    private float spawnTime;

    public void Initialize(float damagePerSecond, float projectileLifetime, float aoeRadius, float freezeTime)
    {
        damage = damagePerSecond;
        lifetime = projectileLifetime;
        radius = aoeRadius;
        freezeDuration = freezeTime;
        spawnTime = Time.time;
    }

    void OnTriggerEnter(Collider other)
    {
        // Freeze Pikmin on contact
        Pikmin pikmin = other.GetComponent<Pikmin>();
        if (pikmin != null)
        {
            PikminType pikminType = other.GetComponent<PikminType>();

            // Check if Pikmin resists ice
            if (pikminType == null || !pikminType.CanSurviveHazard("ice"))
            {
                FreezePikmin(other.gameObject);
            }
        }

        // Explode on contact with ground or obstacles
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            ExplodeAndFreeze();
        }
    }

    void Update()
    {
        // Auto-destroy after lifetime
        if (Time.time - spawnTime >= lifetime)
        {
            ExplodeAndFreeze();
        }
    }

    /// <summary>
    /// Freeze a single Pikmin
    /// </summary>
    void FreezePikmin(GameObject pikminObject)
    {
        // Add freeze effect component
        FreezeEffect freezeEffect = pikminObject.GetComponent<FreezeEffect>();
        if (freezeEffect == null)
        {
            freezeEffect = pikminObject.AddComponent<FreezeEffect>();
        }

        freezeEffect.Freeze(freezeDuration, damage);
    }

    /// <summary>
    /// Explode and freeze all nearby Pikmin
    /// </summary>
    void ExplodeAndFreeze()
    {
        // Area freeze
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var col in colliders)
        {
            Pikmin pikmin = col.GetComponent<Pikmin>();
            if (pikmin != null)
            {
                PikminType pikminType = col.GetComponent<PikminType>();
                if (pikminType == null || !pikminType.CanSurviveHazard("ice"))
                {
                    FreezePikmin(col.gameObject);
                }
            }
        }

        Destroy(gameObject);
    }
}

/// <summary>
/// Component that freezes a Pikmin temporarily
/// </summary>
public class FreezeEffect : MonoBehaviour
{
    private bool isFrozen = false;
    private float freezeEndTime;
    private float damagePerSecond;
    private Material originalMaterial;
    private Renderer pikminRenderer;
    private Rigidbody rb;
    private Pikmin pikmin;

    void Awake()
    {
        pikminRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        pikmin = GetComponent<Pikmin>();

        if (pikminRenderer != null && pikminRenderer.material != null)
        {
            originalMaterial = new Material(pikminRenderer.material);
        }
    }

    public void Freeze(float duration, float damage)
    {
        if (isFrozen) return;

        isFrozen = true;
        freezeEndTime = Time.time + duration;
        damagePerSecond = damage;

        // Apply freeze visual
        if (pikminRenderer != null && pikminRenderer.material != null)
        {
            pikminRenderer.material.color = new Color(0.5f, 0.8f, 1f, 1f);
            pikminRenderer.material.EnableKeyword("_EMISSION");
            pikminRenderer.material.SetColor("_EmissionColor", Color.cyan * 0.5f);
        }

        // Disable movement
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (pikmin != null)
        {
            pikmin.enabled = false;
        }

        Debug.Log($"[FreezeEffect] {gameObject.name} frozen for {duration} seconds!");
    }

    void Update()
    {
        if (!isFrozen) return;

        // Apply freeze damage
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damagePerSecond * Time.deltaTime);
        }

        // Check if freeze duration is over
        if (Time.time >= freezeEndTime)
        {
            Unfreeze();
        }
    }

    void Unfreeze()
    {
        isFrozen = false;

        // Restore visual
        if (pikminRenderer != null && originalMaterial != null)
        {
            pikminRenderer.material = originalMaterial;
        }

        // Re-enable movement
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if (pikmin != null)
        {
            pikmin.enabled = true;
        }

        Debug.Log($"[FreezeEffect] {gameObject.name} unfrozen!");
        Destroy(this);
    }
}
