using UnityEngine;

/// <summary>
/// Camera controller that follows the player with optional rotation
/// Supports top-down and angled camera views typical of Pikmin games
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private bool autoFindPlayer = true;
    [SerializeField] private string playerTag = "Player";

    [Header("Camera Position")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 15f, -10f);
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private bool useSmoothFollow = true;

    [Header("Camera Rotation")]
    [SerializeField] private bool allowRotation = true;
    [SerializeField] private KeyCode rotateLeftKey = KeyCode.Q;
    [SerializeField] private KeyCode rotateRightKey = KeyCode.E;
    [SerializeField] private float rotationSpeed = 90f; // Degrees per second
    [SerializeField] private float rotationSmoothness = 5f;
    [SerializeField] private bool snapRotation = false; // Snap to 90° increments
    [SerializeField] private float snapAngle = 90f;

    [Header("Zoom Settings")]
    [SerializeField] private bool allowZoom = true;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 25f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float currentZoom = 15f;

    [Header("Look At Settings")]
    [SerializeField] private bool lookAtTarget = true;
    [SerializeField] private Vector3 lookAtOffset = Vector3.up;

    [Header("Bounds")]
    [SerializeField] private bool constrainToBounds = false;
    [SerializeField] private Vector3 boundsMin = new Vector3(-50f, 0f, -50f);
    [SerializeField] private Vector3 boundsMax = new Vector3(50f, 50f, 50f);

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    private float currentRotationAngle = 0f;
    private float targetRotationAngle = 0f;
    private Vector3 currentVelocity;

    void Start()
    {
        // Auto-find player if needed
        if (autoFindPlayer && target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                target = player.transform;
                if (showDebugInfo)
                    Debug.Log($"[CameraController] Found target: {player.name}");
            }
            else
            {
                Debug.LogError("[CameraController] No player found with tag 'Player'!");
            }
        }

        currentZoom = offset.y;
        currentRotationAngle = transform.eulerAngles.y;
        targetRotationAngle = currentRotationAngle;

        if (showDebugInfo)
            Debug.Log("[CameraController] Initialized");
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleRotationInput();
        HandleZoomInput();
        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    /// <summary>
    /// Handle camera rotation input
    /// </summary>
    void HandleRotationInput()
    {
        if (!allowRotation) return;

        float rotationInput = 0f;

        // Keyboard rotation
        if (Input.GetKey(rotateLeftKey))
        {
            rotationInput = -1f;
        }
        else if (Input.GetKey(rotateRightKey))
        {
            rotationInput = 1f;
        }

        // Apply rotation
        if (Mathf.Abs(rotationInput) > 0.1f)
        {
            if (snapRotation)
            {
                // Snap to specific angles
                if (rotationInput < 0 && !Input.GetKey(rotateLeftKey))
                {
                    return; // Only rotate on key press
                }
                if (rotationInput > 0 && !Input.GetKey(rotateRightKey))
                {
                    return;
                }

                targetRotationAngle += snapAngle * Mathf.Sign(rotationInput);
            }
            else
            {
                // Smooth rotation
                targetRotationAngle += rotationSpeed * rotationInput * Time.deltaTime;
            }
        }

        // Normalize angle
        targetRotationAngle = Mathf.Repeat(targetRotationAngle, 360f);
    }

    /// <summary>
    /// Handle zoom input
    /// </summary>
    void HandleZoomInput()
    {
        if (!allowZoom) return;

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            currentZoom -= scrollInput * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            if (showDebugInfo)
                Debug.Log($"[CameraController] Zoom: {currentZoom:F2}");
        }
    }

    /// <summary>
    /// Update camera position to follow target
    /// </summary>
    void UpdateCameraPosition()
    {
        // Calculate rotated offset
        Quaternion rotation = Quaternion.Euler(0f, currentRotationAngle, 0f);
        Vector3 rotatedOffset = rotation * offset.normalized * currentZoom;

        // Add horizontal offset
        rotatedOffset += new Vector3(0f, offset.y - currentZoom, 0f);

        // Calculate target position
        Vector3 targetPosition = target.position + rotatedOffset;

        // Constrain to bounds if enabled
        if (constrainToBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, boundsMin.x, boundsMax.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, boundsMin.y, boundsMax.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, boundsMin.z, boundsMax.z);
        }

        // Move camera
        if (useSmoothFollow)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref currentVelocity,
                1f / followSpeed
            );
        }
        else
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                followSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// Update camera rotation to look at target
    /// </summary>
    void UpdateCameraRotation()
    {
        // Smoothly interpolate rotation angle
        currentRotationAngle = Mathf.LerpAngle(
            currentRotationAngle,
            targetRotationAngle,
            rotationSmoothness * Time.deltaTime
        );

        // Look at target if enabled
        if (lookAtTarget && target != null)
        {
            Vector3 lookAtPosition = target.position + lookAtOffset;
            Quaternion targetLookRotation = Quaternion.LookRotation(
                lookAtPosition - transform.position
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetLookRotation,
                rotationSmoothness * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// Set the camera target
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (showDebugInfo)
            Debug.Log($"[CameraController] Target set to: {newTarget.name}");
    }

    /// <summary>
    /// Get the camera's forward direction (flattened)
    /// </summary>
    public Vector3 GetForwardDirection()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    /// <summary>
    /// Get the camera's right direction (flattened)
    /// </summary>
    public Vector3 GetRightDirection()
    {
        Vector3 right = transform.right;
        right.y = 0;
        return right.normalized;
    }

    /// <summary>
    /// Manually set camera rotation angle
    /// </summary>
    public void SetRotationAngle(float angle)
    {
        targetRotationAngle = angle;
        currentRotationAngle = angle;
    }

    /// <summary>
    /// Get current rotation angle
    /// </summary>
    public float GetRotationAngle()
    {
        return currentRotationAngle;
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // Draw camera bounds
        if (constrainToBounds)
        {
            Gizmos.color = Color.yellow;
            Vector3 boundsCenter = (boundsMin + boundsMax) / 2f;
            Vector3 boundsSize = boundsMax - boundsMin;
            Gizmos.DrawWireCube(boundsCenter, boundsSize);
        }

        // Draw target connection
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, target.position);

        // Draw look at point
        if (lookAtTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(target.position + lookAtOffset, 0.5f);
        }
    }
}
