using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyClick
{
    public class PlayerInput : MonoBehaviour
    {
        public static readonly List<PlayerInput> AllPlayers = new List<PlayerInput>();
        public static event System.Action<PlayerInput> OnPlayerJoined = delegate { };
        public static event System.Action<PlayerInput> OnPlayerLeft = delegate { };

        PlayerControls _playerControls;

        public PlayerControls PlayerControls => _playerControls;

        void Awake()
        {
            _playerControls = new PlayerControls();
            AllPlayers.Add(this);
            OnPlayerJoined?.Invoke(this);
        }

        void OnDestroy()
        {
            AllPlayers.Remove(this);
            OnPlayerLeft?.Invoke(this);
        }

        public void RebindControls(PlayerControls newControls)
        {
            _playerControls.Gameplay.Get().ApplyBindingOverrides(newControls.Gameplay.Get().bindings);
        }
    }
}
