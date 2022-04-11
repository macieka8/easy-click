using UnityEngine;

namespace EasyClick
{
    [RequireComponent(typeof(Softbody))]
    public class CharacterSoftbody : MonoBehaviour, ICharacterbody
    {
        [SerializeField] CircleCollider2D _GroundChecker;
        [SerializeField] LayerMask _WhatIsGround;

        Softbody _Softbody;
        float _GroundCheckerDistanceFromCenter;
        bool _TouchingGround;

        private void Awake()
        {
            _Softbody = GetComponent<Softbody>();
        }

        private void Start()
        {
            _GroundCheckerDistanceFromCenter = Vector2.Distance(_GroundChecker.transform.position, transform.position);
        }

        private void Update()
        {
            var up = _Softbody.UpVector;
            var checkUpPosition = _Softbody.Position + up * _GroundCheckerDistanceFromCenter;
            var checkDownPosition = _Softbody.Position + up * -_GroundCheckerDistanceFromCenter;

            _TouchingGround = 
                (Physics2D.OverlapCircle(checkUpPosition, _GroundChecker.radius, _WhatIsGround)) ||
                (Physics2D.OverlapCircle(checkDownPosition, _GroundChecker.radius, _WhatIsGround));

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
