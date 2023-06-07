using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using Amazon.CDK.AWS.IAM;

namespace LambdaExample
{
    public class LambdaExampleStack : Stack
    {
        internal LambdaExampleStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            Function fn = new Function(this, "LambdaExampleHandler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("./LambdaExampleHandler/src/LambdaExampleHandler/bin/Release/net6.0/publish"),
                Handler = "LambdaExampleHandler::LambdaExampleHandler.Function::FunctionHandler"
            });
        }
    }
}