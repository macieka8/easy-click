using UnityEngine.InputSystem;

namespace EasyClick
{
    public interface IRespawnInput
    {
        public event System.Action<IInputData> onRespawn;
    }
}
