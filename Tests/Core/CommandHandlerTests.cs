using System;
using Doocutor.Core;
using Doocutor.Core.Commands;
using Doocutor.Core.Executors;
using Doocutor.Tests;
using NUnit.Framework;

namespace Tests.Core
{
    [TestFixture]
    public class CommandHandlerTests
    {
        private static ICommandHandler _testCommandHandler;

        [SetUp]
        public void Setup()
        {
            Checkbox.TurnOff();
            _testCommandHandler = CommandHandler.GetBuilder()
                .SetCommandRecognizer(new MockCommandRecognizer())
                .SetNativeCommandExecutor(new MockNativeExecutor())
                .SetEditorCommandExecutor(new MockEditorExecutor())
                .Build();
        }
        
        [TearDown]
        public void TearDown() => Checkbox.TurnOff();

        [Test]
        public void HandleWithNativeCommandTest()
        {
            _testCommandHandler.Handle("native");
            Assert.True(Checkbox.State, "Launching the TestCommandHandler with \"native\" argument should turn the checkbox on!");
        }

        [Test]
        public void HandleWithEditorCommandTest()
        {
            _testCommandHandler.Handle("editor");
            Assert.True(Checkbox.State, "Launching the TestCommandHandler with \"editor\" argument should turn the checkbox on!");
        }
    }

    internal class MockNativeExecutor : ICommandExecutor<NativeCommand>
    {
        public void Execute(NativeCommand command) => Checkbox.TurnOn();
    }
    
    internal class MockEditorExecutor : ICommandExecutor<EditorCommand>
    {
        public void Execute(EditorCommand command) => Checkbox.TurnOn();
    }

    internal class MockCommandRecognizer : ICommandRecognizer
    {
        public ICommand Recognize(string command)
            => command switch
            {
                "native" => new NativeCommand(command),
                "editor" => new EditorCommand(command),
                _ => throw new Exception("Should test only native and editor commands!")
            };
    }
}