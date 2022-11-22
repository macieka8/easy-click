using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] Grid _grid;
    [SerializeField] List<DirectionMapEntry> _directionMap = new List<DirectionMapEntry>();

    public IReadOnlyList<DirectionMapEntry> DirectionMap => _directionMap;

    public void SetDirection(Vector2Int coords, Vector2 direction)
    {
        var entry = _directionMap.Find(e => e.Coords == coords);
        if (entry == null)
        {
            _directionMap.Add(new DirectionMapEntry(coords, direction));
        }
        else
        {
            entry.Direction = direction;
        }
    }

    public Vector2 GetDirection(Vector2 position)
    {
        var cellCoords = (Vector2Int)_grid.WorldToCell(position);
        var entry = _directionMap.Find(e => e.Coords == cellCoords);
        if (entry == null)
        {
            Debug.LogWarning($"Direction for coords {cellCoords} was not found.");
            return Vector2.zero;
        }
        return entry.Direction;
    }
}
