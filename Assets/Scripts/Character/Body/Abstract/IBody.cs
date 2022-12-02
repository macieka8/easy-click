using UnityEngine;

namespace EasyClick
{
    public interface IBody
    {
        public Vector2 Position { get; set; }
        public void TeleportTo(Vector2 position);
    }
}
