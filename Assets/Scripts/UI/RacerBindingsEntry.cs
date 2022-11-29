using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace EasyClick
{
    public class RacerBindingsEntry : MonoBehaviour
    {
        [Header("Internal References")]
        [SerializeField] TextMeshProUGUI _racerNameText;
        [SerializeField] Button _rotateLeftButton;
        [SerializeField] TextMeshProUGUI _rotateLeftText;
        [SerializeField] Button _rotateRightButton;
        [SerializeField] TextMeshProUGUI _rotateRightText;
        [SerializeField] Button _respawnButton;
        [SerializeField] TextMeshProUGUI _respawnText;
        [SerializeField] Button _changeCharacterButton;
        [SerializeField] TextMeshProUGUI _changeCharacterText;
        [SerializeField] Button _removeRacerButton;

        RacerEntity _racer;
        MultiplayerMenu _menu;

        public RacerEntity Racer => _racer;

        public void Setup(RacerEntity racer, MultiplayerMenu menu)
        {
            _racer = racer;
            _menu = menu;

            _racerNameText.text = racer.Name;
            if (_racer.IsPlayer)
            {
                CreateBindingEntry(_rotateLeftButton, _rotateLeftText, "Rotate", 1);
                CreateBindingEntry(_rotateRightButton, _rotateRightText, "Rotate", 2);
                CreateBindingEntry(_respawnButton, _respawnText, "Respawn", 0);
            }

            CreateChangeCharacterEntry();
            CreateRemoveRacerEntry();
        }

        void CreateBindingEntry(Button button, TextMeshProUGUI text, string actionName, int bindingIndex)
        {
            var action = _racer.GetComponent<PlayerInput>().PlayerControls.FindAction(actionName);
            text.text = action.bindings[bindingIndex].ToDisplayString();

            button.onClick.AddListener(() =>
            {
                var action = _racer.GetComponent<PlayerInput>().PlayerControls.FindAction(actionName);
                PlayerManager.PerformRebinding(action, bindingIndex,
                () => text.text = action.bindings[bindingIndex].ToDisplayString());
            });
        }

        void CreateChangeCharacterEntry()
        {
            _changeCharacterText.text = "0";

            _changeCharacterButton.onClick.AddListener(() =>
            {
                var characters = _racer.IsPlayer ? GameCharactersManager.PlayableCharacters : GameCharactersManager.BotCharacters;

                var characterId = int.Parse(_changeCharacterText.text) + 1;
                if (characterId > characters.Count - 1)
                    characterId = 0;

                _changeCharacterText.text = characterId.ToString();

                _racer = PlayerManager.ReplacePlayer(_racer, characters[characterId]);
            });
        }

        void CreateRemoveRacerEntry()
        {
            _removeRacerButton.onClick.AddListener(() => _menu.RemoveRacer(_racer));
        }

        void UpdateDisplay(TextMeshProUGUI text, InputAction inputAction, int bindingIndex)
        {
            text.text = inputAction.bindings[bindingIndex].ToDisplayString();
        }

        public void UpdateDisplay()
        {
            var gameplayActions = _racer.GetComponent<PlayerInput>().PlayerControls.Gameplay;
            UpdateDisplay(_rotateLeftText, gameplayActions.Rotate, 1);
            UpdateDisplay(_rotateRightText, gameplayActions.Rotate, 2);
            UpdateDisplay(_respawnText, gameplayActions.Respawn, 0);
        }
    }
}