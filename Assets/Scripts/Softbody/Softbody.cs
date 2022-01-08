using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace EasyClick
{
    public class Softbody : MonoBehaviour
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

        Rigidbody2D[,] _Nodes;

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
                Vector2 up = (_Nodes[0, 0].position + _Nodes[0, _Width - 1].position) / 2 - pos;
                up.Normalize();

                return up;
            }
        }
        public float Rotation { get => Vector2.SignedAngle(Vector2.up, UpVector); }

        private void Awake()
        {
            var nodesCount = transform.Find("Nodes Parent").childCount;

            _Nodes = new Rigidbody2D[_Height, _Width];
            for (int i = 0; i < nodesCount; i++)
            {
                _Nodes[i / _Width, i % _Width] = transform.Find("Nodes Parent").GetChild(i).GetComponent<Rigidbody2D>();
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
            _Nodes = new Rigidbody2D[_Height, _Width];

            var nextPosition = new Vector3(
                -Width / 2f,
                Height / 2f,
                0f);

            float positionIncrement = _NodeDiameter + _DistanceBetweenNodes;

            // Create a transform for nodes
            var nodesParentTransform = new GameObject("Nodes Parent");
            nodesParentTransform.transform.SetParent(transform);
            nodesParentTransform.transform.localPosition = new Vector3(0f, 0f);

            for (int y = 0; y < _Height; y++)
            {
                for (int x = 0; x < _Width; x++)
                {
                    // Create node
                    _Nodes[y, x] = Instantiate(_NodePrefab);
                    _Nodes[y, x].transform.SetParent(nodesParentTransform.transform);
                    _Nodes[y, x].transform.localPosition = nextPosition;
                    _Nodes[y, x].transform.localScale = new Vector3(_NodeDiameter, _NodeDiameter, 1f);
                    _Nodes[y, x].mass = _Mass / (_Width * _Height);

                    nextPosition.x += positionIncrement;
                }
                nextPosition.x = -Width / 2f;
                nextPosition.y -= positionIncrement;
            }
        }

        void ConnectNodes()
        {
            float jointDistanceStraight = _DistanceBetweenNodes + _NodeDiameter;
            float jointDistanceDiagonal = (_DistanceBetweenNodes + _NodeDiameter) * Mathf.Sqrt(2);

            for (int y = 0; y < _Height; y++)
            {
                for (int x = 0; x < _Width; x++)
                {
                    if (x != _Width - 1)
                    {
                        // right
                        AddSpringJoint(_Nodes[y, x], _Nodes[y, x + 1], jointDistanceStraight);
                    }

                    if (y != _Height - 1)
                    {
                        // down
                        AddSpringJoint(_Nodes[y, x], _Nodes[y + 1, x], jointDistanceStraight);
                    }

                    if (y != 0 && x != _Width - 1)
                    {
                        // right-up
                        AddSpringJoint(_Nodes[y, x], _Nodes[y - 1, x + 1], jointDistanceDiagonal);
                    }

                    if (y != _Height - 1 && x != _Width - 1)
                    {
                        // right-down
                        AddSpringJoint(_Nodes[y, x], _Nodes[y + 1, x + 1], jointDistanceDiagonal);
                    }
                }
            }
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

            for (int y = 0; y < _Height; y++)
            {
                for (int x = 0; x < _Width; x++)
                {
                    _Nodes[y, x].position = nextPosition;
                    nextPosition.x += positionIncrement;
                }
                nextPosition.x = _Nodes[0, 0].position.x;
                nextPosition.y -= positionIncrement;
            }
        }

        void InitSpline()
        {
            Spline spline = _SpriteShapeController.spline;
            spline.Clear();

            // Create list with all edges
            _Edges = new List<Rigidbody2D>();
            for (int i = 0; i < _Width; i++)
            {
                _Edges.Add(_Nodes[0, i]);
            }
            for (int i = 1; i < _Height - 1; i++)
            {
                _Edges.Add(_Nodes[i, _Width - 1]);
            }
            for (int i = _Width - 1; i >= 0; i--)
            {
                _Edges.Add(_Nodes[_Height - 1, i]);
            }
            for (int i = _Height - 2; i > 0; i--)
            {
                _Edges.Add(_Nodes[i, 0]);
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
                    if (_Edges[i] == _Nodes[0, 0])
                    {
                        Vector2 pos = Quaternion.Euler(0f, 0f, 45f) * UpVector;
                        spline.SetPosition(i, position + pos * _NodeDiameter / 2f);

                        pos = Vector2.Perpendicular(pos) * 0.1f;
                        spline.SetRightTangent(i, -pos);
                        spline.SetLeftTangent(i, pos);
                    }
                    // top-right
                    else if (_Edges[i] == _Nodes[0, _Width - 1])
                    {
                        Vector2 pos = Quaternion.Euler(0f, 0f, -45f) * UpVector;
                        spline.SetPosition(i, position + pos * _NodeDiameter / 2f);

                        pos = Vector2.Perpendicular(pos) * 0.1f;
                        spline.SetRightTangent(i, -pos);
                        spline.SetLeftTangent(i, pos);
                    }
                    // bottom-right
                    else if (_Edges[i] == _Nodes[_Height - 1, _Width - 1])
                    {
                        Vector2 pos = Quaternion.Euler(0f, 0f, -135f) * UpVector;
                        spline.SetPosition(i, position + pos.normalized * _NodeDiameter / 2f);

                        pos = Vector2.Perpendicular(pos) * 0.1f;
                        spline.SetRightTangent(i, -pos);
                        spline.SetLeftTangent(i, pos);
                    }
                    // bottom-left
                    else if (_Edges[i] == _Nodes[_Height - 1, 0])
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
            foreach (var node in _Nodes)
            {
                node.AddForce(force / _Nodes.Length, mode);
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