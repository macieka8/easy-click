using UnityEngine;

namespace EasyClick
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] SpawnerVariable _spawnerVariable;
        [SerializeField] Attribute _rotateSpeed;
        [SerializeField] Attribute _jumpForce;
        [SerializeField] float _TimeBeforeJump;

        ICharacterbody _Characterbody;
        IMovementInput _MovementInput;

        CharacterState _CharacterState = CharacterState.Flying;
        float _DesiredRotation;
        float _BeforeJumpStopwatch;
        float _DesiredRotationMultiplier = 1f;

        public Attribute JumpForce => _jumpForce;

        void Awake()
        {
            _Characterbody = GetComponent<ICharacterbody>();
            _MovementInput = GetComponent<IMovementInput>();

            _MovementInput.onRotationChanged += OnRotate;
            _MovementInput.onJump += OnJump;
        }

        void Start()
        {
            LevelLoader.OnLevelLoaded += LevelLoader_onLevelLoaded;
        }

        void OnDestroy()
        {
            LevelLoader.OnLevelLoaded -= LevelLoader_onLevelLoaded;
            _MovementInput.onRotationChanged -= OnRotate;
            _MovementInput.onJump -= OnJump;
        }

        void FixedUpdate()
        {
            if (_CharacterState == CharacterState.OnGround)
            {
                if (_Characterbody.Rotation > _DesiredRotation * _DesiredRotationMultiplier)
                {
                    _Characterbody.AddTorque(-_rotateSpeed.Value);
                }
                else if (_Characterbody.Rotation < _DesiredRotation * _DesiredRotationMultiplier)
                {
                    _Characterbody.AddTorque(_rotateSpeed.Value);
                }
            }
            else if (_CharacterState == CharacterState.Jump)
            {
                _Characterbody.AddForce(_Characterbody.Up * _jumpForce.Value, ForceMode2D.Impulse);
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

        void LevelLoader_onLevelLoaded()
        {
            _spawnerVariable.Value.Respawn(_Characterbody);
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

        public void ReverseControls()
        {
            _DesiredRotationMultiplier *= -1f;
        }
    }
}
