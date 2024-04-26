namespace Momento.Sdk.StoreClient;

public abstract class StoreGetResponse
{
}

public abstract class StoreGet
{
    public class Success : StoreGetResponse
    {
        public StoreValue Value { get; }

        public Success(StoreValue value)
        {
            Value = value;
        }
    }

    public class Error : StoreGetResponse
    {
        public string Message { get; }

        public Error(string message)
        {
            Message = message;
        }
    }
}
