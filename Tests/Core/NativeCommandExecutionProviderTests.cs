using Domain.Core;
using Domain.Core.Commands;
using NUnit.Framework;

namespace Tests.Core
{
    [TestFixture]
    internal class NativeCommandExecutionProviderTests
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
            var commandContent = NativeCommandExecutionProvider.SupportedCommands[^1];
            var command = new NativeCommand(commandContent);

            NativeCommandExecutionProvider.GetExecutingFunction(command)();
            
            Assert.True(Checkbox.State, "Given native command :test should turn the checkbox on!");
        }
    }

    internal class MockNativeCommandExecutionProvider : NativeCommandExecutionProvider
    {
        public MockNativeCommandExecutionProvider()
        {
            SupportedCommands.Add(":test");
            AddCommand(":test", ExecuteTestCommand);
        }
            
        private static void ExecuteTestCommand(NativeCommand command) => Checkbox.TurnOn();
    }
}