using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using System;

namespace EasyClick
{
    public class MultiplayerMenu : MonoBehaviour
    {
        [SerializeField] GameObject _EntryPrefab;
        [SerializeField] GameObject _HotKeyPrefab;

        [SerializeField] Transform _EntryParent;
        [SerializeField] Transform _HotKeysParent;

        [SerializeField] PlayerControlsSave _PlayerSaver;

        List<GameObject> _PlayerEntries;
        List<GameObject> _PlayerBindingEntries;

        private void Awake()
        {
            _PlayerEntries = new List<GameObject>();
            _PlayerBindingEntries = new List<GameObject>();
        }

        void Start()
        {
            for (int playerId = 0; playerId < PlayerInput.AllPlayers.Count; playerId++)
            {
                _PlayerSaver.LoadPlayerInputs(PlayerInput.AllPlayers[playerId]);
                CreatePlayerEntry(playerId);
            }
        }

        public void AddPlayer()
        {
            var spawner = FindObjectOfType(typeof(PlayerSpawner)) as PlayerSpawner;
            spawner.Spawn();
            
            var player = PlayerInput.AllPlayers.Last();
            _PlayerSaver.LoadPlayerInputs(player);

            CreatePlayerEntry(PlayerInput.AllPlayers.Count - 1);
        }

        public void RemovePlayer()
        {
            Destroy(PlayerInput.AllPlayers.Last().gameObject);
            RemovePlayerEntry(_PlayerEntries.Count - 1);
        }

        void CreatePlayerEntry(int playerId)
        {
            GameObject newEntry = Instantiate(_EntryPrefab, _EntryParent);
            newEntry.transform
                .Find("Text")
                .GetComponent<TextMeshProUGUI>()
                .text = $"Player {playerId + 1}";

            DisplayPlayerBindings(playerId);

            _PlayerEntries.Add(newEntry);
        }

        void RemovePlayerEntry(int playerId)
        {
            Destroy(_PlayerEntries[playerId].gameObject);
            Destroy(_PlayerBindingEntries[playerId].gameObject);

            _PlayerEntries.RemoveAt(playerId);
            _PlayerBindingEntries.RemoveAt(playerId);
        }

        void DisplayPlayerBindings(int playerId)
        {
            GameObject bindingsDisplayBar = Instantiate(_HotKeyPrefab, _HotKeysParent);
            var gameplayActions = PlayerInput.AllPlayers[playerId].PlayerControls.Gameplay;

            createBindingEntry(bindingsDisplayBar, "Left", gameplayActions.Rotate, 1);
            createBindingEntry(bindingsDisplayBar, "Right", gameplayActions.Rotate, 2);
            createBindingEntry(bindingsDisplayBar, "Respawn", gameplayActions.Respawn, 0);

            createChangeCharacterEntry(bindingsDisplayBar, playerId);

            _PlayerBindingEntries.Add(bindingsDisplayBar);
        }

        void createBindingEntry(GameObject bindingsDisplayBar, string name, InputAction inputAction, int bindingIndex)
        {
            var binding = bindingsDisplayBar.transform.Find(name);
            var bindingText = binding.Find("Text").GetComponent<TextMeshProUGUI>();

            bindingText.text = inputAction.bindings[bindingIndex].ToDisplayString();

            binding.GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerManager.PerformRebinding(inputAction, bindingIndex, () =>
                {
                    bindingText.text = inputAction.bindings[bindingIndex].ToDisplayString();
                });
            });
        }

        void createChangeCharacterEntry(GameObject bindingsDisplayBar, int playerId)
        {
            var character = bindingsDisplayBar.transform.Find("Character");
            var characterText = character.Find("Text").GetComponent<TextMeshProUGUI>();
            characterText.text = "0";

            character.GetComponent<Button>().onClick.AddListener(() => {
                var characterText = character.Find("Text").GetComponent<TextMeshProUGUI>();
                var characterId = uint.Parse(characterText.text);

                // TODO: store player character info
                var characters = GameCharactersManager.PlayableCharacters;
                characterId = (characterId >= characters.Length - 1) ? 0 : characterId + 1;
                characterText.text = characterId.ToString();

                PlayerManager.ReplacePlayer(playerId, GameCharactersManager.PlayableCharacters[characterId]);
            });
        }

        public void UpdateAllBindingsDisplay()
        {
            int playerId = 0;
            foreach (var bindingsEntryBar in _PlayerBindingEntries)
            {
                var gameplayActions = PlayerInput.AllPlayers[playerId++].PlayerControls.Gameplay;

                UpdateBindingDisplay(bindingsEntryBar, "Left", gameplayActions.Rotate, 1);
                UpdateBindingDisplay(bindingsEntryBar, "Right", gameplayActions.Rotate, 2);
                UpdateBindingDisplay(bindingsEntryBar, "Respawn", gameplayActions.Respawn, 0);
            }
        }

        void UpdateBindingDisplay(GameObject bindingsEntryBar, string name, InputAction inputAction, int bindingIndex)
        {
            var bindingText = bindingsEntryBar.transform.Find(name).Find("Text").GetComponent<TextMeshProUGUI>();
            bindingText.text = inputAction.bindings[bindingIndex].ToDisplayString();
        }
    }
}