using UnityEngine;
using System;

namespace EasyClick
{
    public class ChangeLevelOnTrigger : MonoBehaviour
    {
        [SerializeField] PostGameLobbyData _gameResults;
        [SerializeField] LevelTimer _levelTimer;
        [SerializeField] string _levelName;

        public event Action onLevelFinished = delegate { };

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<RacerEntity>(out var raceWinner))
            {
                onLevelFinished?.Invoke();
                var bestScore = LevelTimer.getBestScore(LevelLoader.CurrentLevel);
                float bestScoreValue = bestScore.HasValue ? bestScore.Value : _levelTimer.TimeElapsed;

                var playerName = raceWinner.Name;

                _gameResults.SetupData(
                    _levelTimer.TimeElapsed,
                    bestScoreValue,
                    playerName,
                    _levelName);
                LevelLoader.StartLevel("PostGame Lobby");
            }
        }
    }
}