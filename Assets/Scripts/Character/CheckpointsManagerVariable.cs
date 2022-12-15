using System;
using UnityEngine;

namespace EasyClick
{
    [CreateAssetMenu(menuName = "Variable/CheckpointsManger")]
    public class CheckpointsManagerVariable : ScriptableObject
    {
        CheckpointsManager _checkpointsManager;

        public event Action OnChanged;

        public CheckpointsManager Value => _checkpointsManager;

        public void Register(CheckpointsManager checkpointsManager)
        {
            _checkpointsManager = checkpointsManager;
            Debug.Log($"Chekcpoints Manager Varaible: {Value.name}");
            OnChanged?.Invoke();
        }

        public void Unregister(CheckpointsManager checkpointsManager)
        {
            if (_checkpointsManager == checkpointsManager)
                _checkpointsManager = null;
        }
    }
}