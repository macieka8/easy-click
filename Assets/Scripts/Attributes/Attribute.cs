using System.Collections.Generic;
using System;
using UnityEngine;
using System.Runtime.Serialization;

namespace EasyClick
{
    [Serializable]
    public class Attribute : ISerializationCallbackReceiver
    {
        [NonSerialized]
        float _flatBonus = 0f;

        [NonSerialized]
        float _procentAddBonus = 0f;

        [SerializeField] protected float _baseValue;

        [NonSerialized]
        protected float _value;

        [NonSerialized]
        protected List<AttributeModifier> _attributeModifiers;

        [field: NonSerialized]
        public event Action OnValueChanged;

        public float BaseValue { get => _baseValue; }
        public virtual float Value => _value;

        public Attribute(float baseVal)
        {
            _attributeModifiers = new List<AttributeModifier>();
            _baseValue = baseVal;
            UpdateValue();
        }

        protected virtual void UpdateValue()
        {
            _value = (BaseValue + _flatBonus) * (1 + _procentAddBonus);
            OnValueChanged?.Invoke();
        }

        protected virtual void AddBonus(AttributeModifier modifier)
        {
            if (modifier.Type == AttributeModiferType.Flat)
            {
                _flatBonus += modifier.Value;
            }
            else if (modifier.Type == AttributeModiferType.ProcentAdd)
            {
                _procentAddBonus += modifier.Value;
            }
        }

        protected virtual void RemoveBonus(AttributeModifier modifier)
        {
            if (modifier.Type == AttributeModiferType.Flat)
            {
                _flatBonus -= modifier.Value;
            }
            else if (modifier.Type == AttributeModiferType.ProcentAdd)
            {
                _procentAddBonus -= modifier.Value;
            }
        }

        public virtual void AddModifier(AttributeModifier modifier)
        {
            _attributeModifiers.Add(modifier);
            AddBonus(modifier);
            UpdateValue();
        }

        public virtual bool RemoveModifier(AttributeModifier modifier)
        {
            if (_attributeModifiers.Remove(modifier))
            {
                RemoveBonus(modifier);
                UpdateValue();
                return true;
            }
            return false;
        }

        public virtual void RemoveAllModifersFromSource(object source)
        {
            for (int i = _attributeModifiers.Count - 1; i >= 0; i--)
            {
                if (_attributeModifiers[i].Source == source)
                {
                    RemoveBonus(_attributeModifiers[i]);
                    _attributeModifiers.RemoveAt(i);
                }
            }
            UpdateValue();
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            if (_attributeModifiers == null) _attributeModifiers = new List<AttributeModifier>();
            UpdateValue();
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext ctx)
        {
            OnAfterDeserialize();
        }
    }
}
