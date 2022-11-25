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

        public static event System.Action OnLevelLoaded = delegate { };
        public static event System.Action OnBeforeLevelUnload = delegate { };

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
                OnBeforeLevelUnload?.Invoke();
                SceneManager.UnloadSceneAsync(_CurrentLevel);
            }

            AsyncOperation newSceneHandle = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!newSceneHandle.isDone)
            {
                yield return null;
            }
            yield return new WaitUntil(() => newSceneHandle.isDone);
            _CurrentLevel = sceneName;
            OnLevelLoaded?.Invoke();
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