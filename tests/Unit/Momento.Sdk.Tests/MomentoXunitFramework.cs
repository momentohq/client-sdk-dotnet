namespace Momento.Sdk.Tests.Unit;

using FluentAssertions;
using Xunit.Abstractions;
using Xunit.Sdk;

public class MomentoXunitFramework : XunitTestFramework
{
    public MomentoXunitFramework(IMessageSink messageSink)
        : base(messageSink)
    {
        AssertionOptions.AssertEquivalencyUsing(
            options => options.ComparingRecordsByValue()
        );
    }
}
