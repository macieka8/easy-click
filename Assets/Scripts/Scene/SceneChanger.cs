using UnityEngine;

namespace EasyClick
{
    public class SceneChanger : MonoBehaviour
    {
        public void ChangeScene(string sceneName)
        {
            LevelLoader.StartLevel(sceneName);
        }
    }
}