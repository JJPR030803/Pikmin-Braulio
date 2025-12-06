using UnityEngine;

/// <summary>
/// Poison hazard area that damages non-poison-resistant Pikmin
/// Can be neutralized by White Pikmin
/// Supports multiple 3D models via mesh renderers
/// </summary>
public class PoisonHazard : ObstacleBase
{
    [Header("Poison Visual Effects")]
    [SerializeField] private Color poisonColor = new Color(0.5f, 0f, 0.5f, 0.5f);
    [SerializeField] private float poisonGlowIntensity = 1f;

    [Header("Neutralization Settings")]
    [SerializeField] private bool respawnAfterNeutralized = false;
    [SerializeField] private float respawnTime = 15f;

    private float neutralizeTime = 0f;

    protected override void Start()
    {
        base.Start();

        // Set poison-specific properties
        hazardType = "poison";
        damagePerSecond = 8f;
        vulnerableToPikmin = new PikminColor[] { PikminColor.White };

        // Apply poison visuals
        ApplyPoisonVisuals();
    }

    protected override void Update()
    {
        base.Update();

        // Check for respawn
        if (isDestroyed && respawnAfterNeutralized)
        {
            if (Time.time - neutralizeTime >= respawnTime)
            {
                Reactivate();
            }
        }
    }

    /// <summary>
    /// Neutralize the poison gradually (called when taking damage from White Pikmin)
    /// </summary>
    protected override void OnDamageTaken(float damage, PikminColor attackerType)
    {
        base.OnDamageTaken(damage, attackerType);

        if (attackerType == PikminColor.White)
        {
            Debug.Log($"[PoisonHazard] Being neutralized by White Pikmin - {currentHealth}/{maxHealth}");
        }
    }

    /// <summary>
    /// Called when poison is neutralized
    /// </summary>
    protected override void OnObstacleDestroyed()
    {
        base.OnObstacleDestroyed();
        neutralizeTime = Time.time;
        Debug.Log($"[PoisonHazard] {gameObject.name} neutralized!");
    }

    /// <summary>
    /// Reactivate the poison
    /// </summary>
    void Reactivate()
    {
        currentHealth = maxHealth;
        isDestroyed = false;
        gameObject.SetActive(true);

        Debug.Log($"[PoisonHazard] {gameObject.name} reactivated!");

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
    /// Apply poison visual effects to all renderers
    /// </summary>
    void ApplyPoisonVisuals()
    {
        if (obstacleRenderers != null)
        {
            foreach (var renderer in obstacleRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = poisonColor;
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", poisonColor * poisonGlowIntensity);
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
                    // Update poison color based on strength
                    Color color = poisonColor;
                    color.a = healthRatio * 0.5f;
                    renderer.material.color = color;

                    // Update poison glow
                    renderer.material.EnableKeyword("_EMISSION");
                    Color emissionColor = poisonColor * poisonGlowIntensity * healthRatio;
                    renderer.material.SetColor("_EmissionColor", emissionColor);
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
                    main.startColor = new Color(poisonColor.r, poisonColor.g, poisonColor.b, healthRatio * 0.5f);
                }
            }
        }
    }

    public bool IsNeutralized() => isDestroyed;
    public float GetPoisonStrength() => currentHealth;
    public float GetPoisonStrengthRatio() => GetHealthRatio();
}
