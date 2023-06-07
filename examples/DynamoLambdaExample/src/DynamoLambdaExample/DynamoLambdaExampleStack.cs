using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using Amazon.CDK.AWS.IAM;

namespace DynamoLambdaExample
{
    public class DynamoLambdaExampleStack : Stack
    {
        internal DynamoLambdaExampleStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            Function fn = new Function(this, "DynamoLambdaExampleHandler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("./DynamoLambdaExampleHandler/src/DynamoLambdaExampleHandler/bin/Release/net6.0/publish"),
                Handler = "DynamoLambdaExampleHandler::DynamoLambdaExampleHandler.Function::FunctionHandler",
                FunctionName = "PutItemHandler", 
                MemorySize = 1024,
                Timeout = Duration.Seconds(300)
            });

            Function fn1 = new Function(this, "DynamoLambdaExampleHandler1", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("./DynamoLambdaExampleHandler1/src/DynamoLambdaExampleHandler1/bin/Release/net6.0/publish"),
                Handler = "DynamoLambdaExampleHandler1::DynamoLambdaExampleHandler1.Function::FunctionHandler",
                FunctionName = "GetItemHandler", 
                MemorySize = 1024,
                Timeout = Duration.Seconds(300)
            });
        }
    }
}
