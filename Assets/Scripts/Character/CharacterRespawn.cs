using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EasyClick
{
    public class CharacterRespawn : MonoBehaviour, IRespawnable
    {
        [SerializeField] float _TimeToRespawn = 2.0f;

        IRespawnInput _RespawnInput;
        ICharacterbody _Characterbody;

        int _checkpointIndex;
        bool _isRespawning;

        public event Action<float> OnRespawnStarted;

        [SerializeField] AssetReference _spawnerVariableAssetReference;
        SpawnerVariable _spawner;
        SpawnerVariable _spawnerVariable
        {
            get
            {
                if (_spawner == null)
                {
                    var handler = Addressables.LoadAssetAsync<SpawnerVariable>(_spawnerVariableAssetReference);
                    handler.WaitForCompletion();
                    _spawner = handler.Result;
                }
                return _spawner;
            }
        }

        [SerializeField] AssetReference _checkpointsManagerVariableAssetReference;
        CheckpointsManagerVariable _checkpointManager;
        CheckpointsManagerVariable _checkpointsManagerVariable
        {
            get
            {
                if (_checkpointManager == null)
                {
                    var handler = Addressables.LoadAssetAsync<CheckpointsManagerVariable>(_checkpointsManagerVariableAssetReference);
                    handler.WaitForCompletion();
                    _checkpointManager = handler.Result;
                }
                return _checkpointManager;
            }
        }

        public int CheckpointIndex { get => _checkpointIndex; set => _checkpointIndex = value; }

        void Awake()
        {
            _Characterbody = GetComponent<ICharacterbody>();
            _RespawnInput = GetComponent<IRespawnInput>();
            _RespawnInput.onRespawn += HandleRespawnInput;
            LevelLoader.OnBeforeLevelUnload += ResetCheckpoint;
            LevelLoader.OnLevelLoaded += HandleLevelLoaded;
        }

        void OnEnable()
        {
            _isRespawning = false;
        }

        void OnDestroy()
        {
            _RespawnInput.onRespawn -= HandleRespawnInput;
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
            Debug.Log($"Respawn Character is null: {_spawnerVariable.Value == null}");
            _spawnerVariable.Value.Respawn(_Characterbody);
        }

        IEnumerator MoveBodyToCheckpoint()
        {
            _isRespawning = true;
            yield return new WaitForSeconds(_TimeToRespawn);
            _Characterbody.Position = _checkpointsManagerVariable.Value.GetCheckpointPosition(this);
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