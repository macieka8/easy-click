using UnityEngine;

namespace EasyClick
{
    public class Softbody : MonoBehaviour
    {
        [SerializeField] Rigidbody2D[] _Bones;
        [SerializeField] Rigidbody2D _TopLeftBone;
        [SerializeField] Rigidbody2D _TopRightBone;

        Rigidbody2D _CenterNode;
        Vector2[] _bonesDefaultOffsets;
        int _nodesCount;

        public Vector2 Position
        {
            get => _CenterNode.position;
            set => RepositionNodes(value);
        }

        public Vector2 UpVector
        {
            get
            {
                Vector2 pos = transform.position;
                Vector2 up = (_TopLeftBone.position + _TopRightBone.position) / 2 - pos;
                up.Normalize();

                return up;
            }
        }

        public float Rotation { get => Vector2.SignedAngle(Vector2.up, UpVector); }

        void Awake()
        {
            _CenterNode = GetComponent<Rigidbody2D>();
            
            _nodesCount = (_Bones.Length + 1);
            _bonesDefaultOffsets = new Vector2[_Bones.Length];

            for (int i = 0; i < _Bones.Length; i++)
            {
                _bonesDefaultOffsets[i] = _Bones[i].transform.localPosition;
            }
        }

        public void RepositionNodes()
        {
            RepositionNodes(transform.position);
        }

        public void RepositionNodes(Vector2 position)
        {
            _CenterNode.position = position;
            for (int i = 0; i < _Bones.Length; i++)
            {
                _Bones[i].position = position + _bonesDefaultOffsets[i];
            }
        }

        public void AddForce(Vector2 force, ForceMode2D mode)
        {
            foreach (var node in _Bones)
            {
                node.AddForce(force / _nodesCount, mode);
            }
            _CenterNode.AddForce(force / _nodesCount);
        }

        public void AddTorque(float torque)
        {
            float forcePerBone = torque / _Bones.Length;

            foreach (var bone in _Bones)
            {
                Vector2 centerToNode = bone.position - _CenterNode.position;
                Vector2 dir = Vector2.Perpendicular(centerToNode).normalized;
                bone.AddForce(dir * forcePerBone, ForceMode2D.Impulse);
            }
        }
    }
}