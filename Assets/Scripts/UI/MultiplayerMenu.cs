using System.Collections.Generic;
using UnityEngine;

namespace EasyClick
{
    public class MultiplayerMenu : MonoBehaviour
    {
        [SerializeField] RacerEntityCollection _racerCollection;
        [SerializeField] SpawnerVariable _spawnerVariable;
        [SerializeField] PlayerControlsSave _PlayerSaver;

        [SerializeField] RacerBindingsEntry _HotKeyPrefab;
        [SerializeField] Transform _HotKeysParent;

        List<RacerBindingsEntry> _PlayerBindingEntries = new List<RacerBindingsEntry>();

        void Start()
        {
            foreach (var racer in _racerCollection.Collection)
            {
                if (racer.IsPlayer)
                {
                    _PlayerSaver.LoadPlayerInputs(racer.GetComponent<PlayerInput>());
                }
                CreateRacerEntry(racer);
            }
        }

        void CreateRacerEntry(RacerEntity spawnedRacerEntity)
        {
            var entry = Instantiate(_HotKeyPrefab, _HotKeysParent);
            entry.Setup(spawnedRacerEntity, this);

            _PlayerBindingEntries.Add(entry);
        }

        public void UpdateAllBindingsDisplay()
        {
            foreach (var bindingsEntryBar in _PlayerBindingEntries)
            {
                bindingsEntryBar.UpdateDisplay();
            }
        }

        public void AddPlayer()
        {
            if (PlayerInput.AllPlayers.Count >= 4) return;
            var spawnedRacerEntity = _spawnerVariable.Value.Spawn(true);
            _PlayerSaver.LoadPlayerInputs(spawnedRacerEntity.GetComponent<PlayerInput>());
            CreateRacerEntry(spawnedRacerEntity);
        }

        public void AddBot()
        {
            var spawnedRacerEntity = _spawnerVariable.Value.Spawn(false);
            CreateRacerEntry(spawnedRacerEntity);
        }

        public void RemoveRacer(RacerEntity racer)
        {
            Destroy(racer.gameObject);
            for (int i = 0; i < _PlayerBindingEntries.Count; i++)
            {
                if (_PlayerBindingEntries[i].Racer == racer)
                {
                    Destroy(_PlayerBindingEntries[i].gameObject);
                    _PlayerBindingEntries.RemoveAt(i);
                    break;
                }
            }
        }
    }
}