using System;
using InputActions;
using UnityEngine;

namespace CapTriesUnity
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerActions InputAction { get; set; }
        private Vector2 PlayerInput { get; set; }
        private Rigidbody Rigidbody { get; set; }
        
        [field: SerializeField] private Vector2 MovementSpeed { get; set; }
        
        private void Awake()
        {
            InputAction = new PlayerActions();
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            GetInput();
            ApplyMovement();
        }

        public void ApplyMovement()
        {
            var vec3 = Vector3.zero;
            var currentPos = transform.position;
            vec3.x = PlayerInput.x;
            vec3.z = PlayerInput.y;
            var destination = currentPos + vec3;
            destination *= Time.fixedDeltaTime;
            destination.y = currentPos.y;
            Rigidbody.MovePosition(destination);
        }

        public void GetInput()
        {
            PlayerInput = InputAction.Movement.Movement.ReadValue<Vector2>() * MovementSpeed;
        }

        private void OnEnable()
        {
            InputAction.Enable();
        }

        private void OnDisable()
        {
            InputAction.Disable();
        }
    }
}