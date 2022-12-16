using System;
using System.Collections;
using UnityEngine;

namespace EasyClick
{
    public class CharacterRespawn : MonoBehaviour, IRespawnable
    {
        [SerializeField] AssetReferenceLoaderSpawnerVariable _spawnerVariableLoader;
        [SerializeField] AssetReferenceLoaderCheckpointsManagerVariable _checkpointsManagerVariableLoader;
        [SerializeField] float _TimeToRespawn = 2.0f;

        IRespawnInput _respawnInput;
        ICharacterbody _characterbody;

        int _checkpointIndex;
        bool _isRespawning;

        public event Action<float> OnRespawnStarted;

        public int CheckpointIndex { get => _checkpointIndex; set => _checkpointIndex = value; }

        void Awake()
        {
            _spawnerVariableLoader.LoadAssetAsync();
            _checkpointsManagerVariableLoader.LoadAssetAsync();

            _characterbody = GetComponent<ICharacterbody>();
            _respawnInput = GetComponent<IRespawnInput>();

            _respawnInput.onRespawn += HandleRespawnInput;
            LevelLoader.OnBeforeLevelUnload += ResetCheckpoint;
            LevelLoader.OnLevelLoaded += HandleLevelLoaded;
        }

        void OnEnable()
        {
            _isRespawning = false;
        }

        void OnDestroy()
        {
            _spawnerVariableLoader.Release();
            _checkpointsManagerVariableLoader.Release();

            _respawnInput.onRespawn -= HandleRespawnInput;
            LevelLoader.OnBeforeLevelUnload -= ResetCheckpoint;
            LevelLoader.OnLevelLoaded -= HandleLevelLoaded;
        }

        void HandleRespawnInput(IInputData obj = null)
        {
            if (_isRespawning) return;
            OnRespawnStarted?.Invoke(_TimeToRespawn);
            StartCoroutine(MoveBodyToCheckpoint());
        }

        void HandleLevelLoaded()
        {
            _spawnerVariableLoader.Value.Value.Respawn(_characterbody);
        }

        IEnumerator MoveBodyToCheckpoint()
        {
            _isRespawning = true;
            yield return new WaitForSeconds(_TimeToRespawn);
            _characterbody.Position = _checkpointsManagerVariableLoader.Value.Value.GetCheckpointPosition(this);
            _isRespawning = false;
        }

        void ResetCheckpoint()
        {
            _checkpointIndex = 0;
        }

        public void ForceRespawn()
        {
            HandleRespawnInput();
        }
    }
}