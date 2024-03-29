using UnityEngine;

namespace EasyClick
{
    public class PlayerSpawner : MonoBehaviour, ISpawner, IRespawner
    {
        [SerializeField] AssetReferenceLoaderSpawnerVariable _spawnerVariableLoader;
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
            _spawnerVariableLoader.LoadAssetAsync();
            _spawnerVariableLoader.Value.RegisterVariable(this);
        }

        void OnDestroy()
        {
            _spawnerVariableLoader.Value.UnregisterVariable(this);
            _spawnerVariableLoader.Release();
        }

        public RacerEntity Spawn(bool isPlayer)
        {
            RacerEntity racer;
            if (isPlayer)
                racer = Instantiate(GameCharactersManager.PlayableCharacters[0]).GetComponent<RacerEntity>();
            else
                racer = Instantiate(GameCharactersManager.BotCharacters[0]).GetComponent<RacerEntity>();

            Respawn(racer.Body);
            return racer;
        }

        public void SpawnPlayer()
        {
            Spawn(true);
        }

        public void Respawn(IBody playerBody)
        {
            playerBody.TeleportTo(_SpawnLocations[CurrentSpawnLocationIndex].position);
        }
    }
}