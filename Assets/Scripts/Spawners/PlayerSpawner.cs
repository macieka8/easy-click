using UnityEngine;

namespace EasyClick
{
    public class PlayerSpawner : MonoBehaviour, ISpawner, IRespawner
    {
        [SerializeField] Transform[] _SpawnLocations;

        int _CurrentSpawnLocationIndex = 0;
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