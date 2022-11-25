using System.Collections.Generic;
using UnityEngine;

namespace EasyClick
{
    [CreateAssetMenu(menuName = "Variable/GameEntityCollection")]
    public class RacerEntityCollection : ScriptableObject
    {
        List<RacerEntity> _collection = new List<RacerEntity>();

        public IReadOnlyList<RacerEntity> Collection => _collection;

        public void Add(RacerEntity racer)
        {
            if (!_collection.Contains(racer))
            {
                _collection.Add(racer);
                var name = racer.IsPlayer ? "Player" : "Bot";
                racer.ChangeName($"{name} {_collection.Count}");
            }
        }

        public void Remove(RacerEntity racer)
        {
            _collection.Remove(racer);
        }
    }
}
