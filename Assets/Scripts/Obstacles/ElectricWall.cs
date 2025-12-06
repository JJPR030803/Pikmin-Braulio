using UnityEngine;

/// <summary>
/// Electric wall/obstacle that blocks passage and damages non-electric-resistant Pikmin
/// Can be destroyed by Yellow Pikmin
/// Supports multiple 3D models via mesh renderers
/// </summary>
public class ElectricWall : ObstacleBase
{
    [Header("Electric Wall Visual Effects")]
    [SerializeField] private Color electricColor = Color.yellow;
    [SerializeField] private float electricGlowIntensity = 2f;

    protected override void Start()
    {
        base.Start();

        // Set electric-specific properties
        hazardType = "electric";
        damagePerSecond = 15f;
        vulnerableToPikmin = new PikminColor[] { PikminColor.Yellow };

        // Apply electric color to all renderers
        ApplyElectricVisuals();
    }

    /// <summary>
    /// Apply electric visual effects to all renderers
    /// </summary>
    void ApplyElectricVisuals()
    {
        if (obstacleRenderers != null)
        {
            foreach (var renderer in obstacleRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = electricColor;
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", electricColor * electricGlowIntensity);
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
                    // Fade electric color based on health
                    Color color = electricColor;
                    color.a = healthRatio;
                    renderer.material.color = color;

                    // Update electric glow
                    renderer.material.EnableKeyword("_EMISSION");
                    Color emissionColor = electricColor * electricGlowIntensity * healthRatio;
                    renderer.material.SetColor("_EmissionColor", emissionColor);
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
                    light.intensity = healthRatio * electricGlowIntensity;
                    light.color = electricColor;
                }
            }
        }
    }
}
