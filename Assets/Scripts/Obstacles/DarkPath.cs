using UnityEngine;

/// <summary>
/// Dark path area that only Dark Pikmin can pass through
/// Other Pikmin types are blocked or take damage
/// </summary>
public class DarkPath : ObstacleBase
{
    [Header("Dark Path Settings")]
    [SerializeField] private bool blockNonDarkPikmin = true;
    [Tooltip("If false, non-dark Pikmin take damage instead of being blocked")]
    [SerializeField] private bool damageInsteadOfBlock = false;
    [SerializeField] private float darknessIntensity = 1f;

    [Header("Visual Effects")]
    [SerializeField] private Color darkColor = new Color(0.1f, 0f, 0.2f, 0.8f);
    [SerializeField] private ParticleSystem darkMistEffect;
    [SerializeField] private float mistDensity = 10f;

    [Header("Blocking Settings")]
    [SerializeField] private float pushbackForce = 5f;
    [SerializeField] private LayerMask pikminLayer;

    protected override void Start()
    {
        base.Start();
        hazardType = "dark";
        canBeDestroyed = false; // Dark paths typically cannot be destroyed
        vulnerableToPikmin = new PikminColor[] { }; // No Pikmin can destroy this

        // Update visual appearance
        ApplyDarkVisuals();
    }

    protected override void Update()
    {
        base.Update();
        UpdateDarkMist();
    }

    /// <summary>
    /// Apply dark visual effects to all renderers
    /// </summary>
    void ApplyDarkVisuals()
    {
        if (obstacleRenderers != null)
        {
            foreach (var renderer in obstacleRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = darkColor;

                    // Enable emission for dark glow
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", darkColor * darknessIntensity);
                }
            }
        }
    }

    /// <summary>
    /// Update dark mist particle effect
    /// </summary>
    void UpdateDarkMist()
    {
        if (darkMistEffect != null)
        {
            var emission = darkMistEffect.emission;
            emission.rateOverTimeMultiplier = mistDensity;

            var main = darkMistEffect.main;
            main.startColor = darkColor;
        }
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (isDestroyed) return;

        Pikmin pikmin = other.GetComponent<Pikmin>();
        if (pikmin != null)
        {
            PikminType pikminType = other.GetComponent<PikminType>();

            // Check if this is a Dark Pikmin
            if (pikminType != null && pikminType.CanSurviveHazard("dark"))
            {
                // Dark Pikmin can pass safely
                return;
            }

            // Non-dark Pikmin are affected
            if (damageInsteadOfBlock)
            {
                // Damage non-dark Pikmin
                DamagePikmin(other.gameObject);
            }
            else if (blockNonDarkPikmin)
            {
                // Push back non-dark Pikmin
                PushBackPikmin(other);
            }
        }
    }

    /// <summary>
    /// Push back non-dark Pikmin from the dark path
    /// </summary>
    void PushBackPikmin(Collider pikminCollider)
    {
        Rigidbody rb = pikminCollider.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calculate pushback direction (away from center of dark path)
            Vector3 pushDirection = (pikminCollider.transform.position - transform.position).normalized;
            pushDirection.y = 0; // Keep on horizontal plane

            // Apply pushback force
            rb.AddForce(pushDirection * pushbackForce, ForceMode.Impulse);

            Debug.Log($"[DarkPath] Pushing back {pikminCollider.name} - only Dark Pikmin can pass!");
        }
    }

    protected override void UpdateRenderers(float healthRatio)
    {
        // Override to maintain dark appearance regardless of health
        ApplyDarkVisuals();
    }

    void OnTriggerEnter(Collider other)
    {
        Pikmin pikmin = other.GetComponent<Pikmin>();
        if (pikmin != null)
        {
            PikminType pikminType = other.GetComponent<PikminType>();

            if (pikminType != null && pikminType.CanSurviveHazard("dark"))
            {
                Debug.Log($"[DarkPath] {other.name} (Dark Pikmin) entering dark path");
            }
            else
            {
                Debug.Log($"[DarkPath] {other.name} cannot pass - only Dark Pikmin allowed!");
            }
        }
    }

    // Override damage to prevent destruction
    public override void TakeDamage(float damage, PikminColor attackerType)
    {
        if (!canBeDestroyed)
        {
            Debug.Log($"[DarkPath] Cannot be destroyed!");
            return;
        }

        base.TakeDamage(damage, attackerType);
    }
}
