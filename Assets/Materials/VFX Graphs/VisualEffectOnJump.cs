using UnityEngine;
using UnityEngine.VFX;

namespace EasyClick
{
    public class VisualEffectOnJump : MonoBehaviour
    {
        [SerializeField] VisualEffect _visualEffect;
        [SerializeField] CharacterMovement _characterMovement;

        void Start()
        {
            _characterMovement.OnJumpPerformed += HandleJumpPerformed;
        }

        void HandleJumpPerformed()
        {
            _visualEffect.Play();
        }
    }
}
