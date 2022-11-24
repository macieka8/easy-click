using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class AIDirectionMapEditorWindow : EditorWindow
{
    [SerializeField] Vector2 _targetPosition;
    [SerializeField] int _brushSize;
    [SerializeField] bool _isDrawingEnabled;
    [SerializeField] bool _isEraserEnabled;

    SerializedObject _serializedObject;
    SerializedProperty _targetProp;
    SerializedProperty _brushSizeProp;
    SerializedProperty _isDrawingEnabledProp;
    SerializedProperty _isEraserEnabledProp;

    SerializedObject _serializedMapObject;
    SerializedProperty _serilaizedMapProp;
    SerializedProperty _rectangularMapProp;
    SerializedProperty _rectangularMapStartCoordsProp;
    SerializedProperty _rectangularMapDimensionProp;

    Vector2Field _targetField;
    IntegerField _brushSizeField;
    Toggle _drawToggle;
    HelpBox _drawToggleHelpBox;
    Toggle _eraserToggle;
    Button _rectangulateButton;
    Button _cleanMap;

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
        _isEraserEnabledProp = _serializedObject.FindProperty("_isEraserEnabled");

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
        _drawToggleHelpBox = new HelpBox("Select object with AIDirectionMap component", HelpBoxMessageType.Warning)
        {
            visible = false
        };
        _eraserToggle = new Toggle("Erase Mode");
        _rectangulateButton = new Button(Rectangulate)
        {
            text = "Rectangulate"
        };
        _cleanMap = new Button(CleanMap)
        {
            text = "Clean Map"
        };

        _targetField.BindProperty(_targetProp);
        _brushSizeField.BindProperty(_brushSizeProp);
        _drawToggle.BindProperty(_isDrawingEnabledProp);
        _drawToggle.RegisterValueChangedCallback(HandleDrawToggleChanged);
        _eraserToggle.BindProperty(_isEraserEnabledProp);

        rootVisualElement.Add(_targetField);
        rootVisualElement.Add(_brushSizeField);
        rootVisualElement.Add(_drawToggle);
        rootVisualElement.Add(_drawToggleHelpBox);
        rootVisualElement.Add(_eraserToggle);
        rootVisualElement.Add(_rectangulateButton);
        rootVisualElement.Add(_cleanMap);

    }

    void DuringSceneGui(SceneView sceneView)
    {
        _targetPosition = Handles.PositionHandle(_targetPosition, Quaternion.identity);
        if (_isDrawingEnabled)
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent<AIDirectionMap>(out var aiDirectionMap))
            {
                _serializedMapObject = new SerializedObject(aiDirectionMap);
                _serilaizedMapProp = _serializedMapObject.FindProperty("_directionMap");
                _rectangularMapProp = _serializedMapObject.FindProperty("_rectangularMap");
                _rectangularMapStartCoordsProp = _serializedMapObject.FindProperty("_rectangularMapStartCoords");
                _rectangularMapDimensionProp = _serializedMapObject.FindProperty("_rectangularMapDimension");
                DrawOnDirectionMap(aiDirectionMap);
            }
        }
    }

    void DrawOnDirectionMap(AIDirectionMap aiDirectionMap)
    {
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

    void Rectangulate()
    {
        _serializedMapObject.Update();

        for (int i = 0; i < _rectangularMapProp.arraySize; i++)
        {
            var x = i % _rectangularMapDimensionProp.vector2IntValue.x;
            var y = i / _rectangularMapDimensionProp.vector2IntValue.x;
            var directionValue =  _rectangularMapProp.GetArrayElementAtIndex(i).vector2Value;
            if (directionValue != Vector2.zero)
            {
                _serilaizedMapProp.InsertArrayElementAtIndex(_serilaizedMapProp.arraySize);
                _serilaizedMapProp.GetArrayElementAtIndex(_serilaizedMapProp.arraySize - 1).FindPropertyRelative("Coords").vector2IntValue = new Vector2Int(x, -y) + _rectangularMapStartCoordsProp.vector2IntValue;
                _serilaizedMapProp.GetArrayElementAtIndex(_serilaizedMapProp.arraySize - 1).FindPropertyRelative("Direction").vector2Value = directionValue;
            }
        }
        _rectangularMapProp.ClearArray();

        // Find bounds
        _serializedMapObject.ApplyModifiedProperties();
        var minX = int.MaxValue;
        var maxX = int.MinValue;
        var minY = int.MaxValue;
        var maxY = int.MinValue;
        for (int i = 0; i < _serilaizedMapProp.arraySize; i++)
        {
            var coordsProp = _serilaizedMapProp.GetArrayElementAtIndex(i).FindPropertyRelative("Coords");

            if (coordsProp.vector2IntValue.x < minX)
                minX = coordsProp.vector2IntValue.x;
            if (coordsProp.vector2IntValue.x > maxX)
                maxX = coordsProp.vector2IntValue.x;
            if (coordsProp.vector2IntValue.y > maxY)
                maxY = coordsProp.vector2IntValue.y;
            if (coordsProp.vector2IntValue.y < minY)
                minY = coordsProp.vector2IntValue.y;
        }

        var startCoords = new Vector2Int(minX, maxY);
        var dim = new Vector2Int(maxX - minX + 1, maxY - minY + 1);

        _rectangularMapStartCoordsProp.vector2IntValue = startCoords;
        _rectangularMapDimensionProp.vector2IntValue = dim;
        _rectangularMapProp.arraySize = dim.x * dim.y;
        _serializedMapObject.ApplyModifiedProperties();
        _serializedMapObject.Update();

        // Fill rectangular map
        for (int i = 0; i < _serilaizedMapProp.arraySize; i++)
        {
            var coords = _serilaizedMapProp.GetArrayElementAtIndex(i).FindPropertyRelative("Coords").vector2IntValue;
            var direction = _serilaizedMapProp.GetArrayElementAtIndex(i).FindPropertyRelative("Direction").vector2Value;
            var normalizedCoords = coords - startCoords;
            _rectangularMapProp.GetArrayElementAtIndex((dim.x * -normalizedCoords.y) + normalizedCoords.x).vector2Value = direction;
        }
        _serilaizedMapProp.ClearArray();
        _serializedMapObject.ApplyModifiedProperties();
    }

    void CleanMap()
    {
        _serilaizedMapProp.ClearArray();
        _rectangularMapProp.ClearArray();
        _rectangularMapStartCoordsProp.vector2IntValue = Vector2Int.zero;
        _rectangularMapDimensionProp.vector2IntValue = Vector2Int.zero;

        _serializedMapObject.ApplyModifiedProperties();
    }

    void SetDirectionToProperty(Vector2Int coords, Vector2 direction)
    {
        _serializedMapObject.Update();
        var directionMap = _serializedMapObject.targetObject as AIDirectionMap;
        if (directionMap.InMapBounds(coords))
        {
            var normalizedCoords = coords - _rectangularMapStartCoordsProp.vector2IntValue;
            var elementToChange = _rectangularMapProp.GetArrayElementAtIndex(
                (-normalizedCoords.y * _rectangularMapDimensionProp.vector2IntValue.x) + normalizedCoords.x);
            if (_isEraserEnabled)
            {
                elementToChange.vector2Value = Vector2.zero;
            }
            else
            {
                elementToChange.vector2Value = direction;
            }
        }
        else
        {
            for (int i = 0; i < _serilaizedMapProp.arraySize; i++)
            {
                var coordsProp = _serilaizedMapProp.GetArrayElementAtIndex(i).FindPropertyRelative("Coords");
                var directionProp = _serilaizedMapProp.GetArrayElementAtIndex(i).FindPropertyRelative("Direction");

                if (coordsProp.vector2IntValue == coords)
                {
                    if (_isEraserEnabled)
                        _serilaizedMapProp.DeleteArrayElementAtIndex(i);
                    else
                        directionProp.vector2Value = direction;

                    _serializedMapObject.ApplyModifiedProperties();
                    return;
                }
            }
            if (!_isEraserEnabled)
            {
                _serilaizedMapProp.InsertArrayElementAtIndex(_serilaizedMapProp.arraySize);
                var elementProp = _serilaizedMapProp.GetArrayElementAtIndex(_serilaizedMapProp.arraySize -1);
                elementProp.FindPropertyRelative("Coords").vector2IntValue = coords;
                elementProp.FindPropertyRelative("Direction").vector2Value = direction;
            }
        }

        _serializedMapObject.ApplyModifiedProperties();
    }
}
