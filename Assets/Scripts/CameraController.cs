using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Orbit Settings")]
    [SerializeField] private float sensitivity    = 0.3f;
    [SerializeField] private float minVerticalAngle = -80f;
    [SerializeField] private float maxVerticalAngle =  80f;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask collisionMask  = ~0;   // Bỏ layer của Player khỏi đây
    [SerializeField] private float     collisionRadius = 0.3f; // Bán kính SphereCast
    [SerializeField] private float     minDistance     = 1.0f; // Khoảng cách tối thiểu tới target
    [SerializeField] private float     recoverSmoothTime = 0.4f; // Thời gian SmoothDamp khi khôi phục

    private CinemachineFollow follow;

    private bool    isDragging;
    private Vector2 lastMousePosition;

    private float horizontalAngle;
    private float verticalAngle;
    private float desiredDistance;   // Khoảng cách người dùng muốn (không đổi khi kéo)
    private float currentDistance;   // Khoảng cách thực tế (sau collision)
    private float distanceVelocity;  // Velocity cho SmoothDamp

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        follow = cinemachineCamera.GetComponent<CinemachineFollow>();

        Vector3 initial = follow.FollowOffset;
        desiredDistance = initial.magnitude;
        currentDistance = desiredDistance;

        horizontalAngle = Mathf.Atan2(initial.x, initial.z) * Mathf.Rad2Deg;
        verticalAngle   = Mathf.Asin(Mathf.Clamp(initial.y / desiredDistance, -1f, 1f)) * Mathf.Rad2Deg;
    }

    private void LateUpdate()
    {
        HandleInput();
        ApplyOrbitWithCollision();
    }

    // ── Input ─────────────────────────────────────────────────────────────────

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            isDragging        = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (!isDragging) return;

        Vector2 delta     = (Vector2)Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;

        horizontalAngle += delta.x * sensitivity;
        verticalAngle   -= delta.y * sensitivity;
        verticalAngle    = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
    }

    // ── Orbit + Collision ─────────────────────────────────────────────────────

    private void ApplyOrbitWithCollision()
    {
        // Tính hướng orbit từ góc hiện tại
        float   radH     = horizontalAngle * Mathf.Deg2Rad;
        float   radV     = verticalAngle   * Mathf.Deg2Rad;
        Vector3 orbitDir = new Vector3(
            Mathf.Cos(radV) * Mathf.Sin(radH),
            Mathf.Sin(radV),
            Mathf.Cos(radV) * Mathf.Cos(radH)
        );

        Vector3 targetPos    = GetTargetWorldPosition();
        float   safeDistance = GetSafeDistance(targetPos, orbitDir, desiredDistance);

        // ─── KEY FIX ───────────────────────────────────────────────────────
        // Khi gặp vật cản: SNAP ngay lập tức (không lerp) → không xuyên qua
        // Khi thoáng:      SmoothDamp → khôi phục mượt, không jitter
        if (safeDistance < currentDistance)
        {
            currentDistance   = safeDistance;   // snap ngay
            distanceVelocity  = 0f;             // reset velocity tránh overshoot
        }
        else
        {
            currentDistance = Mathf.SmoothDamp(
                currentDistance, safeDistance,
                ref distanceVelocity, recoverSmoothTime);
        }
        // ───────────────────────────────────────────────────────────────────

        follow.FollowOffset = orbitDir * currentDistance;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// SphereCast từ target → hướng camera.
    /// Trả về khoảng cách an toàn tối đa (≤ wantedDistance).
    /// </summary>
    private float GetSafeDistance(Vector3 origin, Vector3 direction, float wantedDistance)
    {
        if (Physics.SphereCast(
                origin,
                collisionRadius,
                direction,
                out RaycastHit hit,
                wantedDistance,
                collisionMask,
                QueryTriggerInteraction.Ignore))
        {
            // Clamp về minDistance để camera không dính vào target
            return Mathf.Max(hit.distance, minDistance);
        }

        return wantedDistance;
    }

    /// <summary>
    /// Lấy vị trí world của target mà Cinemachine đang follow.
    /// </summary>
    private Vector3 GetTargetWorldPosition()
    {
        return cinemachineCamera.Follow != null
            ? cinemachineCamera.Follow.position
            : transform.position;
    }
}
