using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles the whistle mechanic for calling and gathering Pikmin
/// Creates expanding circle cursor that calls Pikmin within radius
/// </summary>
public class WhistleController : MonoBehaviour
{
    [Header("Whistle Settings")]
    [SerializeField] private KeyCode whistleKey = KeyCode.Mouse1; // Right click
    [SerializeField] private float minRadius = 1f;
    [SerializeField] private float maxRadius = 10f;
    [SerializeField] private float growSpeed = 5f; // Radius growth per second
    [SerializeField] private float shrinkSpeed = 8f; // Radius shrink when released

    [Header("Visual Settings")]
    [SerializeField] private LineRenderer whistleCircle;
    [SerializeField] private int circleSegments = 50;
    [SerializeField] private Color whistleColor = new Color(0, 1, 1, 0.8f); // Cyan
    [SerializeField] private Color callingColor = new Color(0, 1, 0, 1f); // Green when calling
    [SerializeField] private float lineWidth = 0.2f;
    [SerializeField] private Material whistleMaterial;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask groundLayer = -1;
    [SerializeField] private float raycastDistance = 100f;
    [SerializeField] private Camera playerCamera;

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem whistleEffect;
    [SerializeField] private ParticleSystem callSuccessEffect;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip whistleSound;
    [SerializeField] private AudioClip callSound;
    [SerializeField] private bool loopWhistleSound = true;

    [Header("Pikmin Calling")]
    [SerializeField] private LayerMask pikminLayer;
    [SerializeField] private float callInterval = 0.2f; // How often to check for Pikmin
    [SerializeField] private bool autoRegisterWithManager = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private bool showGizmos = true;

    private bool isWhistling = false;
    private float currentRadius = 0f;
    private Vector3 whistlePosition;
    private float lastCallTime = 0f;
    private List<Pikmin> pikminInRange = new List<Pikmin>();

    void Start()
    {
        // Auto-find camera
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // Setup LineRenderer for whistle circle
        if (whistleCircle == null)
        {
            GameObject circleObj = new GameObject("WhistleCircle");
            circleObj.transform.parent = transform;
            whistleCircle = circleObj.AddComponent<LineRenderer>();
        }

        SetupLineRenderer();

        // Setup audio
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = loopWhistleSound;
        audioSource.playOnAwake = false;

        // Start with circle disabled
        whistleCircle.enabled = false;

        if (showDebugInfo)
            Debug.Log("[WhistleController] Initialized");
    }

    void Update()
    {
        HandleWhistleInput();
        UpdateWhistleVisuals();
        UpdateWhistlePosition();

        if (isWhistling)
        {
            CallPikminInRange();
        }
    }

    /// <summary>
    /// Setup the LineRenderer for the whistle circle
    /// </summary>
    void SetupLineRenderer()
    {
        if (whistleCircle == null) return;

        whistleCircle.positionCount = circleSegments + 1;
        whistleCircle.useWorldSpace = true;
        whistleCircle.startWidth = lineWidth;
        whistleCircle.endWidth = lineWidth;
        whistleCircle.loop = true;

        if (whistleMaterial != null)
        {
            whistleCircle.material = whistleMaterial;
        }
        else
        {
            // Create basic material
            whistleCircle.material = new Material(Shader.Find("Sprites/Default"));
        }

        whistleCircle.startColor = whistleColor;
        whistleCircle.endColor = whistleColor;
    }

    /// <summary>
    /// Handle whistle input
    /// </summary>
    void HandleWhistleInput()
    {
        // Start whistling
        if (Input.GetKeyDown(whistleKey))
        {
            StartWhistle();
        }

        // Continue whistling (hold to grow)
        if (Input.GetKey(whistleKey) && isWhistling)
        {
            currentRadius += growSpeed * Time.deltaTime;
            currentRadius = Mathf.Clamp(currentRadius, minRadius, maxRadius);
        }

        // Stop whistling
        if (Input.GetKeyUp(whistleKey) && isWhistling)
        {
            StopWhistle();
        }
    }

    /// <summary>
    /// Start whistling
    /// </summary>
    void StartWhistle()
    {
        isWhistling = true;
        currentRadius = minRadius;
        whistleCircle.enabled = true;
        pikminInRange.Clear();

        // Play whistle sound
        if (audioSource != null && whistleSound != null)
        {
            if (loopWhistleSound)
            {
                audioSource.clip = whistleSound;
                audioSource.Play();
            }
            else
            {
                audioSource.PlayOneShot(whistleSound);
            }
        }

        // Play whistle particle effect
        if (whistleEffect != null)
        {
            whistleEffect.Play();
        }

        if (showDebugInfo)
            Debug.Log("[WhistleController] Started whistling");
    }

    /// <summary>
    /// Stop whistling
    /// </summary>
    void StopWhistle()
    {
        isWhistling = false;
        whistleCircle.enabled = false;
        currentRadius = 0f;

        // Stop whistle sound
        if (audioSource != null && loopWhistleSound)
        {
            audioSource.Stop();
        }

        // Stop particle effect
        if (whistleEffect != null && whistleEffect.isPlaying)
        {
            whistleEffect.Stop();
        }

        // Play call success sound if we called any Pikmin
        if (pikminInRange.Count > 0)
        {
            if (audioSource != null && callSound != null)
            {
                audioSource.PlayOneShot(callSound);
            }

            if (callSuccessEffect != null)
            {
                callSuccessEffect.Play();
            }
        }

        if (showDebugInfo)
            Debug.Log($"[WhistleController] Stopped whistling - Called {pikminInRange.Count} Pikmin");

        pikminInRange.Clear();
    }

    /// <summary>
    /// Update whistle circle position based on mouse
    /// </summary>
    void UpdateWhistlePosition()
    {
        if (!isWhistling || playerCamera == null) return;

        // Raycast from camera to mouse position
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
        {
            whistlePosition = hit.point;

            // Position particle effect if active
            if (whistleEffect != null)
            {
                whistleEffect.transform.position = whistlePosition;
            }
        }
    }

    /// <summary>
    /// Update visual appearance of whistle circle
    /// </summary>
    void UpdateWhistleVisuals()
    {
        if (!isWhistling || whistleCircle == null) return;

        // Update circle positions
        DrawCircle(whistlePosition, currentRadius);

        // Change color based on state
        Color currentColor = pikminInRange.Count > 0 ? callingColor : whistleColor;
        whistleCircle.startColor = currentColor;
        whistleCircle.endColor = currentColor;
    }

    /// <summary>
    /// Draw circle at given position with given radius
    /// </summary>
    void DrawCircle(Vector3 center, float radius)
    {
        float angleStep = 360f / circleSegments;
        Vector3[] points = new Vector3[circleSegments + 1];

        for (int i = 0; i <= circleSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = center.x + Mathf.Cos(angle) * radius;
            float z = center.z + Mathf.Sin(angle) * radius;

            // Slightly above ground to prevent z-fighting
            points[i] = new Vector3(x, center.y + 0.1f, z);
        }

        whistleCircle.SetPositions(points);
    }

    /// <summary>
    /// Call all Pikmin within whistle range
    /// </summary>
    void CallPikminInRange()
    {
        if (Time.time - lastCallTime < callInterval) return;

        lastCallTime = Time.time;

        // Find all colliders in range
        Collider[] colliders = Physics.OverlapSphere(whistlePosition, currentRadius, pikminLayer);

        foreach (Collider col in colliders)
        {
            Pikmin pikmin = col.GetComponent<Pikmin>();

            if (pikmin != null && !pikminInRange.Contains(pikmin))
            {
                // Check if Pikmin is following and not already registered with manager
                if (pikmin.IsFollowing() && !pikmin.IsRegisteredWithManager())
                {
                    // Register with PikminManager if auto-register is enabled
                    if (autoRegisterWithManager && PikminManager.Instance != null)
                    {
                        if (PikminManager.Instance.RegisterPikmin(pikmin))
                        {
                            pikminInRange.Add(pikmin);

                            if (showDebugInfo)
                                Debug.Log($"[WhistleController] Called Pikmin: {pikmin.name}");
                        }
                    }
                    else
                    {
                        // Just mark as called without registering
                        pikminInRange.Add(pikmin);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get current whistle radius
    /// </summary>
    public float GetCurrentRadius()
    {
        return currentRadius;
    }

    /// <summary>
    /// Check if currently whistling
    /// </summary>
    public bool IsWhistling()
    {
        return isWhistling;
    }

    /// <summary>
    /// Get whistle position
    /// </summary>
    public Vector3 GetWhistlePosition()
    {
        return whistlePosition;
    }

    /// <summary>
    /// Get number of Pikmin in range
    /// </summary>
    public int GetPikminInRangeCount()
    {
        return pikminInRange.Count;
    }

    void OnDrawGizmos()
    {
        if (!showGizmos || !isWhistling) return;

        // Draw whistle sphere
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(whistlePosition, currentRadius);

        // Draw center point
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(whistlePosition, 0.3f);

        // Draw lines to called Pikmin
        Gizmos.color = Color.green;
        foreach (Pikmin pikmin in pikminInRange)
        {
            if (pikmin != null)
            {
                Gizmos.DrawLine(whistlePosition, pikmin.transform.position);
            }
        }
    }
}
