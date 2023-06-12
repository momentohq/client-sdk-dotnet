using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MomentoRedisLambdaExample
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new MomentoRedisLambdaExampleStack(app, "MomentoRedisLambdaExampleStack", new StackProps
            {
                Env = new Amazon.CDK.Environment 
                {
                    Account = "287427698164",
                    Region = "us-west-2"
                }
            });
            app.Synth();
        }
    }
}
