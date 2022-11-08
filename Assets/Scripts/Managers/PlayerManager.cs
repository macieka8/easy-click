using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyClick
{
    public class PlayerManager : MonoBehaviour
    {
        static PlayerManager _Instance;
        public static event Action onRebindPerformed = delegate { };
        [SerializeField] SpawnerVariable _SpawnerVariable;

        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
        }

        void ReplacePlayerInner(PlayerInput oldPlayerInput, GameObject playerPrefab)
        {
            if (PlayerInput.AllPlayers.Contains(oldPlayerInput))
            {
                var newPlayerGameObject = Instantiate(playerPrefab);
                var newPlayerInput = newPlayerGameObject.GetComponent<PlayerInput>();
                var newPlayerBody = newPlayerGameObject.GetComponent<ICharacterbody>();

                newPlayerInput.RebindControls(oldPlayerInput.PlayerControls);

                _SpawnerVariable.Value.Respawn(newPlayerBody);

                var newPlayerIndex = PlayerInput.AllPlayers.IndexOf(newPlayerInput);
                PlayerInput.AllPlayers[PlayerInput.AllPlayers.IndexOf(oldPlayerInput)] = newPlayerInput;
                PlayerInput.AllPlayers[newPlayerIndex] = oldPlayerInput;

                Destroy(oldPlayerInput.gameObject);
            }
        }

        public static void ReplacePlayer(PlayerInput oldPlayerInput, GameObject playerPrefab)
        {
            _Instance.ReplacePlayerInner(oldPlayerInput, playerPrefab);
        }

        public static void ReplacePlayer(int oldPlayerId, GameObject playerPrefab)
        {
            if (oldPlayerId >= 0 && oldPlayerId < PlayerInput.AllPlayers.Count)
            {
                ReplacePlayer(PlayerInput.AllPlayers[oldPlayerId], playerPrefab);
            }
        }

        public static void PerformRebinding(InputAction inputAction, int bindingIndex = 0, Action callback = null)
        {
            bool wasEnabled = false;
            if (inputAction.enabled)
            {
                inputAction.Disable();
                wasEnabled = true;
            }

            var op = inputAction.PerformInteractiveRebinding()
                .WithTargetBinding(bindingIndex)
                .WithControlsExcluding("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(op =>
                {
                    if (wasEnabled)
                    {
                        inputAction.Enable();
                    }

                    callback?.Invoke();
                    onRebindPerformed?.Invoke();
                    op.Dispose();
                })
                .Start();
        }
    }
}