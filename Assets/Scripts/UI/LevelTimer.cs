using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

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

        [SerializeField]
        ChangeLevelOnTrigger _levelChanger;
        
        string _scorePath;
        float _timeElapsed;

        public float TimeElapsed { get => _timeElapsed; }

        void Awake()
        {
            _scorePath = Application.persistentDataPath + "/bestTimes.json";
        }

        void Update()
        {
            _timeElapsed += Time.deltaTime;
        }

        private void OnEnable()
        {
            _levelChanger.onLevelFinished += SaveScore;
        }

        private void OnDisable()
        {
            _levelChanger.onLevelFinished -= SaveScore;
        }

        private void SaveScore()
        {
            if (File.Exists(_scorePath))
            {
                var fileContent = File.ReadAllText(_scorePath);
                var data = JsonUtility.FromJson<TimeScoreList>(fileContent);
                if (data != null)
                {
                    var entry = data.Scores.Find(entry => entry.LevelName == LevelLoader.CurrentLevel);
                    if (entry != null && entry.Time > _timeElapsed)
                    {
                        entry.Time = _timeElapsed;
                        File.WriteAllText(_scorePath, JsonUtility.ToJson(data));
                    }
                    else
                    {
                        data.Scores.Add(new TimeScoreEntry(LevelLoader.CurrentLevel, _timeElapsed));
                        File.WriteAllText(_scorePath, JsonUtility.ToJson(data));
                    }
                }
                else
                {
                    data = new TimeScoreList();
                    data.Scores.Add(new TimeScoreEntry(LevelLoader.CurrentLevel, _timeElapsed));
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
    }
}
