using System;
using UnityEngine.AddressableAssets;

namespace EasyClick
{
    [Serializable]
    public class AssetReferenceBuffData : AssetReferenceT<BuffData>
    {
        public AssetReferenceBuffData(string guid) : base(guid) { }
    }

    [Serializable]
    public class AssetReferenceLoaderBuffData
        : AssetReferenceLoader<AssetReferenceBuffData, BuffData>
    { }
}
