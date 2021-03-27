using NLog;

using Doocutor.Core.Executors;
using Doocutor.Core.Exceptions;
using Doocutor.Core.Commands;

namespace Doocutor.Core
{
    class CommandHandler : ICommandHandler
    {
        private readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        private ICommandExecutor nativeCommandExecutor;
        private ICommandExecutor editorCommandExecutor;
        private ICommandRecognizer commandRecognizer;

        /// <summary>
        /// Defautl constructor. Also there is builder that help to testing
        /// this class and set cusotom executors and recognizers.
        /// </summary>
        public CommandHandler()
        {
            this.nativeCommandExecutor = new NativeCommandExecutor();
            this.editorCommandExecutor = new EditorCommandExecutor();
            this.commandRecognizer = new CommandRecognizer();
        }

        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public class Builder
        {
            private readonly CommandHandler commandHandler = new();

            public CommandHandler Build => this.commandHandler;

            public Builder SetNativeCommandExecutor(ICommandExecutor nativeCommandExecutor)
            {
                this.commandHandler.nativeCommandExecutor = nativeCommandExecutor;
                return this;
            }

            public Builder SetEditorCommandExecutor(ICommandExecutor editorCommandExecutor)
            {
                this.commandHandler.editorCommandExecutor = editorCommandExecutor;
                return this;
            }

            public Builder SetCommandRecognizer(ICommandRecognizer commandRecognizer)
            {
                this.commandHandler.commandRecognizer = commandRecognizer;
                return this;
            }
        }

        public void Handle(string command)
        {

            try
            {
                Command recognizedCommand = this.commandRecognizer.Recognize(command);

                if (recognizedCommand.Type.Equals(CommandType.EDITOR_COMMAND))
                    this.editorCommandExecutor.Execute(recognizedCommand);
                if (recognizedCommand.Type.Equals(CommandType.NATIVE_COMMAND))
                    this.nativeCommandExecutor.Execute(recognizedCommand);
            }
            catch (CommandRecognizingException error)
            {
                this.LOGGER.Error(error.Message);
            }
        }
    }
}

