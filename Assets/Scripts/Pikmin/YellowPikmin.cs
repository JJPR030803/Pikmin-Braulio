using UnityEngine;

/// <summary>
/// Yellow Pikmin - Can jump very high, resist electricity, and dig fast
/// Can destroy electric obstacles and reach high places
/// </summary>
public class YellowPikmin : PikminType
{
    [Header("Yellow Pikmin Specifics")]
    [SerializeField] private float extraJumpForce = 15f; // Additional jump power
    [SerializeField] private float digSpeedMultiplier = 2f; // Excavates faster
    [SerializeField] private ParticleSystem electricResistEffect; // Sparks effect
    [SerializeField] private ParticleSystem jumpEffect; // Jump trail
    [SerializeField] private float electricDischargeRadius = 3f;

    [Header("Jump Settings")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space; // Manual jump control
    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private bool autoJumpOverObstacles = true;
    [SerializeField] private float obstacleDetectionRange = 2f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Electric Interaction")]
    [SerializeField] private LayerMask electricLayer;
    [SerializeField] private bool canDestroyElectricWalls = true;
    [SerializeField] private float electricDamageRate = 10f;

    private bool isInElectricZone = false;
    private float lastJumpTime = 0f;
    private Rigidbody rb;
    private bool isGrounded = true;

    protected override void Awake()
    {
        base.Awake();

        // Set Yellow Pikmin properties
        pikminColor = PikminColor.Yellow;
        visualColor = Color.yellow;
        resistsElectricity = true;
        strengthMultiplier = 1.0f;
        speedMultiplier = 1.0f;
        jumpHeightMultiplier = 3.0f; // Can jump very high!

        rb = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("[YellowPikmin] Yellow Pikmin initialized - High Jumper and Electric Resistant!");
    }

    void Update()
    {
        CheckGrounded();

        // Handle jump input (if controlled by player)
        if (Input.GetKeyDown(jumpKey) && CanJump())
        {
            PerformHighJump();
        }

        // Auto jump over obstacles
        if (autoJumpOverObstacles && isGrounded)
        {
            CheckForObstacles();
        }

        // Handle electric zones
        if (isInElectricZone && canDestroyElectricWalls)
        {
            DamageElectricObstacles();
        }
    }

    /// <summary>
    /// Check if grounded
    /// </summary>
    void CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.3f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    /// <summary>
    /// Check if can jump
    /// </summary>
    bool CanJump()
    {
        return isGrounded && Time.time - lastJumpTime >= jumpCooldown;
    }

    /// <summary>
    /// Perform a high jump
    /// </summary>
    public void PerformHighJump()
    {
        if (rb == null || !CanJump()) return;

        // Apply jump force
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * extraJumpForce, ForceMode.Impulse);

        lastJumpTime = Time.time;

        // Play jump effect
        if (jumpEffect != null)
        {
            jumpEffect.Play();
        }

        Debug.Log("[YellowPikmin] High jump!");
    }

    /// <summary>
    /// Check for obstacles ahead and auto jump
    /// </summary>
    void CheckForObstacles()
    {
        RaycastHit hit;
        Vector3 forward = transform.forward;

        // Raycast forward to detect obstacles
        if (Physics.Raycast(transform.position, forward, out hit, obstacleDetectionRange, obstacleLayer))
        {
            // Check if obstacle is jumpable (not too tall)
            float obstacleHeight = hit.collider.bounds.max.y - transform.position.y;

            if (obstacleHeight > 0.5f && obstacleHeight < 5f) // Reasonable jump range
            {
                PerformHighJump();
            }
        }
    }

    /// <summary>
    /// Damage electric obstacles nearby
    /// </summary>
    void DamageElectricObstacles()
    {
        Collider[] electricObjects = Physics.OverlapSphere(transform.position, electricDischargeRadius, electricLayer);

        foreach (Collider electric in electricObjects)
        {
            // Try to get electric wall component
            var electricWall = electric.GetComponent<ElectricWall>();
            if (electricWall != null)
            {
                electricWall.TakeDamage(electricDamageRate * Time.deltaTime, PikminColor.Yellow);
            }
        }
    }

    /// <summary>
    /// Override to handle electric hazard
    /// </summary>
    public override bool CanSurviveHazard(string hazardType)
    {
        if (hazardType.ToLower() == "electricity" || hazardType.ToLower() == "electric")
        {
            // Play electric resist effect
            if (electricResistEffect != null && !electricResistEffect.isPlaying)
            {
                electricResistEffect.Play();
            }
            return true;
        }

        return base.CanSurviveHazard(hazardType);
    }

    /// <summary>
    /// Special ability: Super high jump
    /// </summary>
    public override void ActivateSpecialAbility()
    {
        Debug.Log("[YellowPikmin] Activating super jump!");

        if (rb != null && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * extraJumpForce * 2f, ForceMode.Impulse); // Double jump!

            if (jumpEffect != null)
            {
                jumpEffect.Play();
            }
        }
    }

    /// <summary>
    /// Yellow Pikmin can perform digging tasks faster
    /// </summary>
    public override bool CanPerformTask(string taskType)
    {
        if (taskType.ToLower() == "dig" || taskType.ToLower() == "excavate")
        {
            return true;
        }

        return base.CanPerformTask(taskType);
    }

    void OnTriggerEnter(Collider other)
    {
        // Detect electric zones
        if (other.CompareTag("Electric") || other.GetComponent<ElectricWall>() != null)
        {
            isInElectricZone = true;

            if (electricResistEffect != null && !electricResistEffect.isPlaying)
            {
                electricResistEffect.Play();
            }

            Debug.Log("[YellowPikmin] Entered electric zone - resistant!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Exit electric zones
        if (other.CompareTag("Electric") || other.GetComponent<ElectricWall>() != null)
        {
            isInElectricZone = false;

            if (electricResistEffect != null && electricResistEffect.isPlaying)
            {
                electricResistEffect.Stop();
            }

            Debug.Log("[YellowPikmin] Exited electric zone");
        }
    }

    public float GetDigSpeedMultiplier() => digSpeedMultiplier;
    public bool IsInElectricZone() => isInElectricZone;

    void OnDrawGizmosSelected()
    {
        // Draw obstacle detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * obstacleDetectionRange);

        // Draw electric discharge radius
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, electricDischargeRadius);

        // Draw jump arc preview
        if (isGrounded)
        {
            Gizmos.color = Color.green;
            Vector3 jumpPeak = transform.position + Vector3.up * (extraJumpForce / 2f);
            Gizmos.DrawLine(transform.position, jumpPeak);
        }
    }
}
