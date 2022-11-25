using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace EasyClick
{
    public class CameraManager : MonoBehaviour
    {
        class CameraStruct
        {
            public CinemachineVirtualCamera virtualCamera;
            public Camera unityCamera;

            public void SetEnable(bool enabled)
            {
                unityCamera.enabled = enabled;
                virtualCamera.enabled = enabled;
            }
        }
        List<CameraStruct> _Cameras;

        [SerializeField] Camera _CameraPrefab;
        [SerializeField] CinemachineVirtualCamera _VirtualCameraPrefab;
        [SerializeField] Transform _CamerasParent;

        int _CurrentLayout;
        List<List<Rect>> _ViewPortRects = new List<List<Rect>>()
        { 
            // One player
            new List<Rect>(){ new Rect(0f, 0f, 1f, 1f) },
            // Two players
            new List<Rect>()
            { 
                new Rect(0f, 0f, 0.5f, 1f),
                new Rect(0.5f, 0f, 0.5f, 1f)
            },
            // Three players
            new List<Rect>()
            {
                new Rect(0f, 0.5f, 0.5f, 0.5f),
                new Rect(0.5f, 0.5f, 0.5f, 0.5f),
                new Rect(0.25f, 0f, 0.5f, 0.5f)
            },
            // Four players
            new List<Rect>()
            {
                new Rect(0f, 0.5f, 0.5f, 0.5f),
                new Rect(0.5f, 0.5f, 0.5f, 0.5f),
                new Rect(0f, 0f, 0.5f, 0.5f),
                new Rect(0.5f, 0f, 0.5f, 0.5f),
            },
        };
        [SerializeField] List<GameObject> _Layouts = new List<GameObject>();

        List<int> _PlayerLayers = new List<int>();

        private void Awake()
        {
            _Cameras = new List<CameraStruct>();

            foreach (var layout in _Layouts)
            {
                layout.SetActive(false);
            }

            _PlayerLayers.Add(LayerMask.NameToLayer("Player 1"));
            _PlayerLayers.Add(LayerMask.NameToLayer("Player 2"));
            _PlayerLayers.Add(LayerMask.NameToLayer("Player 3"));
            _PlayerLayers.Add(LayerMask.NameToLayer("Player 4"));
        }

        private void Start()
        {
            // Add new cameras for every existing player
            foreach (var player in PlayerInput.AllPlayers)
            {
                AddNewCamera();
            }

            // Change layout / camera settings on specified event
            PlayerInput.onPlayerJoined += OnPlayerJoined;
            PlayerInput.onPlayerLeft += OnPlayerLeft;

            LevelLoader.OnLevelLoaded += OnLevelLoaded;
        }

        void OnPlayerJoined(PlayerInput newPlayer)
        {
            _CurrentLayout = PlayerInput.AllPlayers.Count > _ViewPortRects.Count
                ? _ViewPortRects.Count - 1
                : PlayerInput.AllPlayers.Count - 1;
            if (_CurrentLayout >= 0)
            {
                ChangeLayout();
            }
        }

        void OnPlayerLeft(PlayerInput removedPlayer)
        {
            _CurrentLayout = PlayerInput.AllPlayers.Count > _ViewPortRects.Count
                ? _ViewPortRects.Count - 1
                : PlayerInput.AllPlayers.Count - 1;
            if (_CurrentLayout >= 0)
            {
                ChangeLayout();
            }
        }

        private void OnLevelLoaded()
        {
            // Disable layouts
            foreach (var layout in _Layouts)
            {
                layout.SetActive(false);
            }

            // Use this object's cameras if no camera exist in scene
            if (GameObject.Find("Cameras") == null)
            {
                for (int i = 0; i < PlayerInput.AllPlayers.Count; i++)
                {
                    ChangeLayout();
                    EnableCamera(i);

                    // Set Active layout
                    if (_CurrentLayout < _Layouts.Count)
                    {
                        _Layouts[_CurrentLayout].SetActive(true);
                    }
                }
            }
            else
            {
                for (int i = 0; i < PlayerInput.AllPlayers.Count; i++)
                {
                    DisableCamera(i);
                }
            }
        }

        void ChangeLayout()
        {
            // Setup Cameras
            int i;
            for (i = 0; i < _ViewPortRects[_CurrentLayout].Count; i++)
            {
                if (i >= _Cameras.Count)
                {
                    AddNewCamera();
                }

                SetupCamera(i);
            }
        }

        void AddNewCamera()
        {
            CameraStruct newCameras = new CameraStruct()
            {
                unityCamera = Instantiate(_CameraPrefab, _CamerasParent),
                virtualCamera = Instantiate(_VirtualCameraPrefab, _CamerasParent)
            };

            // Set camera layer
            int camerasLayer = LayerMask.NameToLayer($"Player {_Cameras.Count + 1}");
            newCameras.virtualCamera.gameObject.layer = camerasLayer;

            // Set camera culling mask
            int cullingMask = 0;
            foreach (var layer in _PlayerLayers)
            {
                if (layer != camerasLayer)
                {
                    cullingMask |= (1 << layer);
                }
            }
            newCameras.unityCamera.cullingMask = ~cullingMask;

            DisableCamera(newCameras);
            _Cameras.Add(newCameras);
        }

        void SetupCamera(int cameraIndex)
        {
            if (_Cameras[cameraIndex].unityCamera != null && _Cameras[cameraIndex].virtualCamera != null)
            {
                _Cameras[cameraIndex].unityCamera.rect = _ViewPortRects[_CurrentLayout][cameraIndex];
                _Cameras[cameraIndex].virtualCamera.Follow = PlayerInput.AllPlayers[cameraIndex].transform;
            }
        }

        void DisableCamera(int cameraIndex)
        {
            _Cameras[cameraIndex].SetEnable(false);
        }

        void DisableCamera(CameraStruct cameras)
        {
            cameras.SetEnable(false);
        }

        void EnableCamera(int cameraIndex)
        {
            _Cameras[cameraIndex].SetEnable(true);
        }
    }
}
