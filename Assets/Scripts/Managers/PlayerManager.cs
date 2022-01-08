using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyClick
{
    public class PlayerManager : MonoBehaviour
    {
        public static event Action onRebindPerformed = delegate { };

        public static void ReplacePlayer(PlayerInput oldPlayerInput, GameObject playerPrefab)
        {
            if (PlayerInput.AllPlayers.Contains(oldPlayerInput))
            {
                var newPlayerGameObject = Instantiate(playerPrefab);
                var newPlayerInput = newPlayerGameObject.GetComponent<PlayerInput>();
                var newPlayerBody = newPlayerGameObject.GetComponent<ICharacterbody>();

                newPlayerInput.RebindControls(oldPlayerInput.PlayerControls);

                var playerSpawner = FindObjectOfType(typeof(PlayerSpawner)) as PlayerSpawner;
                playerSpawner.Respawn(newPlayerBody);

                var newPlayerIndex = PlayerInput.AllPlayers.IndexOf(newPlayerInput);
                PlayerInput.AllPlayers[PlayerInput.AllPlayers.IndexOf(oldPlayerInput)] = newPlayerInput;
                PlayerInput.AllPlayers[newPlayerIndex] = oldPlayerInput;

                Destroy(oldPlayerInput.gameObject);
            }
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