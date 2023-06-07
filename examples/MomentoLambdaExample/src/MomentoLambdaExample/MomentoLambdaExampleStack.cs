using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using Amazon.CDK.AWS.IAM;

namespace MomentoLambdaExample
{
    public class MomentoLambdaExampleStack : Stack
    {
        internal MomentoLambdaExampleStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            Function fn1 = new Function(this, "MomentoLambdaExampleHandler1", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("./MomentoLambdaExampleHandler1/src/MomentoLambdaExampleHandler1/bin/Debug/net6.0"),
                Handler = "MomentoLambdaExampleHandler1::MomentoLambdaExampleHandler1.Function::FunctionHandler",
                FunctionName = "GetItemHandler",
                MemorySize = 1024,
                Timeout = Duration.Seconds(300)
            });
        }
    }
}
