using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(AIDirectionMap))]
public class AIDirectionMapEditor : Editor
{
    int _controlId;
    SerializedProperty _gridProp;
    SerializedProperty _directionMapProp;

    Vector2Int? _ignoreCell;

    void OnEnable()
    {
        _controlId = GUIUtility.GetControlID(FocusType.Passive);
        _gridProp = serializedObject.FindProperty("_grid");
        _directionMapProp = serializedObject.FindProperty("_directionMap");
        SceneView.duringSceneGui += DuringSceneGui;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGui;
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

    void DuringSceneGui(SceneView sceneView)
    {
        var grid = (Grid)_gridProp.objectReferenceValue;
        if (grid == null)
        {
            Debug.LogError("Grid is null");
            return;
        }
        var screenPos = Event.current.mousePosition;
        var worldPos = HandleUtility.GUIPointToWorldRay(screenPos).origin;
        var cellCoords = (Vector2Int)grid.WorldToCell(worldPos);
        var drawPos = (Vector2)grid.CellToWorld((Vector3Int)cellCoords);
        //Debug.Log($"Screen: {screenPos} & {Event.current.mousePosition} | World: {worldPos} | CellCoords : {cellCoords} | DrawPos: {drawPos}");
        Handles.DrawSolidRectangleWithOutline(new Rect(drawPos, Vector2.one), new Color(0f, 0.5f, 0.5f, 0.25f), Color.black);
        SceneView.RepaintAll();
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.alt)
        {
            Event.current.Use();
            GUIUtility.hotControl = _controlId;
            var map = serializedObject.targetObject as AIDirectionMap;
            map.SetDirection(cellCoords, new Vector2(0.7f, 0.7f).normalized);
        }
        else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && !Event.current.alt)
        {
            if (!_ignoreCell.HasValue || cellCoords != _ignoreCell.Value)
            {
                Event.current.Use();
                GUIUtility.hotControl = _controlId;
                var map = serializedObject.targetObject as AIDirectionMap;
                map.SetDirection(cellCoords, new Vector2(0.7f, 0.7f).normalized);

                _ignoreCell = cellCoords;
            }
        }
        else if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && !Event.current.alt)
        {
            GUIUtility.hotControl = 0;
            _ignoreCell = null;
        }
    }
}
