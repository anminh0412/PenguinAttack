using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Unity.Cinemachine;

namespace Manager
{
    public class PlayerController : Entity
    {
        public CinemachineCamera cinemachineCamera;
        public GetUIInputEvent uiInputEvent;
        public float rotateSpeed   = 90f;
        public float maxArrowScale = 5f;
        public float scaleSpeed    = 3f;
        public float joystickDeadzone = 0.15f;

        private bool    isHolding;
        private float   savedYRotation;
        private float   currentArrowScale;
        private Vector2 pressPosition;
 
        protected override void OnEnable()
        {
            base.OnEnable();
            if (uiInputEvent != null)
            {
                uiInputEvent.onPointerDownEvent += OnPointerDown;
                uiInputEvent.onPointerUpEvent += OnPointerUp;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (uiInputEvent != null)
            {
                uiInputEvent.onPointerDownEvent -= OnPointerDown;
                uiInputEvent.onPointerUpEvent -= OnPointerUp;
            }
        }

        private void OnPointerDown(PointerEventData eventData)
        {
            if (isHolding) return;

            isHolding         = true;
            savedYRotation    = transform.eulerAngles.y;
            currentArrowScale = 0f;
            pressPosition     = eventData.position;
            UpdateArrowScale();
        }

        private void OnPointerUp(PointerEventData eventData)
        {
            if (!isHolding) return;
            isHolding = false;

            var launchScale = currentArrowScale;
            var launchDir   = transform.forward;
            var rot  = transform.eulerAngles;
            var rot2 = transform.eulerAngles;

            rb.linearVelocity  = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            arrow.localScale = new Vector3(1f, 1f, 0f);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, savedYRotation, transform.eulerAngles.z);

            currentArrowScale = 0f;
            UpdateArrowScale();

            shotData = new ShotData
            {
                LaunchScale = launchScale,
                LaunchDir   = launchDir,
                Rot         = rot,
                Rot2        = rot2
            };
        }

        public override void Shoot() {
            if (isHolding) {
                OnPointerUp(null);
            }
            base.Shoot();
        }


        public override void Tick()
        {
            if (!isHolding) return;
            UpdateRotation();
            UpdateArrowScale();
        }

        private void UpdateRotation()
        {
            if (Mouse.current == null && Touchscreen.current == null) return;

            Vector2 currentPos = pressPosition;
            if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            {
                currentPos = Touchscreen.current.touches[0].position.ReadValue();
            }
            else if (Mouse.current != null)
            {
                currentPos = Mouse.current.position.ReadValue();
            }

            var delta = currentPos - pressPosition;
            
            if (delta.sqrMagnitude > 0.01f)
            {
                var inputAngle = -Vector2.SignedAngle(Vector2.up, delta);

                if (cinemachineCamera != null)
                {
                    Vector2 camPosXZ = new Vector2(cinemachineCamera.transform.position.x, cinemachineCamera.transform.position.z);
                    Vector2 playerPosXZ = new Vector2(transform.position.x, transform.position.z);
                    Vector2 camDirXZ = (playerPosXZ - camPosXZ).normalized;

                    if (camDirXZ.sqrMagnitude > 0.01f)
                    {
                        float camAngle = -Vector2.SignedAngle(Vector2.up, camDirXZ);
                        inputAngle += camAngle;
                    }
                }

                var rot = transform.eulerAngles;
                rot.y = inputAngle;
                transform.eulerAngles = rot;
            }
            
            var distance = Vector2.Distance(currentPos, pressPosition);
            currentArrowScale = Mathf.Clamp(distance * scaleSpeed * 0.01f, 0f, maxArrowScale);
        }

        private void UpdateArrowScale()
        {
            if (arrow == null) return;
            arrow.localScale = new Vector3(1f, 1f, currentArrowScale);
        }
    }
}
