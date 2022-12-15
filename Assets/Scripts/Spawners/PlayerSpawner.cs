using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EasyClick
{
    public class PlayerSpawner : MonoBehaviour, ISpawner, IRespawner
    {
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

        [SerializeField] AssetReference _spawnerVariableAssetReference;
        SpawnerVariable _spawner;
        SpawnerVariable _spawnerVariable
        {
            get
            {
                if (_spawner == null)
                {
                    var handler = Addressables.LoadAssetAsync<SpawnerVariable>(_spawnerVariableAssetReference);
                    handler.WaitForCompletion();
                    _spawner = handler.Result;
                }
                return _spawner;
            }
        }

        void Awake()
        {
            _spawnerVariable.RegisterVariable(this);
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