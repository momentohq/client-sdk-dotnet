using Amazon;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MomentoLambdaExampleHandler1;

public class Function
{

    const string CACHE_NAME = "MomentoLambda";
    public string bigString = new string('x', 1024*1024/2);
    public int numMomentoGetRequests = 10;
    public int numMomentoSetRequests = 10;
    public Dictionary<Task<CacheSetResponse>, System.Diagnostics.Stopwatch> momentoSetTasks = new Dictionary<Task<CacheSetResponse>, System.Diagnostics.Stopwatch>();
    public Dictionary<Task<CacheGetResponse>, System.Diagnostics.Stopwatch> momentoGetTasks = new Dictionary<Task<CacheGetResponse>, System.Diagnostics.Stopwatch>();

    public async Task<string> FunctionHandler(ILambdaContext context)
    {
        DoStuff();
        return "Task completed";
    }

    public async void DoStuff() {
        Console.WriteLine("Doing stuff");
        ICredentialProvider authProvider = new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN");
        var client = new CacheClient(
            Configurations.Laptop.V1().WithClientTimeout(TimeSpan.FromSeconds(15)),
            authProvider,
            TimeSpan.FromSeconds(30)
        );

        DoMomentoSets(client);
        DoMomentoGets(client);

        Console.WriteLine("Awaiting pending tasks...");
        Task.WaitAll(momentoSetTasks.Keys.ToArray());
        Task.WaitAll(momentoGetTasks.Keys.ToArray());
        Console.WriteLine("Done awaiting tasks");

        foreach(var entry in momentoSetTasks) {
            var key = await entry.Key;
            if (key is CacheSetResponse.Error err) {
               Console.WriteLine("ERROR: " + err.Message);
           } else if (key is CacheSetResponse.Success hit) {
               Console.WriteLine("Set in " + entry.Value.ElapsedMilliseconds);
           }
        }

        foreach(var entry in momentoGetTasks)
        {
            var key = await entry.Key;
            if (key is CacheGetResponse.Miss) {
                Console.WriteLine("Unexpected MISS in " + entry.Value.ElapsedMilliseconds);
            } else if (key is CacheGetResponse.Error err) {
                Console.WriteLine("ERROR: " + err.Message);
            } else if (key is CacheGetResponse.Hit hit) {
                Console.WriteLine("Got " + hit.ValueString.Length + " in " + entry.Value.ElapsedMilliseconds);
            }
        }
    }

    private void DoMomentoSets(CacheClient client) {
        for (int i = 1; i <= numMomentoSetRequests; i++) {
            Console.WriteLine("Setting inside the loop");
            string key = $"setKey-{i}";
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            Task<CacheSetResponse> response = client.SetAsync(CACHE_NAME, key, bigString);
            momentoSetTasks[response] = startTime;
            response.ContinueWith(
                (r) => {
                    momentoSetTasks[response].Stop();
                    return r;
                }
            );
            Console.WriteLine("Stopped stopwatch");
        }
    }

    private void DoMomentoGets(CacheClient client) {
        for (int i = 1; i <= numMomentoGetRequests; i++) {
            Console.WriteLine("Getting inside the loop");
            string keyAttributeValue = $"key-{i}";

            var startTime = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Starting the stopwatch");
            Task<CacheGetResponse> response = client.GetAsync(CACHE_NAME, keyAttributeValue);

            momentoGetTasks[response] = startTime;
            response.ContinueWith(
                (r) => {
                    momentoGetTasks[response].Stop();
                    return r;
                }
            );
            Console.WriteLine("Stopped stopwatch");
        }
    }

    static void Main(string[] args) {
        try {
            new Function().DoStuff();
        } catch(Exception ex) {
            Console.WriteLine("Caught Exception");
            Console.WriteLine(ex);
        }
    }

}
