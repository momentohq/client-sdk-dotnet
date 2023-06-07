using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MomentoLambdaExample
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new MomentoLambdaExampleStack(app, "MomentoLambdaExampleStack");

            app.Synth();
        }
    }
}
