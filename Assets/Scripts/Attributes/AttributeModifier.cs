namespace EasyClick
{
    public enum AttributeModiferType
    {
        Flat = 100,
        ProcentAdd = 200,
    }

    public class AttributeModifier
    {
        public float Value { get; private set; }
        public AttributeModiferType Type { get; private set; }
        public int Order { get; private set; }
        public object Source { get; private set; }

        public AttributeModifier(float value, AttributeModiferType type, object source)
        {
            Value = value;
            Type = type;
            Source = source;
        }

        public AttributeModifier(float value, AttributeModiferType type) : this(value, type, null) { }
    }
}
