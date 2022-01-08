using UnityEngine;
using UnityEditor;

namespace EasyClick
{
    [CustomEditor(typeof(SoftbodyEmptyInside))]
    public class SoftbodyEmptyInsideEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SoftbodyEmptyInside softbody = target as SoftbodyEmptyInside;

            if (GUI.changed)
            {
                if (softbody.transform.Find("Nodes Parent"))
                {
                    DestroyImmediate(softbody.transform.Find("Nodes Parent").gameObject);
                }
                softbody.GenerateBody();
            }

            if (GUILayout.Button("Repair nodes"))
            {
                softbody.RepositionNodes();
            }
        }
    }
}