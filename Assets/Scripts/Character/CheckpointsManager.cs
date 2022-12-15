using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EasyClick
{
    public class CheckpointsManager : MonoBehaviour
    {
        [SerializeField] List<Checkpoint> _checkpoints;

        [SerializeField] AssetReference _checkpointsManagerVariableAssetReference;
        CheckpointsManagerVariable _checkpointManager;
        CheckpointsManagerVariable _variable
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
        
        void Awake()
        {
            foreach (var checkpoint in _checkpoints)
            {
                checkpoint.OnRespawnableReachedCheckpoint += HandleBodyReachedCheckpoint;
            }

            _variable.Register(this);
        }

        void OnDestroy()
        {
            _variable.Unregister(this);
        }

        public Vector3 GetCheckpointPosition(IRespawnable respawnable)
        {
            return _checkpoints[respawnable.CheckpointIndex].transform.position;
        }

        void HandleBodyReachedCheckpoint(IRespawnable respawnable, Checkpoint checkpoint)
        {
            var newCheckpointIndex = _checkpoints.IndexOf(checkpoint);
            if (respawnable.CheckpointIndex < newCheckpointIndex)
                respawnable.CheckpointIndex = newCheckpointIndex;
        }
    }
}