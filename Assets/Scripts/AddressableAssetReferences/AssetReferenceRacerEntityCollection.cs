using System;
using UnityEngine.AddressableAssets;

namespace EasyClick
{
    [Serializable]
    public class AssetReferenceRacerEntityCollection : AssetReferenceT<RacerEntityCollection>
    {
        public AssetReferenceRacerEntityCollection(string guid) : base(guid) { }
    }

    [Serializable]
    public class AssetReferenceLoaderRacerEntityCollection
        : AssetReferenceLoader<AssetReferenceRacerEntityCollection, RacerEntityCollection>
    { }
}
