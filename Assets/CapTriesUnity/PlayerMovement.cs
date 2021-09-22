using InputActions;
using UnityEngine;

namespace CapTriesUnity
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerActions _inputAction;
        private Vector2 _moveInput;
        private Vector2 _mouseInput;
        private Rigidbody _rigidbody;
        private Rigidbody _camRigidbody;
        private Transform _camTransform;
        private bool _sprinting;
        
        [Header("Camera")] 
        [SerializeField] private Camera playerCamera;
        [SerializeField] private bool invertY;
        [SerializeField] private Vector2 mouseSensitivity;
        [SerializeField, Range(-180, 180)] private float yClampMin;
        [SerializeField, Range(-180, 180)] private float yClampMax;
        [SerializeField, Range(0f, 1f)] private float axisSmoothing;

        [Header("Movement")]
        [SerializeField] private Vector2 accelerationAmount;
        [SerializeField] private float torqueAmount;
        [SerializeField, Range(0f, 1f)] private float counterMovement;
        [SerializeField, Range(1, 3f)] private float sprintMultiplier;


        private void Awake()
        {
            _inputAction = new PlayerActions();
            _rigidbody = GetComponent<Rigidbody>();
            _camRigidbody = playerCamera.GetComponent<Rigidbody>();
            _camTransform = playerCamera.transform;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void FixedUpdate()
        {
            GetInput();
            CameraStuff();
            ApplyMovement();
        }

        private void CameraStuff()
        {
            var smoothingModifier = 1 - axisSmoothing;
            _rigidbody.AddTorque(-_rigidbody.angularVelocity * smoothingModifier, ForceMode.VelocityChange);
            _rigidbody.AddTorque(Vector3.up * (_mouseInput.x * torqueAmount * smoothingModifier), ForceMode.VelocityChange);
            _camRigidbody.AddTorque(-_camRigidbody.angularVelocity * smoothingModifier, ForceMode.VelocityChange);
            _camRigidbody.AddTorque(_camTransform.right * (_mouseInput.y * torqueAmount * (invertY ? -1 : 1) * smoothingModifier), ForceMode.VelocityChange);
            var camEulerAngles = _camTransform.eulerAngles;
            camEulerAngles.y = _rigidbody.transform.eulerAngles.y;
            camEulerAngles.z = 0;
            _camTransform.eulerAngles = camEulerAngles;
        }

        private void ApplyMovement()
        {
            var transform1 = transform;
            var velocity = transform1.forward * _moveInput.y + transform1.right * _moveInput.x;
            velocity *= _sprinting ? sprintMultiplier : 1f;
            /*velocity.y = _rigidbody.velocity.y;*/
            if (_moveInput == Vector2.zero)
                _rigidbody.AddForce(-_rigidbody.velocity * counterMovement, ForceMode.VelocityChange);
            else
                _rigidbody.velocity = Vector3.MoveTowards(_rigidbody.velocity, velocity, accelerationAmount.magnitude * Time.fixedDeltaTime);
        }

        private void GetInput()
        {
            _sprinting = _inputAction.Movement.Run.ReadValue<float>() != 0;
            _moveInput = _inputAction.Movement.Movement.ReadValue<Vector2>() * (Time.fixedDeltaTime * accelerationAmount);
            _mouseInput = _inputAction.Mouse.Look.ReadValue<Vector2>() * (Time.fixedDeltaTime * mouseSensitivity);
        }

        private void OnEnable()
        {
            _inputAction.Enable();
        }

        private void OnDisable()
        {
            _inputAction.Disable();
        }

        private void OnValidate()
        {
            _camRigidbody = playerCamera.GetComponent<Rigidbody>();
        }
    }
}