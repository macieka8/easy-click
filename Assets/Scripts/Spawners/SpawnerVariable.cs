using UnityEngine;

namespace EasyClick
{
    [CreateAssetMenu(menuName = "Variable/Spawner")]
    public class SpawnerVariable : ScriptableObject
    {
        PlayerSpawner _spawner;
        public PlayerSpawner Value => _spawner;

        public void RegisterVariable(PlayerSpawner spawner)
        {
            _spawner = spawner;
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