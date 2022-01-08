using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace EasyClick
{
    public class SoftbodyEmptyInside : MonoBehaviour
    {
        [SerializeField] Transform _BodyTransform;

        [SerializeField] Rigidbody2D _NodePrefab;
        [SerializeField] int _Width;
        [SerializeField] int _Height;

        [SerializeField] float _NodeDiameter;
        [SerializeField] float _DistanceBetweenNodes;

        [Header("Rigidbody Settings")]
        [SerializeField] float _Mass;

        [Header("Joint Settings")]
        [SerializeField] bool _SelfCollision;
        [SerializeField] float _JointDumping;
        [SerializeField] float _JointFrequency;
        [SerializeField] float _JointBreakForce;

        Rigidbody2D[] _Nodes;

        SpriteShapeController _SpriteShapeController;
        List<Rigidbody2D> _Edges;

        public float Width { get => (_NodeDiameter + _DistanceBetweenNodes) * (_Width - 1) + _NodeDiameter; }
        public float Height { get => (_NodeDiameter + _DistanceBetweenNodes) * (_Height - 1) + _NodeDiameter; }
        public Vector2 Position
        {
            get
            {
                Vector2 position = new Vector2(0f, 0f);

                foreach (var node in _Nodes)
                {
                    position += node.position;

                }

                return position / _Nodes.Length;
            }

            set
            {
                RepositionNodes(value);
            }
        }
        public Vector2 UpVector
        {
            get
            {
                Vector2 pos = transform.position;
                Vector2 up = (_Nodes[0].position + _Nodes[_Width - 1].position) / 2 - pos;
                up.Normalize();

                return up;
            }
        }
        public float Rotation { get => Vector2.SignedAngle(Vector2.up, UpVector); }

        private void Awake()
        {
            var nodesCount = transform.Find("Nodes Parent").childCount;

            _Nodes = new Rigidbody2D[(_Height - 2) * 2 + 2 * _Width];
            for (int i = 0; i < nodesCount; i++)
            {
                _Nodes[i] = transform.Find("Nodes Parent").GetChild(i).GetComponent<Rigidbody2D>();
            }

            GetComponent<CircleCollider2D>().radius = Width / 4f;

            _SpriteShapeController = gameObject.GetComponent<SpriteShapeController>();
            InitSpline();
        }

        private void Update()
        {
            _BodyTransform.position = Position;
            _BodyTransform.eulerAngles = new Vector3(0f, 0f, Rotation);
            RepositionSpline();
        }

        public void GenerateBody()
        {
            CreateNodes();
            ConnectNodes();
        }

        void CreateNodes()
        {
            _Nodes = new Rigidbody2D[(_Height - 2) * 2 + 2 * _Width];
            var nextPosition = new Vector3(
                -Width / 2f,
                Height / 2f,
                0f);

            float positionIncrement = _NodeDiameter + _DistanceBetweenNodes;

            // Create a transform for nodes
            var nodesParentTransform = new GameObject("Nodes Parent");
            nodesParentTransform.transform.SetParent(transform);
            nodesParentTransform.transform.localPosition = new Vector3(0f, 0f);

            int node;
            // Top row
            for (node = 0; node < _Width; node++)
            {
                // Create node
                _Nodes[node] = Instantiate(_NodePrefab);
                _Nodes[node].transform.SetParent(nodesParentTransform.transform);
                _Nodes[node].transform.localPosition = nextPosition;
                _Nodes[node].transform.localScale = new Vector3(_NodeDiameter, _NodeDiameter, 1f);
                _Nodes[node].mass = _Mass / (_Width * _Height);

                nextPosition.x += positionIncrement;
            }
            nextPosition.y -= positionIncrement;
            nextPosition.x = -Width / 2f;
            
            // Middle rows
            for (; node < (_Height - 2) * 2 + _Width; node++)
            {
                // Create node
                _Nodes[node] = Instantiate(_NodePrefab);
                _Nodes[node].transform.SetParent(nodesParentTransform.transform);
                _Nodes[node].transform.localPosition = nextPosition;
                _Nodes[node].transform.localScale = new Vector3(_NodeDiameter, _NodeDiameter, 1f);
                _Nodes[node].mass = _Mass / (_Width * _Height);

                nextPosition.x += (_Width - 1) * positionIncrement;

                if ((node - _Width) % 2 == 1)
                {
                    // Move to new row
                    nextPosition.x = -Width / 2f;
                    nextPosition.y -= positionIncrement;
                }
            }

            // Bottom row
            for (; node < _Nodes.Length; node++)
            {
                // Create node
                _Nodes[node] = Instantiate(_NodePrefab);
                _Nodes[node].transform.SetParent(nodesParentTransform.transform);
                _Nodes[node].transform.localPosition = nextPosition;
                _Nodes[node].transform.localScale = new Vector3(_NodeDiameter, _NodeDiameter, 1f);
                _Nodes[node].mass = _Mass / (_Width * _Height);

                nextPosition.x += positionIncrement;
            }
        }

        void ConnectNodes()
        {
            CrossConnection();
        }

        void SquareConnection()
        {
            float jointDistanceToAdjacent = _DistanceBetweenNodes + _NodeDiameter;
            float jointDistanceDiagonal = Mathf.Sqrt(
                Mathf.Pow(
                    (_Width - 1) * jointDistanceToAdjacent, 2) +
                    Mathf.Pow((_Height - 1) * jointDistanceToAdjacent,
                    2)
                    );

            for (int node = 0; node < (_Height - 2) * 2 + 2 * _Width; node++)
            {
                Vector2Int coords = GetCoordsFromNodeIndex(node);

                // No right column
                if (coords.x != _Width - 1)
                {
                    // Top and bottom rows
                    if (coords.y == 0 || coords.y == _Height - 1)
                    {
                        // Connect node on right
                        AddSpringJoint(_Nodes[node], _Nodes[node + 1], jointDistanceToAdjacent);
                    }
                    // Middle rows
                    else
                    {
                        // Connect node on right
                        AddSpringJoint(_Nodes[node], _Nodes[node + 1], (_Width - 1) * jointDistanceToAdjacent);
                    }
                }
                // No bottom row
                if (coords.y != _Height - 1)
                {
                    // Left and right columns
                    if (coords.x == 0 || coords.x == _Width - 1)
                    {
                        if (node == 0 || node == (_Height - 2) * 2 + _Width - 1)
                        {
                            // Connect node to node below it
                            AddSpringJoint(_Nodes[node], _Nodes[node + _Width], jointDistanceToAdjacent);
                        }
                        else
                        {
                            // Connect node to node below it
                            AddSpringJoint(_Nodes[node], _Nodes[node + 2], jointDistanceToAdjacent);
                        }
                    }
                    // Middle columns
                    else
                    {
                        // Connect node to node below it
                        AddSpringJoint(_Nodes[node], _Nodes[(_Height - 2) * 2 + _Width + node], (_Height - 1) * jointDistanceToAdjacent);
                    }
                }
                // Diagonal top-left to bottom-right
                if (node == 0)
                {
                    AddSpringJoint(_Nodes[node], _Nodes[_Nodes.Length - 1], jointDistanceDiagonal);
                }
                // Diagonal top-right to bottom-left
                else if (node == _Width - 1)
                {
                    AddSpringJoint(_Nodes[node], _Nodes[(_Height - 2) * 2 + _Width], jointDistanceDiagonal);
                }
            }
        }

        void CrossConnection()
        {
            float jointDistanceToAdjacent = _DistanceBetweenNodes + _NodeDiameter;
            float jointDistanceVertical = Mathf.Sqrt(
                Mathf.Pow(jointDistanceToAdjacent, 2) +
                Mathf.Pow((_Height - 1) * jointDistanceToAdjacent,2)
                );
            float jointDistanceHorizontal = Mathf.Sqrt(
                Mathf.Pow(jointDistanceToAdjacent, 2) +
                Mathf.Pow((_Width - 1) * jointDistanceToAdjacent, 2)
                );

            for (int node = 0; node < (_Height - 2) * 2 + 2 * _Width; node++)
            {
                Vector2Int coords = GetCoordsFromNodeIndex(node);

                // No right column
                if (coords.x != _Width - 1)
                {
                    // Connect adjacent on right
                    // Top and bottom rows
                    if (coords.y == 0 || coords.y == _Height - 1)
                    {
                        AddSpringJoint(_Nodes[node], _Nodes[node + 1], jointDistanceToAdjacent);
                    }

                    // Connection crossed top-left to bottom-right
                    if (coords.y == 0)
                    {
                        AddSpringJoint(
                            _Nodes[node],
                            _Nodes[(_Height - 2) * 2 + _Width + node + 1],
                            jointDistanceVertical
                            );
                    }
                    // Connection crossed bottom-left to top-right
                    else if (coords.y == _Height - 1)
                    {
                        AddSpringJoint(
                            _Nodes[node],
                            _Nodes[node - ((_Height - 2) * 2 + _Width) + 1],
                            jointDistanceVertical
                            );
                    }
                }
                // No bottom row
                if (coords.y != _Height - 1)
                {
                    // Left and right columns
                    if (coords.x == 0 || coords.x == _Width - 1)
                    {
                        if (node == 0 || node == (_Height - 2) * 2 + _Width - 1)
                        {
                            // Connect node to node below it
                            AddSpringJoint(_Nodes[node], _Nodes[node + _Width], jointDistanceToAdjacent);
                        }
                        else
                        {
                            // Connect node to node below it
                            AddSpringJoint(_Nodes[node], _Nodes[node + 2], jointDistanceToAdjacent);
                        }
                    }

                    // Connection crossed top-left to bottom-right
                    if (coords.x == 0)
                    {
                        // Middle
                        if (!(coords.y == 0 || coords.y == _Height - 2))
                        {
                            AddSpringJoint(
                                _Nodes[node], 
                                _Nodes[node + 3],
                                jointDistanceHorizontal
                                );
                        }
                        // Top and bottom
                        else
                        {
                            AddSpringJoint(
                                _Nodes[node],
                                _Nodes[node + _Width + 1],
                                jointDistanceHorizontal
                                );
                        }
                    }
                    // Connection crossed top-right to bottom-left
                    else if (coords.x == _Width - 1)
                    {
                        AddSpringJoint(
                            _Nodes[node],
                            _Nodes[node + 1],
                            jointDistanceHorizontal
                            );
                    }
                }
            }
        }

        Vector2Int GetCoordsFromNodeIndex(int nodeIndex)
        {
            Vector2Int coords = new Vector2Int();

            // Top row
            if (nodeIndex < _Width)
            {
                coords.y = 0;
                coords.x = nodeIndex;
            }
            // Bottom row
            else if (nodeIndex >= (_Height - 2) * 2 + _Width)
            {
                coords.y = _Height - 1;
                coords.x = nodeIndex - ((_Height - 2) * 2 + _Width);
            }
            // middle rows
            else
            {
                if ((nodeIndex - _Width) % 2 == 0)
                {
                    coords.x = 0;
                }
                else
                {
                    coords.x = _Width - 1;
                }
                coords.y = 1 + (nodeIndex - _Width) / 2;
            }

            return coords;
        }

        void AddSpringJoint(Rigidbody2D nodeFrom, Rigidbody2D nodeTo, float jointDistance)
        {
            var springJoint = nodeFrom.gameObject.AddComponent<SpringJoint2D>();
            springJoint.connectedBody = nodeTo;

            springJoint.enableCollision = _SelfCollision;
            springJoint.dampingRatio = _JointFrequency;
            springJoint.frequency = _JointFrequency;
            springJoint.breakForce = _JointBreakForce;

            springJoint.autoConfigureDistance = false;
            springJoint.distance = jointDistance;
        }

        public void RepositionNodes()
        {
            RepositionNodes(transform.position);
        }

        public void RepositionNodes(Vector2 position)
        {
            Vector2 nextPosition = new Vector2(
                position.x - Width / 2f + _NodeDiameter / 2f,
                position.y + Height / 2f - _NodeDiameter / 2f);

            float positionIncrement = _NodeDiameter + _DistanceBetweenNodes;

            int node;
            // Top row
            for (node = 0; node < _Width; node++)
            {
                // Create node
                _Nodes[node].transform.localPosition = nextPosition;
                nextPosition.x += positionIncrement;
            }
            nextPosition.y -= positionIncrement;
            nextPosition.x = -Width / 2f;

            // Middle rows
            for (; node < (_Height - 2) * 2 + _Width; node++)
            {
                // Create node
                _Nodes[node].transform.localPosition = nextPosition;
                nextPosition.x += (_Width - 1) * positionIncrement;

                if ((node - _Width) % 2 == 1)
                {
                    // Move to new row
                    nextPosition.x = -Width / 2f;
                    nextPosition.y -= positionIncrement;
                }
            }

            // Bottom row
            for (; node < _Nodes.Length; node++)
            {
                // Create node
                _Nodes[node].transform.localPosition = nextPosition;
                nextPosition.x += positionIncrement;
            }
        }

        void InitSpline()
        {
            Spline spline = _SpriteShapeController.spline;
            spline.Clear();

            // Create list with all edges
            _Edges = new List<Rigidbody2D>();
            // Top row
            for (int i = 0; i < _Width; i++)
            {
                _Edges.Add(_Nodes[i]);
            }
            // Right column
            for (int i = _Width + 1; i < (_Height - 2) * 2 + _Width; i+=2)
            {
                _Edges.Add(_Nodes[i]);
            }
            // Bottom row
            for (int i = _Nodes.Length - 1; i >= (_Height - 2) * 2 + _Width; i--)
            {
                _Edges.Add(_Nodes[i]);
            }
            // Left column
            for (int i = (_Height - 2) * 2 + _Width - 2; i >= _Width; i -= 2)
            {
                _Edges.Add(_Nodes[i]);
            }

            // Creates points
            for (int i = 0; i < _Edges.Count; i++)
            {
                spline.InsertPointAt(i, Vector3.one * i);
                spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            }

            RepositionSpline();
        }

        void RepositionSpline()
        {
            Spline spline = _SpriteShapeController.spline;
            for (int i = 0; i < _Edges.Count; i++)
            {
                Vector2 position = _Edges[i].transform.localPosition;

                try
                {
                    // top-left
                    if (i == 0)
                    {
                        Vector2 pos = Quaternion.Euler(0f, 0f, 45f) * UpVector;
                        spline.SetPosition(i, position + pos * _NodeDiameter / 2f);

                        pos = Vector2.Perpendicular(pos) * 0.1f;
                        spline.SetRightTangent(i, -pos);
                        spline.SetLeftTangent(i, pos);
                    }
                    // top-right
                    else if (i == _Width - 1)
                    {
                        Vector2 pos = Quaternion.Euler(0f, 0f, -45f) * UpVector;
                        spline.SetPosition(i, position + pos * _NodeDiameter / 2f);

                        pos = Vector2.Perpendicular(pos) * 0.1f;
                        spline.SetRightTangent(i, -pos);
                        spline.SetLeftTangent(i, pos);
                    }
                    // bottom-right
                    else if (i == _Width - 1 + _Height - 1)
                    {
                        Vector2 pos = Quaternion.Euler(0f, 0f, -135f) * UpVector;
                        spline.SetPosition(i, position + pos.normalized * _NodeDiameter / 2f);

                        pos = Vector2.Perpendicular(pos) * 0.1f;
                        spline.SetRightTangent(i, -pos);
                        spline.SetLeftTangent(i, pos);
                    }
                    // bottom-left
                    else if (i == _Width - 1 + _Height - 1 + _Width - 1)
                    {
                        Vector2 pos = Quaternion.Euler(0f, 0f, 135f) * UpVector;
                        spline.SetPosition(i, position + pos.normalized * _NodeDiameter / 2f);

                        pos = Vector2.Perpendicular(pos) * 0.1f;
                        spline.SetRightTangent(i, -pos);
                        spline.SetLeftTangent(i, pos);
                    }
                    // top
                    else if (i < _Width)
                    {
                        Vector2 pos = Vector2.Perpendicular(UpVector) * 0.2f;
                        spline.SetPosition(i, position + UpVector * _NodeDiameter / 2f);

                        spline.SetRightTangent(i, -pos);
                        spline.SetLeftTangent(i, pos);
                    }
                    // right
                    else if (i < _Width + _Height - 1)
                    {
                        Vector2 pos = UpVector * 0.2f;
                        spline.SetPosition(i, position + -Vector2.Perpendicular(UpVector) * _NodeDiameter / 2f);

                        spline.SetRightTangent(i, -pos);
                        spline.SetLeftTangent(i, pos);
                    }
                    // bottom
                    else if (i < _Width + _Height - 1 + _Width - 1)
                    {
                        Vector2 pos = -Vector2.Perpendicular(UpVector) * 0.2f;
                        spline.SetPosition(i, position + -UpVector * _NodeDiameter / 2f);

                        spline.SetRightTangent(i, -pos);
                        spline.SetLeftTangent(i, pos);
                    }
                    // left
                    else
                    {
                        Vector2 pos = -UpVector * 0.2f;
                        spline.SetPosition(i, position + Vector2.Perpendicular(UpVector) * _NodeDiameter / 2f);

                        spline.SetRightTangent(i, -pos);
                        spline.SetLeftTangent(i, pos);
                    }
                }
                catch
                {
                    Debug.Log("Spline points too close to each other");
                }
            }
        }

        public void AddForce(Vector2 force, ForceMode2D mode)
        {
            Vector2 forcePerNode = force / _Nodes.Length;
            foreach (var node in _Nodes)
            {
                node.AddForce(forcePerNode, mode);
            }
        }

        public void AddTorque(float torque)
        {
            Vector2 position = transform.position;
            float forcePerNode = torque / _Nodes.Length;

            foreach (var node in _Nodes)
            {
                Vector2 centerToNode = node.position - position;
                Vector2 dir = Vector2.Perpendicular(centerToNode).normalized;
                node.AddForce(dir * forcePerNode, ForceMode2D.Impulse);
            }
        }
    }
}