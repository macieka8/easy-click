using UnityEngine;
using System;

namespace EasyClick
{
    public class ChangeLevelOnTrigger : MonoBehaviour
    {
        [SerializeField]
        string _levelName;

        [SerializeField]
        PostGameLobbyData _gameResults;

        [SerializeField]
        LevelTimer _levelTimer;

        public event Action onLevelFinished = delegate { };

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                onLevelFinished?.Invoke();
                var bestScore = LevelTimer.getBestScore(LevelLoader.CurrentLevel);
                float bestScoreValue = bestScore.HasValue
                    ? bestScore.Value
                    : _levelTimer.TimeElapsed;

                var playerName = $"Player {PlayerInput.AllPlayers.IndexOf(collider.GetComponent<PlayerInput>())}";

                _gameResults.SetupData(
                    LevelLoader.CurrentLevel,
                    _levelTimer.TimeElapsed,
                    bestScoreValue,
                    playerName,
                    _levelName);
                LevelLoader.StartLevel("PostGame Lobby");
            }
        }
    }
}