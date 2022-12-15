using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EasyClick
{
    public class BuffPickup : MonoBehaviour
    {
        [SerializeField] AssetReference _buffDataAssetReference;
        BuffData _buffData;
        bool _isUsed;

        void Start()
        {
            var handle = Addressables.LoadAssetAsync<BuffData>(_buffDataAssetReference);
            handle.WaitForCompletion();
            _buffData = handle.Result;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (_isUsed) return;
            if (collider.TryGetComponent<BuffableEntity>(out var buffable))
            {
                buffable.AddBuff(_buffData.InitializeBuff(buffable.gameObject));
                _isUsed = true;
                Destroy(gameObject);
            }
        }
    }
}
