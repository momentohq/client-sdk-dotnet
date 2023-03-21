using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using LambdaExampleHandler;

namespace LambdaExampleHandler.Tests;

public class FunctionTest
{
    private const string Expected = "Hello World!";


    [Fact]
    public void HelloWorldTest()
    {
        var function = new Function();
        // Invoke the lambda function and confirm Hello World! .
        using (var sw = new StringWriter())
         {
            Console.SetOut(sw);
            // HelloWorld.Program.Main();
            function.FunctionHandler();

            var result = sw.ToString().Trim();
            Assert.Equal(Expected, result);
         }
    }
}
