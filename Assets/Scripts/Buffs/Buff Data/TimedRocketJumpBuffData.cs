using UnityEngine;

namespace EasyClick
{
    [CreateAssetMenu(menuName = "Buffs/RocketJump")]
    public class TimedRocketJumpBuffData : TimedBuffData
    {
        [Min(0f)]
        [SerializeField] float _jumpForceMultiplier;

        public float JumpForceMultiplier => _jumpForceMultiplier;

        public override Buff InitializeBuff(GameObject owner)
        {
            return new RocketJumpBuff(this, owner);
        }
    }
}
