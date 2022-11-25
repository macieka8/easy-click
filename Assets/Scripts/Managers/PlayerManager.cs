using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyClick
{
    public class PlayerManager : MonoBehaviour
    {
        static PlayerManager _Instance;
        public static event Action onRebindPerformed = delegate { };
        [SerializeField] RacerEntityCollection _racerCollection;
        [SerializeField] SpawnerVariable _SpawnerVariable;

        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
        }

        RacerEntity ReplacePlayerInner(RacerEntity oldRacerEntity, GameObject playerPrefab)
        {
            var newPlayerGameObject = Instantiate(playerPrefab);
            var newPlayerBody = newPlayerGameObject.GetComponent<ICharacterbody>();

            if (oldRacerEntity.IsPlayer)
            {
                var oldPlayerInput = oldRacerEntity.GetComponent<PlayerInput>();
                var newPlayerInput = newPlayerGameObject.GetComponent<PlayerInput>();
                newPlayerInput.RebindControls(oldPlayerInput.PlayerControls);

                var newPlayerIndex = PlayerInput.AllPlayers.IndexOf(newPlayerInput);
                PlayerInput.AllPlayers[PlayerInput.AllPlayers.IndexOf(oldPlayerInput)] = newPlayerInput;
                PlayerInput.AllPlayers[newPlayerIndex] = oldPlayerInput;
            }

            _SpawnerVariable.Value.Respawn(newPlayerBody);
            Destroy(oldRacerEntity.gameObject);

            return newPlayerGameObject.GetComponent<RacerEntity>();
        }

        public static RacerEntity ReplacePlayer(RacerEntity oldRacerEntity, GameObject playerPrefab)
        {
            return _Instance.ReplacePlayerInner(oldRacerEntity, playerPrefab);
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