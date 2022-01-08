namespace EasyClick
{
    public interface IInputData
    {
        public TValue ReadValue<TValue>() where TValue : struct;
    }
}
