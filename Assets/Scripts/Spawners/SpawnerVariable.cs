using UnityEngine;

namespace EasyClick
{
    [CreateAssetMenu(menuName = "Variable/Spawner")]
    public class SpawnerVariable : ScriptableObject
    {
        PlayerSpawner _spawner;
        public PlayerSpawner Value => _spawner;

        void OnEnable()
        {
            Debug.Log("SpawnerVariable OnEnable");
        }

        void OnDisable()
        {
            Debug.Log("SpawnerVariable OnDisable");
        }

        public void RegisterVariable(PlayerSpawner spawner)
        {
            _spawner = spawner;
            Debug.Log($"Spawner Varaible: {Value.name}");
        }

        public void UnregisterVariable(PlayerSpawner spawner)
        {
            if (_spawner == spawner)
            {
                _spawner = null;
            }
        }
    }
}