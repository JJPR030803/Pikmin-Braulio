using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// White Pikmin - Can find treasures underground, resist poison, and are fastest
/// Can detect buried items and are toxic when eaten
/// </summary>
public class WhitePikmin : PikminType
{
    [Header("White Pikmin Specifics")]
    [SerializeField] private float treasureDetectionRadius = 5f; // Range to detect buried treasures
    [SerializeField] private float digSpeed = 2f; // Speed of digging up treasures
    [SerializeField] private ParticleSystem poisonResistEffect; // Purple/green particles
    [SerializeField] private ParticleSystem treasureDetectionEffect; // Visual ping for treasures
    [SerializeField] private float poisonDamageToEnemies = 50f; // Damage when eaten by enemies

    [Header("Treasure Detection")]
    [SerializeField] private LayerMask treasureLayer;
    [SerializeField] private bool showTreasureIndicators = true;
    [SerializeField] private GameObject treasureIndicatorPrefab; // Visual marker for found treasures
    [SerializeField] private float detectionInterval = 1f; // How often to scan for treasures

    [Header("Poison Interaction")]
    [SerializeField] private LayerMask poisonLayer;
    [SerializeField] private bool canNeutralizePoisonGas = true;
    [SerializeField] private float poisonNeutralizationRadius = 2f;

    private bool isInPoisonZone = false;
    private float lastDetectionTime = 0f;
    private List<GameObject> detectedTreasures = new List<GameObject>();
    private Dictionary<GameObject, GameObject> treasureIndicators = new Dictionary<GameObject, GameObject>();

    protected override void Awake()
    {
        base.Awake();

        // Set White Pikmin properties
        pikminColor = PikminColor.White;
        visualColor = Color.white;
        resistsPoison = true;
        canDigUnderground = true;
        strengthMultiplier = 1.0f;
        speedMultiplier = 1.5f; // Fastest Pikmin!
        jumpHeightMultiplier = 1.0f;
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("[WhitePikmin] White Pikmin initialized - Treasure Finder and Poison Resistant!");
    }

    void Update()
    {
        // Periodically scan for buried treasures
        if (Time.time - lastDetectionTime >= detectionInterval)
        {
            DetectBuriedTreasures();
            lastDetectionTime = Time.time;
        }

        // Neutralize poison gas if in poison zone
        if (isInPoisonZone && canNeutralizePoisonGas)
        {
            NeutralizePoisonGas();
        }

        // Clean up indicators for treasures that have been collected
        CleanupTreasureIndicators();
    }

    /// <summary>
    /// Detect buried treasures underground
    /// </summary>
    void DetectBuriedTreasures()
    {
        Collider[] treasures = Physics.OverlapSphere(transform.position, treasureDetectionRadius, treasureLayer);

        foreach (Collider treasure in treasures)
        {
            // Check if it's a buried treasure (has a component marking it as such)
            var buriedTreasure = treasure.GetComponent<BuriedTreasure>();
            if (buriedTreasure != null && !buriedTreasure.IsRevealed())
            {
                if (!detectedTreasures.Contains(treasure.gameObject))
                {
                    // Found a new treasure!
                    detectedTreasures.Add(treasure.gameObject);
                    RevealTreasure(buriedTreasure);

                    Debug.Log($"[WhitePikmin] Detected buried treasure: {treasure.gameObject.name}");
                }
            }
        }
    }

    /// <summary>
    /// Reveal a buried treasure
    /// </summary>
    void RevealTreasure(BuriedTreasure treasure)
    {
        treasure.Reveal();

        // Play detection effect
        if (treasureDetectionEffect != null)
        {
            ParticleSystem effect = Instantiate(treasureDetectionEffect, treasure.transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 2f);
        }

        // Create visual indicator
        if (showTreasureIndicators && treasureIndicatorPrefab != null)
        {
            GameObject indicator = Instantiate(treasureIndicatorPrefab, treasure.transform.position + Vector3.up * 2f, Quaternion.identity);
            treasureIndicators[treasure.gameObject] = indicator;
        }
    }

    /// <summary>
    /// Clean up indicators for collected treasures
    /// </summary>
    void CleanupTreasureIndicators()
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var kvp in treasureIndicators)
        {
            if (kvp.Key == null) // Treasure was collected/destroyed
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value);
                }
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var key in toRemove)
        {
            treasureIndicators.Remove(key);
            detectedTreasures.Remove(key);
        }
    }

    /// <summary>
    /// Neutralize poison gas in the area
    /// </summary>
    void NeutralizePoisonGas()
    {
        Collider[] poisonAreas = Physics.OverlapSphere(transform.position, poisonNeutralizationRadius, poisonLayer);

        foreach (Collider poison in poisonAreas)
        {
            var poisonHazard = poison.GetComponent<PoisonHazard>();
            if (poisonHazard != null)
            {
                poisonHazard.TakeDamage(Time.deltaTime, PikminColor.White);
            }
        }
    }

    /// <summary>
    /// Override to handle poison hazard
    /// </summary>
    public override bool CanSurviveHazard(string hazardType)
    {
        if (hazardType.ToLower() == "poison")
        {
            // Play poison resist effect
            if (poisonResistEffect != null && !poisonResistEffect.isPlaying)
            {
                poisonResistEffect.Play();
            }
            return true;
        }

        return base.CanSurviveHazard(hazardType);
    }

    /// <summary>
    /// White Pikmin can perform digging tasks
    /// </summary>
    public override bool CanPerformTask(string taskType)
    {
        if (taskType.ToLower() == "dig" || taskType.ToLower() == "find_treasure")
        {
            return true;
        }

        return base.CanPerformTask(taskType);
    }

    /// <summary>
    /// Special ability: Scan for all nearby treasures
    /// </summary>
    public override void ActivateSpecialAbility()
    {
        Debug.Log("[WhitePikmin] Activating treasure scan!");

        // Temporarily increase detection radius for a powerful scan
        float originalRadius = treasureDetectionRadius;
        treasureDetectionRadius *= 2f;

        DetectBuriedTreasures();

        // Play effect
        if (treasureDetectionEffect != null)
        {
            treasureDetectionEffect.Play();
        }

        // Restore original radius after a delay
        Invoke(nameof(RestoreDetectionRadius), 0.5f);
    }

    void RestoreDetectionRadius()
    {
        treasureDetectionRadius /= 2f;
    }

    /// <summary>
    /// Called when this Pikmin is eaten by an enemy (poison damage)
    /// </summary>
    public void OnEatenByEnemy(GameObject enemy)
    {
        // White Pikmin are toxic!
        var enemyHealth = enemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(poisonDamageToEnemies);
            Debug.Log($"[WhitePikmin] Enemy {enemy.name} poisoned for {poisonDamageToEnemies} damage!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Detect poison zones
        if (other.CompareTag("Poison") || other.GetComponent<PoisonHazard>() != null)
        {
            isInPoisonZone = true;

            if (poisonResistEffect != null && !poisonResistEffect.isPlaying)
            {
                poisonResistEffect.Play();
            }

            Debug.Log("[WhitePikmin] Entered poison zone - resistant!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Exit poison zones
        if (other.CompareTag("Poison") || other.GetComponent<PoisonHazard>() != null)
        {
            isInPoisonZone = false;

            if (poisonResistEffect != null && poisonResistEffect.isPlaying)
            {
                poisonResistEffect.Stop();
            }

            Debug.Log("[WhitePikmin] Exited poison zone");
        }
    }

    void OnDestroy()
    {
        // Clean up all treasure indicators when this Pikmin is destroyed
        foreach (var indicator in treasureIndicators.Values)
        {
            if (indicator != null)
            {
                Destroy(indicator);
            }
        }
    }

    public float GetDigSpeed() => digSpeed;
    public List<GameObject> GetDetectedTreasures() => new List<GameObject>(detectedTreasures);
    public bool IsInPoisonZone() => isInPoisonZone;

    void OnDrawGizmosSelected()
    {
        // Draw treasure detection radius
        Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, treasureDetectionRadius);

        // Draw poison neutralization radius
        Gizmos.color = new Color(0.5f, 0f, 0.5f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, poisonNeutralizationRadius);

        // Draw lines to detected treasures
        Gizmos.color = Color.yellow;
        foreach (var treasure in detectedTreasures)
        {
            if (treasure != null)
            {
                Gizmos.DrawLine(transform.position, treasure.transform.position);
            }
        }
    }
}
