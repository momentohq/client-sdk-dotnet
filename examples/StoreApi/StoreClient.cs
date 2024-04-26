namespace Momento.Sdk.StoreClient;

public class StoreClient
{
    public StoreClient()
    {
    }

    public async Task CreateStoreAsync(string storeName)
    {
        // These are so the compiler doesn't complain no awaits.
        await Task.Delay(0);
    }

    public async Task DeleteStoreAsync(string storeName)
    {
        await Task.Delay(0);
    }

    public async Task ListStoresAsync()
    {
        await Task.Delay(0);
    }

    public async Task<StoreGetResponse> GetAsync(string storeName, string key)
    {
        await Task.Delay(0);
        if (key == "string-value")
        {
            return new StoreGet.Success("string-value");
        }
        else if (key == "int-value")
        {
            return new StoreGet.Success(42);
        }
        else if (key == "double-value")
        {
            return new StoreGet.Success(3.14);
        }
        else if (key == "bool-value")
        {
            return new StoreGet.Success(true);
        }
        else
        {
            return new StoreGet.Error("Key not found.");
        }
    }

    public async Task PutAsync(string storeName, string key, StoreValue value)
    {
        await Task.Delay(0);
    }

    public async Task DeleteAsync(string storeName, string key)
    {
        await Task.Delay(0);
    }
}
