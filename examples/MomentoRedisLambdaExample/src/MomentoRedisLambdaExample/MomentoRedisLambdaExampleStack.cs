using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.EC2;

namespace MomentoRedisLambdaExample
{
    public class MomentoRedisLambdaExampleStack : Stack
    {
        internal MomentoRedisLambdaExampleStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var vpc = new Vpc(this, "VPC", new VpcProps
            {
	            SubnetConfiguration = new ISubnetConfiguration[] {
                new SubnetConfiguration{
                    Name = "public-subnet",
                    SubnetType = SubnetType.PUBLIC,
                    CidrMask = 24,
                },
                new SubnetConfiguration{
                    Name = "private-subnet",
                    SubnetType = SubnetType.PRIVATE_WITH_EGRESS,
                    CidrMask = 24,
                },
                }
                // maxAzs: 2,
                // natGateways: 2,
            });

            var privateSubnet = Subnet.FromSubnetId(this, "PrivateSubnet", "subnet-07fff86b0e418963c");  // Specify the desired private subnet ID

            // Create the IAM role for the Lambda function
            Role lambdaRole = new Role(this, "LambdaRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
                RoleName = "MomentoRedisExampleLambdaRole"
            });

            // Attach the managed policies
            lambdaRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole"));
            lambdaRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonEC2FullAccess"));
            lambdaRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElastiCacheFullAccess"));
            lambdaRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonS3FullAccess"));

            var MOMENTO_AUTH_TOKEN = "replace with your momento auth token";

            Function fn1 = new Function(this, "MomentoRedisLambdaExampleHandler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("./MomentoRedisExampleLambdaHandler/src/MomentoRedisExampleLambdaHandler/bin/Release/net6.0/publish"),
                Handler = "MomentoRedisExampleLambdaHandler::MomentoRedisExampleLambdaHandler.Function::FunctionHandler",
                FunctionName = "MomentoRedisLambdaExampleHandler", 
                MemorySize = 1024,
                Timeout = Duration.Seconds(300),
                Role = lambdaRole,
                Vpc = vpc,
                VpcSubnets = new SubnetSelection { Subnets = new[] { privateSubnet } },
                Environment = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "MOMENTO_AUTH_TOKEN", MOMENTO_AUTH_TOKEN}
                } 
            });
        }
    }
}
