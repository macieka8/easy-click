using System.Collections;
using UnityEngine;

namespace EasyClick
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BatMovement : MonoBehaviour
    {
        [SerializeField] AssetReferenceLoaderRacerEntityCollection _racerCollectionLoader;
        [SerializeField] float _pushForce;
        [SerializeField] float _jumpForce;
        [SerializeField] float _secondsBatweenPushes;
        [SerializeField] float _metersAboveGround;
        [SerializeField] float _aggresionRange;
        [SerializeField] LayerMask _groundLayer;

        Rigidbody2D _rigidbody;
        float _aggresionRangeSqr;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _aggresionRangeSqr = _aggresionRange * _aggresionRange;

            _racerCollectionLoader.LoadAssetAsync();
        }

        void Start()
        {
            StartCoroutine(UpdateMovement());
        }

        void Update()
        {
            var hit = Physics2D.Raycast(_rigidbody.position, -transform.up, _metersAboveGround, _groundLayer);
            if (hit)
            {
                _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            }
        }

        void OnDestroy()
        {
            _racerCollectionLoader.Release();
        }

        IEnumerator UpdateMovement()
        {
            FlyTorwardDestination();

            yield return new WaitForSeconds(_secondsBatweenPushes);
            yield return UpdateMovement();
        }

        void FlyTorwardDestination()
        {
            if (TryFindClosestRacer(out var foundRacer))
            {
                Vector2 dirToDest = foundRacer.transform.position - transform.position;
                dirToDest.Normalize();

                _rigidbody.AddForce(dirToDest * _pushForce, ForceMode2D.Impulse);
            }
        }

        bool TryFindClosestRacer(out RacerEntity foundRacer)
        {
            RacerEntity closestRacer = null;
            var minSqrMagnitude = float.MaxValue;
            foreach (var racer in _racerCollectionLoader.Value.Collection)
            {
                var sqrMagnitude = (racer.transform.position - transform.position).sqrMagnitude;
                if (sqrMagnitude > _aggresionRangeSqr) continue;
                if (sqrMagnitude < minSqrMagnitude)
                {
                    minSqrMagnitude = sqrMagnitude;
                    closestRacer = racer;
                }
            }

            foundRacer = closestRacer;
            return minSqrMagnitude != float.MaxValue;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _aggresionRange);
        }
    }
}