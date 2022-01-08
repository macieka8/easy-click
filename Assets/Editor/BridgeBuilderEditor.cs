using UnityEngine;
using UnityEditor;

namespace EasyClick
{
    [CustomEditor(typeof(BridgeBuilder))]
    public class BridgeBuilderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BridgeBuilder bridgeBuilder = (BridgeBuilder)target;

            if (GUI.changed)
            {
                if (bridgeBuilder.transform.Find("Nodes Parent"))
                {
                    DestroyImmediate(bridgeBuilder.transform.Find("Nodes Parent").gameObject);
                }
                bridgeBuilder.BuildBridge();
            }
        }
    }
}