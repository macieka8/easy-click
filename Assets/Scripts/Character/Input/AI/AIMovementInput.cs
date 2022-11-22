using System;
using UnityEngine;

namespace EasyClick
{
    public class AIMovementInput : MonoBehaviour, IMovementInput
    {
        public event Action<IInputData> onRotationChanged;
        public event Action<IInputData> onJump;

        ICharacterbody _Body;
        [SerializeField] Transform _Target;

        Vector2 _TargetDir;
        [SerializeField] float _MaxTimeTillJump;
        float _TimeTillLastJump;

        void Awake()
        {
            _Body = GetComponent<ICharacterbody>();
        }

        void Update()
        {
            _TimeTillLastJump += Time.deltaTime;

            if (_Target != null)
            {
                Vector2 targetPosition = _Target.position;
                _TargetDir = targetPosition - _Body.Position;
                _TargetDir.Normalize();

                DecideRotation();
                DecideJump();
            }
        }

        void DecideRotation()
        {
            float angle = Vector2.SignedAngle(Vector2.up, _TargetDir);
            if (angle > 15f)
            {
                RotateLeft();
            }
            else if (angle > -15f)
            {
                RotateCenter();
            }
            else
            {
                RotateRight();
            }
        }

        void DecideJump()
        {
            float dot = Vector2.Dot(_Body.Up, _TargetDir);
            if (dot > 0.9)
            {
                Jump();
            }
            else if (_TimeTillLastJump > _MaxTimeTillJump)
            {
                Jump();
            }
        }

        void RotateLeft()
        {
            onRotationChanged?.Invoke(new MovementInputData(1f));
        }

        void RotateRight()
        {
            onRotationChanged?.Invoke(new MovementInputData(-1f));
        }

        void RotateCenter()
        {
            onRotationChanged?.Invoke(new MovementInputData(0f));
        }

        void Jump()
        {
            _TimeTillLastJump = 0f;
            onJump?.Invoke(new MovementInputData(0f));
        }
    }
}
