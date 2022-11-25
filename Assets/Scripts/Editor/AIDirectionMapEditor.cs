using UnityEngine;
using UnityEditor;

namespace EasyClick.Editor
{
    [CustomEditor(typeof(AIDirectionMap))]
    public class AIDirectionMapEditor : UnityEditor.Editor
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

            if (directionMap.RectangularMap.Count > 0)
            {
                Vector2 borderPos = (Vector2)directionMap.StartCoords;
                var size = (Vector2)directionMap.Dimension;
                borderPos.y -= size.y - 1f;
                var rect = new Rect(borderPos, size);
                Handles.DrawSolidRectangleWithOutline(rect, new Color(0, 0, 0, 0.1f), Color.cyan);

                for (int y = 0; y < directionMap.Dimension.y; y++)
                {
                    for (int x = 0; x < directionMap.Dimension.x; x++)
                    {
                        var pos = new Vector2(directionMap.StartCoords.x + x, directionMap.StartCoords.y - y) + new Vector2(grid.cellSize.x / 2f, grid.cellSize.y / 2f);
                        var point = pos + (directionMap.RectangularMap[(y * directionMap.Dimension.x) + x] / 2f);
                        Handles.color = Color.cyan;
                        Handles.DrawAAPolyLine(5f, new Vector3[] {pos, point});
                    }
                }
            }
        }
    }
}
