using System.Collections.Generic;
using UnityEngine;

namespace EasyClick
{
    public class CheckpointsManager : MonoBehaviour
    {
        [SerializeField] CheckpointsManagerVariable _variable;
        [SerializeField] List<Checkpoint> _checkpoints;

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