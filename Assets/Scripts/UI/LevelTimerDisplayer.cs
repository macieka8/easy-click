using UnityEngine;
using TMPro;
using System;

namespace EasyClick
{
    public class LevelTimerDisplayer : MonoBehaviour
    {
        TextMeshProUGUI _timeText;

        [SerializeField]
        LevelTimer _levelTimer;

        private void Awake()
        {
            _timeText = GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(_levelTimer.TimeElapsed);
            _timeText.text = $"{timeSpan.Minutes:d2}:{timeSpan.Seconds:d2}";
        }
    }
}
