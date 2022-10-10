open Momento.Sdk
open Momento.Sdk.Config
open Momento.Sdk.Auth
open Momento.Sdk.Responses
open System

let CACHE_NAME = "cache"
let DEFAULT_TTL_SECONDS = 60u
let authProvider = new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN")

let exerciseCache() = (
    printfn "Howdy"
    using(new SimpleCacheClient(Configurations.Laptop.Latest(), authProvider, DEFAULT_TTL_SECONDS)) (fun client ->
        let createCacheResult = client.CreateCacheAsync(CACHE_NAME) |> Async.AwaitTask
        
        printfn("Listing caches:")        
        let resp = client.ListCachesAsync(authProvider.AuthToken) |> Async.AwaitTask |> Async.RunSynchronously
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
