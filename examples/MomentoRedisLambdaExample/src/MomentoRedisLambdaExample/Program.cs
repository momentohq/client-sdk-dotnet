using Amazon.CDK;

namespace MomentoRedisLambdaExample
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new MomentoRedisLambdaExampleStack(app, "MomentoRedisLambdaExampleStack", new StackProps
            {
                Env = new Environment 
                {
                    Account = Aws.ACCOUNT_ID,
                    Region = Aws.REGION
                }
            });
            app.Synth();
        }
    }
}
