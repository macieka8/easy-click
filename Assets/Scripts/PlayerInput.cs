using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyClick
{
    public class PlayerInput : MonoBehaviour
    {
        public static readonly List<PlayerInput> AllPlayers = new List<PlayerInput>();
        public static event System.Action<PlayerInput> onPlayerJoined = delegate { };
        public static event System.Action<PlayerInput> onPlayerLeft = delegate { };

        PlayerControls _PlayerControls;

        public PlayerControls PlayerControls
        {
            get => _PlayerControls;
        }

        private void Awake()
        {
            _PlayerControls = new PlayerControls();
        }

        private void OnEnable()
        {
            AllPlayers.Add(this);
            onPlayerJoined?.Invoke(this);
        }

        private void OnDisable()
        {
            AllPlayers.Remove(this);
            onPlayerLeft?.Invoke(this);
        }

        public void RebindControls(PlayerControls newControls)
        {
            _PlayerControls.Gameplay.Get().ApplyBindingOverrides(newControls.Gameplay.Get().bindings);
        }
    }
}
