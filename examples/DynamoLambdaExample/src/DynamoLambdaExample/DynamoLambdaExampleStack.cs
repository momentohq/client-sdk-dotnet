using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.EC2;

namespace DynamoLambdaExample
{
    public class DynamoLambdaExampleStack : Stack
    {
        internal DynamoLambdaExampleStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // Existing VPC configuration
            var vpc = Vpc.FromLookup(this, "DefaultVpc", new VpcLookupOptions
            {
                VpcId = "vpc-00e94e9613dde8d21"
            });

            var existingSubnet = Subnet.FromSubnetId(this, "ExistingSubnet", "subnet-09e0ce2f2c6b2faac"); 

            // Create the IAM role for the Lambda function
            Role lambdaRole = new Role(this, "LambdaRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
                RoleName = "LambdaDynamoDBRole"
            });

            // Attach the managed policies
            lambdaRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole"));
            lambdaRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonDynamoDBFullAccess"));
            lambdaRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonEC2FullAccess"));
            lambdaRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElastiCacheFullAccess"));
            lambdaRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AWSLambdaInvocation-DynamoDB"));

            Function fn = new Function(this, "DynamoLambdaExampleHandler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("./DynamoLambdaExampleHandler/src/DynamoLambdaExampleHandler/bin/Release/net6.0/publish"),
                Handler = "DynamoLambdaExampleHandler::DynamoLambdaExampleHandler.Function::FunctionHandler",
                FunctionName = "PutItemHandler", 
                MemorySize = 1024,
                Timeout = Duration.Seconds(300),
                Role = lambdaRole,
                Vpc = vpc,
                VpcSubnets = new SubnetSelection { Subnets = new[] { existingSubnet } },
                AllowPublicSubnet = true
            });

            Function fn1 = new Function(this, "DynamoLambdaExampleHandler1", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("./DynamoLambdaExampleHandler1/src/DynamoLambdaExampleHandler1/bin/Release/net6.0/publish"),
                Handler = "DynamoLambdaExampleHandler1::DynamoLambdaExampleHandler1.Function::FunctionHandler",
                FunctionName = "GetItemHandler", 
                MemorySize = 1024,
                Timeout = Duration.Seconds(300),
                Role = lambdaRole,
                Vpc = vpc,
                VpcSubnets = new SubnetSelection { Subnets = new[] { existingSubnet } },
                AllowPublicSubnet = true
            });
        }
    }
}
