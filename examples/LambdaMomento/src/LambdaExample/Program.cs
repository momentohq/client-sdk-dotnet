using Amazon.CDK;

namespace LambdaExample
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new LambdaExampleStack(app, "LambdaExampleStack");

            app.Synth();
        }
    }
}
