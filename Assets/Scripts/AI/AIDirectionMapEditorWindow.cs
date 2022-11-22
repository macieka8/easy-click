using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class AIDirectionMapEditorWindow : EditorWindow
{
    Vector2Field _targetField;
    Toggle _drawToggle;
    HelpBox _drawToggleHelpBox;

    int _controlId;
    Vector2Int? _ignoreCell;

    [MenuItem("Tools/AIDirectionMap Editor")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<AIDirectionMapEditorWindow>();
        wnd.titleContent = new GUIContent("Direction Map Editor");
    }

    public void OnEnable()
    {
        SceneView.duringSceneGui += DuringSceneGui;
        Selection.selectionChanged += HandleSelectionChanged;
        _controlId = GUIUtility.GetControlID(FocusType.Passive);
    }

    public void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGui;
    }

    void DuringSceneGui(SceneView sceneView)
    {
        if (_targetField != null)
        {
            _targetField.value = Handles.PositionHandle(_targetField.value, Quaternion.identity);
        }
        if (_drawToggle.value)
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent<AIDirectionMap>(out var aiDirectionMap))
            {
                DrawOnDirectionMap(aiDirectionMap);
            }
        }
    }

    void DrawOnDirectionMap(AIDirectionMap aiDirectionMap)
    {
        var grid = aiDirectionMap.Grid;
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
            var direction = (_targetField.value - drawPos).normalized;
            aiDirectionMap.SetDirection(cellCoords, direction);
        }
        else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && !Event.current.alt)
        {
            if (!_ignoreCell.HasValue || cellCoords != _ignoreCell.Value)
            {
                Event.current.Use();
                GUIUtility.hotControl = _controlId;
                var direction = (_targetField.value - drawPos).normalized;
                aiDirectionMap.SetDirection(cellCoords, direction);

                _ignoreCell = cellCoords;
            }
        }
        else if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && !Event.current.alt)
        {
            GUIUtility.hotControl = 0;
            _ignoreCell = null;
        }
    }

    void HandleSelectionChanged()
    {
        _drawToggleHelpBox.visible = _drawToggle.value &&
            Selection.activeGameObject != null &&
            !Selection.activeGameObject.TryGetComponent<AIDirectionMap>(out var _);
    }

    void HandleDrawToggleChanged(ChangeEvent<bool> changeEvent)
    {
        if (!changeEvent.newValue)
        {
            _drawToggleHelpBox.visible = false;
        }
        else
        {
            HandleSelectionChanged();
        }
    }

    public void CreateGUI()
    {
        _targetField = new Vector2Field("Target Direction");
        _drawToggle = new Toggle("Draw");
        _drawToggle.RegisterValueChangedCallback(HandleDrawToggleChanged);
        rootVisualElement.Add(_targetField);
        rootVisualElement.Add(_drawToggle);

        _drawToggleHelpBox = new HelpBox("Select object with AIDirectionMap component", HelpBoxMessageType.Warning)
        {
            visible = false
        };
        rootVisualElement.Add(_drawToggleHelpBox);
    }
}
