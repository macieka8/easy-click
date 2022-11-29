using UnityEngine;

namespace EasyClick
{
    public class BridgeBuilder : MonoBehaviour
    {
        [SerializeField] GameObject _BridgeNode;

        [Header("Node Settings")]
        [Min(1)] [SerializeField] int _NodesCount;
        [SerializeField] float _NodeWidth;
        [SerializeField] float _NodeHeight;
        [SerializeField] float _DistanceFromNodes;
        [SerializeField] float _BridgeMass;

        [Header("Edges Settings")]
        [SerializeField] bool _FirstNodeAttached;
        [SerializeField] bool _LastNodeAttached;

        [HideInInspector] [SerializeField] GameObject[] _Nodes;

        public void BuildBridge()
        {
            CreateNodes();
            ConnectNodes();
        }

        void CreateNodes()
        {
            _Nodes = new GameObject[_NodesCount];

            Vector2 nodePosition = new Vector2(0f, 0f);

            // Create a transform for nodes
            var nodesParent = new GameObject("Nodes Parent");
            nodesParent.transform.SetParent(transform);
            nodesParent.transform.localPosition = new Vector3(0f, 0f);
            nodesParent.transform.localRotation = Quaternion.identity;

            for (int nodeIndex = 0; nodeIndex < _NodesCount; nodeIndex++)
            {
                _Nodes[nodeIndex] = Instantiate(_BridgeNode, nodesParent.transform);
                _Nodes[nodeIndex].transform.localPosition = nodePosition;
                _Nodes[nodeIndex].transform.localScale = new Vector2(_NodeWidth, _NodeHeight);

                _Nodes[nodeIndex].GetComponent<Rigidbody2D>().mass = _BridgeMass / _NodesCount;

                nodePosition.x += _NodeWidth + _DistanceFromNodes;
            }
        }

        void ConnectNodes()
        {
            HingeJoint2D joint = null;
            if (_FirstNodeAttached)
            {
                joint = _Nodes[0].AddComponent<HingeJoint2D>();
                joint.autoConfigureConnectedAnchor = true;
                joint.anchor = new Vector2(-_NodeWidth / 2f, 0f);
                joint.connectedAnchor = new Vector2(
                    transform.position.x - _NodeWidth / 2f,
                    transform.position.y);
            }

            for (int nodeIndex = 0; nodeIndex < _NodesCount - 1; nodeIndex++)
            {
                joint = _Nodes[nodeIndex].AddComponent<HingeJoint2D>();

                joint.autoConfigureConnectedAnchor = true;
                joint.anchor = new Vector2(_NodeWidth / 2f, 0f);
                joint.connectedBody = _Nodes[nodeIndex + 1].GetComponent<Rigidbody2D>();
                joint.connectedAnchor = new Vector2(-_NodeWidth / 2f, 0f);
            }

            if (_LastNodeAttached)
            {
                joint = _Nodes[_NodesCount - 1].AddComponent<HingeJoint2D>();
                joint.autoConfigureConnectedAnchor = true;
                joint.anchor = new Vector2(_NodeWidth / 2f, 0f);
                joint.connectedAnchor = new Vector2(
                    _Nodes[_NodesCount - 1].transform.position.x + _NodeWidth / 2f,
                    transform.position.y);
            }
        }
    }
}