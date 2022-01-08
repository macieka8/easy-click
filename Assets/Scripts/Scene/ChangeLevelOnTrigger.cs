using UnityEngine;

namespace EasyClick
{
    public class ChangeLevelOnTrigger : MonoBehaviour
    {
        [SerializeField]
        string _levelName;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                LevelLoader.StartLevel(_levelName);
            }
        }
    }
}