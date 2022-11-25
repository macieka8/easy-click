using UnityEngine;

namespace EasyClick
{
    public class BuffForOthersPickup : MonoBehaviour
    {
        [SerializeField] RacerEntityCollection _racerCollection;
        [SerializeField] BuffData _buffData;
        bool _isUsed;

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
