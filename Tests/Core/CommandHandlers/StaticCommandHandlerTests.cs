using Domain.Core.CommandHandlers;
using Domain.Core.CommandRecognizers;
using Domain.Core.Commands;
using Domain.Core.Exceptions;
using Domain.Core.Executors;
using NUnit.Framework;
using StaticEditor.Core.CommandHandlers;

namespace Tests.Core.CommandHandlers
{
    [TestFixture]
    internal class StaticCommandHandlerTests
    {
        private ICommandHandler _commandHandler;

        [SetUp]
        public void Setup()
        {
            Checkbox.TurnOff();
            _commandHandler = StaticCommandHandler.GetBuilder()
                .SetCommandRecognizer(new MockCommandRecognizer())
                .SetNativeCommandExecutor(new MockNativeExecutor())
                .SetEditorCommandExecutor(new MockEditorExecutor())
                .Build();
        }

        [TearDown]
        public void TearDown() => Checkbox.TurnOff();

        [Test]
        public void HandleWithANativeCommandTest()
        {
            _commandHandler.Handle("native");
            Assert.True(Checkbox.State,
                "Launching the StaticCommandHandler with the \"native\" argument should turn the checkbox on!");
        }

        [Test]
        public void HandleWithAnEditorCommandTest()
        {
            _commandHandler.Handle("editor");

            Assert.True(Checkbox.State,
                "Launching the StaticCommandHandler with the \"editor\" argument should turn the checkbox on!");
        }

        [Test]
        public void HandleWithAnIncorrectCommandTest()
        {
            _commandHandler.Handle("someIncorrectCommand");

            Assert.True(!Checkbox.State,
                $"Launching the StaticCommandHandler with an incorrect argument shouldn't turn the checkbox on!");
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
        public ICommand Recognize(string command) => command switch
        {
            "native" => new NativeCommand(command),
            "editor" => new EditorCommand(command),
            _ => throw new UnsupportedCommandException("Should test only native and editor commands!")
        };

        public ICommand? TryRecognize(string command)
        {
            try
            {
                return Recognize(command);
            }
            catch (UnsupportedCommandException)
            {
                return null;
            }
        }
    }
}