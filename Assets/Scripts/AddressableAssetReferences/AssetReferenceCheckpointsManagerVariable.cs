using System;
using UnityEngine.AddressableAssets;

namespace EasyClick
{
    [Serializable]
    public class AssetReferenceCheckpointsManagerVariable : AssetReferenceT<CheckpointsManagerVariable>
    {
        public AssetReferenceCheckpointsManagerVariable(string guid) : base(guid) { }
    }

    [Serializable]
    public class AssetReferenceLoaderCheckpointsManagerVariable
        : AssetReferenceLoader<AssetReferenceCheckpointsManagerVariable, CheckpointsManagerVariable>
    { }
}
