using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyClick
{
    public class CharacterRespawn : MonoBehaviour
    {
        IRespawnInput _RespawnInput;
        ICharacterbody _Characterbody;

        [SerializeField] List<Collider2D> _Checkpoints;
        int _CurrentCheckpointId = 0;

        [SerializeField] float _TimeToRespawn = 2.0f;


        private void Awake()
        {
            _Characterbody = GetComponent<ICharacterbody>();
            _RespawnInput = GetComponent<IRespawnInput>();
            _RespawnInput.onRespawn += OnRespawn;
        }

        private void Start()
        {
            var checkpoints = GameObject.Find("Gameplay Logic").transform.Find("Checkpoints").transform;
            int checkpointCount = checkpoints.childCount;
            for (int i = 0; i < checkpointCount; i++)
            {
                _Checkpoints.Add(checkpoints.GetChild(i).GetComponent<Collider2D>());
            }

            LevelLoader.onLevelLoaded += LevelLoader_onLevelLoaded;
        }

        private void OnDestroy()
        {
            LevelLoader.onLevelLoaded -= LevelLoader_onLevelLoaded;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Checkpoint"))
            {
                if (_CurrentCheckpointId + 1 < _Checkpoints.Count)
                {
                    if (collider == _Checkpoints[_CurrentCheckpointId + 1])
                    {
                        _CurrentCheckpointId++;
                    }
                }
            }
        }

        private void OnRespawn(IInputData obj)
        {
            if (obj.ReadValue<bool>())
            {
                StartCoroutine(MoveBodyToCheckpoint());
            }
        }
        IEnumerator MoveBodyToCheckpoint()
        {
            yield return new WaitForSeconds(_TimeToRespawn);
            _Characterbody.Position = _Checkpoints[_CurrentCheckpointId].transform.position;
        }

        private void LevelLoader_onLevelLoaded()
        {
            _Checkpoints.Clear();
            _CurrentCheckpointId = 0;

            var checkpoints = GameObject.Find("Gameplay Logic")?.transform.Find("Checkpoints")?.transform;
            if (checkpoints)
            {
                int checkpointCount = checkpoints.childCount;
                for (int i = 0; i < checkpointCount; i++)
                {
                    _Checkpoints.Add(checkpoints.GetChild(i).GetComponent<Collider2D>());
                }
            }
        }
    }
}