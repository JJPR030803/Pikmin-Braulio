using UnityEngine;

/// <summary>
/// Manages the flower that holds a pellet. When the flower is destroyed,
/// it drops its pellet and respawns after a delay.
/// </summary>
public class PelletFlower : MonoBehaviour
{
    [Header("Flower Properties")]
    [SerializeField] private int flowerHealth = 1; // How many hits to destroy
    [SerializeField] private GameObject pelletPrefab; // The pellet to spawn when destroyed
    [SerializeField] private Transform pelletSpawnPoint; // Where the pellet spawns

    [Header("Respawn Settings")]
    [SerializeField] private bool canRespawn = true;
    [SerializeField] private float respawnTime = 30f; // Time until flower regrows
    [SerializeField] private bool respawnWithPellet = true; // Does it respawn with pellet?

    [Header("Visual Settings")]
    [SerializeField] private GameObject flowerVisual; // Visual representation
    [SerializeField] private ParticleSystem destroyEffect;
    [SerializeField] private ParticleSystem respawnEffect;

    [Header("Audio")]
    [SerializeField] private AudioClip destroySound;
    [SerializeField] private AudioClip respawnSound;

    private int currentHealth;
    private bool isDestroyed = false;
    private float destroyTime = 0f;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = flowerHealth;

        // Auto-find pellet spawn point if not set
        if (pelletSpawnPoint == null)
        {
            pelletSpawnPoint = transform;
        }

        // Auto-find flower visual if not set
        if (flowerVisual == null)
        {
            // Try to find a child called "Visual" or use self
            Transform visualChild = transform.Find("Visual");
            if (visualChild != null)
            {
                flowerVisual = visualChild.gameObject;
            }
            else
            {
                flowerVisual = gameObject;
            }
        }

        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (destroySound != null || respawnSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Validate pellet prefab
        if (pelletPrefab == null)
        {
            Debug.LogWarning($"[PelletFlower] {gameObject.name} has no pellet prefab assigned!");
        }
    }

    void Update()
    {
        // Check for respawn
        if (isDestroyed && canRespawn)
        {
            if (Time.time - destroyTime >= respawnTime)
            {
                RespawnFlower();
            }
        }
    }

    /// <summary>
    /// Damage the flower
    /// </summary>
    public void TakeDamage(int damage = 1)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            DestroyFlower();
        }
    }

    /// <summary>
    /// Destroy the flower and spawn pellet
    /// </summary>
    void DestroyFlower()
    {
        if (isDestroyed) return;

        isDestroyed = true;
        destroyTime = Time.time;

        // Spawn pellet
        if (pelletPrefab != null)
        {
            GameObject pellet = Instantiate(pelletPrefab, pelletSpawnPoint.position, Quaternion.identity);

            // Add a small upward velocity to the pellet so it pops out
            Rigidbody pelletRb = pellet.GetComponent<Rigidbody>();
            if (pelletRb != null)
            {
                pelletRb.linearVelocity = Vector3.up * 3f;
            }

            Debug.Log($"[PelletFlower] {gameObject.name} destroyed, pellet spawned");
        }

        // Play destroy effect
        if (destroyEffect != null)
        {
            ParticleSystem effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 3f);
        }

        // Play destroy sound
        if (audioSource != null && destroySound != null)
        {
            audioSource.PlayOneShot(destroySound);
        }

        // Hide visual
        if (flowerVisual != null)
        {
            flowerVisual.SetActive(false);
        }

        // Disable collider so it can't be hit again
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    /// <summary>
    /// Respawn the flower
    /// </summary>
    void RespawnFlower()
    {
        isDestroyed = false;
        currentHealth = flowerHealth;

        // Show visual
        if (flowerVisual != null)
        {
            flowerVisual.SetActive(true);
        }

        // Enable collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        // Play respawn effect
        if (respawnEffect != null)
        {
            ParticleSystem effect = Instantiate(respawnEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 3f);
        }

        // Play respawn sound
        if (audioSource != null && respawnSound != null)
        {
            audioSource.PlayOneShot(respawnSound);
        }

        Debug.Log($"[PelletFlower] {gameObject.name} respawned");
    }

    /// <summary>
    /// Kill the flower instantly (for Pikmin attacks)
    /// </summary>
    public void Kill()
    {
        TakeDamage(currentHealth);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if a Pikmin attacked this flower
        Pikmin pikmin = collision.gameObject.GetComponent<Pikmin>();
        if (pikmin != null)
        {
            // Pikmin can damage flowers
            TakeDamage(1);
        }
    }

    // Public getters
    public bool IsDestroyed() => isDestroyed;
    public int GetCurrentHealth() => currentHealth;
    public float GetRespawnProgress() => isDestroyed ? Mathf.Clamp01((Time.time - destroyTime) / respawnTime) : 1f;

    void OnDrawGizmosSelected()
    {
        // Draw pellet spawn point
        if (pelletSpawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pelletSpawnPoint.position, 0.3f);
        }

        // Draw respawn timer visualization
        if (isDestroyed && canRespawn)
        {
            Gizmos.color = Color.cyan;
            float progress = GetRespawnProgress();
            Gizmos.DrawWireCube(transform.position + Vector3.up * 2f, Vector3.one * progress);
        }
    }
}
