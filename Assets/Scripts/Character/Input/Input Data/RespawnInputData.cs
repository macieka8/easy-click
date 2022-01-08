namespace EasyClick
{
    public struct RespawnInputData : IInputData
    {
        bool respawn;

        public RespawnInputData(bool respawn)
        {
            this.respawn = respawn;
        }

        public TValue ReadValue<TValue>() where TValue : struct
        {
            return (TValue) System.Convert.ChangeType(respawn, typeof(TValue));
        }
    }
}
