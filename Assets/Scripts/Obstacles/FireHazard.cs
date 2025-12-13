using UnityEngine;

/// <summary>
/// Fire hazard that damages non-fire-resistant Pikmin
/// Can be extinguished by Red Pikmin
/// Supports multiple 3D models via mesh renderers
/// </summary>
public class FireHazard : ObstacleBase
{
    [Header("Fire Visual Effects")]
    [SerializeField] private Color fireColor = new Color(1f, 0.5f, 0f);
    [SerializeField] private float fireGlowIntensity = 2f;
    [SerializeField] private float minLightIntensity = 0f;
    [SerializeField] private float maxLightIntensity = 2f;

    [Header("Extinguish Settings")]
    [SerializeField] private bool respawnAfterExtinguish = true;
    [SerializeField] private float respawnTime = 10f;

    private float extinguishTime = 0f;

    protected override void Start()
    {
        base.Start();

        // Set fire-specific properties
        hazardType = "fire";
        damagePerSecond = 10f;
        vulnerableToPikmin = new PikminColor[] { PikminColor.Red };

        // Apply fire visuals
        ApplyFireVisuals();
    }

    protected override void Update()
    {
        base.Update();

        // Check for respawn
        if (isDestroyed && respawnAfterExtinguish)
        {
            if (Time.time - extinguishTime >= respawnTime)
            {
                Reignite();
            }
        }
    }

    /// <summary>
    /// Extinguish the fire gradually (called when taking damage from Red Pikmin)
    /// </summary>
    protected override void OnDamageTaken(float damage, PikminColor attackerType)
    {
        base.OnDamageTaken(damage, attackerType);

        if (attackerType == PikminColor.Red)
        {
            Debug.Log($"[FireHazard] Being extinguished by Red Pikmin - {currentHealth}/{maxHealth}");
        }
    }

    /// <summary>
    /// Called when fire is extinguished
    /// </summary>
    protected override void OnObstacleDestroyed()
    {
        base.OnObstacleDestroyed();
        extinguishTime = Time.time;
        Debug.Log($"[FireHazard] {gameObject.name} extinguished!");
    }

    /// <summary>
    /// Reignite the fire
    /// </summary>
    void Reignite()
    {
        currentHealth = maxHealth;
        isDestroyed = false;
        gameObject.SetActive(true);

        Debug.Log($"[FireHazard] {gameObject.name} reignited!");

        // Restart particle effects
        if (particleEffects != null)
        {
            foreach (var effect in particleEffects)
            {
                if (effect != null)
                {
                    effect.Play();
                }
            }
        }
    }

    /// <summary>
    /// Apply fire visual effects to all renderers
    /// </summary>
    void ApplyFireVisuals()
    {
        if (obstacleRenderers != null)
        {
            foreach (var renderer in obstacleRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = fireColor;

                    // Only apply emission if material supports it
                    if (renderer.material.HasProperty("_EmissionColor"))
                    {
                        renderer.material.EnableKeyword("_EMISSION");
                        renderer.material.SetColor("_EmissionColor", fireColor * fireGlowIntensity);
                    }
                }
            }
        }
    }

    protected override void UpdateRenderers(float healthRatio)
    {
        if (obstacleRenderers != null)
        {
            foreach (var renderer in obstacleRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    // Update fire color based on strength
                    Color color = fireColor;
                    color.a = healthRatio;
                    renderer.material.color = color;

                    // Update fire glow - only if material supports emission
                    if (renderer.material.HasProperty("_EmissionColor"))
                    {
                        renderer.material.EnableKeyword("_EMISSION");
                        Color emissionColor = fireColor * fireGlowIntensity * healthRatio;
                        renderer.material.SetColor("_EmissionColor", emissionColor);
                    }
                }
            }
        }
    }

    protected override void UpdateLights(float healthRatio)
    {
        if (obstacleLights != null)
        {
            foreach (var light in obstacleLights)
            {
                if (light != null)
                {
                    light.enabled = !isDestroyed;
                    light.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, healthRatio);
                    light.color = fireColor;
                }
            }
        }
    }

    protected override void UpdateParticleEffects(float healthRatio)
    {
        if (particleEffects != null)
        {
            foreach (var effect in particleEffects)
            {
                if (effect != null)
                {
                    var emission = effect.emission;
                    emission.enabled = !isDestroyed;

                    var main = effect.main;
                    main.startSizeMultiplier = healthRatio;
                }
            }
        }
    }

    public bool IsExtinguished() => isDestroyed;
    public float GetFireStrength() => currentHealth;
    public float GetFireStrengthRatio() => GetHealthRatio();
}
