<head>
  <meta name="Momento .NET Client Library Documentation" content=".NET client software development kit for Momento Serverless Cache">
</head>
<img src="https://docs.momentohq.com/img/logo.svg" alt="logo" width="400"/>

# Momento .NET Client Library: 上級者レベルコンフィグ

## クライアントタイムアウトの変更

もしクライアントサイドのタイムアウトが唯一の設定であれば、あらかじめ用意されているコンフィグと共に `WithClientTimeout`のメソッドを使用して設定できます：

```csharp
new SimpleCacheClient(
    Configurations.InRegion.Default.Latest().WithClientTimeout(TimeSpan.FromSeconds(2))
)
```

## カスタムコンフィグ

ほとんどの場合はあらかじめ用意されているコンフィグで間に合うかと思います。しかし、もしこれらのデフォルト設定を上書きしたい場合は、`IConfiguration`インターフェイスを実装するご自身のコンフィグオブジェクトが作成可能です。

一番簡単な方法は、`Configuration`クラスを使用する事です。こちらのクラスは４つの引数を受け取ります：

```csharp
new Configuration(loggerFactory, retryStrategy, middlewares, transportStrategy)
```

あらかじめ用意されたコンフィグがどの様に構成されているのかは[Configurations.cs](../src/Momento.Sdk/Config/Configurations.cs)にあるソースコードをご確認ください。カスタムコンフィグを作成するのに良い例が記載されています。

以下は各引数に関する詳細です。

### loggerFactory

これは.NET の`ILoggerFactory`のインスタンスです。これは`IConfiguration`を実装する全ての Momento クラスに対するログをコンフィグします。

### retryStrategy

失敗したリクエストがリトライされるのか、またどの様にリトライされるのかを管理します。この引数のインターフェースは`IRetryStrategy`です。`FixedCountRetryStrategy`の様なシンプルなあらかじめ用意したものがありますが、カスタムのリトライ動作を作成するために、このインターフェースをご自身で実装していただけます。

### middlewares

この引数に対して０もしくはそれ以上の`IMiddleware`インスタンスを渡すことができます。こちらはリクエストやレスポンスのインターセプタとして機能します。こちらはデバッグのログやパフォーマンスの情報収集など、様々なケースに使用していただけます。シンプルな例は `LoggingMiddleware`を参照してください。

### transportStrategy

こちらは Momento サービスとのコミュニケーションに対する低レベルのネットワーキングの設定をコンフィグします。
詳細は`ITransportStrategy`インターフェイスを参照してください。
