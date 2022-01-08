using UnityEngine;

namespace EasyClick
{
    public interface IMovementInput
    {
        public event System.Action<IInputData> onRotationChanged;
        public event System.Action<IInputData> onJump;
    }
}
