using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace EasyClick
{
    public class LevelTimer : MonoBehaviour
    {
        public static string SaveFileName = "bestTimes.json";

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

        [SerializeField] ChangeLevelOnTrigger _levelChanger;

        string _scorePath;
        float _timeElapsed;

        public float TimeElapsed { get => _timeElapsed; }

        void Awake()
        {
            _scorePath = $"{Application.dataPath}/{SaveFileName}";
        }

        void Update()
        {
            _timeElapsed += Time.deltaTime;
        }

        void OnEnable()
        {
            _levelChanger.onLevelFinished += SaveScore;
        }

        void OnDisable()
        {
            _levelChanger.onLevelFinished -= SaveScore;
        }

        void SaveScore()
        {
            TimeScoreList data;
            if (!File.Exists(_scorePath))
            {
                data = new TimeScoreList();
            }
            else
            {
                var fileContent = File.ReadAllText(_scorePath);
                data = JsonUtility.FromJson<TimeScoreList>(fileContent);
            }

            var entry = data.Scores.Find(entry => entry.LevelName == LevelLoader.CurrentLevel);
            if (entry != null)
            {
                if (entry.Time > _timeElapsed)
                    entry.Time = _timeElapsed;
            }
            else
            {
                data.Scores.Add(new TimeScoreEntry(LevelLoader.CurrentLevel, _timeElapsed));
            }

            File.WriteAllText(_scorePath, JsonUtility.ToJson(data));
        }

        public static float? getBestScore(string levelName)
        {
            string filePath = $"{Application.dataPath}/{SaveFileName}";
            if (File.Exists(filePath))
            {
                var fileContent = File.ReadAllText(filePath);
                var data = JsonUtility.FromJson<TimeScoreList>(fileContent);
                if (data != null)
                {
                    var entry = data.Scores.Find(entry => entry.LevelName == levelName);
                    if (entry != null)
                    {
                        return entry.Time;
                    }
                }
            }
            return null;
        }
    }
}
