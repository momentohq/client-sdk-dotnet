using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamoLambdaExample
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new DynamoLambdaExampleStack(app, "DynamoLambdaExampleStack");
            
            app.Synth();
        }
    }
}
