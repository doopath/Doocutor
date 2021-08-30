using Domain.Core.CommandRecognizers;
using Domain.Core.Commands;
using Domain.Core.Exceptions;
using Domain.Core.Executors;
using Libraries.Core;

namespace Domain.Core.CommandHandlers
{
    public class CommandHandler : ICommandHandler
    {
        private ICommandExecutor<NativeCommand> _nativeCommandExecutor;
        private ICommandExecutor<EditorCommand> _editorCommandExecutor;
        private ICommandRecognizer _commandRecognizer;

        /// <summary>
        /// A default constructor. Also there is builder that help testing
        /// this class and set custom executors and recognizers.
        /// </summary>
        public CommandHandler()
        {
            _nativeCommandExecutor = new NativeCommandExecutor();
            _editorCommandExecutor = new EditorCommandExecutor();
            _commandRecognizer = new CommandRecognizer();
        }

        public static Builder GetBuilder()
        {
            return new();
        }

        public class Builder
        {
            private readonly CommandHandler _commandHandler = new();

            public CommandHandler Build() => _commandHandler;

            public Builder SetNativeCommandExecutor(ICommandExecutor<NativeCommand> nativeCommandExecutor)
            {
                _commandHandler._nativeCommandExecutor = nativeCommandExecutor;
                return this;
            }

            public Builder SetEditorCommandExecutor(ICommandExecutor<EditorCommand> editorCommandExecutor)
            {
                _commandHandler._editorCommandExecutor = editorCommandExecutor;
                return this;
            }

            public Builder SetCommandRecognizer(ICommandRecognizer commandRecognizer)
            {
                _commandHandler._commandRecognizer = commandRecognizer;
                return this;
            }
        }

        public void Handle(string command)
        {
            try
            {
                var recognizedCommand = _commandRecognizer.Recognize(command);

                switch (recognizedCommand.Type)
                {
                    case CommandType.EDITOR_COMMAND:
                        _editorCommandExecutor.Execute((EditorCommand) recognizedCommand);
                        break;
                    case CommandType.NATIVE_COMMAND:
                        _nativeCommandExecutor.Execute((NativeCommand) recognizedCommand);
                        break;
                    default:
                        throw new UnsupportedCommandException($"Given command {command} is not supported!");
                }
            }
            catch (CommandRecognizingException error)
            {
                ErrorHandling.showError(error);
            }
        }
    }
}
