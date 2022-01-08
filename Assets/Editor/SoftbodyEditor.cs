using UnityEngine;
using UnityEditor;

namespace EasyClick
{
    [CustomEditor(typeof(Softbody))]
    public class SoftbodyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Softbody softbody = target as Softbody;

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