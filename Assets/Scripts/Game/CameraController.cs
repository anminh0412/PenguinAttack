using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Zoom Settings")]
    [SerializeField] private float minZoomDistance = 3f;
    [SerializeField] private float maxZoomDistance = 20f;
    [SerializeField] private float zoomSpeed       = 0.01f;
    [SerializeField] private float scrollZoomSpeed = 2f;

    private CinemachineOrbitalFollow    orbitalFollow;
    private CinemachineInputAxisController inputAxisController;

    private float lastPinchDistance;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        orbitalFollow       = cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
        inputAxisController = cinemachineCamera.GetComponent<CinemachineInputAxisController>();

        // Tắt input controller mặc định, chỉ bật khi Click Hold
        inputAxisController.enabled = false;

        // Clamp radius ban đầu vào giới hạn zoom
        orbitalFollow.Radius = Mathf.Clamp(orbitalFollow.Radius, minZoomDistance, maxZoomDistance);
    }

    private void LateUpdate()
    {
        HandleOrbitInput();
        HandleZoom();
    }

    // ── Input ─────────────────────────────────────────────────────────────────

    private void HandleOrbitInput()
    {
        // Không xử lý orbit khi đang pinch
        if (Input.touchCount >= 2)
        {
            inputAxisController.enabled = false;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Bỏ qua nếu click trúng UI
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            inputAxisController.enabled = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            inputAxisController.enabled = false;
        }
    }

    // ── Zoom ──────────────────────────────────────────────────────────────────

    private void HandleZoom()
    {
        // Mobile: pinch 2 ngón tay
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            float currentPinchDistance = Vector2.Distance(touch0.position, touch1.position);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                lastPinchDistance = currentPinchDistance;
                return;
            }

            float pinchDelta  = lastPinchDistance - currentPinchDistance;
            lastPinchDistance = currentPinchDistance;

            orbitalFollow.Radius = Mathf.Clamp(orbitalFollow.Radius + pinchDelta * zoomSpeed, minZoomDistance, maxZoomDistance);
            return;
        }

        // PC: mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
            orbitalFollow.Radius = Mathf.Clamp(orbitalFollow.Radius - scroll * scrollZoomSpeed, minZoomDistance, maxZoomDistance);
    }
}
