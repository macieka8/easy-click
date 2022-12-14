using UnityEngine;

namespace EasyClick
{
    public class DirectionMapVariation : MonoBehaviour
    {
        [SerializeField] Transform _targetLocation;
        [SerializeField] float _distanceToInactivitiy;
        Collider2D _collider;

        void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        public bool IsPositionInBounds(Vector2 position)
        {
            return _collider.bounds.Contains(position);
        }

        public Vector2 GetDirection(Vector2 position)
        {
            var direction = (Vector2)_targetLocation.position - position;
            var distanceCheck = (Vector2)_targetLocation.position - (Vector2)transform.position;
            if (distanceCheck.magnitude > _distanceToInactivitiy)
                return Vector2.zero;
            return direction.normalized;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _distanceToInactivitiy);
            if (_targetLocation != null)
                Gizmos.DrawLine(transform.position, _targetLocation.position);
        }
    }
}
