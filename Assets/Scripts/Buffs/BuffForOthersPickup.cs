using UnityEngine;

namespace EasyClick
{
    public class BuffForOthersPickup : MonoBehaviour
    {
        [SerializeField] AssetReferenceLoaderBuffData _buffDataLoader;
        [SerializeField] AssetReferenceLoaderRacerEntityCollection _racerCollectionLoader;
        bool _isUsed;

        void Awake()
        {
            _buffDataLoader.LoadAssetAsync();
            _racerCollectionLoader.LoadAssetAsync();
        }

        void OnDestroy()
        {
            _buffDataLoader.Release();
            _racerCollectionLoader.Release();
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (_isUsed) return;
            if (collider.TryGetComponent<RacerEntity>(out var foundRacer))
            {
                foreach (var racer in _racerCollectionLoader.Value.Collection)
                {
                    if (racer == foundRacer) continue;
                    racer.BuffableEntity.AddBuff(_buffDataLoader.Value.InitializeBuff(racer.gameObject));
                }
                _isUsed = true;
                Destroy(gameObject);
            }
        }
    }
}
