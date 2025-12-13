using UnityEngine;

/// <summary>
/// Main player controller for the Captain character
/// Handles movement, rotation, and input
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("Input Settings")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private bool useMouseForRotation = true;
    [SerializeField] private bool useKeyboardForRotation = true;

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer = -1;
    [SerializeField] private Transform groundCheckPoint;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool autoFindCamera = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private bool showGizmos = true;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector3 lookDirection;
    private bool isGrounded = false;
    private bool isSprinting = false;
    private float currentSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Setup rigidbody
        if (rb != null)
        {
            rb.freezeRotation = true; // Prevent physics rotation
            rb.useGravity = true;
        }

        // Auto-find camera if needed
        if (autoFindCamera && cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                cameraTransform = mainCam.transform;
            }
            else
            {
                Debug.LogWarning("[PlayerController] No main camera found!");
            }
        }

        // Setup ground check point if not assigned
        if (groundCheckPoint == null)
        {
            GameObject checkPoint = new GameObject("GroundCheckPoint");
            checkPoint.transform.parent = transform;
            checkPoint.transform.localPosition = Vector3.down * 0.5f;
            groundCheckPoint = checkPoint.transform;
        }

        if (showDebugInfo)
            Debug.Log("[PlayerController] Initialized");
    }

    void Update()
    {
        CheckGrounded();
        HandleInput();
        HandleRotation();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    /// <summary>
    /// Check if player is grounded
    /// </summary>
    void CheckGrounded()
    {
        if (groundCheckPoint == null) return;

        RaycastHit hit;
        isGrounded = Physics.Raycast(
            groundCheckPoint.position,
            Vector3.down,
            out hit,
            groundCheckDistance,
            groundLayer
        );
    }

    /// <summary>
    /// Handle player input
    /// </summary>
    void HandleInput()
    {
        // Get movement input
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxisRaw("Vertical");     // W/S or Up/Down

        // Calculate move direction relative to camera
        Vector3 forward = cameraTransform != null ? cameraTransform.forward : transform.forward;
        Vector3 right = cameraTransform != null ? cameraTransform.right : transform.right;

        // Flatten directions (no vertical movement)
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Combine inputs
        moveDirection = (forward * vertical + right * horizontal).normalized;

        // Sprint input
        isSprinting = Input.GetKey(sprintKey) && moveDirection.magnitude > 0.1f;

        // Set look direction for rotation (keyboard movement)
        if (moveDirection.magnitude > 0.1f && useKeyboardForRotation)
        {
            lookDirection = moveDirection;
        }

        // Mouse rotation overrides keyboard if enabled
        if (useMouseForRotation)
        {
            HandleMouseRotation();
        }
    }

    /// <summary>
    /// Handle mouse-based rotation
    /// </summary>
    void HandleMouseRotation()
    {
        if (cameraTransform == null) return;

        // Raycast from camera to mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            // Calculate direction to mouse position
            Vector3 targetPosition = hit.point;
            Vector3 directionToMouse = targetPosition - transform.position;
            directionToMouse.y = 0; // Keep horizontal

            if (directionToMouse.magnitude > 0.1f)
            {
                lookDirection = directionToMouse.normalized;
            }
        }
    }

    /// <summary>
    /// Handle player movement via Rigidbody
    /// </summary>
    void HandleMovement()
    {
        if (rb == null) return;

        // Calculate target speed
        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
        targetSpeed = moveDirection.magnitude > 0.1f ? targetSpeed : 0f;

        // Smoothly accelerate/decelerate
        float accelRate = moveDirection.magnitude > 0.1f ? acceleration : deceleration;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, accelRate * Time.fixedDeltaTime);

        // Calculate velocity
        Vector3 targetVelocity = moveDirection * currentSpeed;

        // Apply movement (preserve vertical velocity for gravity)
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);

        if (showDebugInfo && moveDirection.magnitude > 0.1f)
        {
            Debug.Log($"[PlayerController] Moving: {moveDirection} Speed: {currentSpeed:F2}");
        }
    }

    /// <summary>
    /// Handle player rotation
    /// </summary>
    void HandleRotation()
    {
        if (lookDirection.magnitude < 0.1f) return;

        // Calculate target rotation
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        // Smoothly rotate
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Get the direction the player is facing
    /// </summary>
    public Vector3 GetForwardDirection()
    {
        return transform.forward;
    }

    /// <summary>
    /// Get the current move direction
    /// </summary>
    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    /// <summary>
    /// Check if player is moving
    /// </summary>
    public bool IsMoving()
    {
        return moveDirection.magnitude > 0.1f;
    }

    /// <summary>
    /// Check if player is sprinting
    /// </summary>
    public bool IsSprinting()
    {
        return isSprinting;
    }

    /// <summary>
    /// Check if player is grounded
    /// </summary>
    public bool IsGrounded()
    {
        return isGrounded;
    }

    /// <summary>
    /// Get current speed
    /// </summary>
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Draw ground check
        if (groundCheckPoint != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(
                groundCheckPoint.position,
                groundCheckPoint.position + Vector3.down * groundCheckDistance
            );
        }

        // Draw movement direction
        if (Application.isPlaying && moveDirection.magnitude > 0.1f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + Vector3.up, moveDirection * 2f);
        }

        // Draw look direction
        if (Application.isPlaying && lookDirection.magnitude > 0.1f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + Vector3.up, lookDirection * 2f);
        }
    }
}
