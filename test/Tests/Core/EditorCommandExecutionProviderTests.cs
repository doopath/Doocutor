using CommandHandling;
using CommandHandling.Commands;
using NUnit.Framework;

namespace Tests.Core;

[TestFixture]
internal class EditorCommandExecutionProviderTests
{
    [SetUp]
    public void SetUp()
    {
        Checkbox.TurnOff();
        _ = new MockNativeCommandExecutionProvider();
    }

    [TearDown]
    public void TearDown() => Checkbox.TurnOff();

    [Test]
    public void GetExecutingFunctionTest()
    {
        var commandContent = EditorCommandExecutionProvider.SupportedCommands[^1];
        var command = new EditorCommand(commandContent);

        EditorCommandExecutionProvider.GetExecutingFunction(command)();

        Assert.True(Checkbox.State, "Given native command :test should turn the checkbox on!");
    }
}

internal class MockNativeCommandExecutionProvider : EditorCommandExecutionProvider
{
    public MockNativeCommandExecutionProvider()
    {
        SupportedCommands.Add(":test");
        EditorCommandsMap.Map.Add(":test", ExecuteTestCommand);
    }

    private static void ExecuteTestCommand(EditorCommand command) => Checkbox.TurnOn();
}
