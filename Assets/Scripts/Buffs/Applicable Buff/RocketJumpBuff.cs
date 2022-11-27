using UnityEngine;

namespace EasyClick
{
    public class RocketJumpBuff : TimedBuff
    {
        CharacterMovement _movementComponent;
        AttributeModifier _modifier;

        public TimedRocketJumpBuffData TimedRocketJumpBuffData => BuffData as TimedRocketJumpBuffData;

        public RocketJumpBuff(TimedBuffData buff, GameObject obj) : base(buff, obj)
        {
            _movementComponent = _owner.GetComponent<CharacterMovement>();
            _modifier = new AttributeModifier(TimedRocketJumpBuffData.JumpForceMultiplier, AttributeModiferType.ProcentAdd, this);
        }

        protected override void ApplyEffect()
        {
            _movementComponent.JumpForce.AddModifier(_modifier);
        }

        public override void End()
        {
            _movementComponent.JumpForce.RemoveModifier(_modifier);
        }
    }
}
