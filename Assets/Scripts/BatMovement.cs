using System.Collections;
using UnityEngine;

namespace EasyClick
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BatMovement : MonoBehaviour
    {
        [SerializeField] float _PushForce;
        [SerializeField] float _PushRotation;
        [SerializeField] float _SecondsBatweenPushes;
        [SerializeField] float _MetersAboveGround;
        [SerializeField] LayerMask _GroundLayer;

        [SerializeField] Transform _Destination;

        Rigidbody2D _Rigidbody;

        private void Awake()
        {
            _Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            StartCoroutine(UpdateMovement());
        }

        private void Update()
        {
            var hit = Physics2D.Raycast(_Rigidbody.position, -transform.up, _MetersAboveGround, _GroundLayer);
            if (hit)
            {
                _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, _PushForce);
            }

        }

        IEnumerator UpdateMovement()
        {
            FlyTorwardDestination();

            yield return new WaitForSeconds(_SecondsBatweenPushes);
            yield return UpdateMovement();
        }


        void FlyTorwardDestination()
        {
            Vector2 dirToDest = _Destination.position - transform.position;
            dirToDest.Normalize();

            _Rigidbody.AddForce(dirToDest * _PushForce, ForceMode2D.Impulse);
        }
    }
}