open Momento.Sdk
open Momento.Sdk.Exceptions
open System


let MOMENTO_AUTH_TOKEN = Environment.GetEnvironmentVariable("MOMENTO_AUTH_TOKEN")
let CACHE_NAME = "cache"
let DEFAULT_TTL_SECONDS = 60u

let exerciseCache() = (
    printfn "Howdy"
    using(new SimpleCacheClient(MOMENTO_AUTH_TOKEN, DEFAULT_TTL_SECONDS)) (fun client ->
        try
            client.CreateCache(CACHE_NAME)
        with
            | :? AlreadyExistsException -> printfn $"Cache with name {CACHE_NAME} already exists\n"; null
        |> ignore
        

        printfn("Listing caches:")
        let resp = client.ListCaches()
        for cacheInfo in resp.Caches do
            printfn $"{cacheInfo.Name}"
        

        null
    )

)

[<EntryPoint>]
let main(argv :string[]) =
    exerciseCache()
    0
