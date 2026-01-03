using UnityEngine;

/// <summary>
/// Water hazard area that only swimming Pikmin can pass through
/// Non-swimming Pikmin will drown
/// </summary>
public class WaterHazard : ObstacleBase
{
    [Header("Water Settings")]
    [SerializeField] private float waterLevel = 0f;
    [SerializeField] private bool deepWater = true;
    [Tooltip("Time before non-swimming Pikmin start drowning")]
    [SerializeField] private float drowningDelay = 1f;
    [SerializeField] private float drowningDamagePerSecond = 20f;

    [Header("Visual Effects")]
    [SerializeField] private Color waterColor = new Color(0.2f, 0.5f, 0.8f, 0.6f);
    [SerializeField] private ParticleSystem waterSplashEffect;
    [SerializeField] private ParticleSystem bubblesEffect;
    [SerializeField] private float waterWaveSpeed = 1f;
    [SerializeField] private float waterWaveAmount = 0.1f;

    [Header("Physics")]
    [SerializeField] private float waterDrag = 2f;
    [SerializeField] private float buoyancyForce = 5f;

    private System.Collections.Generic.Dictionary<GameObject, float> pikminInWater = new System.Collections.Generic.Dictionary<GameObject, float>();

    protected override void Start()
    {
        base.Start();
        hazardType = "water";
        canBeDestroyed = false; // Water cannot be destroyed
        vulnerableToPikmin = new PikminColor[] { }; // No Pikmin can destroy water

        // Update visual appearance
        ApplyWaterVisuals();
    }

    protected override void Update()
    {
        base.Update();
        AnimateWater();
        CheckDrowningPikmin();
    }

    /// <summary>
    /// Apply water visual effects
    /// </summary>
    void ApplyWaterVisuals()
    {
        if (obstacleRenderers != null)
        {
            foreach (var renderer in obstacleRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = waterColor;

                    // Make water transparent - works for both Standard and URP shaders
                    // Standard Shader
                    if (renderer.material.HasProperty("_Mode"))
                    {
                        renderer.material.SetFloat("_Mode", 3); // Transparent mode
                        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        renderer.material.SetInt("_ZWrite", 0);
                        renderer.material.DisableKeyword("_ALPHATEST_ON");
                        renderer.material.EnableKeyword("_ALPHABLEND_ON");
                        renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        renderer.material.renderQueue = 3000;
                    }
                    // URP Shader
                    else if (renderer.material.HasProperty("_Surface"))
                    {
                        renderer.material.SetFloat("_Surface", 1); // Transparent
                        renderer.material.SetFloat("_Blend", 0); // Alpha blending
                        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        renderer.material.SetInt("_ZWrite", 0);
                        renderer.material.renderQueue = 3000;
                        renderer.material.SetShaderPassEnabled("DepthOnly", false);
                    }

                    // Add subtle glow
                    if (renderer.material.HasProperty("_EmissionColor"))
                    {
                        renderer.material.EnableKeyword("_EMISSION");
                        renderer.material.SetColor("_EmissionColor", waterColor * 0.3f);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Animate water surface
    /// </summary>
    void AnimateWater()
    {
        if (obstacleRenderers != null)
        {
            foreach (var renderer in obstacleRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    // Simple wave animation using UV offset
                    float offset = Mathf.Sin(Time.time * waterWaveSpeed) * waterWaveAmount;
                    renderer.material.mainTextureOffset = new Vector2(offset, offset);
                }
            }
        }
    }

    /// <summary>
    /// Check for drowning Pikmin
    /// </summary>
    void CheckDrowningPikmin()
    {
        // Create a copy of keys to avoid modification during iteration
        var pikminList = new System.Collections.Generic.List<GameObject>(pikminInWater.Keys);

        foreach (var pikmin in pikminList)
        {
            if (pikmin == null)
            {
                pikminInWater.Remove(pikmin);
                continue;
            }

            float timeInWater = pikminInWater[pikmin];
            timeInWater += Time.deltaTime;
            pikminInWater[pikmin] = timeInWater;

            // Check if drowning delay has passed
            if (timeInWater >= drowningDelay)
            {
                PikminType pikminType = pikmin.GetComponent<PikminType>();
                if (pikminType == null || !pikminType.CanSwim())
                {
                    // Drown the Pikmin
                    Health health = pikmin.GetComponent<Health>();
                    if (health != null)
                    {
                        health.TakeDamage(drowningDamagePerSecond * Time.deltaTime);
                    }
                }
            }
        }
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (isDestroyed) return;

        Pikmin pikmin = other.GetComponent<Pikmin>();
        if (pikmin != null)
        {
            PikminType pikminType = other.GetComponent<PikminType>();

            // Check if Pikmin can swim
            if (pikminType != null && pikminType.CanSwim())
            {
                // Swimming Pikmin are safe
                ApplyBuoyancy(other);
                return;
            }

            // Non-swimming Pikmin are in danger
            if (!pikminInWater.ContainsKey(other.gameObject))
            {
                pikminInWater[other.gameObject] = 0f;
                Debug.Log($"[WaterHazard] {other.name} entered water - cannot swim!");

                // Play splash effect
                if (waterSplashEffect != null)
                {
                    waterSplashEffect.transform.position = other.transform.position;
                    waterSplashEffect.Play();
                }
            }

            // Apply water drag
            ApplyWaterDrag(other);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Pikmin pikmin = other.GetComponent<Pikmin>();
        if (pikmin != null)
        {
            PikminType pikminType = other.GetComponent<PikminType>();

            if (pikminType != null && pikminType.CanSwim())
            {
                Debug.Log($"[WaterHazard] {other.name} (can swim) entering water safely");
            }
            else
            {
                Debug.Log($"[WaterHazard] {other.name} entering water - will drown!");
            }

            // Play splash
            if (waterSplashEffect != null)
            {
                waterSplashEffect.transform.position = other.transform.position;
                waterSplashEffect.Play();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (pikminInWater.ContainsKey(other.gameObject))
        {
            pikminInWater.Remove(other.gameObject);
            Debug.Log($"[WaterHazard] {other.name} exited water");
        }
    }

    /// <summary>
    /// Apply water drag to slow down Pikmin
    /// </summary>
    void ApplyWaterDrag(Collider pikminCollider)
    {
        Rigidbody rb = pikminCollider.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearDamping = waterDrag;
        }
    }

    /// <summary>
    /// Apply buoyancy force to swimming Pikmin
    /// </summary>
    void ApplyBuoyancy(Collider pikminCollider)
    {
        Rigidbody rb = pikminCollider.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Apply upward force to simulate buoyancy
            rb.AddForce(Vector3.up * buoyancyForce * Time.deltaTime, ForceMode.Acceleration);
            rb.linearDamping = waterDrag * 0.5f; // Less drag for swimming Pikmin
        }
    }

    protected override void UpdateRenderers(float healthRatio)
    {
        // Override to maintain water appearance
        ApplyWaterVisuals();
    }

    // Override to prevent destruction
    public override void TakeDamage(float damage, PikminColor attackerType)
    {
        Debug.Log($"[WaterHazard] Water cannot be destroyed!");
    }
}
