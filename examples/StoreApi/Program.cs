using Momento.Sdk.StoreClient;

namespace Driver;

public class Program
{
    public static async Task Main(string[] args)
    {
        var client = new StoreClient();
        await client.CreateStoreAsync("my-store");

        // Can use implicit cast to create StoreValue.
        await client.PutAsync("my-store", "my-key", "my-value");
        await client.PutAsync("my-store", "my-key", 42);
        await client.PutAsync("my-store", "my-key", 3.14);
        await client.PutAsync("my-store", "my-key", true);

        // The compiler will detect invalid casts.
        // await client.PutAsync("my-store", "my-key", new string[] { "a", "b" });

        // This demos usage where the developer is sure of the type.
        {
            var response = await client.GetAsync("my-store", "string-value");
            if (response is StoreGet.Success success)
            {
                // This uses an implicit cast from StoreValue to string.
                string value = success.Value;
                Console.WriteLine($"Success: {value}");
            }
            else if (response is StoreGet.Error error)
            {
                Console.WriteLine($"Error: {error.Message}");
            }
        }

        // A more cautious developer would handle the unfortunate case where the cast fails.
        {
            var response = await client.GetAsync("my-store", "bool-value");
            if (response is StoreGet.Success success)
            {
                try
                {
                    // This uses an implicit cast from StoreValue to long, but it's actually a bool.
                    long value = success.Value;
                    Console.WriteLine($"Success: {value}");
                }
                catch (InvalidCastException)
                {
                    Console.WriteLine($"Error: Invalid cast to long. Type was actually {success.Value.Type}");
                }
            }
            else if (response is StoreGet.Error error)
            {
                Console.WriteLine($"Error: {error.Message}");
            }
        }

        // This demos usage where the developer wants to do something specific based on the type.
        {
            foreach (var key in new string[] { "string-value", "int-value", "double-value", "bool-value", "unknown-value" })
            {
                var response = await client.GetAsync("my-store", key);
                if (response is StoreGet.Success success)
                {
                    var message = success.Value.Type switch
                    {
                        StoreValueType.String => "string",
                        StoreValueType.Integer64 => "integer",
                        StoreValueType.Double => "double",
                        StoreValueType.Bool => "boolean",
                        _ => "unknown type :("
                    };
                    Console.WriteLine($"Success: {key} is a {message}.");
                }
                else if (response is StoreGet.Error error)
                {
                    Console.WriteLine($"Error when getting key {key}: {error.Message}");
                }
            }
        }

        await client.DeleteStoreAsync("my-store");
    }
}

