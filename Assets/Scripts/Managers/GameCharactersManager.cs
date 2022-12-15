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

        AsyncOperationHandle<IList<GameObject>> _playableCharactersHandler;
        AsyncOperationHandle<IList<GameObject>> _botCharactersHandler;

        public static IList<GameObject> PlayableCharacters
        {
             get
             {
                if (Instance._playableCharacters == null)
                {
                    Instance._playableCharactersHandler.WaitForCompletion();
                }
                return Instance._playableCharacters;
             }
        }
        public static IList<GameObject> BotCharacters
        {
            get
            {
                if (Instance._botCharacters == null)
                {
                    Instance._botCharactersHandler.WaitForCompletion();
                }
                return Instance._botCharacters;
            }
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                _playableCharactersHandler = Addressables.LoadAssetsAsync<GameObject>(
                    _playableCharactersAssetLabel,
                    addressable => { });
                _playableCharactersHandler.Completed += obj =>
                {
                    if (obj.Status != AsyncOperationStatus.Succeeded)
                        Debug.LogWarning("Could not load playable character assets");

                    _playableCharacters = obj.Result;
                };

                _botCharactersHandler = Addressables.LoadAssetsAsync<GameObject>(
                    _botCharactersAssetLabel,
                    addressable => { });
                _botCharactersHandler.Completed += obj =>
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
