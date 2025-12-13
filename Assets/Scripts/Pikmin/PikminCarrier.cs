using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles Pikmin carrying objects (pellets, treasures, enemies)
/// Manages group carrying mechanics and pathfinding to destination
/// </summary>
public class PikminCarrier : MonoBehaviour
{
    [Header("Carrying Settings")]
    [SerializeField] private float carrySpeed = 3f;
    [SerializeField] private float carryHeight = 1f; // Height above ground when carrying
    [SerializeField] private float arrivalDistance = 1f; // Distance to destination to consider arrived

    [Header("Formation Settings")]
    [SerializeField] private float carrierSpacing = 1f; // Space between carrying Pikmin
    [SerializeField] private bool formCircleAroundObject = true;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask carryableLayer;
    [SerializeField] private float attachDelay = 0.5f; // Delay before starting to carry

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private bool showGizmos = true;

    // Carrying state
    private GameObject currentCarryObject;
    private Pellet currentPellet;
    private BuriedTreasure currentTreasure;
    private Rigidbody carryObjectRb;
    private Vector3 carryDestination;
    private Transform destinationTransform;

    private List<Pikmin> attachedCarriers = new List<Pikmin>();
    private float requiredCarriers = 1f;
    private bool isCarrying = false;
    private float attachTime = 0f;
    private bool hasArrived = false;

    // Pikmin state
    private Pikmin pikmin;
    private bool isAttachedToObject = false;
    private Vector3 carrierOffset; // Offset position around the object

    void Start()
    {
        pikmin = GetComponent<Pikmin>();

        if (pikmin == null)
        {
            Debug.LogError($"[PikminCarrier] {gameObject.name} needs a Pikmin component!");
            enabled = false;
            return;
        }

        if (showDebugInfo)
            Debug.Log($"[PikminCarrier] Initialized on {gameObject.name}");
    }

    void Update()
    {
        if (!isAttachedToObject)
        {
            CheckForCarryableObjects();
        }
        else if (isCarrying)
        {
            MoveObjectToDestination();
            UpdateCarrierPosition();
        }
    }

    /// <summary>
    /// Check for nearby carryable objects
    /// </summary>
    void CheckForCarryableObjects()
    {
        // Only check if we're following the player and not doing something else
        if (!pikmin.IsFollowing()) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, carryableLayer);

        foreach (Collider col in colliders)
        {
            // Check if it's a pellet
            Pellet pellet = col.GetComponent<Pellet>();
            if (pellet != null && pellet.CanBeCarried() && !pellet.IsBeingCarried())
            {
                TryAttachToObject(col.gameObject, pellet);
                return;
            }

            // Check if it's a treasure
            BuriedTreasure treasure = col.GetComponent<BuriedTreasure>();
            if (treasure != null && treasure.IsFullyExcavated())
            {
                // Treasures can be carried like pellets
                TryAttachToObject(col.gameObject, null, treasure);
                return;
            }
        }
    }

    /// <summary>
    /// Try to attach to a carryable object
    /// </summary>
    void TryAttachToObject(GameObject obj, Pellet pellet = null, BuriedTreasure treasure = null)
    {
        // Get or create carrier manager
        CarrierManager manager = obj.GetComponent<CarrierManager>();
        if (manager == null)
        {
            manager = obj.AddComponent<CarrierManager>();
            manager.Initialize(obj, pellet, treasure);
        }

        // Try to join the carrying group
        if (manager.AddCarrier(this))
        {
            isAttachedToObject = true;
            currentCarryObject = obj;
            currentPellet = pellet;
            currentTreasure = treasure;
            attachTime = Time.time;

            if (showDebugInfo)
                Debug.Log($"[PikminCarrier] {gameObject.name} attached to {obj.name}");
        }
    }

    /// <summary>
    /// Called by CarrierManager to set carrying state
    /// </summary>
    public void StartCarrying(GameObject obj, Vector3 destination, Transform destTransform, Rigidbody rb, Vector3 offset)
    {
        currentCarryObject = obj;
        carryDestination = destination;
        destinationTransform = destTransform;
        carryObjectRb = rb;
        carrierOffset = offset;
        isCarrying = true;
        hasArrived = false;

        if (showDebugInfo)
            Debug.Log($"[PikminCarrier] {gameObject.name} started carrying {obj.name}");
    }

    /// <summary>
    /// Move the carried object towards destination
    /// </summary>
    void MoveObjectToDestination()
    {
        if (currentCarryObject == null || hasArrived) return;

        // Get current destination (might be moving target)
        Vector3 targetPosition = destinationTransform != null ?
            destinationTransform.position : carryDestination;

        // Check if we've arrived
        float distance = Vector3.Distance(currentCarryObject.transform.position, targetPosition);
        if (distance <= arrivalDistance)
        {
            OnArriveAtDestination();
            return;
        }

        // This is handled by CarrierManager to move the object
        // Individual Pikmin just follow their offset positions
    }

    /// <summary>
    /// Update this Pikmin's position relative to the object
    /// </summary>
    void UpdateCarrierPosition()
    {
        if (currentCarryObject == null) return;

        // Calculate target position (around the object)
        Vector3 targetPosition = currentCarryObject.transform.position + carrierOffset;

        // Keep at carry height
        targetPosition.y = currentCarryObject.transform.position.y;

        // Move towards target position
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            carrySpeed * Time.deltaTime
        );

        // Face the destination
        if (destinationTransform != null)
        {
            Vector3 direction = (destinationTransform.position - transform.position).normalized;
            if (direction.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    5f * Time.deltaTime
                );
            }
        }
    }

    /// <summary>
    /// Called when object reaches destination
    /// </summary>
    void OnArriveAtDestination()
    {
        if (hasArrived) return;

        hasArrived = true;

        if (showDebugInfo)
            Debug.Log($"[PikminCarrier] {gameObject.name} arrived at destination");

        // Handle pellet delivery
        if (currentPellet != null)
        {
            // Find nearby onion
            PikminOnion nearestOnion = FindNearestOnion();
            if (nearestOnion != null)
            {
                currentPellet.MarkReadyForAbsorption(nearestOnion);
            }
        }

        // Handle treasure collection
        if (currentTreasure != null)
        {
            currentTreasure.Collect();
        }

        // Detach from object
        DetachFromObject();
    }

    /// <summary>
    /// Find nearest Onion
    /// </summary>
    PikminOnion FindNearestOnion()
    {
        PikminOnion[] onions = FindObjectsOfType<PikminOnion>();
        PikminOnion nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var onion in onions)
        {
            if (onion.IsActive())
            {
                float dist = Vector3.Distance(transform.position, onion.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = onion;
                }
            }
        }

        return nearest;
    }

    /// <summary>
    /// Detach from current object
    /// </summary>
    public void DetachFromObject()
    {
        if (currentCarryObject != null)
        {
            CarrierManager manager = currentCarryObject.GetComponent<CarrierManager>();
            if (manager != null)
            {
                manager.RemoveCarrier(this);
            }
        }

        isAttachedToObject = false;
        isCarrying = false;
        currentCarryObject = null;
        currentPellet = null;
        currentTreasure = null;
        carryObjectRb = null;
        hasArrived = false;

        if (showDebugInfo)
            Debug.Log($"[PikminCarrier] {gameObject.name} detached from object");
    }

    /// <summary>
    /// Public getters
    /// </summary>
    public bool IsCarrying() => isCarrying;
    public bool IsAttachedToObject() => isAttachedToObject;
    public GameObject GetCurrentCarryObject() => currentCarryObject;
    public Pikmin GetPikmin() => pikmin;

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // Draw detection radius
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (isCarrying && currentCarryObject != null)
        {
            // Draw line to carry object
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentCarryObject.transform.position);

            // Draw line to destination
            if (destinationTransform != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(currentCarryObject.transform.position, destinationTransform.position);
            }
        }
    }

    void OnDestroy()
    {
        if (isAttachedToObject)
        {
            DetachFromObject();
        }
    }
}

/// <summary>
/// Manages all Pikmin carrying a single object
/// Attached dynamically to carryable objects
/// </summary>
public class CarrierManager : MonoBehaviour
{
    private GameObject carryObject;
    private Pellet pellet;
    private BuriedTreasure treasure;
    private Rigidbody rb;

    private List<PikminCarrier> carriers = new List<PikminCarrier>();
    private float requiredCarriers = 1f;
    private bool isBeingCarried = false;

    private Vector3 destination;
    private Transform destinationTransform;
    private float carrySpeed = 3f;
    private float carryHeight = 1f;

    /// <summary>
    /// Initialize the carrier manager
    /// </summary>
    public void Initialize(GameObject obj, Pellet p = null, BuriedTreasure t = null)
    {
        carryObject = obj;
        pellet = p;
        treasure = t;
        rb = obj.GetComponent<Rigidbody>();

        // Determine required carriers based on weight
        if (pellet != null)
        {
            requiredCarriers = pellet.GetWeight();
        }
        else
        {
            requiredCarriers = 1f;
        }

        // Find destination (nearest onion)
        FindDestination();
    }

    /// <summary>
    /// Find the destination for this object
    /// </summary>
    void FindDestination()
    {
        PikminOnion[] onions = FindObjectsOfType<PikminOnion>();
        PikminOnion nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var onion in onions)
        {
            if (onion.IsActive())
            {
                float dist = Vector3.Distance(transform.position, onion.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = onion;
                }
            }
        }

        if (nearest != null)
        {
            destinationTransform = nearest.transform;
            destination = nearest.transform.position;
        }
    }

    void Update()
    {
        if (isBeingCarried)
        {
            MoveToDestination();
        }
    }

    /// <summary>
    /// Add a carrier to this object
    /// </summary>
    public bool AddCarrier(PikminCarrier carrier)
    {
        if (carriers.Contains(carrier)) return false;

        carriers.Add(carrier);

        // Calculate offset position for this carrier
        Vector3 offset = CalculateCarrierOffset(carriers.Count - 1);

        // Notify carrier
        carrier.StartCarrying(carryObject, destination, destinationTransform, rb, offset);

        // Check if we have enough carriers to start moving
        if (carriers.Count >= Mathf.CeilToInt(requiredCarriers) && !isBeingCarried)
        {
            StartCarrying();
        }

        return true;
    }

    /// <summary>
    /// Remove a carrier from this object
    /// </summary>
    public void RemoveCarrier(PikminCarrier carrier)
    {
        carriers.Remove(carrier);

        // Clean up null entries
        carriers.RemoveAll(c => c == null);

        // Stop carrying if not enough carriers
        if (carriers.Count < Mathf.CeilToInt(requiredCarriers) && isBeingCarried)
        {
            StopCarrying();
        }

        // Destroy this manager if no carriers left
        if (carriers.Count == 0)
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Calculate offset position for carrier
    /// </summary>
    Vector3 CalculateCarrierOffset(int index)
    {
        // Arrange carriers in a circle around the object
        float angle = (index * 360f / Mathf.Max(carriers.Count, 1)) * Mathf.Deg2Rad;
        float radius = 1f;

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        return new Vector3(x, 0, z);
    }

    /// <summary>
    /// Start carrying the object
    /// </summary>
    void StartCarrying()
    {
        isBeingCarried = true;

        // Notify pellet
        if (pellet != null)
        {
            pellet.StartCarrying();
        }

        // Make object kinematic so carriers control it
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Debug.Log($"[CarrierManager] Started carrying {carryObject.name} with {carriers.Count} Pikmin");
    }

    /// <summary>
    /// Stop carrying the object
    /// </summary>
    void StopCarrying()
    {
        isBeingCarried = false;

        // Notify pellet
        if (pellet != null)
        {
            pellet.StopCarrying();
        }

        // Restore physics
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Debug.Log($"[CarrierManager] Stopped carrying {carryObject.name}");
    }

    /// <summary>
    /// Move object towards destination
    /// </summary>
    void MoveToDestination()
    {
        if (destinationTransform == null) return;

        // Calculate movement direction
        Vector3 direction = (destinationTransform.position - transform.position).normalized;

        // Move object
        transform.position = Vector3.MoveTowards(
            transform.position,
            destinationTransform.position,
            carrySpeed * Time.deltaTime
        );

        // Keep at carry height
        Vector3 pos = transform.position;
        pos.y = Mathf.Max(pos.y, carryHeight);
        transform.position = pos;
    }

    public List<PikminCarrier> GetCarriers() => carriers;
    public bool IsBeingCarried() => isBeingCarried;
    public int GetCarrierCount() => carriers.Count;
    public int GetRequiredCarriers() => Mathf.CeilToInt(requiredCarriers);
}
