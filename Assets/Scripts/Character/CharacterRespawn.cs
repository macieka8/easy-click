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

        void OnDestroy()
        {
            _RespawnInput.onRespawn -= HandleRespawnInput;
            LevelLoader.OnBeforeLevelUnload -= ResetCheckpoint;
            LevelLoader.OnLevelLoaded -= HandleLevelLoaded;
        }

        void HandleRespawnInput(IInputData obj)
        {
            OnRespawnStarted?.Invoke(_TimeToRespawn);
            StartCoroutine(MoveBodyToCheckpoint());
        }

        void HandleLevelLoaded()
        {
            _spawnerVariable.Value.Respawn(_Characterbody);
        }

        IEnumerator MoveBodyToCheckpoint()
        {
            yield return new WaitForSeconds(_TimeToRespawn);
            _Characterbody.Position = _checkpointsManagerVariable.Value.GetCheckpointPosition(this);
        }

        void ResetCheckpoint()
        {
            _checkpointIndex = 0;
        }
    }
}