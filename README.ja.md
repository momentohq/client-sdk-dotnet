<head>
  <meta name="Momento .NET Client Library Documentation" content=".NET client software development kit for Momento Serverless Cache">
</head>
<img src="https://docs.momentohq.com/img/logo.svg" alt="logo" width="400"/>

[![project status](https://momentohq.github.io/standards-and-practices/badges/project-status-official.svg)](https://github.com/momentohq/standards-and-practices/blob/main/docs/momento-on-github.md)
[![project stability](https://momentohq.github.io/standards-and-practices/badges/project-stability-stable.svg)](https://github.com/momentohq/standards-and-practices/blob/main/docs/momento-on-github.md)

# Momento .NET Client Library

Momento Serverless Cache の .NET クライアント SDK：従来のキャッシュが必要とするオペレーションオーバーヘッドが全く無く、速くて、シンプルで、従量課金のキャッシュです！

## さあ、使用開始 :running:

### 必要条件

[`dotnet` runtime そしてコマンドラインツール](https://dotnet.microsoft.com/en-us/download)が必要です。インストール後は PATH に`dotnet`のコマンドがある事を確認してください。

**IDE に関する注意事項**: [Microsoft Visual Studio](https://visualstudio.microsoft.com/vs), [JetBrains Rider](https://www.jetbrains.com/rider/)や[Microsoft Visual Studio Code](https://code.visualstudio.com/)の様な .NET 開発をサポートできる IDE が必要となります。

### 使用方法

使用開始の準備万全ですか？ではこちらの[examples](./examples/README.md)ディレクトリを参照してください！

### Momento レスポンスタイプ

Momento の`SimpleCacheClient`クラスの戻り値は IDE が容易にエラーを含む、有り得るレスポンスを予測できる様にデザインしてあります。[パターンマッチング](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching)を使用し、様々なレスポンスタイプを区別します。そうすることで、それらの API を使用する時、ランタイムでバグを発見するよりも、コンパイル時での安全性があります。

以下が使用例です:

```csharp
CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
if (getResponse is CacheGetResponse.Hit hitResponse)
{
  Console.WriteLine($"Looked up value: {hitResponse.ValueString}, Stored value: {VALUE}");
}
else if (getResponse is CacheGetResponse.Error getError)
{
  Console.WriteLine($"Error getting value: {getError.Message}");
}
```

詳細は下記にある[エラーの対処法](#エラーの処理法)を参照してください。

### インストール

新規に.NET プロジェクトを作成し、Momento client library を dependency として追加してください：

```bash
mkdir my-momento-dotnet-project
cd my-momento-dotnet-project
dotnet new console
dotnet add package Momento.Sdk
```

### 使用方法

以下がご自身のプロジェクトに使用できるクイックスタートです：

```csharp
using System;
using Momento.Sdk;
using Momento.Sdk.Auth;
using Momento.Sdk.Config;
using Momento.Sdk.Responses;

ICredentialProvider authProvider = new EnvMomentoTokenProvider("MOMENTO_AUTH_TOKEN");
const string CACHE_NAME = "cache";
const string KEY = "MyKey";
const string VALUE = "MyData";
TimeSpan DEFAULT_TTL = TimeSpan.FromSeconds(60);

using (SimpleCacheClient client = new SimpleCacheClient(Configurations.Laptop.Latest(), authProvider, DEFAULT_TTL))
{
    var createCacheResponse = await client.CreateCacheAsync(CACHE_NAME);
    if (createCacheResponse is CreateCacheResponse.Error createError)
    {
        Console.WriteLine($"Error creating cache: {createError.Message}. Exiting.");
        Environment.Exit(1);
    }

    Console.WriteLine($"Setting key: {KEY} with value: {VALUE}");
    var setResponse = await client.SetAsync(CACHE_NAME, KEY, VALUE);
    if (setResponse is CacheSetResponse.Error setError)
    {
        Console.WriteLine($"Error setting value: {setError.Message}. Exiting.");
        Environment.Exit(1);
    }

    Console.WriteLine($"Get value for key: {KEY}");
    CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
    if (getResponse is CacheGetResponse.Hit hitResponse)
    {
        Console.WriteLine($"Looked up value: {hitResponse.ValueString}, Stored value: {VALUE}");
    }
    else if (getResponse is CacheGetResponse.Error getError)
    {
        Console.WriteLine($"Error getting value: {getError.Message}");
    }
}

```

上記のコードは MOMENTO_AUTH_TOKEN という環境変数が必要です。またこの値は有効な[Momento オーセンティケーショントークン](https://docs.momentohq.com/docs/getting-started#obtain-an-auth-token)でなければなりません。

### エラーの処理法

従来の例外処理とは異なり、SimpleCacheClient のメソッドを呼び出した際に起こるエラーは戻り値として浮上します。こうすることで、エラーの可視化を行い、必要なエラーのみを処理する際に IDE が更に役立つ様になります。（もっと例外についての私達の哲学を知りたい方はこちらの[例外はバグだ](https://www.gomomento.com/blog/exceptions-are-bugs)をぜひお読みください。またフィードバックもお待ちしております！）

SimpleCacheClient メソッドからの戻り値の好ましい対応の仕方は[パターンマッチング](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching)を使用する方法です。こちらが簡単な例です：

```csharp
CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
if (getResponse is CacheGetResponse.Hit hitResponse)
{
    Console.WriteLine($"\nLooked up value: {hitResponse.ValueString}, Stored value: {VALUE}");
} else {
      // `else if`内でパターンマッチングを使用し、他のケースも対処可能です。
      // またデフォルトのケースであれば`else`ブロックで対処可能です。
      // 各戻り値に対して、IDE上で有り得るタイプを提案してくれるはずです。
      // この場合だと`CacheGetResponse.Miss`と `CacheGetResponse.Error`です。
}
```

こちらのアプローチだと、キャッシュヒットの場合タイプが保証された`hitResponse`オブジェクトを受け取ります。キャッシュ読み込みの結果がミスもしくはエラーの場合でも何が起こったかの詳細な情報を含み、タイプの保証されたオブジェクトを受け取ることができます。

エラーのレスポンスを受け取った場合、`Error`タイプはエラータイプを確認できる`ErrorCode` をいつも含んでいます：

```csharp
CacheGetResponse getResponse = await client.GetAsync(CACHE_NAME, KEY);
if (getResponse is CacheGetResponse.Error errorResponse)
{
    if (errorResponse.ErrorCode == MomentoErrorCode.TIMEOUT_ERROR) {
       // こちらはクライアント側のタイムアウト意味しており、これが起こった場合元のデータを使用する事が可能です。
    }
}
```

SimpleCacheClient の戻り値以外では例外が起こる可能性があり、それらは通常通り処理する必要があるのでご注意ください。
例えば、SimpleCacheClient のインスタンスを生成する際、無効なオーセンティケーショントークンは IllegalArgumentException の発生原因になります。

### チューニング

Momento client-libraries はあらかじめ用意されたコンフィグがすぐに使用できるようになっています。お客さまがビジネスにフォーカスできるよう、様々な環境に適したチューニングを私達の方で担いたいと思っています。
(ブログシリーズも存在ます！ [ショックなほどシンプル: 大変な作業を担ってくれるキャッシュクライアント](https://www.gomomento.com/blog/shockingly-simple-cache-clients-that-do-the-hard-work-for-you))

`Configurations`ネームスペースにてあらかじめ用意されたコンフィグを確認していただけます。以下がそれらのコンフィグになります：

- `Configurations.Laptop` - こちらは開発環境に適しており、Momento の使用テストを行いたい場合に最適です。タイムアウトは緩く設定されているので、ネットワークレイテンシーは少し高くなる事が予想されます。
- `Configurations.InRegion.Default` - こちらはご自身のクライアントが Momento と同じリージョンで実行されている場合に最適なデフォルト設置です。タイムアウトとリトライは Laptop 設定よりも厳しく設定してあります。
- `Configurations.InRegion.LowLatency` - こちらの設定は p99.9 のレイテンシを最優先しており、そのためスループットが低くなる場合があります。こちらの設定はキャッシュの不可用性が高いレイテンシーを起こしたくない場合に使用してください。

これらのコンフィグが大半のお客様のニーズにお応えできると思いますが、不足している部分などありましたらお気軽に GitHub でチケット作成または`support@momentohq.com`にご連絡ください。お客様の使用例を聞いて、より優れたコンフィグを提供したいと考えております。

あらかじめ用意されたコンフィグ以外にカスタムのコンフィグを作成されたい場合はこちらの
[上級者レベルコンフィグガイド](./docs/advanced-config.md)をご覧ください。

---

更なる詳細は私達のウェブサイト[https://jp.gomomento.com/](https://jp.gomomento.com/)をご確認ください！
