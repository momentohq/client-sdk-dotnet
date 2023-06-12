using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;

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

            var MOMENTO_AUTH_TOKEN = "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJyaXNodGlAbW9tZW50b2hxLmNvbSIsImNwIjoiY29udHJvbC5jZWxsLTQtdXMtd2VzdC0yLTEucHJvZC5hLm1vbWVudG9ocS5jb20iLCJjIjoiY2FjaGUuY2VsbC00LXVzLXdlc3QtMi0xLnByb2QuYS5tb21lbnRvaHEuY29tIn0.eGgz9D7-R_6T9TVL-zsE8rnNf6TaVG6qKh7G0PmuaOBkWDC61hVOvZPwvu-n43Ld5NV5-sfTFQwY5BL0iL9Dpw";

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

            // Create an event rule with a schedule expression for every 5 minutes
            // var rule = new Rule(this, "MomentoRedisExampleLambdaTriggerRule", new RuleProps
            // {
            //     Schedule = Schedule.Cron(new CronOptions
            //     {
            //         Minute = "*/5" // Run every 5 minutes
            //     })
            // });

            // Add the Lambda function as a target for the event rule
            // rule.AddTarget(new LambdaFunction(fn1));
        }
    }
}
