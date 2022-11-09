using UnityEngine;
using System;

namespace EasyClick
{
    public abstract class BuffData : ScriptableObject
    {
        [SerializeField] Sprite _icon;
        public Sprite Icon { get => _icon; }

        public abstract Buff InitializeBuff(GameObject owner);
    }

    public abstract class Buff
    {
        public bool IsFinished;
        protected GameObject _owner;
        public BuffData BuffData { get; private set; }
        public event Action<Buff> OnEnd = delegate { };

        public Buff(BuffData buffData, GameObject owner)
        {
            BuffData = buffData;
            _owner = owner;
        }

        public abstract void Tick(float deltaTime);
        public abstract void Activate();
        protected abstract void ApplyEffect();
        public abstract void End();

        protected void InvokeOnEnd()
        {
            OnEnd?.Invoke(this);
        }
    }
}
