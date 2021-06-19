using Doocutor.Core;
using Doocutor.Core.Commands;
using Doocutor.Tests;
using NUnit.Framework;

namespace Tests.Core
{
    [TestFixture]
    public class NativeCommandExecutionProviderTests
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
            var commandContent = MockNativeCommandExecutionProvider.SupportedCommands[^1];
            var command = new NativeCommand(commandContent);
            
            MockNativeCommandExecutionProvider.GetExecutingFunction(command)();
            
            Assert.True(Checkbox.State, "Given native command :test should turn the checkbox on!");
        }
    }

    internal class MockNativeCommandExecutionProvider : NativeCommandExecutionProvider
    {
        public MockNativeCommandExecutionProvider()
        {
            SupportedCommands.Add(":test");
            AddExecutingCommand(":test", ExecuteTestCommand);
        }
        
        private static void ExecuteTestCommand(NativeCommand command) => Checkbox.TurnOn();
    }
}