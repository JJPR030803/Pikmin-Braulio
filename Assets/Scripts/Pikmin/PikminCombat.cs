using UnityEngine;

/// <summary>
/// Handles Pikmin combat - attacking enemies, latching on, dealing damage
/// </summary>
public class PikminCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private float attackInterval = 1f; // Attacks per second
    [SerializeField] private float latchChance = 0.8f; // Chance to successfully latch

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private bool autoAttackNearbyEnemies = true;

    [Header("Latching")]
    [SerializeField] private bool canLatchOntoEnemies = true;
    [SerializeField] private float latchHeight = 1f; // Height on enemy to latch
    [SerializeField] private float latchOffset = 0.5f; // Offset from enemy center
    [SerializeField] private float latchDuration = 5f; // Max time before falling off
    [SerializeField] private float shakeOffResistance = 0.7f; // Resistance to being shaken off

    [Header("Attack Animation")]
    [SerializeField] private float attackWindupTime = 0.2f;
    [SerializeField] private float attackRecoveryTime = 0.1f;
    [SerializeField] private ParticleSystem attackEffect;

    [Header("Type Bonuses")]
    [SerializeField] private bool useTypeBonuses = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private bool showGizmos = true;

    // Combat state
    private Pikmin pikmin;
    private PikminType pikminType;
    private GameObject currentTarget;
    private Health targetHealth;
    private bool isAttacking = false;
    private bool isLatched = false;
    private float lastAttackTime = 0f;
    private float latchTime = 0f;
    private Vector3 latchPosition;

    // State tracking
    private enum CombatState
    {
        Idle,
        Approaching,
        Attacking,
        Latched,
        Recovering
    }

    private CombatState currentState = CombatState.Idle;

    void Start()
    {
        pikmin = GetComponent<Pikmin>();
        pikminType = GetComponent<PikminType>();

        if (pikmin == null)
        {
            Debug.LogError($"[PikminCombat] {gameObject.name} needs a Pikmin component!");
            enabled = false;
            return;
        }

        if (showDebugInfo)
            Debug.Log($"[PikminCombat] Initialized on {gameObject.name}");
    }

    void Update()
    {
        switch (currentState)
        {
            case CombatState.Idle:
                if (autoAttackNearbyEnemies)
                {
                    SearchForEnemies();
                }
                break;

            case CombatState.Approaching:
                ApproachTarget();
                break;

            case CombatState.Attacking:
                PerformAttack();
                break;

            case CombatState.Latched:
                UpdateLatchedState();
                break;

            case CombatState.Recovering:
                // Wait for recovery
                break;
        }
    }

    /// <summary>
    /// Search for nearby enemies to attack
    /// </summary>
    void SearchForEnemies()
    {
        // Only search if following player and not doing other tasks
        if (!pikmin.IsFollowing()) return;

        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            Health health = enemy.GetComponent<Health>();
            if (health != null && !health.IsDead())
            {
                SetTarget(enemy.gameObject);
                currentState = CombatState.Approaching;
                return;
            }
        }
    }

    /// <summary>
    /// Set combat target
    /// </summary>
    public void SetTarget(GameObject target)
    {
        currentTarget = target;
        targetHealth = target.GetComponent<Health>();

        if (showDebugInfo)
            Debug.Log($"[PikminCombat] {gameObject.name} targeting {target.name}");
    }

    /// <summary>
    /// Approach the target enemy
    /// </summary>
    void ApproachTarget()
    {
        if (currentTarget == null)
        {
            currentState = CombatState.Idle;
            return;
        }

        // Check if target is dead
        if (targetHealth != null && targetHealth.IsDead())
        {
            ClearTarget();
            return;
        }

        // Move towards target
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        // Check if close enough to attack
        if (distance <= 1.5f)
        {
            // Try to latch if enabled
            if (canLatchOntoEnemies && Random.value <= latchChance)
            {
                LatchOntoEnemy();
            }
            else
            {
                currentState = CombatState.Attacking;
            }
        }
    }

    /// <summary>
    /// Latch onto enemy
    /// </summary>
    void LatchOntoEnemy()
    {
        if (currentTarget == null) return;

        isLatched = true;
        currentState = CombatState.Latched;
        latchTime = Time.time;

        // Calculate latch position on enemy
        Bounds enemyBounds = GetEnemyBounds();
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float x = Mathf.Cos(angle) * latchOffset;
        float z = Mathf.Sin(angle) * latchOffset;

        latchPosition = new Vector3(x, latchHeight, z);

        if (showDebugInfo)
            Debug.Log($"[PikminCombat] {gameObject.name} latched onto {currentTarget.name}");
    }

    /// <summary>
    /// Get enemy bounds
    /// </summary>
    Bounds GetEnemyBounds()
    {
        Collider col = currentTarget.GetComponent<Collider>();
        if (col != null)
        {
            return col.bounds;
        }

        return new Bounds(currentTarget.transform.position, Vector3.one);
    }

    /// <summary>
    /// Update latched state
    /// </summary>
    void UpdateLatchedState()
    {
        if (currentTarget == null)
        {
            UnlatchFromEnemy();
            return;
        }

        // Check if target is dead
        if (targetHealth != null && targetHealth.IsDead())
        {
            UnlatchFromEnemy();
            return;
        }

        // Check if latch duration expired
        if (Time.time - latchTime >= latchDuration)
        {
            UnlatchFromEnemy();
            return;
        }

        // Update position to follow enemy
        transform.position = currentTarget.transform.position + latchPosition;

        // Face the enemy
        Vector3 faceDirection = (currentTarget.transform.position - transform.position).normalized;
        if (faceDirection.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(faceDirection);
        }

        // Attack while latched
        PerformAttack();

        // Check if enemy shakes us off
        // This would be called by enemy when it shakes
    }

    /// <summary>
    /// Unlatch from enemy
    /// </summary>
    public void UnlatchFromEnemy()
    {
        isLatched = false;
        currentState = CombatState.Idle;
        ClearTarget();

        if (showDebugInfo)
            Debug.Log($"[PikminCombat] {gameObject.name} unlatched from enemy");
    }

    /// <summary>
    /// Perform attack on current target
    /// </summary>
    void PerformAttack()
    {
        if (currentTarget == null || targetHealth == null) return;

        // Check attack cooldown
        if (Time.time - lastAttackTime < attackInterval) return;

        lastAttackTime = Time.time;

        // Calculate damage with type bonuses
        float damage = CalculateDamage();

        // Apply damage
        targetHealth.TakeDamage(damage);

        // Play attack effect
        if (attackEffect != null)
        {
            attackEffect.Play();
        }

        if (showDebugInfo)
            Debug.Log($"[PikminCombat] {gameObject.name} dealt {damage} damage to {currentTarget.name}");

        // Check if enemy died
        if (targetHealth.IsDead())
        {
            OnEnemyKilled();
        }
    }

    /// <summary>
    /// Calculate attack damage with type bonuses
    /// </summary>
    float CalculateDamage()
    {
        float damage = attackDamage;

        if (useTypeBonuses && pikminType != null)
        {
            // Apply strength multiplier
            damage *= pikminType.GetStrengthMultiplier();

            // Apply type-specific bonuses (e.g., Red Pikmin do more damage)
            if (pikminType is RedPikmin redPikmin)
            {
                damage *= redPikmin.GetAttackDamageBonus();
            }
        }

        return damage;
    }

    /// <summary>
    /// Called when enemy is killed
    /// </summary>
    void OnEnemyKilled()
    {
        if (showDebugInfo)
            Debug.Log($"[PikminCombat] {gameObject.name} killed {currentTarget.name}!");

        // Unlatch if latched
        if (isLatched)
        {
            UnlatchFromEnemy();
        }

        ClearTarget();
        currentState = CombatState.Idle;
    }

    /// <summary>
    /// Clear current target
    /// </summary>
    void ClearTarget()
    {
        currentTarget = null;
        targetHealth = null;
        isAttacking = false;
    }

    /// <summary>
    /// Called by enemy when it shakes off Pikmin
    /// </summary>
    public void OnShakenOff(float shakeForce)
    {
        if (!isLatched) return;

        // Check resistance
        if (Random.value > shakeOffResistance)
        {
            // Shaken off!
            UnlatchFromEnemy();

            // Add some force to throw the Pikmin
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 throwDirection = (transform.position - currentTarget.transform.position).normalized;
                throwDirection += Vector3.up;
                rb.AddForce(throwDirection * shakeForce, ForceMode.Impulse);
            }

            if (showDebugInfo)
                Debug.Log($"[PikminCombat] {gameObject.name} was shaken off!");
        }
    }

    /// <summary>
    /// Public methods for external control
    /// </summary>
    public void AttackTarget(GameObject target)
    {
        SetTarget(target);
        currentState = CombatState.Approaching;
    }

    public void StopAttacking()
    {
        if (isLatched)
        {
            UnlatchFromEnemy();
        }

        ClearTarget();
        currentState = CombatState.Idle;
    }

    public bool IsAttacking() => currentState != CombatState.Idle;
    public bool IsLatched() => isLatched;
    public GameObject GetCurrentTarget() => currentTarget;

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // Draw detection radius
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw line to target
        if (currentTarget != null)
        {
            Gizmos.color = isLatched ? Color.red : Color.yellow;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);

            if (isLatched)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(currentTarget.transform.position + latchPosition, 0.3f);
            }
        }
    }

    void OnDestroy()
    {
        if (isLatched)
        {
            UnlatchFromEnemy();
        }
    }
}
