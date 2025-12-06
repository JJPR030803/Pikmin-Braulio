using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Generic health component for any GameObject (Pikmin, enemies, etc.)
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 0f;
    [SerializeField] private GameObject deathEffect;

    [Header("Events")]
    public UnityEvent<float> OnHealthChanged; // Sends current health percentage (0-1)
    public UnityEvent<float> OnDamageTaken; // Sends damage amount
    public UnityEvent OnDeath;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(1f);
    }

    /// <summary>
    /// Take damage
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (showDebugInfo)
            Debug.Log($"[Health] {gameObject.name} took {damage} damage. Current: {currentHealth}/{maxHealth}");

        // Invoke events
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        OnDamageTaken?.Invoke(damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Heal health
    /// </summary>
    public void Heal(float amount)
    {
        if (isDead) return;

        float oldHealth = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (showDebugInfo)
            Debug.Log($"[Health] {gameObject.name} healed {currentHealth - oldHealth}. Current: {currentHealth}/{maxHealth}");

        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    /// <summary>
    /// Set health to a specific value
    /// </summary>
    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Kill instantly
    /// </summary>
    public void Kill()
    {
        TakeDamage(currentHealth);
    }

    /// <summary>
    /// Handle death
    /// </summary>
    void Die()
    {
        if (isDead) return;

        isDead = true;

        if (showDebugInfo)
            Debug.Log($"[Health] {gameObject.name} died!");

        // Invoke death event
        OnDeath?.Invoke();

        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Destroy GameObject if configured
        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    /// <summary>
    /// Revive with full health
    /// </summary>
    public void Revive()
    {
        isDead = false;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(1f);

        if (showDebugInfo)
            Debug.Log($"[Health] {gameObject.name} revived!");
    }

    // Public getters
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
    public bool IsDead() => isDead;
    public bool IsFullHealth() => currentHealth >= maxHealth;

    // Public setters
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }
}
