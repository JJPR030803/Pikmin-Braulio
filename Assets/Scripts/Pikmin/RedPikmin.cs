using UnityEngine;

/// <summary>
/// Red Pikmin - Fire resistant, good in combat
/// Can stay in fire areas and extinguish flames
/// </summary>
public class RedPikmin : PikminType
{
    [Header("Red Pikmin Specifics")]
    [SerializeField] private float attackDamageBonus = 1.5f; // Red Pikmin are stronger in combat
    [SerializeField] private ParticleSystem fireResistEffect; // Visual effect when in fire
    [SerializeField] private float fireExtinguishRadius = 2f; // Radius to extinguish fires
    [SerializeField] private bool canExtinguishFires = true;

    [Header("Fire Interaction")]
    [SerializeField] private LayerMask fireLayer;
    [SerializeField] private float extinguishRate = 1f; // How fast fires are put out
    private bool isInFire = false;

    protected override void Awake()
    {
        base.Awake();

        // Set Red Pikmin properties
        pikminColor = PikminColor.Red;
        visualColor = Color.red;
        resistsFire = true;
        strengthMultiplier = 1.5f; // Red Pikmin are stronger
        speedMultiplier = 1.0f;
        jumpHeightMultiplier = 1.0f;
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("[RedPikmin] Red Pikmin initialized - Fire Resistant!");
    }

    void Update()
    {
        // Check for nearby fires to extinguish
        if (canExtinguishFires && isInFire)
        {
            CheckForFiresToExtinguish();
        }
    }

    /// <summary>
    /// Check for nearby fires and attempt to extinguish them
    /// </summary>
    void CheckForFiresToExtinguish()
    {
        Collider[] fires = Physics.OverlapSphere(transform.position, fireExtinguishRadius, fireLayer);

        foreach (Collider fire in fires)
        {
            // Try to get a fire component
            var fireHazard = fire.GetComponent<FireHazard>();
            if (fireHazard != null)
            {
                fireHazard.TakeDamage(extinguishRate * Time.deltaTime, PikminColor.Red);
            }
        }
    }

    /// <summary>
    /// Override to handle fire damage
    /// </summary>
    public override bool CanSurviveHazard(string hazardType)
    {
        if (hazardType.ToLower() == "fire")
        {
            // Play fire resist effect
            if (fireResistEffect != null && !fireResistEffect.isPlaying)
            {
                fireResistEffect.Play();
            }
            return true;
        }

        return base.CanSurviveHazard(hazardType);
    }

    /// <summary>
    /// Special ability: Extinguish nearby fires
    /// </summary>
    public override void ActivateSpecialAbility()
    {
        Debug.Log("[RedPikmin] Activating fire extinguish ability!");

        Collider[] fires = Physics.OverlapSphere(transform.position, fireExtinguishRadius, fireLayer);

        foreach (Collider fire in fires)
        {
            var fireHazard = fire.GetComponent<FireHazard>();
            if (fireHazard != null)
            {
                fireHazard.TakeDamage(100f, PikminColor.Red); // Instant extinguish
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Detect fire zones
        if (other.CompareTag("Fire") || other.GetComponent<FireHazard>() != null)
        {
            isInFire = true;

            if (fireResistEffect != null && !fireResistEffect.isPlaying)
            {
                fireResistEffect.Play();
            }

            Debug.Log("[RedPikmin] Entered fire zone - resistant!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Exit fire zones
        if (other.CompareTag("Fire") || other.GetComponent<FireHazard>() != null)
        {
            isInFire = false;

            if (fireResistEffect != null && fireResistEffect.isPlaying)
            {
                fireResistEffect.Stop();
            }

            Debug.Log("[RedPikmin] Exited fire zone");
        }
    }

    public float GetAttackDamageBonus() => attackDamageBonus;

    void OnDrawGizmosSelected()
    {
        // Draw fire extinguish radius
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, fireExtinguishRadius);
    }
}
