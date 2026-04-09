using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 0.3f;

    private CinemachineFollow follow;
    private CinemachineRotationComposer rotationComposer;
    private bool isDragging;
    private Vector2 lastMousePosition;

    // Orbital state
    private float horizontalAngle;
    private float verticalAngle;
    private float followDistance;
    private Vector3 followOffset;

    private void Awake()
    {
        follow = cinemachineCamera.GetComponent<CinemachineFollow>();
        rotationComposer = cinemachineCamera.GetComponent<CinemachineRotationComposer>();

        // Cache initial offset values
        followOffset = follow.FollowOffset;
        followDistance = followOffset.magnitude;
        horizontalAngle = Mathf.Atan2(followOffset.x, followOffset.z) * Mathf.Rad2Deg;
        verticalAngle = Mathf.Asin(followOffset.y / followDistance) * Mathf.Rad2Deg;
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // If pointer is over an UI element, do not start dragging
            if (UnityEngine.EventSystems.EventSystem.current != null && 
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (!isDragging) return;

        Vector2 currentMousePosition = Input.mousePosition;
        Vector2 delta = currentMousePosition - lastMousePosition;
        lastMousePosition = currentMousePosition;

        horizontalAngle += delta.x * sensitivity;
        verticalAngle -= delta.y * sensitivity;
        verticalAngle = Mathf.Clamp(verticalAngle, -80f, 80f);

        float radH = horizontalAngle * Mathf.Deg2Rad;
        float radV = verticalAngle * Mathf.Deg2Rad;

        Vector3 newOffset;
        newOffset.x = followDistance * Mathf.Cos(radV) * Mathf.Sin(radH);
        newOffset.y = followDistance * Mathf.Sin(radV);
        newOffset.z = followDistance * Mathf.Cos(radV) * Mathf.Cos(radH);

        follow.FollowOffset = newOffset;
    }
}
