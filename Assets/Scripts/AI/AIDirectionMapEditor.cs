using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(AIDirectionMap))]
public class AIDirectionMapEditor : Editor
{
    SerializedProperty _gridProp;

    void OnEnable()
    {
        _gridProp = serializedObject.FindProperty("_grid");
    }

    public void OnSceneGUI()
    {
        var grid = (Grid)_gridProp.objectReferenceValue;
        if (grid == null)
        {
            Debug.LogError("Grid is null");
            return;
        }

        var directionMap = serializedObject.targetObject as AIDirectionMap;
        foreach (var entry in directionMap.DirectionMap)
        {
            var worldPos = grid.CellToWorld((Vector3Int)entry.Coords)+ new Vector3(grid.cellSize.x / 2f, grid.cellSize.y / 2f);
            var point = worldPos + ((Vector3)entry.Direction / 2f);
            Handles.color = Color.blue;
            Handles.DrawAAPolyLine(5f , new Vector3[] {worldPos, point});
        }
    }
}
