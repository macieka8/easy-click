using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EasyClick
{
    public class GameCharactersManager : MonoBehaviour
    {
        [SerializeField] AssetLabelReference _playableCharactersAssetLabel;
        [SerializeField] AssetLabelReference _botCharactersAssetLabel;
        public static GameCharactersManager Instance;

        IList<GameObject> _playableCharacters;
        IList<GameObject> _botCharacters;

        public static IList<GameObject> PlayableCharacters { get => Instance._playableCharacters; }
        public static IList<GameObject> BotCharacters { get => Instance._botCharacters; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            if (Instance == this)
            {
                var playerHandle = Addressables.LoadAssetsAsync<GameObject>(
                    _playableCharactersAssetLabel,
                    addressable => { });
                playerHandle.Completed += obj =>
                {
                    if (obj.Status != AsyncOperationStatus.Succeeded)
                        Debug.LogWarning("Could not load playable character assets");

                    _playableCharacters = obj.Result;
                };

                var botHandle = Addressables.LoadAssetsAsync<GameObject>(
                    _botCharactersAssetLabel,
                    addressable => { });
                botHandle.Completed += obj =>
                {
                    if (obj.Status != AsyncOperationStatus.Succeeded)
                        Debug.LogWarning("Could not load bot character assets");

                    _botCharacters = obj.Result;
                };
            }
        }

        public static GameObject GetNextPlayableCharacterPrefab(RacerEntity current)
        {
            var index = (PlayableCharacters.IndexOf(current.Prefab) + 1) % PlayableCharacters.Count;
            return Instance._playableCharacters[index];
        }
    }
}
