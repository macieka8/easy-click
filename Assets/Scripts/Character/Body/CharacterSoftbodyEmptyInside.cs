using UnityEngine;

namespace EasyClick
{
    [RequireComponent(typeof(SoftbodyEmptyInside))]
    public class CharacterSoftbodyEmptyInside : MonoBehaviour, ICharacterbody
    {
        SoftbodyEmptyInside _Softbody;
        bool _TouchingGround;
        [SerializeField] LayerMask _WhatIsGround;

        private void Awake()
        {
            _Softbody = GetComponent<SoftbodyEmptyInside>();
        }

        private void Update()
        {
            _TouchingGround = Physics2D.OverlapCircle(
                _Softbody.Position + _Softbody.UpVector * _Softbody.Height * 0.5f,
                _Softbody.Width * 0.51f,
                _WhatIsGround) ||
            Physics2D.OverlapCircle(
                _Softbody.Position - _Softbody.UpVector * _Softbody.Height * 0.5f,
                _Softbody.Width * 0.51f,
                _WhatIsGround
            );
        }

        public Vector2 Position { get => _Softbody.Position; set => _Softbody.Position = value; }

        public float Rotation => _Softbody.Rotation;

        public bool TouchingGround => _TouchingGround;

        public Vector2 Up => _Softbody.UpVector;

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
