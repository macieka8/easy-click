using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyClick
{
    public class PlayerControlsSave : MonoBehaviour
    {
        public void SaveAllPlayersInput()
        {
            foreach (var player in PlayerInput.AllPlayers)
            {
                SavePlayerInputs(player);
            }
        }

        public void LoadAllPlayersInput()
        {
            foreach (var player in PlayerInput.AllPlayers)
            {
                LoadPlayerInputs(player);
            }
        }

        public void SavePlayerInputs(PlayerInput player)
        {
            foreach(var map in player.PlayerControls.Gameplay.Get())
            {
                foreach (var binding in map.bindings)
                {
                    if (!string.IsNullOrEmpty(binding.overridePath))
                    {
                        string key = $"{PlayerInput.AllPlayers.IndexOf(player)}{map.name}{binding.name}";
                        PlayerPrefs.SetString(key, binding.overridePath);
                    }
                }
            }
        }

        public void LoadPlayerInputs(PlayerInput player)
        {
            foreach (var map in player.PlayerControls.Gameplay.Get())
            {
                var bindings = map.bindings;
                for(int bindingIndex = 0; bindingIndex < bindings.Count; bindingIndex++)
                {
                    string key = $"{PlayerInput.AllPlayers.IndexOf(player)}{map.name}{bindings[bindingIndex].name}";
                    if (PlayerPrefs.HasKey(key))
                    {
                        string overridePath = PlayerPrefs.GetString(key);
                        map.ApplyBindingOverride(bindingIndex, new InputBinding { overridePath = overridePath });
                    }
                }
            }
        }
    }
}
