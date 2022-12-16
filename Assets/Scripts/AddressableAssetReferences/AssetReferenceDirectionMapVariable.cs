using System;
using UnityEngine.AddressableAssets;

namespace EasyClick
{
    [Serializable]
    public class AssetReferenceDirectionMapVariable : AssetReferenceT<DirectionMapVariable>
    {
        public AssetReferenceDirectionMapVariable(string guid) : base(guid) { }
    }

    [Serializable]
    public class AssetReferenceLoaderDirectionMapVariable
        : AssetReferenceLoader<AssetReferenceDirectionMapVariable, DirectionMapVariable>
    { }
}
