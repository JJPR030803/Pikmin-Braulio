using UnityEngine;
using System.Collections;

/// <summary>
/// Micro volcano that shoots fire projectiles
/// Can be destroyed by Red Pikmin
/// </summary>
public class MicroVolcano : ObstacleBase
{
    [Header("Volcano Settings")]
    [SerializeField] private GameObject fireProjectilePrefab;
    [SerializeField] private Transform[] shootPoints;
    [SerializeField] private float shootInterval = 3f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifetime = 5f;
    [SerializeField] private float projectileRadius = 2f;

    [Header("Volcano Visual Effects")]
    [SerializeField] private ParticleSystem smokeEffect;
    [SerializeField] private ParticleSystem lavaEffect;
    [SerializeField] private Material lavaMaterial;
    [SerializeField] private float lavaGlowIntensity = 2f;

    private float lastShootTime;
    private bool isShooting = false;

    protected override void Start()
    {
        base.Start();
        hazardType = "fire";
        vulnerableToPikmin = new PikminColor[] { PikminColor.Red };

        // Auto-detect shoot points if not set
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
            ShootFire();
            lastShootTime = Time.time;
        }
    }

    /// <summary>
    /// Shoot fire projectiles from the volcano
    /// </summary>
    void ShootFire()
    {
        if (shootPoints == null || shootPoints.Length == 0) return;

        foreach (var shootPoint in shootPoints)
        {
            if (shootPoint == null) continue;

            // Create fire projectile
            if (fireProjectilePrefab != null)
            {
                GameObject projectile = Instantiate(fireProjectilePrefab, shootPoint.position, Quaternion.identity);

                // Add velocity to projectile
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 shootDirection = shootPoint.forward;
                    // Add slight arc
                    shootDirection = (shootDirection + Vector3.up * 0.3f).normalized;
                    rb.linearVelocity = shootDirection * projectileSpeed;
                }

                // Add fire projectile behavior
                FireProjectile fireBehavior = projectile.GetComponent<FireProjectile>();
                if (fireBehavior == null)
                {
                    fireBehavior = projectile.AddComponent<FireProjectile>();
                }
                fireBehavior.Initialize(damagePerSecond, projectileLifetime, projectileRadius);

                Destroy(projectile, projectileLifetime);
            }
            else
            {
                // Fallback: create a simple fire sphere
                CreateFireSphere(shootPoint.position);
            }
        }

        // Play shoot effects
        if (smokeEffect != null)
        {
            smokeEffect.Play();
        }
    }

    /// <summary>
    /// Create a simple fire sphere projectile if no prefab is set
    /// </summary>
    void CreateFireSphere(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * 0.5f;
        sphere.name = "FireProjectile";

        // Add visual
        Renderer renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(1f, 0.3f, 0f);
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", Color.red * 2f);
        }

        // Add physics
        Rigidbody rb = sphere.AddComponent<Rigidbody>();
        rb.useGravity = true;
        Vector3 shootDirection = (transform.forward + Vector3.up * 0.3f).normalized;
        rb.linearVelocity = shootDirection * projectileSpeed;

        // Add trigger collider
        SphereCollider collider = sphere.GetComponent<SphereCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        // Add fire projectile component
        FireProjectile fireBehavior = sphere.AddComponent<FireProjectile>();
        fireBehavior.Initialize(damagePerSecond, projectileLifetime, projectileRadius);

        Destroy(sphere, projectileLifetime);
    }

    protected override void UpdateRenderers(float healthRatio)
    {
        base.UpdateRenderers(healthRatio);

        // Update lava glow based on health
        if (lavaMaterial != null)
        {
            lavaMaterial.EnableKeyword("_EMISSION");
            Color emissionColor = Color.red * lavaGlowIntensity * healthRatio;
            lavaMaterial.SetColor("_EmissionColor", emissionColor);
        }

        // Update lava effect
        if (lavaEffect != null)
        {
            var main = lavaEffect.main;
            main.startColor = new Color(1f, healthRatio * 0.5f, 0f);
        }
    }

    protected override void OnObstacleDestroyed()
    {
        base.OnObstacleDestroyed();

        // Stop all effects
        if (smokeEffect != null) smokeEffect.Stop();
        if (lavaEffect != null) lavaEffect.Stop();
    }
}

/// <summary>
/// Component for fire projectiles shot by volcanoes
/// </summary>
public class FireProjectile : MonoBehaviour
{
    private float damage;
    private float lifetime;
    private float radius;
    private float spawnTime;

    public void Initialize(float damagePerSecond, float projectileLifetime, float aoeRadius)
    {
        damage = damagePerSecond;
        lifetime = projectileLifetime;
        radius = aoeRadius;
        spawnTime = Time.time;
    }

    void OnTriggerEnter(Collider other)
    {
        // Damage Pikmin on contact
        Pikmin pikmin = other.GetComponent<Pikmin>();
        if (pikmin != null)
        {
            PikminType pikminType = other.GetComponent<PikminType>();
            if (pikminType == null || !pikminType.CanSurviveHazard("fire"))
            {
                Health health = other.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }
        }

        // Destroy on contact with ground or obstacles
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Explode();
        }
    }

    void Update()
    {
        // Auto-destroy after lifetime
        if (Time.time - spawnTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        // Area damage
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var col in colliders)
        {
            Pikmin pikmin = col.GetComponent<Pikmin>();
            if (pikmin != null)
            {
                PikminType pikminType = col.GetComponent<PikminType>();
                if (pikminType == null || !pikminType.CanSurviveHazard("fire"))
                {
                    Health health = col.GetComponent<Health>();
                    if (health != null)
                    {
                        health.TakeDamage(damage);
                    }
                }
            }
        }

        Destroy(gameObject);
    }
}
