using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles enemy combat behaviors - attacking Pikmin/player, shaking off Pikmin, death
/// </summary>
[RequireComponent(typeof(Health))]
public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private LayerMask targetLayers; // Pikmin and Player
    [SerializeField] private bool canAttackPlayer = true;
    [SerializeField] private bool canEatPikmin = true;

    [Header("Eating Pikmin")]
    [SerializeField] private float eatDuration = 1f; // Time to eat a Pikmin
    [SerializeField] private int maxPikminToEat = 3; // Max Pikmin to eat at once
    [SerializeField] private ParticleSystem eatEffect;

    [Header("Shake Off Settings")]
    [SerializeField] private bool canShakeOffPikmin = true;
    [SerializeField] private float shakeInterval = 3f; // Time between shakes
    [SerializeField] private float shakeForce = 10f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private int maxLatchedPikmin = 5; // Max Pikmin before forced shake

    [Header("Death Settings")]
    [SerializeField] private bool convertToCorpseOnDeath = true;
    [SerializeField] private GameObject corpsePrefab; // Optional custom corpse
    [SerializeField] private float corpseWeight = 5f; // How many Pikmin needed to carry
    [SerializeField] private int corpseValue = 5; // Pikmin value when returned

    [Header("Visual Feedback")]
    [SerializeField] private ParticleSystem attackEffect;
    [SerializeField] private ParticleSystem shakeEffect;
    [SerializeField] private Animator animator;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private bool showGizmos = true;

    // State
    private Health health;
    private float lastAttackTime = 0f;
    private float lastShakeTime = 0f;
    private bool isShaking = false;
    private bool isDead = false;

    // Latched Pikmin tracking
    private List<PikminCombat> latchedPikmin = new List<PikminCombat>();

    // Attack state
    private GameObject currentAttackTarget;
    private bool isEating = false;
    private float eatStartTime = 0f;
    private GameObject pikminBeingEaten;

    void Start()
    {
        health = GetComponent<Health>();

        if (health == null)
        {
            Debug.LogError($"[EnemyCombat] {gameObject.name} needs a Health component!");
            enabled = false;
            return;
        }

        // Subscribe to death event
        health.OnDeath.AddListener(OnDeath);

        if (showDebugInfo)
            Debug.Log($"[EnemyCombat] Initialized on {gameObject.name}");
    }

    void Update()
    {
        if (isDead) return;

        // Check for latched Pikmin
        CheckForLatchedPikmin();

        // Shake off Pikmin if too many or on interval
        if (canShakeOffPikmin && !isShaking)
        {
            if (ShouldShakeOffPikmin())
            {
                ShakeOffPikmin();
            }
        }

        // Handle eating
        if (isEating)
        {
            UpdateEating();
        }
        else
        {
            // Look for targets to attack
            SearchForTargets();
        }
    }

    /// <summary>
    /// Check for Pikmin latched onto this enemy
    /// </summary>
    void CheckForLatchedPikmin()
    {
        // Find all Pikmin with combat component
        PikminCombat[] allPikmin = FindObjectsOfType<PikminCombat>();

        latchedPikmin.Clear();

        foreach (var pikminCombat in allPikmin)
        {
            if (pikminCombat.IsLatched() && pikminCombat.GetCurrentTarget() == gameObject)
            {
                latchedPikmin.Add(pikminCombat);
            }
        }
    }

    /// <summary>
    /// Check if should shake off Pikmin
    /// </summary>
    bool ShouldShakeOffPikmin()
    {
        // Shake if too many Pikmin latched
        if (latchedPikmin.Count >= maxLatchedPikmin)
        {
            return true;
        }

        // Shake on interval if any Pikmin latched
        if (latchedPikmin.Count > 0 && Time.time - lastShakeTime >= shakeInterval)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Shake off latched Pikmin
    /// </summary>
    void ShakeOffPikmin()
    {
        if (latchedPikmin.Count == 0) return;

        isShaking = true;
        lastShakeTime = Time.time;

        // Play shake animation
        if (animator != null)
        {
            animator.SetTrigger("Shake");
        }

        // Play shake effect
        if (shakeEffect != null)
        {
            shakeEffect.Play();
        }

        // Shake off each latched Pikmin
        foreach (var pikminCombat in latchedPikmin)
        {
            if (pikminCombat != null)
            {
                pikminCombat.OnShakenOff(shakeForce);
            }
        }

        if (showDebugInfo)
            Debug.Log($"[EnemyCombat] {gameObject.name} shook off {latchedPikmin.Count} Pikmin");

        latchedPikmin.Clear();

        // End shaking after duration
        Invoke(nameof(EndShaking), shakeDuration);
    }

    void EndShaking()
    {
        isShaking = false;
    }

    /// <summary>
    /// Search for targets to attack
    /// </summary>
    void SearchForTargets()
    {
        if (isEating || isShaking) return;
        if (Time.time - lastAttackTime < attackCooldown) return;

        // Find targets in range
        Collider[] targets = Physics.OverlapSphere(transform.position, attackRange, targetLayers);

        foreach (Collider target in targets)
        {
            // Check if it's a Pikmin
            if (canEatPikmin)
            {
                Pikmin pikmin = target.GetComponent<Pikmin>();
                if (pikmin != null)
                {
                    TryEatPikmin(pikmin.gameObject);
                    return;
                }
            }

            // Check if it's the player
            if (canAttackPlayer && target.CompareTag("Player"))
            {
                AttackTarget(target.gameObject);
                return;
            }
        }
    }

    /// <summary>
    /// Attack a target
    /// </summary>
    void AttackTarget(GameObject target)
    {
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth == null) return;

        lastAttackTime = Time.time;
        currentAttackTarget = target;

        // Deal damage
        targetHealth.TakeDamage(attackDamage);

        // Play attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Play attack effect
        if (attackEffect != null)
        {
            attackEffect.Play();
        }

        if (showDebugInfo)
            Debug.Log($"[EnemyCombat] {gameObject.name} attacked {target.name} for {attackDamage} damage");
    }

    /// <summary>
    /// Try to eat a Pikmin
    /// </summary>
    void TryEatPikmin(GameObject pikmin)
    {
        if (isEating) return;

        isEating = true;
        eatStartTime = Time.time;
        pikminBeingEaten = pikmin;
        lastAttackTime = Time.time;

        // Play eat animation
        if (animator != null)
        {
            animator.SetTrigger("Eat");
        }

        if (showDebugInfo)
            Debug.Log($"[EnemyCombat] {gameObject.name} started eating {pikmin.name}");
    }

    /// <summary>
    /// Update eating process
    /// </summary>
    void UpdateEating()
    {
        if (pikminBeingEaten == null)
        {
            isEating = false;
            return;
        }

        // Check if eat duration completed
        if (Time.time - eatStartTime >= eatDuration)
        {
            FinishEating();
        }
    }

    /// <summary>
    /// Finish eating Pikmin
    /// </summary>
    void FinishEating()
    {
        if (pikminBeingEaten != null)
        {
            // Check if it's a White Pikmin (poisonous)
            WhitePikmin whitePikmin = pikminBeingEaten.GetComponent<WhitePikmin>();
            if (whitePikmin != null)
            {
                whitePikmin.OnEatenByEnemy(gameObject);
            }

            // Play eat effect
            if (eatEffect != null)
            {
                Instantiate(eatEffect, pikminBeingEaten.transform.position, Quaternion.identity);
            }

            // Destroy the Pikmin
            Destroy(pikminBeingEaten);

            if (showDebugInfo)
                Debug.Log($"[EnemyCombat] {gameObject.name} finished eating Pikmin");
        }

        isEating = false;
        pikminBeingEaten = null;
    }

    /// <summary>
    /// Called when enemy dies
    /// </summary>
    void OnDeath()
    {
        isDead = true;

        if (showDebugInfo)
            Debug.Log($"[EnemyCombat] {gameObject.name} died");

        // Shake off all latched Pikmin
        if (latchedPikmin.Count > 0)
        {
            foreach (var pikminCombat in latchedPikmin)
            {
                if (pikminCombat != null)
                {
                    pikminCombat.UnlatchFromEnemy();
                }
            }
            latchedPikmin.Clear();
        }

        // Convert to corpse if enabled
        if (convertToCorpseOnDeath)
        {
            ConvertToCorpse();
        }
    }

    /// <summary>
    /// Convert enemy to corpse/pellet
    /// </summary>
    void ConvertToCorpse()
    {
        GameObject corpse;

        if (corpsePrefab != null)
        {
            // Use custom corpse prefab
            corpse = Instantiate(corpsePrefab, transform.position, transform.rotation);
        }
        else
        {
            // Create a corpse pellet automatically
            corpse = new GameObject($"{gameObject.name}_Corpse");
            corpse.transform.position = transform.position;
            corpse.transform.rotation = transform.rotation;

            // Copy visual (optional - could use original mesh)
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            if (meshFilter != null)
            {
                MeshFilter corpseMesh = corpse.AddComponent<MeshFilter>();
                corpseMesh.mesh = meshFilter.mesh;

                MeshRenderer corpseRenderer = corpse.AddComponent<MeshRenderer>();
                corpseRenderer.materials = meshRenderer.materials;

                // Darken the corpse
                foreach (var mat in corpseRenderer.materials)
                {
                    mat.color = Color.gray;
                }
            }

            // Add Rigidbody
            Rigidbody rb = corpse.AddComponent<Rigidbody>();
            rb.mass = corpseWeight;

            // Add Collider
            BoxCollider collider = corpse.AddComponent<BoxCollider>();
            Bounds bounds = GetComponent<Collider>().bounds;
            collider.size = bounds.size;
        }

        // Add Pellet component to corpse
        Pellet pellet = corpse.GetComponent<Pellet>();
        if (pellet == null)
        {
            pellet = corpse.AddComponent<Pellet>();
        }

        // Configure pellet
        pellet.SetWeight(corpseWeight);
        pellet.SetPikminValue(corpseValue);

        if (showDebugInfo)
            Debug.Log($"[EnemyCombat] Converted {gameObject.name} to corpse");
    }

    /// <summary>
    /// Public getters
    /// </summary>
    public int GetLatchedPikminCount() => latchedPikmin.Count;
    public bool IsShaking() => isShaking;
    public bool IsEating() => isEating;

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // Draw attack range
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw lines to latched Pikmin
        if (Application.isPlaying && latchedPikmin != null)
        {
            Gizmos.color = Color.red;
            foreach (var pikmin in latchedPikmin)
            {
                if (pikmin != null)
                {
                    Gizmos.DrawLine(transform.position, pikmin.transform.position);
                }
            }
        }

        // Draw target line
        if (Application.isPlaying && currentAttackTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentAttackTarget.transform.position);
        }
    }

    void OnDestroy()
    {
        // Clean up event listener
        if (health != null)
        {
            health.OnDeath.RemoveListener(OnDeath);
        }
    }
}
