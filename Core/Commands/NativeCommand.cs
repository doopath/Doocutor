
namespace Doocutor.Core.Commands
{
    internal class NativeCommand : ICommand
    {
        public CommandType Type { get; private set; } = CommandType.NATIVE_COMMAND;
        public string Content { get; private set; }

        public NativeCommand(string content)
        {
            this.Content = content;
        }
    }
}
