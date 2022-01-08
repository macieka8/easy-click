using System;
using UnityEngine.InputSystem;

using UnityEngine;

namespace EasyClick
{
    public class PlayerRespawnInput : MonoBehaviour, IRespawnInput
    {
        PlayerInput _PlayerInput;

        public event Action<IInputData> onRespawn = delegate { };

        private void Awake()
        {
            _PlayerInput = GetComponent<PlayerInput>();
        }

        public void OnEnable()
        {
            _PlayerInput.PlayerControls.Gameplay.Respawn.performed += OnRespawn;
            _PlayerInput.PlayerControls.Gameplay.Respawn.Enable();
        }

        public void OnDisable()
        {
            _PlayerInput.PlayerControls.Gameplay.Respawn.performed -= OnRespawn;
            _PlayerInput.PlayerControls.Gameplay.Respawn.Disable();
        }

        private void OnRespawn(InputAction.CallbackContext obj)
        {
            onRespawn?.Invoke(new RespawnInputData(true));
        }
    }
}
