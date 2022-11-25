using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

namespace EasyClick
{
    public class MultiplayerMenu : MonoBehaviour
    {
        [SerializeField] RacerEntityCollection _racerCollection;
        [SerializeField] SpawnerVariable _spawnerVariable;

        [SerializeField] GameObject _EntryPrefab;
        [SerializeField] GameObject _HotKeyPrefab;

        [SerializeField] Transform _EntryParent;
        [SerializeField] Transform _HotKeysParent;

        [SerializeField] PlayerControlsSave _PlayerSaver;

        List<GameObject> _racerEntries;
        List<GameObject> _PlayerBindingEntries;

        private void Awake()
        {
            _racerEntries = new List<GameObject>();
            _PlayerBindingEntries = new List<GameObject>();
        }

        void Start()
        {
            foreach (var racer in _racerCollection.Collection)
            {
                if (racer.IsPlayer)
                {
                    _PlayerSaver.LoadPlayerInputs(racer.GetComponent<PlayerInput>());
                }
                CreateRacerEntry(racer);
            }
        }

        void CreateRacerEntry(RacerEntity spawnedRacerEntity)
        {
            GameObject newEntry = Instantiate(_EntryPrefab, _EntryParent);
            newEntry.transform
                .Find("Text")
                .GetComponent<TextMeshProUGUI>()
                .text = spawnedRacerEntity.Name;

            DisplayPlayerBindings(spawnedRacerEntity);

            _racerEntries.Add(newEntry);
        }

        void RemovePlayerEntry(int playerId)
        {
            Destroy(_racerEntries[playerId].gameObject);
            Destroy(_PlayerBindingEntries[playerId].gameObject);

            _racerEntries.RemoveAt(playerId);
            _PlayerBindingEntries.RemoveAt(playerId);
        }

        void DisplayPlayerBindings(RacerEntity racer)
        {
            GameObject bindingsDisplayBar = Instantiate(_HotKeyPrefab, _HotKeysParent);

            if (racer.IsPlayer)
            {
                var gameplayActions = racer.GetComponent<PlayerInput>().PlayerControls.Gameplay;
                CreateBindingEntry(bindingsDisplayBar, "Left", gameplayActions.Rotate, 1);
                CreateBindingEntry(bindingsDisplayBar, "Right", gameplayActions.Rotate, 2);
                CreateBindingEntry(bindingsDisplayBar, "Respawn", gameplayActions.Respawn, 0);
            }

            CreateChangeCharacterEntry(bindingsDisplayBar, racer);

            _PlayerBindingEntries.Add(bindingsDisplayBar);
        }

        void CreateBindingEntry(GameObject bindingsDisplayBar, string name, InputAction inputAction, int bindingIndex)
        {
            var binding = bindingsDisplayBar.transform.Find(name);
            var bindingText = binding.Find("Text").GetComponent<TextMeshProUGUI>();

            bindingText.text = inputAction.bindings[bindingIndex].ToDisplayString();

            binding.GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerManager.PerformRebinding(inputAction, bindingIndex,
                () => bindingText.text = inputAction.bindings[bindingIndex].ToDisplayString());
            });
        }

        void CreateChangeCharacterEntry(GameObject bindingsDisplayBar, RacerEntity racer)
        {
            var character = bindingsDisplayBar.transform.Find("Character");
            var characterText = character.Find("Text").GetComponent<TextMeshProUGUI>();
            characterText.text = "0";

            character.GetComponent<Button>().onClick.AddListener(() =>
            {
                var characterText = character.Find("Text").GetComponent<TextMeshProUGUI>();

                // TODO: store player character info
                var characters = racer.IsPlayer ? GameCharactersManager.PlayableCharacters : GameCharactersManager.BotCharacters;

                var characterId = int.Parse(characterText.text) + 1;
                if (characterId > characters.Count - 1)
                    characterId = 0;

                characterText.text = characterId.ToString();

                racer = PlayerManager.ReplacePlayer(racer, characters[characterId]);
            });
        }

        void UpdateBindingDisplay(GameObject bindingsEntryBar, string name, InputAction inputAction, int bindingIndex)
        {
            var bindingText = bindingsEntryBar.transform.Find(name).Find("Text").GetComponent<TextMeshProUGUI>();
            bindingText.text = inputAction.bindings[bindingIndex].ToDisplayString();
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

        public void AddPlayer()
        {
            var spawnedRacerEntity = _spawnerVariable.Value.Spawn(true);
            _PlayerSaver.LoadPlayerInputs(spawnedRacerEntity.GetComponent<PlayerInput>());
            CreateRacerEntry(spawnedRacerEntity);
        }

        public void AddBot()
        {
            var spawnedRacerEntity = _spawnerVariable.Value.Spawn(false);
            CreateRacerEntry(spawnedRacerEntity);
        }

        public void RemovePlayer()
        {
            Destroy(PlayerInput.AllPlayers.Last().gameObject);
            RemovePlayerEntry(_racerEntries.Count - 1);
        }
    }
}