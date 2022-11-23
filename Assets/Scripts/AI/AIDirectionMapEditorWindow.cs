using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class AIDirectionMapEditorWindow : EditorWindow
{
    [SerializeField] Vector2 _targetPosition;
    [SerializeField] int _brushSize;
    [SerializeField] bool _isDrawingEnabled;

    SerializedObject _serializedObject;
    SerializedProperty _targetProp;
    SerializedProperty _brushSizeProp;
    SerializedProperty _isDrawingEnabledProp;

    SerializedObject _serializedMapObject;
    SerializedProperty _serilaizedMapProp;

    Vector2Field _targetField;
    IntegerField _brushSizeField;
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

    void OnEnable()
    {
        _serializedObject = new SerializedObject(this);
        _targetProp = _serializedObject.FindProperty("_targetPosition");
        _brushSizeProp = _serializedObject.FindProperty("_brushSize");
        _isDrawingEnabledProp = _serializedObject.FindProperty("_isDrawingEnabled");

        _controlId = GUIUtility.GetControlID(FocusType.Passive);
        SceneView.duringSceneGui += DuringSceneGui;
        Selection.selectionChanged += HandleSelectionChanged;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGui;
        Selection.selectionChanged -= HandleSelectionChanged;
    }

    void CreateGUI()
    {
        _targetField = new Vector2Field("Target Direction");
        _brushSizeField = new IntegerField("Brush Size");
        _drawToggle = new Toggle("Draw");

        _targetField.BindProperty(_targetProp);
        _brushSizeField.BindProperty(_brushSizeProp);
        _drawToggle.BindProperty(_isDrawingEnabledProp);
        _drawToggle.RegisterValueChangedCallback(HandleDrawToggleChanged);

        rootVisualElement.Add(_targetField);
        rootVisualElement.Add(_brushSizeField);
        rootVisualElement.Add(_drawToggle);

        _drawToggleHelpBox = new HelpBox("Select object with AIDirectionMap component", HelpBoxMessageType.Warning)
        {
            visible = false
        };
        rootVisualElement.Add(_drawToggleHelpBox);
    }

    void DuringSceneGui(SceneView sceneView)
    {
        _targetPosition = Handles.PositionHandle(_targetPosition, Quaternion.identity);
        if (_isDrawingEnabled)
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent<AIDirectionMap>(out var aiDirectionMap))
            {
                DrawOnDirectionMap(aiDirectionMap);
            }
        }
    }

    void DrawOnDirectionMap(AIDirectionMap aiDirectionMap)
    {
        _serializedMapObject = new SerializedObject(aiDirectionMap);
        _serilaizedMapProp = _serializedMapObject.FindProperty("_directionMap");
        _serializedMapObject.Update();
        var grid = aiDirectionMap.Grid;
        if (grid == null)
        {
            Debug.LogError("Grid is null");
            return;
        }
        var screenPos = Event.current.mousePosition;
        var worldPos = HandleUtility.GUIPointToWorldRay(screenPos).origin;
        var cellCoords = (Vector2Int)grid.WorldToCell(worldPos);
        var drawPos = (Vector2)grid.CellToWorld((Vector3Int)cellCoords) - new Vector2(_brushSize / 2, _brushSize / 2);

        Handles.DrawSolidRectangleWithOutline(new Rect(drawPos, Vector2.one * _brushSize), new Color(0f, 0.5f, 0.5f, 0.25f), Color.black);
        SceneView.RepaintAll();

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.alt)
        {
            DrawDirectionsOnSquare(aiDirectionMap, cellCoords);
        }
        else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && !Event.current.alt)
        {
            if (!_ignoreCell.HasValue || cellCoords != _ignoreCell.Value)
            {
                DrawDirectionsOnSquare(aiDirectionMap, cellCoords);
                _ignoreCell = cellCoords;
            }
        }
        else if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && !Event.current.alt)
        {
            GUIUtility.hotControl = 0;
            _ignoreCell = null;
        }
    }

    void DrawDirectionsOnSquare(AIDirectionMap aiDirectionMap, Vector2Int centerCoords)
    {
        Event.current.Use();
        GUIUtility.hotControl = _controlId;
        for (int i = 0; i < _brushSize; i++)
        {
            for (int j = 0; j < _brushSize; j++)
            {
                var coords = new Vector2Int(centerCoords.x + i - _brushSize / 2, centerCoords.y + j - _brushSize / 2);
                var cellPos = (Vector2)aiDirectionMap.Grid.CellToWorld((Vector3Int)coords);
                var direction = (_targetPosition - cellPos).normalized;
                SetDirectionToProperty(coords, direction);
            }
        }
    }

    void HandleSelectionChanged()
    {
        if (_drawToggleHelpBox == null) return;
        _drawToggleHelpBox.visible = _isDrawingEnabled &&
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

    void SetDirectionToProperty(Vector2Int coords, Vector2 direction)
    {
        _serializedMapObject.Update();
        for (int i = 0; i < _serilaizedMapProp.arraySize; i++)
        {
            var coordsProp = _serilaizedMapProp.GetArrayElementAtIndex(i).FindPropertyRelative("Coords");
            var directionProp = _serilaizedMapProp.GetArrayElementAtIndex(i).FindPropertyRelative("Direction");

            if (coordsProp.vector2IntValue == coords)
            {
                directionProp.vector2Value = direction;
                _serializedMapObject.ApplyModifiedProperties();
                return;
            }
        }
        _serilaizedMapProp.InsertArrayElementAtIndex(_serilaizedMapProp.arraySize);
        var elementProp = _serilaizedMapProp.GetArrayElementAtIndex(_serilaizedMapProp.arraySize -1);
        elementProp.FindPropertyRelative("Coords").vector2IntValue = coords;
        elementProp.FindPropertyRelative("Direction").vector2Value = direction;

        _serializedMapObject.ApplyModifiedProperties();
    }
}
