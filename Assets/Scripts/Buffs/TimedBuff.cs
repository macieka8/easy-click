using UnityEngine;

namespace EasyClick
{
    public abstract class TimedBuffData : BuffData
    {
        [SerializeField] bool _isPermanent;
        [SerializeField] float _duration;
        [SerializeField] bool _isDurationStackable;
        [SerializeField] bool _isEffectStackable;

        public bool IsPermanent { get => _isPermanent; }
        public float Duration { get => _duration; }
        public bool IsDurationStackable { get => _isDurationStackable; }
        public bool IsEffectStackable { get => _isEffectStackable; }
    }

    public abstract class TimedBuff : Buff
    {
        public float TimeLeft { get; protected set; }
        protected int _EffectStacks;
        protected TimedBuffData TimedBuffData => BuffData as TimedBuffData;

        public TimedBuff(TimedBuffData buff, GameObject obj) : base(buff, obj)
        {
        }

        public override void Tick(float deltaTime)
        {
            if (!TimedBuffData.IsPermanent)
            {
                TimeLeft -= deltaTime;
            }

            // End Buff
            if (TimeLeft <= 0)
            {
                End();

                InvokeOnEnd();

                IsFinished = true;
            }
        }

        public override void Activate()
        {
            // Check if buff effect is stackable / First Activation
            if (TimedBuffData.IsEffectStackable || TimeLeft <= 0)
            {
                ApplyEffect();
                _EffectStacks++;
            }

            // Check if buff duration is stackable / First Activation
            if (TimedBuffData.IsDurationStackable || TimeLeft <= 0)
            {
                TimeLeft += TimedBuffData.Duration;
            }
            else
            {
                TimeLeft = TimedBuffData.Duration;
            }
        }

        protected override abstract void ApplyEffect();
        public override abstract void End();
    }
}