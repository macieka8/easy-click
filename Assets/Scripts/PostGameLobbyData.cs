using UnityEngine;

namespace EasyClick
{
    [CreateAssetMenu(menuName = "PostGame Lobby/Data", fileName = "Data")]
    public class PostGameLobbyData : ScriptableObject
    {
        [SerializeField] string _levelName;
        [SerializeField] float _timeScore;
        [SerializeField] float _bestTime;
        [SerializeField] string _winnerName;

        [SerializeField] string _nextLevel;

        public string LevelName { get => _levelName; }
        public float TimeScore { get => _timeScore; }
        public float BestTime { get => _bestTime; }
        public string WinnerName { get => _winnerName; }
        public string NextLevel { get => _nextLevel; set => _nextLevel = value; }

        public void SetupData(string levelName, float scoredTime, float bestTime, string winnerName, string nextLevel)
        {
            _levelName = levelName;
            _timeScore = scoredTime;
            _bestTime = bestTime;
            _winnerName = winnerName;
            _nextLevel = nextLevel;
        }
    }
}
