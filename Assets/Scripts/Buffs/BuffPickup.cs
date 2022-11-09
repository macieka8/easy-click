using UnityEngine;

namespace EasyClick
{
    public class BuffPickup : MonoBehaviour
    {
        [SerializeField] BuffData _buffData;
        bool _isUsed;

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
