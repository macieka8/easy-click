using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyClick
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterRigidbody : MonoBehaviour, ICharacterbody
    {
        Rigidbody2D _rigidbody;
        bool _touchingGround;

        [SerializeField] Transform _groundChecker;
        [SerializeField] ContactFilter2D _contactFilter;
        [SerializeField] List<Collider2D> _ignoredColliders;
        readonly float _checkRadius = 0.4f;

        public Vector2 Position { get => _rigidbody.position; set => _rigidbody.position = value; }
        public float Rotation => _rigidbody.rotation;
        public bool TouchingGround => _touchingGround;
        public Vector2 Up => transform.up;

        Collider2D[] _foundColliders;

        void Awake()
        {
            _foundColliders = new Collider2D[_ignoredColliders.Count + 1];
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            Array.Clear(_foundColliders, 0, _foundColliders.Length);
            Physics2D.OverlapCircle(_groundChecker.position, _checkRadius, _contactFilter, _foundColliders);
            _touchingGround = IgnoreCollisionHelper.CheckIfNotIgnoredColliderExist(_foundColliders, _ignoredColliders);
        }

        void FixedUpdate()
        {
            _rigidbody.rotation = NormalizeAngle(_rigidbody.rotation);
        }

        float NormalizeAngle(float angle)
        {
            if (angle >= 360 || angle <= -360)
                angle %= 360;

            if (angle >= 180)
                angle = -360 + angle;
            if (angle <= -180)
                angle = 360 + angle;

            return angle;
        }

        public void AddForce(Vector2 force, ForceMode2D mode)
        {
            _rigidbody.AddForce(force, mode);
        }

        public void AddTorque(float torque)
        {
            _rigidbody.AddTorque(torque);
        }
    }
}
