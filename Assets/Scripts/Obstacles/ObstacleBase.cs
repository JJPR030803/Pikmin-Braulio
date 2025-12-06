using UnityEngine;

/// <summary>
/// Base class for all obstacles in the game
/// Supports multiple 3D models via mesh renderers and configurable destruction/deactivation
/// </summary>
public abstract class ObstacleBase : MonoBehaviour
{
    [Header("Obstacle Properties")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth = 100f;
    [SerializeField] protected float damagePerSecond = 10f;

    [Header("Destruction Settings")]
    [SerializeField] protected bool canBeDestroyed = true;
    [Tooltip("If true, destroys the GameObject. If false, just deactivates it.")]
    [SerializeField] protected bool destroyOnZeroHealth = true;
    [SerializeField] protected GameObject destructionEffect;

    [Header("Visual Components (Supports Multiple Models)")]
    [Tooltip("All renderers in the obstacle - supports any 3D models")]
    [SerializeField] protected Renderer[] obstacleRenderers;
    [SerializeField] protected ParticleSystem[] particleEffects;
    [SerializeField] protected Light[] obstacleLights;

    [Header("Hazard Settings")]
    [SerializeField] protected string hazardType = "generic";
    [Tooltip("Pikmin types that can destroy/neutralize this obstacle")]
    [SerializeField] protected PikminColor[] vulnerableToPikmin;

    protected bool isDestroyed = false;
    protected Material[] originalMaterials;

    protected virtual void Awake()
    {
        // Auto-detect renderers if not set
        if (obstacleRenderers == null || obstacleRenderers.Length == 0)
        {
            obstacleRenderers = GetComponentsInChildren<Renderer>();
        }

        // Store original materials
        CacheOriginalMaterials();
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        UpdateVisuals();
    }

    protected virtual void Update()
    {
        UpdateVisuals();
    }

    /// <summary>
    /// Cache original materials for restoration/modification
    /// </summary>
    protected virtual void CacheOriginalMaterials()
    {
        if (obstacleRenderers != null && obstacleRenderers.Length > 0)
        {
            originalMaterials = new Material[obstacleRenderers.Length];
            for (int i = 0; i < obstacleRenderers.Length; i++)
            {
                if (obstacleRenderers[i] != null && obstacleRenderers[i].material != null)
                {
                    originalMaterials[i] = new Material(obstacleRenderers[i].material);
                }
            }
        }
    }

    /// <summary>
    /// Take damage from Pikmin attacks
    /// </summary>
    public virtual void TakeDamage(float damage, PikminColor attackerType)
    {
        if (!canBeDestroyed || isDestroyed) return;

        // Check if this Pikmin type can damage this obstacle
        if (vulnerableToPikmin != null && vulnerableToPikmin.Length > 0)
        {
            bool canDamage = false;
            foreach (var pikminType in vulnerableToPikmin)
            {
                if (pikminType == attackerType)
                {
                    canDamage = true;
                    break;
                }
            }

            if (!canDamage)
            {
                Debug.Log($"[{GetType().Name}] {attackerType} Pikmin cannot damage this obstacle!");
                return;
            }
        }

        currentHealth -= damage;
        OnDamageTaken(damage, attackerType);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            DestroyObstacle();
        }
    }

    /// <summary>
    /// Called when damage is taken (override for custom behavior)
    /// </summary>
    protected virtual void OnDamageTaken(float damage, PikminColor attackerType)
    {
        // Override in derived classes
    }

    /// <summary>
    /// Destroy or deactivate the obstacle
    /// </summary>
    protected virtual void DestroyObstacle()
    {
        if (isDestroyed) return;

        isDestroyed = true;
        Debug.Log($"[{GetType().Name}] {gameObject.name} destroyed!");

        // Spawn destruction effect
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        OnObstacleDestroyed();

        if (destroyOnZeroHealth)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when obstacle is destroyed (override for custom behavior)
    /// </summary>
    protected virtual void OnObstacleDestroyed()
    {
        // Override in derived classes
    }

    /// <summary>
    /// Update visual effects based on obstacle state
    /// </summary>
    protected virtual void UpdateVisuals()
    {
        if (isDestroyed) return;

        float healthRatio = maxHealth > 0 ? currentHealth / maxHealth : 0;

        // Update particle effects
        UpdateParticleEffects(healthRatio);

        // Update lights
        UpdateLights(healthRatio);

        // Update renderers
        UpdateRenderers(healthRatio);
    }

    /// <summary>
    /// Update all particle effects
    /// </summary>
    protected virtual void UpdateParticleEffects(float healthRatio)
    {
        if (particleEffects != null)
        {
            foreach (var effect in particleEffects)
            {
                if (effect != null)
                {
                    var emission = effect.emission;
                    emission.enabled = !isDestroyed;
                    emission.rateOverTimeMultiplier = healthRatio * 50f;
                }
            }
        }
    }

    /// <summary>
    /// Update all lights
    /// </summary>
    protected virtual void UpdateLights(float healthRatio)
    {
        if (obstacleLights != null)
        {
            foreach (var light in obstacleLights)
            {
                if (light != null)
                {
                    light.enabled = !isDestroyed;
                    light.intensity = healthRatio * 2f;
                }
            }
        }
    }

    /// <summary>
    /// Update all renderers (works with any 3D model)
    /// </summary>
    protected virtual void UpdateRenderers(float healthRatio)
    {
        if (obstacleRenderers != null)
        {
            foreach (var renderer in obstacleRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    Color color = renderer.material.color;
                    color.a = healthRatio;
                    renderer.material.color = color;
                }
            }
        }
    }

    /// <summary>
    /// Handle collision with Pikmin
    /// </summary>
    protected virtual void OnTriggerStay(Collider other)
    {
        if (isDestroyed) return;

        Pikmin pikmin = other.GetComponent<Pikmin>();
        if (pikmin != null)
        {
            PikminType pikminType = other.GetComponent<PikminType>();

            // Check if Pikmin can survive this hazard
            if (pikminType == null || !pikminType.CanSurviveHazard(hazardType))
            {
                DamagePikmin(other.gameObject);
            }
        }
    }

    /// <summary>
    /// Damage a Pikmin that touches the obstacle
    /// </summary>
    protected virtual void DamagePikmin(GameObject pikminObject)
    {
        var health = pikminObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    // Public getters
    public float GetHealth() => currentHealth;
    public float GetHealthRatio() => maxHealth > 0 ? currentHealth / maxHealth : 0;
    public bool IsDestroyed() => isDestroyed;
    public string GetHazardType() => hazardType;
}
