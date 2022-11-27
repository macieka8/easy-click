using UnityEngine;

namespace EasyClick
{
    [CreateAssetMenu(menuName = "Variable/DirectionMap")]
    public class DirectionMapVariable : ScriptableObject
    {
        AIDirectionMap _directionMap;
        public AIDirectionMap Value => _directionMap;

        public void RegisterVariable(AIDirectionMap directionMap)
        {
            _directionMap = directionMap;
        }

        public void UnregisterVariable(AIDirectionMap directionMap)
        {
            if (_directionMap == directionMap)
            {
                _directionMap = null;
            }
        }
    }
}