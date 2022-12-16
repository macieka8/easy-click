using System;
using UnityEngine.AddressableAssets;

namespace EasyClick
{
    [Serializable]
    public class AssetReferenceSpawnerVariable : AssetReferenceT<SpawnerVariable>
    {
        public AssetReferenceSpawnerVariable(string guid) : base(guid) { }
    }

    [Serializable]
    public class AssetReferenceLoaderSpawnerVariable
        : AssetReferenceLoader<AssetReferenceSpawnerVariable, SpawnerVariable>
    { }
}
