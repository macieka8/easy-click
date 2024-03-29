using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EasyClick
{
    [System.Serializable]
    public class DirectionMapEntry
    {
        public Vector2Int Coords;
        public Vector2 Direction;

        public DirectionMapEntry(Vector2Int coords, Vector2 direction)
        {
            Coords = coords;
            Direction = direction;
        }
    }
    public class AIDirectionMap : MonoBehaviour
    {
        [SerializeField] AssetReferenceLoaderDirectionMapVariable _directionMapVariableLoader;
        [SerializeField] Grid _grid;
        [SerializeField] DirectionMapVariation[] _variations = new DirectionMapVariation[0];

        [HideInInspector] [SerializeField] List<DirectionMapEntry> _directionMap = new List<DirectionMapEntry>();
        [HideInInspector] [SerializeField] List<Vector2> _rectangularMap = new List<Vector2>();
        [HideInInspector] [SerializeField] Vector2Int _rectangularMapStartCoords;
        [HideInInspector] [SerializeField] Vector2Int _rectangularMapDimension;

        public Grid Grid => _grid;
        public IReadOnlyList<DirectionMapEntry> DirectionMap => _directionMap;
        public IReadOnlyList<Vector2> RectangularMap => _rectangularMap;
        public Vector2Int StartCoords => _rectangularMapStartCoords;
        public Vector2Int Dimension => _rectangularMapDimension;

        void Awake()
        {
            _directionMapVariableLoader.LoadAssetAsync();
        }

        void OnEnable()
        {
            _directionMapVariableLoader.Value.RegisterVariable(this);
        }

        void OnDisable()
        {
            _directionMapVariableLoader.Value.UnregisterVariable(this);
        }

        void OnDestroy()
        {
            _directionMapVariableLoader.Release();
        }

        public Vector2 GetDirection(Vector2 position)
        {
            foreach (var variation in _variations)
            {
                if (variation.IsPositionInBounds(position))
                {
                    return variation.GetDirection(position);
                }
            }
            var cellCoords = (Vector2Int)_grid.WorldToCell(position);

            if (InMapBounds(cellCoords))
            {
                var normalizedCoords = cellCoords - _rectangularMapStartCoords;
                return _rectangularMap[(-normalizedCoords.y * _rectangularMapDimension.x) + normalizedCoords.x];
            }
            else
            {
                var entry = _directionMap.Find(e => e.Coords == cellCoords);
                if (entry == null)
                {
                    Debug.LogWarning($"Direction for coords {cellCoords} was not found.");
                    return Vector2.zero;
                }
                else
                {
                    Debug.Log($"Direction for coords {cellCoords} not found in Rectangular Map.");
                }
                return entry.Direction;
            }
        }

        public bool InMapBounds(Vector2Int coords)
        {
            var normalizedCoords = coords - _rectangularMapStartCoords;
            return normalizedCoords.x >= 0 &&
                normalizedCoords.x < _rectangularMapDimension.x &&
                normalizedCoords.y <= 0 &&
                normalizedCoords.y > -_rectangularMapDimension.y;
        }
    }
}
