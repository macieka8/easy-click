using UnityEngine;
using Cinemachine;

namespace EasyClick
{
    public class MainMenuCameraController : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera _Vcam;

        public void SetFollowTarget(Transform transform)
        {
            _Vcam.Follow = transform;
        }
    }
}