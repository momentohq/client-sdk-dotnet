open Momento.Sdk
open Momento.Sdk.Config
open Momento.Sdk.Auth
open Momento.Sdk.Responses
open System

let CACHE_NAME = "cache"
let DEFAULT_TTL = TimeSpan.FromSeconds(60.0)
let authProvider = new EnvMomentoV2TokenProvider()

let exerciseCache() = (
    printfn "Howdy"
    using(new CacheClient(Configurations.Laptop.V1(), authProvider, DEFAULT_TTL)) (fun client ->
        let createCacheResult = client.CreateCacheAsync(CACHE_NAME) |> Async.AwaitTask
        
        printfn("Listing caches:")        
        let resp = client.ListCachesAsync() |> Async.AwaitTask |> Async.RunSynchronously
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
