using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace EasyClick
{
    public class PlaygroundMenu : MonoBehaviour
    {
        [SerializeField] Button _ChangeCameraButtonPrefab;
        [SerializeField] MainMenuCameraController _CamController;

        List<Button> _CharacterButtons;

        private void Awake()
        {
            _CharacterButtons = new List<Button>();
        }

        private void OnEnable()
        {
            foreach (var player in PlayerInput.AllPlayers)
            {
                var newButton = Instantiate(_ChangeCameraButtonPrefab, transform);
                newButton.onClick.AddListener(() => { _CamController.SetFollowTarget(player.transform); });
                newButton.transform
                    .Find("Text")
                    .GetComponent<TMPro.TextMeshProUGUI>()
                    .text = $"Player {PlayerInput.AllPlayers.IndexOf(player) + 1}";

                _CharacterButtons.Add(newButton);
            }
        }

        private void OnDisable()
        {
            foreach (var button in _CharacterButtons)
            {
                Destroy(button.gameObject);
            }
            _CharacterButtons.Clear();
        }
    }
}