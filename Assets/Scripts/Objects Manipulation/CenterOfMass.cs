using UnityEngine;

namespace EasyClick
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CenterOfMass : MonoBehaviour
    {
        Rigidbody2D _Rigidbody;
        [SerializeField] Vector2 _MassCenter;

        void Start()
        {
            _Rigidbody = GetComponent<Rigidbody2D>();
            _Rigidbody.centerOfMass = _MassCenter;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + transform.rotation * _MassCenter, 0.2f);
        }
    }
}