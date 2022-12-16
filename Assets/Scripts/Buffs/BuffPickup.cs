using UnityEngine;

namespace EasyClick
{
    public class BuffPickup : MonoBehaviour
    {
        [SerializeField] AssetReferenceLoaderBuffData _buffDataLoader;
        bool _isUsed;

        void Awake()
        {
            _buffDataLoader.LoadAssetAsync();
        }

        void OnDestroy()
        {
            _buffDataLoader.Release();
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (_isUsed) return;
            if (collider.TryGetComponent<BuffableEntity>(out var buffable))
            {
                buffable.AddBuff(_buffDataLoader.Value.InitializeBuff(buffable.gameObject));
                _isUsed = true;
                Destroy(gameObject);
            }
        }
    }
}
