using UnityEngine;

/// <summary>
/// Blue Pikmin - Can swim and breathe underwater
/// Can rescue drowning Pikmin from water
/// </summary>
public class BluePikmin : PikminType
{
    [Header("Blue Pikmin Specifics")]
    [SerializeField] private float swimSpeed = 3f;
    [SerializeField] private float waterDrag = 2f; // Water resistance
    [SerializeField] private ParticleSystem swimEffect; // Bubbles or water splash
    [SerializeField] private float rescueRadius = 2f; // Radius to rescue drowning Pikmin
    [SerializeField] private bool canRescuePikmin = true;

    [Header("Swimming Physics")]
    [SerializeField] private float swimUpForce = 5f; // Buoyancy
    [SerializeField] private float swimDownForce = 3f;
    [SerializeField] private LayerMask waterLayer;

    private bool isInWater = false;
    private Rigidbody rb;
    private float originalDrag;

    protected override void Awake()
    {
        base.Awake();

        // Set Blue Pikmin properties
        pikminColor = PikminColor.Blue;
        visualColor = Color.blue;
        resistsWater = true;
        canSwim = true;
        strengthMultiplier = 1.0f;
        speedMultiplier = 1.0f;
        jumpHeightMultiplier = 1.0f;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            originalDrag = rb.linearDamping;
        }
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("[BluePikmin] Blue Pikmin initialized - Can Swim!");
    }

    void Update()
    {
        // Handle swimming controls if in water
        if (isInWater)
        {
            HandleSwimming();

            // Check for drowning Pikmin to rescue
            if (canRescuePikmin)
            {
                CheckForDrowningPikmin();
            }
        }
    }

    /// <summary>
    /// Handle swimming movement in water
    /// </summary>
    void HandleSwimming()
    {
        if (rb == null) return;

        // Apply buoyancy - Blue Pikmin float naturally
        rb.AddForce(Vector3.up * swimUpForce, ForceMode.Acceleration);

        // Adjust drag for swimming
        if (rb.linearDamping != waterDrag)
        {
            rb.linearDamping = waterDrag;
        }

        // Play swim effect
        if (swimEffect != null && !swimEffect.isPlaying)
        {
            swimEffect.Play();
        }
    }

    /// <summary>
    /// Check for nearby drowning Pikmin and rescue them
    /// </summary>
    void CheckForDrowningPikmin()
    {
        Collider[] nearbyPikmin = Physics.OverlapSphere(transform.position, rescueRadius);

        foreach (Collider col in nearbyPikmin)
        {
            Pikmin otherPikmin = col.GetComponent<Pikmin>();
            if (otherPikmin != null && otherPikmin != basePikmin)
            {
                // Check if the other Pikmin can't swim (is drowning)
                PikminType otherType = col.GetComponent<PikminType>();
                if (otherType == null || !otherType.CanSwim())
                {
                    // Try to rescue them (pull them to shore/surface)
                    RescuePikmin(otherPikmin);
                }
            }
        }
    }

    /// <summary>
    /// Rescue a drowning Pikmin
    /// </summary>
    void RescuePikmin(Pikmin drowningPikmin)
    {
        // Simple rescue: push them towards surface
        Rigidbody drowningRb = drowningPikmin.GetComponent<Rigidbody>();
        if (drowningRb != null)
        {
            drowningRb.AddForce(Vector3.up * swimUpForce * 2f, ForceMode.Acceleration);
            Debug.Log("[BluePikmin] Rescuing drowning Pikmin!");
        }
    }

    /// <summary>
    /// Override to handle water hazard
    /// </summary>
    public override bool CanSurviveHazard(string hazardType)
    {
        if (hazardType.ToLower() == "water")
        {
            return true;
        }

        return base.CanSurviveHazard(hazardType);
    }

    /// <summary>
    /// Special ability: Create water splash to push objects
    /// </summary>
    public override void ActivateSpecialAbility()
    {
        Debug.Log("[BluePikmin] Creating water splash!");

        // Create a force push in water
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, rescueRadius);

        foreach (Collider col in nearbyObjects)
        {
            Rigidbody objRb = col.GetComponent<Rigidbody>();
            if (objRb != null && objRb != rb)
            {
                Vector3 pushDirection = (col.transform.position - transform.position).normalized;
                objRb.AddForce(pushDirection * 10f, ForceMode.Impulse);
            }
        }

        if (swimEffect != null)
        {
            swimEffect.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Detect water zones
        if (other.CompareTag("Water") || ((1 << other.gameObject.layer) & waterLayer) != 0)
        {
            isInWater = true;

            if (swimEffect != null && !swimEffect.isPlaying)
            {
                swimEffect.Play();
            }

            Debug.Log("[BluePikmin] Entered water - can swim!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Exit water zones
        if (other.CompareTag("Water") || ((1 << other.gameObject.layer) & waterLayer) != 0)
        {
            isInWater = false;

            // Restore original drag
            if (rb != null)
            {
                rb.linearDamping = originalDrag;
            }

            if (swimEffect != null && swimEffect.isPlaying)
            {
                swimEffect.Stop();
            }

            Debug.Log("[BluePikmin] Exited water");
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Continuously check if in water
        if (other.CompareTag("Water") || ((1 << other.gameObject.layer) & waterLayer) != 0)
        {
            isInWater = true;
        }
    }

    public bool IsInWater() => isInWater;

    void OnDrawGizmosSelected()
    {
        // Draw rescue radius
        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, rescueRadius);

        // Draw water indicator
        if (isInWater)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + Vector3.up, Vector3.one * 0.5f);
        }
    }
}
