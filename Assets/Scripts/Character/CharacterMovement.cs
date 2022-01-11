using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyClick
{
    public class CharacterMovement : MonoBehaviour
    {
        ICharacterbody _Characterbody;
        IMovementInput _MovementInput;

        [SerializeField] float _RotateSpeed = 5f;
        [SerializeField] float _JumpForce = 4f;

        CharacterState _CharacterState = CharacterState.Flying;
        float _DesiredRotation = 0f;

        [SerializeField] float _TimeBeforeJump;
        float _BeforeJumpStopwatch;

        void Awake()
        {
            _Characterbody = GetComponent<ICharacterbody>();
            _MovementInput = GetComponent<IMovementInput>();

            _MovementInput.onRotationChanged += OnRotate;
            _MovementInput.onJump += OnJump;
        }

        void Start()
        {
            LevelLoader.onLevelLoaded += LevelLoader_onLevelLoaded;
        }

        private void OnDestroy()
        {
            LevelLoader.onLevelLoaded -= LevelLoader_onLevelLoaded;
            _MovementInput.onRotationChanged -= OnRotate;
            _MovementInput.onJump -= OnJump;
        }

        void FixedUpdate()
        {
            if (_CharacterState == CharacterState.OnGround)
            {
                if (_Characterbody.Rotation > _DesiredRotation)
                {
                    _Characterbody.AddTorque(-_RotateSpeed);
                }
                else if (_Characterbody.Rotation < _DesiredRotation)
                {
                    _Characterbody.AddTorque(_RotateSpeed);
                }
            }
            else if (_CharacterState == CharacterState.Jump)
            {
                _Characterbody.AddForce(_Characterbody.Up * _JumpForce, ForceMode2D.Impulse);
                _CharacterState = CharacterState.Flying;
                _BeforeJumpStopwatch = _TimeBeforeJump;
            }
            else if (_CharacterState == CharacterState.Flying)
            {
                if (_Characterbody.TouchingGround)
                    _BeforeJumpStopwatch -= Time.deltaTime;
                else
                    _BeforeJumpStopwatch = _TimeBeforeJump;

                if (_BeforeJumpStopwatch <= 0f)
                    _CharacterState = CharacterState.OnGround;
            }
        }

        private void LevelLoader_onLevelLoaded()
        {
            var spawner = PlayerSpawner.Spawner;
            spawner?.Respawn(_Characterbody);
        }

        void OnRotate(IInputData obj)
        {
            _DesiredRotation = obj.ReadValue<float>() * 45f;
        }

        void OnJump(IInputData obj)
        {
            _DesiredRotation = 45f * obj.ReadValue<float>();
            if (_CharacterState == CharacterState.OnGround)
            {
                if (_Characterbody.TouchingGround)
                    _CharacterState = CharacterState.Jump;
            }
        }
    }
}
