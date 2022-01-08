using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyClick
{
    public class LevelLoader : MonoBehaviour
    {
        public static LevelLoader Instance;
        string _CurrentLevel = "";
        public static string CurrentLevel { get => Instance._CurrentLevel; }

        public static event System.Action onLevelLoaded = delegate { };
        public static event System.Action onLevelUnload = delegate { };

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                StartLevel("Menu");
            }
        }

        IEnumerator LoadLevel(string sceneName)
        {
            if (_CurrentLevel != "")
            {
                onLevelUnload?.Invoke();
                SceneManager.UnloadSceneAsync(_CurrentLevel);
            }

            AsyncOperation asyncNew = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncNew.isDone)
            {
                yield return null;
            }
            _CurrentLevel = sceneName;
            onLevelLoaded?.Invoke();
        }

        public static void StartLevel(string sceneName)
        {
            Instance.StartCoroutine(Instance.LoadLevel(sceneName));
        }

        public static void StartLevel(int sceneId)
        {
            Instance.StartCoroutine(
                Instance.LoadLevel(
                    SceneManager.GetSceneByBuildIndex(sceneId).name
                    )
                );
        }
    }
}