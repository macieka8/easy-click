using UnityEditor;
using UnityEngine;

namespace EasyClick.Editor
{
    [CustomPropertyDrawer(typeof(Attribute))]
    public class AttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var rect = new Rect(position.x, position.y, position.width, position.height);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("_baseValue"), GUIContent.none);
            EditorGUI.EndProperty();
        }
    }
}