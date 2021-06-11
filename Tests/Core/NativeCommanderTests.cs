using Doocutor.Core;
using Doocutor.Core.Commands;
using Doocutor.Tests;
using NUnit.Framework;

namespace Tests.Core
{
    [TestFixture]
    public class NativeCommanderTests
    {
        [SetUp]
        public void SetUp()
        {
            Checkbox.TurnOff();
            _ = new MockNativeCommander();
        }

        [TearDown]
        public void TearDown() => Checkbox.TurnOff();
        
        [Test]
        public void GetExecutingFunctionTest()
        {
            var commandContent = MockNativeCommander.SupportedCommands[^1];
            var command = new NativeCommand(commandContent);
            
            MockNativeCommander.GetExecutingFunction(command)();
            
            Assert.True(Checkbox.State, "Given native command :test should turn the checkbox on!");
        }
    }

    internal class MockNativeCommander : NativeCommander
    {
        public MockNativeCommander()
        {
            SupportedCommands.Add(":test");
            AddExecutingCommand(":test", ExecuteTestCommand);
        }
        
        private static void ExecuteTestCommand(NativeCommand command) => Checkbox.TurnOn();
    }
}