namespace EasyClick
{
    public struct MovementInputData : IInputData
    {
        float rotation;

        public MovementInputData(float rotation)
        {
            this.rotation = rotation;
        }

        public TValue ReadValue<TValue>() where TValue : struct
        {
            return (TValue) System.Convert.ChangeType(rotation, typeof(TValue));
        }
    }
}
