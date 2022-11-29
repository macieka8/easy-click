using System;
using UnityEngine;

namespace EasyClick
{
    public class Checkpoint : MonoBehaviour
    {
        public event Action<IRespawnable, Checkpoint> OnRespawnableReachedCheckpoint;
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<IRespawnable>(out var respawnable))
            {
                OnRespawnableReachedCheckpoint?.Invoke(respawnable, this);
            }
        }
    }
}