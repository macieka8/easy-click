using UnityEngine;

namespace EasyClick
{
    public class ReversedControlsBuff : TimedBuff
    {
        CharacterMovement _movementComponent;

        public ReversedControlsBuff(TimedBuffData buff, GameObject obj) : base(buff, obj)
        {
            _movementComponent = _owner.GetComponent<CharacterMovement>();
        }

        protected override void ApplyEffect()
        {
            _movementComponent.ReverseControls();
        }

        public override void End()
        {
            _movementComponent.ReverseControls();
        }
    }
}
