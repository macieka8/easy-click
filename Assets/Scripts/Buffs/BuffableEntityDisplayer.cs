using UnityEngine;

namespace EasyClick
{
    public class BuffableEntityDisplayer : MonoBehaviour
    {
        [SerializeField] BuffDisplayer _buffDisplayerPrefab;
        [SerializeField] BuffableEntity _buffableEntity;

        void OnEnable()
        {
            _buffableEntity.OnNewBuffAdd += HandleBuffAdded;
        }

        void OnDisable()
        {
            _buffableEntity.OnNewBuffAdd -= HandleBuffAdded;
        }

        void HandleBuffAdded(Buff buff)
        {
            var displayer = Instantiate(_buffDisplayerPrefab, transform);
            if (!(buff is TimedBuff))
            {
                Debug.LogError("Added Buff is not TimedBuff");
            }
            displayer.Setup(buff as TimedBuff);
        }
    }
}
