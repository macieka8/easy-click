using UnityEngine;

namespace EasyClick
{
    public class ChangeEyeColor : MonoBehaviour
    {
        [SerializeField] CharacterMovement _movement;
        [SerializeField] SpriteRenderer _renderer;

        [SerializeField] Color _canJumpColor;
        [SerializeField] Color _canNotJumpColor;

        void Update()
        {
            if (_movement.CanJump && _renderer.color == _canNotJumpColor)
                _renderer.color = _canJumpColor;
            else if (!_movement.CanJump && _renderer.color == _canJumpColor)
                _renderer.color = _canNotJumpColor;
        }
    }
}
