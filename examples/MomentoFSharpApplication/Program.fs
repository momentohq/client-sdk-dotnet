open Momento.Sdk
open Momento.Sdk.Config
open Momento.Sdk.Responses
open System



let MOMENTO_AUTH_TOKEN = Environment.GetEnvironmentVariable("MOMENTO_AUTH_TOKEN")
let CACHE_NAME = "cache"
let DEFAULT_TTL_SECONDS = 60u

let exerciseCache() = (
    printfn "Howdy"
    using(new SimpleCacheClient(Configurations.Laptop.Latest, MOMENTO_AUTH_TOKEN, DEFAULT_TTL_SECONDS)) (fun client ->
        let createCacheResult = client.CreateCacheAsync(CACHE_NAME) |> Async.AwaitTask
        
        printfn("Listing caches:")        
        let resp = client.ListCachesAsync(MOMENTO_AUTH_TOKEN) |> Async.AwaitTask |> Async.RunSynchronously
        let _ = match resp with
                | :? ListCachesResponse.Success as successResponse ->
                    for cacheInfo in successResponse.Caches do
                        printfn $"{cacheInfo.Name}"
                | _ -> ()

        null
    )

)

[<EntryPoint>]
let main(argv :string[]) =
    let _ = exerciseCache()
    0
