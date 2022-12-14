using System;
using System.Collections;
using UnityEngine;

namespace EasyClick
{
    public class CharacterRespawn : MonoBehaviour, IRespawnable
    {
        [SerializeField] SpawnerVariable _spawnerVariable;
        [SerializeField] CheckpointsManagerVariable _checkpointsManagerVariable;
        [SerializeField] float _TimeToRespawn = 2.0f;

        IRespawnInput _RespawnInput;
        ICharacterbody _Characterbody;

        int _checkpointIndex;
        bool _isRespawning;

        public event Action<float> OnRespawnStarted;

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