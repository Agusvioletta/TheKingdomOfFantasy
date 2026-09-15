using UnityEngine;
using UnityEngine.InputSystem;

namespace TKOF.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movimiento")]
        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float sprintSpeed = 6.5f;
        [SerializeField] private float crouchSpeed = 1.8f;
        [SerializeField] private float mouseSensitivity = 0.12f;
        [SerializeField] private float standingHeight = 1.8f;
        [SerializeField] private float crouchingHeight = 1.0f;

        [Header("Cámara en tercera persona")]
        [SerializeField] private Transform cameraPivot;      
        [SerializeField] private Transform cameraTransform;  
        [SerializeField] private Vector3 shoulderOffset = new Vector3(0.5f, 0.3f, -2.5f); 
        [SerializeField] private float cameraCollisionRadius = 0.25f;
        [SerializeField] private LayerMask cameraObstacles;

        [Header("Animación")]
        [SerializeField] private Animator animator; 

        [Header("Stamina")]
        [SerializeField] private PlayerStamina stamina; 

        [Header("Empujar cajas físicas")]
        [SerializeField] private float pushForce = 3f;

        [Header("Input Actions")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private InputActionReference sprintAction;
        [SerializeField] private InputActionReference crouchAction;

        private CharacterController _controller;
        private float _pitch;
        private bool _isCrouching;
        private float _verticalVelocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable()
        {
            moveAction.action.Enable();
            lookAction.action.Enable();
            sprintAction.action.Enable();
            crouchAction.action.Enable();
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            lookAction.action.Disable();
            sprintAction.action.Disable();
            crouchAction.action.Disable();
        }

        private void Update()
        {
            if (Time.timeScale == 0f) return; // pausado (panel de victoria/derrota abierto)

            HandleLook();
            HandleMove();
            HandleCrouch();
        }

        private void LateUpdate()
        {
            PositionCamera();
        }

        private void HandleLook()
        {
            Vector2 look = lookAction.action.ReadValue<Vector2>();
            transform.Rotate(Vector3.up * look.x * mouseSensitivity);
            _pitch = Mathf.Clamp(_pitch - look.y * mouseSensitivity, -40f, 70f);
            cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void HandleMove()
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            Vector3 move = (transform.right * input.x + transform.forward * input.y).normalized;

            bool wantsSprint = sprintAction.action.IsPressed();
            bool canActuallySprint = stamina == null || stamina.CanSprint;

            float speed = _isCrouching ? crouchSpeed
                : (wantsSprint && canActuallySprint) ? sprintSpeed
                : walkSpeed;

            if (_controller.isGrounded) _verticalVelocity = -1f;
            else _verticalVelocity -= 9.81f * Time.deltaTime;

            Vector3 velocity = move * speed + Vector3.up * _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);

            if (animator != null)
                animator.SetFloat("Speed", move.magnitude * (speed / walkSpeed));
        }

        private void HandleCrouch()
        {
            if (crouchAction.action.WasPerformedThisFrame()) _isCrouching = !_isCrouching;
            _controller.height = Mathf.Lerp(_controller.height,
                _isCrouching ? crouchingHeight : standingHeight, Time.deltaTime * 8f);
        }

        /// <summary>
        /// Ubica la cámara en el offset  deseado, pero si hay
        /// una pared en el medio, la acerca para
        /// no atravesarla.
        /// </summary>
        private void PositionCamera()
        {
            Vector3 desiredWorldPos = cameraPivot.TransformPoint(shoulderOffset);
            Vector3 direction = desiredWorldPos - cameraPivot.position;
            float desiredDistance = direction.magnitude;
            direction.Normalize();

            float finalDistance = desiredDistance;
            if (Physics.SphereCast(cameraPivot.position, cameraCollisionRadius, direction,
                    out RaycastHit hit, desiredDistance, cameraObstacles))
            {
                finalDistance = hit.distance;
            }

            cameraTransform.position = cameraPivot.position + direction * finalDistance;
            cameraTransform.LookAt(cameraPivot.position + cameraPivot.forward * 5f);
        }

        public bool IsCrouching => _isCrouching;
        public bool IsSprinting => sprintAction.action.IsPressed() && (stamina == null || stamina.CanSprint);

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic) return;

            if (hit.moveDirection.y < -0.3f) return;

            Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            body.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
    }
}