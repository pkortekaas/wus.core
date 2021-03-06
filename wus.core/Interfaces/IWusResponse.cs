namespace CoreWUS
{
    public interface IWusResponse
    {
        T HandleResponse<T>(string response) where T : class;
    }
}