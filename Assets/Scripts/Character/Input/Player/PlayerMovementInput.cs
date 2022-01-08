using System;
using UnityEngine.InputSystem;

using UnityEngine;

namespace EasyClick
{
    public class PlayerMovementInput : MonoBehaviour, IMovementInput
    {
        PlayerInput _PlayerInput;

        public event Action<IInputData> onRotationChanged = delegate { };
        public event Action<IInputData> onJump = delegate { };

        private void Awake()
        {
            _PlayerInput = GetComponent<PlayerInput>();
        }

        private void OnEnable()
        {
            _PlayerInput.PlayerControls.Gameplay.Rotate.performed += OnRotationChanged;
            _PlayerInput.PlayerControls.Gameplay.Rotate.canceled += OnJump;
            _PlayerInput.PlayerControls.Gameplay.Rotate.Enable();
        }

        public void OnDisable()
        {
            _PlayerInput.PlayerControls.Gameplay.Rotate.performed -= OnRotationChanged;
            _PlayerInput.PlayerControls.Gameplay.Rotate.canceled -= OnJump;
            _PlayerInput.PlayerControls.Gameplay.Rotate.Disable();
        }

        private void OnJump(InputAction.CallbackContext obj)
        {
            onJump?.Invoke(new MovementInputData(obj.ReadValue<float>()));
        }

        private void OnRotationChanged(InputAction.CallbackContext obj)
        {
            onRotationChanged?.Invoke(new MovementInputData(obj.ReadValue<float>()));
        }
    }
}
