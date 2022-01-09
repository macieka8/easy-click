using UnityEngine;
using System;

namespace EasyClick
{
    public class ChangeLevelOnTrigger : MonoBehaviour
    {
        [SerializeField]
        string _levelName;

        public event Action onLevelFinished = delegate { };

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                onLevelFinished?.Invoke();
                LevelLoader.StartLevel(_levelName);
            }
        }
    }
}