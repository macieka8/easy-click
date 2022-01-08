using UnityEngine;

namespace EasyClick
{
    public interface ICharacterbody : IBody
    {
        public Vector2 Up { get; }
        public float Rotation { get; }
        public bool TouchingGround { get; }
        public void AddForce(Vector2 force, ForceMode2D mode);
        public void AddTorque(float torque);
    }
}
