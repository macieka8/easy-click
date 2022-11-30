using UnityEngine;

namespace EasyClick
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterRigidbody : MonoBehaviour, ICharacterbody
    {
        Rigidbody2D _Rigidbody;
        bool _TouchingGround;

        [SerializeField] Transform _GroundChecker;
        [SerializeField] LayerMask _WhatIsGround;
        readonly float _CheckRadius = 0.4f;

        public Vector2 Position { get => _Rigidbody.position; set => _Rigidbody.position = value; }
        public float Rotation => _Rigidbody.rotation;
        public bool TouchingGround => _TouchingGround;
        public Vector2 Up => transform.up;

        void Awake()
        {
            _Rigidbody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            _TouchingGround = Physics2D.OverlapCircle(_GroundChecker.position, _CheckRadius, _WhatIsGround);
        }

        void FixedUpdate()
        {
            _Rigidbody.rotation = NormalizeAngle(_Rigidbody.rotation);
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
            _Rigidbody.AddForce(force, mode);
        }

        public void AddTorque(float torque)
        {
            _Rigidbody.AddTorque(torque);
        }
    }
}
