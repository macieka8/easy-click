using System;
using UnityEngine;

namespace EasyClick
{
    public class AIMovementInput : MonoBehaviour, IMovementInput
    {
        [SerializeField] AssetReferenceLoaderDirectionMapVariable _directionMapVariableLoader;
        [SerializeField] float _maxTimeTillJump;

        ICharacterbody _body;
        Transform _transform;

        Vector2 _targetDir;
        float _timeTillLastJump;

        public event Action<IInputData> onRotationChanged;
        public event Action<IInputData> onJump;

        void Awake()
        {
            _directionMapVariableLoader.LoadAssetAsync();

            _transform = transform;
            _body = GetComponent<ICharacterbody>();
        }

        void Update()
        {
            if (_directionMapVariableLoader.Value.Value != null)
            {
                _targetDir = _directionMapVariableLoader.Value.Value.GetDirection(_transform.position);
                if (_targetDir == Vector2.zero) return;
                _timeTillLastJump += Time.deltaTime;

                _targetDir.Normalize();
                DecideRotation();
                DecideJump();
            }
        }

        void DecideRotation()
        {
            float angle = Vector2.SignedAngle(Vector2.up, _targetDir);
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
            float dot = Vector2.Dot(_body.Up, _targetDir);
            if (dot > 0.9)
            {
                Jump();
            }
            else if (_timeTillLastJump > _maxTimeTillJump)
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
            _timeTillLastJump = 0f;
            onJump?.Invoke(new MovementInputData(0f));
        }
    }
}
