using UnityEngine;
using UnityEngine.Events;

namespace EasyClick
{
    public class UnityEventOnCharacterMovement : MonoBehaviour
    {
        [Header("Unity Events")]
        [SerializeField] UnityEvent _onJumpEvent;
        [SerializeField] BuffData _buff;
        [SerializeField] UnityEvent _onJumpWithBuffEvent;
        [SerializeField] UnityEvent _onFlyingStateExitEvent;

        [Header("Character References")]
        [SerializeField] CharacterMovement _characterMovement;
        [SerializeField] BuffableEntity _buffableEntity;

        void OnEnable()
        {
            _characterMovement.OnJumpPerformed += HandleJumpPerformed;
            _characterMovement.OnStateChanged += HandleStateChanged;
        }

        void OnDisable()
        {
            _characterMovement.OnJumpPerformed -= HandleJumpPerformed;
            _characterMovement.OnStateChanged -= HandleStateChanged;
        }

        void HandleJumpPerformed()
        {
            _onJumpEvent.Invoke();
            if (_buffableEntity.Contains(_buff))
            {
                _onJumpWithBuffEvent.Invoke();
            }
        }

        void HandleStateChanged(CharacterState prevState, CharacterState currentState)
        {
            if (prevState == CharacterState.Flying && currentState != CharacterState.Flying)
            {
                _onFlyingStateExitEvent.Invoke();
            }
        }
    }
}
