using UnityEngine;

/// <summary>
/// Represents a treasure buried underground that can be found by White Pikmin
/// </summary>
public class BuriedTreasure : MonoBehaviour
{
    [Header("Treasure Properties")]
    [SerializeField] private int treasureValue = 100; // Points or currency
    [SerializeField] private GameObject treasureVisual; // The visible treasure object
    [SerializeField] private bool startBuried = true;

    [Header("Buried Settings")]
    [SerializeField] private float buriedDepth = 2f; // How deep underground
    [SerializeField] private float riseSpeed = 1f; // Speed of emerging
    [SerializeField] private bool requiresDigging = true; // Needs Pikmin to dig it up
    [SerializeField] private float digProgressRequired = 100f; // Amount of digging needed
    [SerializeField] private float currentDigProgress = 0f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem revealEffect; // Effect when detected
    [SerializeField] private ParticleSystem digEffect; // Dirt particles while digging
    [SerializeField] private GameObject undergroundIndicator; // Shows where it's buried

    private bool isRevealed = false; // Detected by White Pikmin
    private bool isFullyExcavated = false; // Completely dug up
    private Vector3 buriedPosition;
    private Vector3 surfacePosition;

    void Start()
    {
        if (startBuried)
        {
            // Store surface position
            surfacePosition = transform.position;

            // Calculate buried position
            buriedPosition = transform.position - Vector3.up * buriedDepth;

            // Hide treasure visual initially
            if (treasureVisual != null)
            {
                treasureVisual.SetActive(false);
            }

            // Show underground indicator
            if (undergroundIndicator != null)
            {
                undergroundIndicator.SetActive(true);
            }
        }
        else
        {
            isRevealed = true;
            isFullyExcavated = true;
            surfacePosition = transform.position;
            buriedPosition = transform.position;

            if (treasureVisual != null)
            {
                treasureVisual.SetActive(true);
            }

            if (undergroundIndicator != null)
            {
                undergroundIndicator.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Reveal the treasure (detected by White Pikmin)
    /// </summary>
    public void Reveal()
    {
        if (isRevealed) return;

        isRevealed = true;

        Debug.Log($"[BuriedTreasure] {gameObject.name} revealed!");

        // Play reveal effect
        if (revealEffect != null)
        {
            ParticleSystem effect = Instantiate(revealEffect, surfacePosition, Quaternion.identity);
            Destroy(effect.gameObject, 3f);
        }

        // Show indicator that it's here
        if (undergroundIndicator != null)
        {
            undergroundIndicator.SetActive(true);
        }
    }

    /// <summary>
    /// Progress digging up the treasure
    /// </summary>
    public void Dig(float digAmount)
    {
        if (!isRevealed || isFullyExcavated) return;

        currentDigProgress += digAmount;

        // Play dig effect
        if (digEffect != null && !digEffect.isPlaying)
        {
            digEffect.Play();
        }

        // Check if fully excavated
        if (currentDigProgress >= digProgressRequired)
        {
            Excavate();
        }
    }

    /// <summary>
    /// Fully excavate the treasure
    /// </summary>
    void Excavate()
    {
        isFullyExcavated = true;

        Debug.Log($"[BuriedTreasure] {gameObject.name} excavated!");

        // Show treasure visual
        if (treasureVisual != null)
        {
            treasureVisual.SetActive(true);
        }

        // Hide underground indicator
        if (undergroundIndicator != null)
        {
            undergroundIndicator.SetActive(false);
        }

        // Move treasure to surface
        StartCoroutine(RiseToSurface());
    }

    /// <summary>
    /// Make the treasure rise to the surface
    /// </summary>
    System.Collections.IEnumerator RiseToSurface()
    {
        float elapsed = 0f;
        float duration = buriedDepth / riseSpeed;

        Vector3 startPos = buriedPosition;
        Vector3 endPos = surfacePosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        transform.position = surfacePosition;

        // Stop dig effect
        if (digEffect != null && digEffect.isPlaying)
        {
            digEffect.Stop();
        }
    }

    /// <summary>
    /// Collect the treasure
    /// </summary>
    public void Collect()
    {
        if (!isFullyExcavated) return;

        Debug.Log($"[BuriedTreasure] Collected {gameObject.name} - Value: {treasureValue}");

        // Here you could add points/currency to player
        // PlayerInventory.AddTreasureValue(treasureValue);

        Destroy(gameObject);
    }

    public bool IsRevealed() => isRevealed;
    public bool IsFullyExcavated() => isFullyExcavated;
    public int GetTreasureValue() => treasureValue;
    public float GetDigProgress() => currentDigProgress / digProgressRequired;

    void OnDrawGizmosSelected()
    {
        // Draw buried position
        if (startBuried || Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Vector3 buriedPos = Application.isPlaying ? buriedPosition : transform.position - Vector3.up * buriedDepth;
            Gizmos.DrawWireSphere(buriedPos, 0.5f);

            // Draw line from buried to surface
            Gizmos.color = Color.green;
            Vector3 surfacePos = Application.isPlaying ? surfacePosition : transform.position;
            Gizmos.DrawLine(buriedPos, surfacePos);
        }
    }
}
