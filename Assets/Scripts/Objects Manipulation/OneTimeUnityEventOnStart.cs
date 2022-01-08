using UnityEngine;
using UnityEngine.Events;

namespace EasyClick
{
    public class OneTimeUnityEventOnStart : MonoBehaviour
    {
        static bool _Finished = false;
        [SerializeField] UnityEvent _Event;

        void Start()
        {
            if (!_Finished)
            {
                _Event?.Invoke();
                _Finished = true;
            }
            Destroy(gameObject);
        }
    }
}
