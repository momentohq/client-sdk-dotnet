open Momento.Sdk
open Momento.Sdk.Exceptions
open Momento.Sdk.Config
open Momento.Sdk.Responses
open System



let MOMENTO_AUTH_TOKEN = Environment.GetEnvironmentVariable("MOMENTO_AUTH_TOKEN")
let CACHE_NAME = "cache"
let DEFAULT_TTL_SECONDS = 60u

let exerciseCache() = (
    printfn "Howdy"
    using(new SimpleCacheClient(Configurations.Laptop.Latest, MOMENTO_AUTH_TOKEN, DEFAULT_TTL_SECONDS)) (fun client ->
        try
            client.CreateCache(CACHE_NAME)
        with
            | :? AlreadyExistsException -> printfn $"Cache with name {CACHE_NAME} already exists\n"; null
        |> ignore
        

        printfn("Listing caches:")
        let resp = client.ListCaches()
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
