using System;
using UnityEngine;

namespace EasyClick
{
    public class CharacterMovement : MonoBehaviour
    {
        public delegate void CharacterStateChangedDelegate(CharacterState prevState, CharacterState currentState);

        [SerializeField] Attribute _rotateSpeed;
        [SerializeField] Attribute _jumpForce;
        [SerializeField] float _timeBeforeJump;

        ICharacterbody _characterbody;
        IMovementInput _movementInput;

        CharacterState _characterState = CharacterState.Flying;
        float _desiredRotation;
        float _beforeJumpStopwatch;
        float _desiredRotationMultiplier = 1f;

        public Attribute JumpForce => _jumpForce;
        public bool CanJump => _characterState == CharacterState.OnGround;

        public event Action OnJumpPerformed;
        public event CharacterStateChangedDelegate OnStateChanged;

        void Awake()
        {
            _characterbody = GetComponent<ICharacterbody>();
            _movementInput = GetComponent<IMovementInput>();

            _movementInput.onRotationChanged += OnRotate;
            _movementInput.onJump += OnJump;
        }

        void OnDestroy()
        {
            _movementInput.onRotationChanged -= OnRotate;
            _movementInput.onJump -= OnJump;
        }

        void FixedUpdate()
        {
            if (_characterState == CharacterState.OnGround)
            {
                if (_characterbody.Rotation > _desiredRotation * _desiredRotationMultiplier)
                {
                    _characterbody.AddTorque(-_rotateSpeed.Value);
                }
                else if (_characterbody.Rotation < _desiredRotation * _desiredRotationMultiplier)
                {
                    _characterbody.AddTorque(_rotateSpeed.Value);
                }
            }
            else if (_characterState == CharacterState.Jump)
            {
                _characterbody.AddForce(_characterbody.Up * _jumpForce.Value, ForceMode2D.Impulse);
                _characterState = CharacterState.Flying;
                _beforeJumpStopwatch = _timeBeforeJump;
                OnStateChanged?.Invoke(CharacterState.Jump, _characterState);
                OnJumpPerformed?.Invoke();
            }
            else if (_characterState == CharacterState.Flying)
            {
                if (_characterbody.TouchingGround)
                    _beforeJumpStopwatch -= Time.deltaTime;
                else
                    _beforeJumpStopwatch = _timeBeforeJump;

                if (_beforeJumpStopwatch <= 0f)
                {
                    _characterState = CharacterState.OnGround;
                    OnStateChanged?.Invoke(CharacterState.Flying, _characterState);
                }
            }
        }

        void OnRotate(IInputData obj)
        {
            _desiredRotation = obj.ReadValue<float>() * 45f;
        }

        void OnJump(IInputData obj)
        {
            _desiredRotation = 45f * obj.ReadValue<float>();
            if (_characterState == CharacterState.OnGround)
            {
                if (_characterbody.TouchingGround)
                    _characterState = CharacterState.Jump;
                else
                    _characterState = CharacterState.Flying;

                OnStateChanged?.Invoke(CharacterState.OnGround, _characterState);
            }
        }

        public void ReverseControls()
        {
            _desiredRotationMultiplier *= -1f;
        }
    }
}
