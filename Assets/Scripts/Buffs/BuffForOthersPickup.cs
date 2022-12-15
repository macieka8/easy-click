using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EasyClick
{
    public class BuffForOthersPickup : MonoBehaviour
    {
        [SerializeField] AssetReference _buffDataAssetReference;
        BuffData _buffData;
        bool _isUsed;

        [SerializeField] AssetReference _racerCollectionAssetReference;
        RacerEntityCollection _racerCollectionInner;
        RacerEntityCollection _racerCollection
        {
            get
            {
                if (_racerCollectionInner == null)
                {
                    var handler = Addressables.LoadAssetAsync<RacerEntityCollection>(_racerCollectionAssetReference);
                    handler.WaitForCompletion();
                    _racerCollectionInner = handler.Result;
                }
                return _racerCollectionInner;
            }
        }

        void Start()
        {
            var handle = Addressables.LoadAssetAsync<BuffData>(_buffDataAssetReference);
            handle.WaitForCompletion();
            _buffData = handle.Result;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (_isUsed) return;
            if (collider.TryGetComponent<RacerEntity>(out var foundRacer))
            {
                foreach (var racer in _racerCollection.Collection)
                {
                    if (racer == foundRacer) continue;
                    racer.BuffableEntity.AddBuff(_buffData.InitializeBuff(racer.gameObject));
                }
                _isUsed = true;
                Destroy(gameObject);
            }
        }
    }
}
