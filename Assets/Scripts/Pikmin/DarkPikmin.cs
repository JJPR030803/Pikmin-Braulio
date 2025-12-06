using UnityEngine;

/// <summary>
/// Dark Pikmin - Can traverse dark paths and areas
/// Resistant to darkness hazards
/// </summary>
public class DarkPikmin : PikminType
{
    [Header("Dark Pikmin Specifics")]
    [SerializeField] private ParticleSystem darkAuraEffect;
    [SerializeField] private Light darkGlow;
    [SerializeField] private Color darkColor = new Color(0.2f, 0f, 0.3f);
    [SerializeField] private float darkVisionRadius = 5f;

    [Header("Dark Path Abilities")]
    [SerializeField] private bool canIlluminateDarkness = true;
    [SerializeField] private float illuminationIntensity = 1f;
    [SerializeField] private LayerMask darkPathLayer;

    private bool isInDarkZone = false;

    protected override void Awake()
    {
        base.Awake();

        // Set Dark Pikmin properties
        pikminColor = PikminColor.Dark;
        visualColor = darkColor;
        resistsDark = true;
        strengthMultiplier = 1.2f;
        speedMultiplier = 1.1f;
        jumpHeightMultiplier = 1.0f;
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("[DarkPikmin] Dark Pikmin initialized - Can traverse dark paths!");

        // Set up dark glow
        if (darkGlow != null)
        {
            darkGlow.color = darkColor;
            darkGlow.intensity = illuminationIntensity;
        }
    }

    void Update()
    {
        // Update dark aura effect
        if (isInDarkZone && darkAuraEffect != null && !darkAuraEffect.isPlaying)
        {
            darkAuraEffect.Play();
        }
        else if (!isInDarkZone && darkAuraEffect != null && darkAuraEffect.isPlaying)
        {
            darkAuraEffect.Stop();
        }
    }

    /// <summary>
    /// Override to handle dark hazard
    /// </summary>
    public override bool CanSurviveHazard(string hazardType)
    {
        if (hazardType.ToLower() == "dark" || hazardType.ToLower() == "darkness")
        {
            // Play dark aura effect
            if (darkAuraEffect != null && !darkAuraEffect.isPlaying)
            {
                darkAuraEffect.Play();
            }

            // Enable dark glow
            if (darkGlow != null)
            {
                darkGlow.enabled = true;
            }

            return true;
        }

        return base.CanSurviveHazard(hazardType);
    }

    /// <summary>
    /// Special ability: Illuminate dark areas
    /// </summary>
    public override void ActivateSpecialAbility()
    {
        Debug.Log("[DarkPikmin] Activating illumination ability!");

        if (darkGlow != null)
        {
            // Temporarily boost illumination
            darkGlow.intensity = illuminationIntensity * 3f;
            darkGlow.range = darkVisionRadius * 2f;

            // Restore after delay
            Invoke(nameof(RestoreIllumination), 5f);
        }

        if (darkAuraEffect != null)
        {
            darkAuraEffect.Play();
        }
    }

    void RestoreIllumination()
    {
        if (darkGlow != null)
        {
            darkGlow.intensity = illuminationIntensity;
            darkGlow.range = darkVisionRadius;
        }
    }

    /// <summary>
    /// Dark Pikmin can perform dark-related tasks
    /// </summary>
    public override bool CanPerformTask(string taskType)
    {
        if (taskType.ToLower() == "traverse_dark" || taskType.ToLower() == "illuminate")
        {
            return true;
        }

        return base.CanPerformTask(taskType);
    }

    void OnTriggerEnter(Collider other)
    {
        // Detect dark zones
        if (other.CompareTag("Dark") || other.GetComponent<DarkPath>() != null)
        {
            isInDarkZone = true;

            if (darkAuraEffect != null && !darkAuraEffect.isPlaying)
            {
                darkAuraEffect.Play();
            }

            if (darkGlow != null)
            {
                darkGlow.enabled = true;
            }

            Debug.Log("[DarkPikmin] Entered dark zone - can traverse safely!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Exit dark zones
        if (other.CompareTag("Dark") || other.GetComponent<DarkPath>() != null)
        {
            isInDarkZone = false;

            if (darkAuraEffect != null && darkAuraEffect.isPlaying)
            {
                darkAuraEffect.Stop();
            }

            Debug.Log("[DarkPikmin] Exited dark zone");
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Continuously check if in dark zone
        if (other.CompareTag("Dark") || other.GetComponent<DarkPath>() != null)
        {
            isInDarkZone = true;
        }
    }

    public bool IsInDarkZone() => isInDarkZone;

    void OnDrawGizmosSelected()
    {
        // Draw dark vision radius
        Gizmos.color = new Color(0.2f, 0f, 0.3f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, darkVisionRadius);

        // Draw dark indicator
        if (isInDarkZone)
        {
            Gizmos.color = darkColor;
            Gizmos.DrawWireCube(transform.position + Vector3.up, Vector3.one * 0.5f);
        }
    }
}
