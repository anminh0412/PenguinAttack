using Plugins.Tick;
using Tick;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager
{
    public class PlayerController : Entity, ITickable, IShoot
    {
        public InputActionAsset inputAsset;
        public float rotateSpeed   = 90f;
        public float maxArrowScale = 5f;
        public float scaleSpeed    = 3f;

        private InputAction moveAction;
        private InputAction attackAction;
        private InputAction lookAction;
        private bool    isHolding;
        private float   savedYRotation;
        private float   currentArrowScale;
        private Vector2 pressPosition;

        protected override void OnEnable()
        {
            base.OnEnable();
            var playerMap = inputAsset.FindActionMap("Player");
            moveAction   = playerMap.FindAction("Move");
            attackAction = playerMap.FindAction("Attack");
            lookAction   = playerMap.FindAction("Look");
            moveAction.Enable();
            attackAction.Enable();
            lookAction.Enable();

            attackAction.started  += OnHoldStarted;
            attackAction.canceled += OnHoldCanceled;
        }

        protected override void OnDisable()
        {
            attackAction.started  -= OnHoldStarted;
            attackAction.canceled -= OnHoldCanceled;
            moveAction.Disable();
            attackAction.Disable();
            lookAction.Disable();
            base.OnDisable();
        }

        private void OnHoldStarted(InputAction.CallbackContext ctx)
        {
            if (isHolding) return;
            isHolding         = true;
            savedYRotation    = transform.eulerAngles.y;
            currentArrowScale = 0f;
            pressPosition     = Mouse.current.position.ReadValue();
            UpdateArrowScale();
        }

        private void OnHoldCanceled(InputAction.CallbackContext ctx)
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

        public override void Tick()
        {
            if (!isHolding) return;

            var currentPos = Mouse.current.position.ReadValue();
            var delta      = currentPos - pressPosition;

            var rotationY = delta.x * rotateSpeed * 0.01f;
            var rot = transform.eulerAngles;
            rot.y = savedYRotation + rotationY;
            transform.eulerAngles = rot;

            currentArrowScale = Mathf.Clamp(delta.y * scaleSpeed * 0.01f, 0f, maxArrowScale);
            UpdateArrowScale();
        }

        private void UpdateArrowScale()
        {
            if (arrow == null) return;
            arrow.localScale = new Vector3(1f, 1f, currentArrowScale);
        }
    }
}
