using UnityEngine;

namespace EasyClick
{
    [CreateAssetMenu(menuName = "Buffs/ReverseControls")]
    public class TimedReverseControlsBuffData : TimedBuffData
    {
        public override Buff InitializeBuff(GameObject owner)
        {
            return new ReversedControlsBuff(this, owner);
        }
    }
}
