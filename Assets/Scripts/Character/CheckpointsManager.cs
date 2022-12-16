using System.Collections.Generic;
using UnityEngine;

namespace EasyClick
{
    public class CheckpointsManager : MonoBehaviour
    {
        [SerializeField] AssetReferenceLoaderCheckpointsManagerVariable _checkpointsManagerVariableLoader;
        [SerializeField] List<Checkpoint> _checkpoints;

        void Awake()
        {
            _checkpointsManagerVariableLoader.LoadAssetAsync();

            foreach (var checkpoint in _checkpoints)
            {
                checkpoint.OnRespawnableReachedCheckpoint += HandleBodyReachedCheckpoint;
            }

            _checkpointsManagerVariableLoader.Value.Register(this);
        }

        void OnDestroy()
        {
            _checkpointsManagerVariableLoader.Value.Unregister(this);
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