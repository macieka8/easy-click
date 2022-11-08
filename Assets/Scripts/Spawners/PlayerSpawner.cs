using UnityEngine;

namespace EasyClick
{
    public class PlayerSpawner : MonoBehaviour, ISpawner, IRespawner
    {
        [SerializeField] SpawnerVariable _spawnerVariable;
        [SerializeField] Transform[] _SpawnLocations;

        int _CurrentSpawnLocationIndex;
        int CurrentSpawnLocationIndex
        {
            get
            {
                if (_CurrentSpawnLocationIndex >= _SpawnLocations.Length)
                {
                    _CurrentSpawnLocationIndex = 0;
                }
                return _CurrentSpawnLocationIndex++;
            }
        }

        void Awake()
        {
            _spawnerVariable.RegisterVariable(this);
        }

        public void Spawn()
        {
            var player = Instantiate(GameCharactersManager.PlayableCharacters[0]);
            Respawn(player.GetComponent<IBody>());
        }

        public void Respawn(IBody playerBody)
        {
            playerBody.Position = _SpawnLocations[CurrentSpawnLocationIndex].position;
        }
    }
}