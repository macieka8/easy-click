using UnityEngine;
using TMPro;
using System;

namespace EasyClick
{
    public class PostGameLobby : MonoBehaviour
    {
        [SerializeField] PostGameLobbyData _gameResults;

        [SerializeField] TextMeshProUGUI _winnerNameText;
        [SerializeField] TextMeshProUGUI _winnerTimeText;
        [SerializeField] TextMeshProUGUI _bestTimeText;

        void Start()
        {
            TimeSpan winnerTime = TimeSpan.FromSeconds(_gameResults.TimeScore);
            _winnerTimeText.text = $"{winnerTime.Minutes:d2}:{winnerTime.Seconds:d2}";

            TimeSpan bestTime = TimeSpan.FromSeconds(_gameResults.BestTime);
            _bestTimeText.text = $"{bestTime.Minutes:d2}:{bestTime.Seconds:d2}";

            _winnerNameText.text = _gameResults.WinnerName;
        }

        public void OnMainMenu()
        {
            LevelLoader.StartLevel("Menu");
        }

        public void OnNextMatch()
        {
            LevelLoader.StartLevel(_gameResults.NextLevel);
        }
    }
}
