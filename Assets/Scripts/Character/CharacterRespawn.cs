using System;
using System.Collections;
using UnityEngine;

namespace EasyClick
{
    public interface IRespawnable
    {
        public int CheckpointIndex { get; set; }
    }
    public class CharacterRespawn : MonoBehaviour, IRespawnable
    {
        IRespawnInput _RespawnInput;
        ICharacterbody _Characterbody;

        [SerializeField] CheckpointsManagerVariable _checkpointsManagerVariable;
        [SerializeField] float _TimeToRespawn = 2.0f;

        int _checkpointIndex;

        public event Action<float> OnRespawnStarted;

        public int CheckpointIndex { get => _checkpointIndex; set => _checkpointIndex = value; }

        void Awake()
        {
            _Characterbody = GetComponent<ICharacterbody>();
            _RespawnInput = GetComponent<IRespawnInput>();
            _RespawnInput.onRespawn += HandleRespawnInput;
            LevelLoader.OnBeforeLevelUnload += ResetCheckpoint;
        }

        void Destroy()
        {
            LevelLoader.OnBeforeLevelUnload -= ResetCheckpoint;
        }

        void HandleRespawnInput(IInputData obj)
        {
            OnRespawnStarted?.Invoke(_TimeToRespawn);
            StartCoroutine(MoveBodyToCheckpoint());
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