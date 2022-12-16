using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EasyClick
{
    public class AssetReferenceLoader<TObjectAssetReference, TObject>
        where TObjectAssetReference : AssetReference
        where TObject : Object
    {
        [SerializeField] TObjectAssetReference _assetReference;
        TObject _asset;
        AsyncOperationHandle<TObject> _handle;

        bool _isLoaded;

        public TObject Value
        {
            get
            {
                if (!_isLoaded)
                {
                    _handle.WaitForCompletion();
                }
                return _asset;
            }
        }

        public void LoadAssetAsync()
        {
            _handle = Addressables.LoadAssetAsync<TObject>(_assetReference);
            _handle.Completed += _ =>
            {
                _asset = _handle.Result;
                _isLoaded = true;
            };
        }

        public void Release()
        {
            Addressables.Release(_handle);
        }
    }
}