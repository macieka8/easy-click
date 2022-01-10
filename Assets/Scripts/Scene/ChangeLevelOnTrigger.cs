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

                _gameResults.SetupData(
                    LevelLoader.CurrentLevel,
                    _levelTimer.TimeElapsed,
                    bestScoreValue,
                    "TODO: Winner name!",
                    _levelName);
                LevelLoader.StartLevel("PostGame Lobby");
            }
        }
    }
}