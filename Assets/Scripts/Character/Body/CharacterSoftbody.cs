using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyClick
{
    [RequireComponent(typeof(Softbody))]
    public class CharacterSoftbody : MonoBehaviour, ICharacterbody
    {
        [SerializeField] CircleCollider2D _groundChecker;
        [SerializeField] ContactFilter2D _contactFilter;
        [SerializeField] List<Collider2D> _ignoredColliders;

        Softbody _Softbody;
        float _GroundCheckerDistanceFromCenter;
        bool _touchingGround;

        Collider2D[] _foundColliders;

        public Vector2 Position { get => _Softbody.Position; set => _Softbody.Position = value; }
        public float Rotation => _Softbody.Rotation;
        public bool TouchingGround => _touchingGround;
        public Vector2 Up => _Softbody.UpVector;

        void Awake()
        {
            _foundColliders = new Collider2D[_ignoredColliders.Count + 1];
            _Softbody = GetComponent<Softbody>();
        }

        void Start()
        {
            _GroundCheckerDistanceFromCenter = Vector2.Distance(_groundChecker.transform.position, transform.position);
        }

        void Update()
        {
            var up = _Softbody.UpVector;
            var checkUpPosition = _Softbody.Position + (up * _GroundCheckerDistanceFromCenter);
            var checkDownPosition = _Softbody.Position + (up * -_GroundCheckerDistanceFromCenter);

            Array.Clear(_foundColliders, 0, _foundColliders.Length);
            Physics2D.OverlapCircle(checkUpPosition, _groundChecker.radius, _contactFilter, _foundColliders);
            _touchingGround = IgnoreCollisionHelper.CheckIfNotIgnoredColliderExist(_foundColliders, _ignoredColliders);

            if (_touchingGround) return;

            Array.Clear(_foundColliders, 0, _foundColliders.Length);
            Physics2D.OverlapCircle(checkDownPosition, _groundChecker.radius, _contactFilter, _foundColliders);
            _touchingGround = IgnoreCollisionHelper.CheckIfNotIgnoredColliderExist(_foundColliders, _ignoredColliders);

        }

        public void AddForce(Vector2 force, ForceMode2D mode)
        {
            _Softbody.AddForce(force, mode);
        }

        public void AddTorque(float torque)
        {
            _Softbody.AddTorque(torque * Time.deltaTime);
        }
    }
}
