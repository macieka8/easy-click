using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using System.Linq;

namespace EasyClick
{
    public class LevelTimer : MonoBehaviour
    {
        [Serializable]
        class TimeScoreList
        {
            public List<TimeScoreEntry> Scores = new List<TimeScoreEntry>();
        }

        [Serializable]
        class TimeScoreEntry
        {
            public string LevelName;
            public float Time;

            public TimeScoreEntry(string name, float time)
            {
                LevelName = name;
                Time = time;
            }
        }

        string _scorePath;

        float _timeElapsed;
        TextMeshProUGUI _timeText;

        void Awake()
        {
            _timeText = GetComponent<TextMeshProUGUI>();
            _scorePath = Application.persistentDataPath + "/bestTimes.json";
        }

        void OnEnable()
        {
            LevelLoader.onLevelLoaded += ResetTimer;
            LevelLoader.onLevelUnload += SaveDataOnLevelUnload;
        }

        void OnDisable()
        {
            LevelLoader.onLevelLoaded -= ResetTimer;
            LevelLoader.onLevelUnload -= SaveDataOnLevelUnload;
        }

        void Update()
        {
            _timeElapsed += Time.deltaTime;
            UpdateTimeText();
        }

        void ResetTimer()
        {
            _timeElapsed = 0f;
        }

        void SaveDataOnLevelUnload()
        {
            if (File.Exists(_scorePath))
            {
                var fileContent = File.ReadAllText(_scorePath);
                var data = JsonUtility.FromJson<TimeScoreList>(fileContent);
                bool changed = false;
                if (data != null)
                {
                    var entry = data.Scores.Find(entry => entry.LevelName == LevelLoader.CurrentLevel);
                    if (entry != null && entry.Time > _timeElapsed)
                    {
                        entry.Time = _timeElapsed;
                        changed = true;
                    }
                    else
                    {
                        data.Scores.Add(new TimeScoreEntry(LevelLoader.CurrentLevel, _timeElapsed));
                        changed = true;
                    }
                }
                else
                {
                    data = new TimeScoreList();
                    data.Scores.Add(new TimeScoreEntry(LevelLoader.CurrentLevel, _timeElapsed));
                    changed = true;
                }

                if (changed)
                {
                    File.WriteAllText(_scorePath, JsonUtility.ToJson(data));
                }
            }
            else
            {
                File.Create(_scorePath);

                var data = new TimeScoreList();
                data.Scores.Add(new TimeScoreEntry(LevelLoader.CurrentLevel, _timeElapsed));

                File.WriteAllText(_scorePath, JsonUtility.ToJson(data));
            }
        }

        void UpdateTimeText()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(_timeElapsed);
            _timeText.text = $"{timeSpan.Minutes:d2}:{timeSpan.Seconds:d2}";
        }
    }
}
