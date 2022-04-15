using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EasyClick
{
    public class GameCharactersManager : MonoBehaviour
    {
        [SerializeField] AssetLabelReference _playableCharactersAssetLabel;
        public static GameCharactersManager Instance;

        IList<GameObject> _PlayableCharacters;

        public static IList<GameObject> PlayableCharacters { get => Instance._PlayableCharacters; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            if (Instance == this)
            {
                var handle = Addressables.LoadAssetsAsync<GameObject>(
                    _playableCharactersAssetLabel,
                    addressable => { });
                handle.Completed += obj =>
                {
                    if (obj.Status != AsyncOperationStatus.Succeeded)
                        Debug.LogWarning("Could not load assets");

                    _PlayableCharacters = obj.Result;
                };
            }
        }
    }
}
